﻿using Data.Entities;
using Data.Repositories.Connect;
using Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public class ChartLossRepository : IChartLossRepository
    {
        private ApplicationContext _context;

        public ChartLossRepository(ApplicationContext context)
        {
            _context = context;
        }

        public int AddHostname(Loss newHost)
        {
            _context.Losses.Add(newHost);
            _context.SaveChanges();
            return newHost.Id;
        }

        public void UpdateLoss(Loss newValue)
        {
            _context.Losses.Update(newValue);
            _context.SaveChanges();
        }

        public Loss GetHostById(int id)
        {
            return _context.Losses.FirstOrDefault(x => x.Id == id);
        }

        public Task<List<Loss>> GetAllHostInfo()
        {
            using (var dublicateConnect = new DublicateContext())
            {
                return dublicateConnect.Losses.ToListAsync();
            }
        }

        public void ClearTable()
        {
            var tmp = _context.Losses.ToList();
            if (tmp.Count != 0)
            {
                _context.Losses.RemoveRange(tmp);
                _context.SaveChanges();
            }
        }
    }
}
