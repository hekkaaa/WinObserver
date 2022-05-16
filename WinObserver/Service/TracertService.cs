﻿using NetObserver.PingUtility;
using NetObserver.TracerouteUtility;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WinObserver.Algorithms;
using WinObserver.Model;

namespace WinObserver.Service
{
    public class TracertService : INotifyPropertyChanged
    {
        private ObservableCollection<TracertModel> _innerTracertValue;
        public readonly ReadOnlyObservableCollection<TracertModel> _tracertValue;

        public readonly DataGridModel _gridTracert;
        private readonly Traceroute _tracerouteHelper;

        static CancellationTokenSource? _cancellationTokenSource = new CancellationTokenSource();
        CancellationToken token = _cancellationTokenSource!.Token;

        public TracertService()
        {
            _innerTracertValue = new ObservableCollection<TracertModel>();
            _tracertValue = new ReadOnlyObservableCollection<TracertModel>(_innerTracertValue);
            _gridTracert = new DataGridModel();
            _tracerouteHelper = new Traceroute();
        }

        public void StartTraceroute(string hostname)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(obj =>
            {
                IEnumerable<string> objectTracertResult = _tracerouteHelper.GetIpTraceRoute(hostname);

                ClearoldTable();
                FillingNewtable(objectTracertResult);

                while (true)
                {
                    var test = token;
                    Task.Delay(1000).Wait();
                    UpdateStatistic();

                    if (token.IsCancellationRequested)
                    {   
                        _cancellationTokenSource!.Dispose();
                        RestartToken();
                        break;
                    }
                }
            }), token);
        }

        public void StopTraceroute()
        {
            _cancellationTokenSource!.Cancel();
        }


        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }


        private void RestartToken()
        {
            _cancellationTokenSource = new CancellationTokenSource();
            token = _cancellationTokenSource.Token;
        }

        private void ClearoldTable()
        {
            App.Current.Dispatcher.BeginInvoke((System.Action)delegate
            {
                _innerTracertValue.Clear();
                OnPropertyChanged();
            });
        }

        private void UpdateStatistic()
        {
            IcmpRequestSender icmpUtilite = new IcmpRequestSender();

            foreach (TracertModel objectCollection in _innerTracertValue)
            {   
                PingReply tmpResult = icmpUtilite.RequestIcmp(objectCollection.Hostname);
                TracertModel tempValue = objectCollection;

                if (tmpResult.Status == IPStatus.Success)
                {
                    tempValue.LastDelay = (int)tmpResult.RoundtripTime;
                    tempValue.ArhivePingList!.Add((int)tmpResult.RoundtripTime);
                    DataGridStatisticAlgorithm.UpdateMinMaxPing(ref tempValue, (int)tmpResult.RoundtripTime);
                    DataGridStatisticAlgorithm.MiddlePing(ref tempValue);
                    tempValue.CounterPacket++;
                }
                else
                {
                    tempValue.LastDelay = 0;
                    tempValue.CounterPacket++;
                    tempValue.CounterLossPacket++;
                }

                tempValue.PercentLossPacket = DataGridStatisticAlgorithm.RateLosses(tempValue.CounterPacket, tempValue.CounterLossPacket);
            }
        }

        private void FillingNewtable(IEnumerable<string> collectionIpAddres)
        {
            int countHostname = 1;

            foreach (string addres in collectionIpAddres)
            {
                App.Current.Dispatcher.BeginInvoke((System.Action)delegate
                {
                    _innerTracertValue.Add(new TracertModel { NumberHostname = countHostname, Hostname = addres });
                    countHostname++;
                    OnPropertyChanged();
                });
            }
        }
    }
}
