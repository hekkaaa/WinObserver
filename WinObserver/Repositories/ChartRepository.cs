﻿using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinObserver.Repositories.Interface;

namespace WinObserver.Repositories
{
    public class ChartRepository : IChartRepository
    {
        private ObservableCollection<ISeries> _innerLoss;
        public readonly ReadOnlyObservableCollection<ISeries> _lossList;

        private List<Axis> _innerObjectXAxes;
        public readonly List<Axis> _ObjectXAxes;
        private List<Axis> _innerObjectYAxes;
        public readonly List<Axis> _ObjectYAxes;
        private List<string> _collectionTimeXAxes;

        public ChartRepository()
        {
            _innerLoss = new ObservableCollection<ISeries>();
            _lossList = new ReadOnlyObservableCollection<ISeries>(_innerLoss);
            DefaultValuesForViewChart();
            _ObjectXAxes = new List<Axis>(_innerObjectXAxes);
            _ObjectYAxes = new List<Axis>(_innerObjectYAxes);

        }

        public void AddHops(string hostname)
        {
            var newHost = new LineSeries<double>
            {
                GeometryStroke = null,
                GeometryFill = null,
                Values = new List<double>() { 0 },
                Name = hostname
            };

            _innerLoss.Add(newHost);
        }

        public void AddValueLossCollection(int numberHop, double newValueLoss)
        {
            List<double>? tmpCollectionLoss = (List<double>)_innerLoss[numberHop].Values;

            if (tmpCollectionLoss == null) 
            {
                int maxLoss = 100;
                tmpCollectionLoss.Add(maxLoss);
                _innerLoss[numberHop].Values = tmpCollectionLoss;
            }
            else
            {
                tmpCollectionLoss.Add(newValueLoss);
                _innerLoss[numberHop].Values = tmpCollectionLoss;
            }
        }

        public void ClearChart()
        {
            _innerLoss.Clear();
            _collectionTimeXAxes.Clear();
        }

        public void UpdateTimeXAxes()
        {
            DateTime date1 = DateTime.Now;
            _collectionTimeXAxes.Add(date1.ToString("T"));
            _innerObjectXAxes[0].Labels = _collectionTimeXAxes;
        }

        private void DefaultValuesForViewChart()
        {
            _collectionTimeXAxes = new List<string>() { "00:00:00" };
            _innerObjectXAxes = new List<Axis>
                {
                    new Axis
                    {   
                        LabelsRotation = 15,
                        Labels = _collectionTimeXAxes,
                    }
                };

            _innerObjectYAxes = new List<Axis>
            {
                 new Axis
                {
                    MinLimit = 0,
                    MaxLimit = 100,
                    MinStep = 10,
                }
            };
        }
    }
}
