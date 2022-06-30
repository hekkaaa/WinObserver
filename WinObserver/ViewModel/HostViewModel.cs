﻿using Apparat.Helpers;
using Apparat.ViewModel.Interfaces;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using WinObserver.Model;
using WinObserver.Service;

namespace Apparat.ViewModel
{
    public class HostViewModel : INotifyPropertyChanged, IHostViewModel
    {
        private readonly TracertService? _tracerService;
        private bool _statusWorkDataGrid = false;
        public ReadOnlyObservableCollection<TracertModel>? TracertObject { get; set; }
        private string? _hostnameView;
        private GeneralPanelModel? _generalPanelModel;

        public string? HostnameView
        {
            get { return _hostnameView; }
            set { _hostnameView = value; OnPropertyChanged(); }
        }

        public HostViewModel()
        {
            _tracerService = new TracertService();
            TracertObject = _tracerService._tracertValue;
        }


        private DelegateCommand? _startCommand { get; }
        public DelegateCommand StartCommand
        {
            get
            {
                return _startCommand ?? new DelegateCommand((obj) =>
                {
                    if (_statusWorkDataGrid)
                    {
                        ControlBtnHost = IconeMap.Restart;
                        _tracerService!.StopTraceroute();

                        _statusWorkDataGrid = false;
                        //ControlBtnName = ViewStatusStringBtn.Start.ToString();
                    }
                    else
                    {
                        //RestartInfoInDataGrid();

                        ControlBtnHost = IconeMap.Stop;
                        _tracerService.StartTraceroute(HostnameView, this);
                        _statusWorkDataGrid = true;
                        //RemoveInfoinTextBoxPanel();
                    }
                    OnPropertyChanged();
                });
            }
        }

        public string ControlBtnName
        {
            get { return _generalPanelModel!.NameControlBtn; }
            set { _generalPanelModel!.NameControlBtn = value; OnPropertyChanged(); }
        }

        // Можно потом придумать глобальный стиль для всех.
        private string _TextErrorToolTip = "The hostname is entered incorrectly";
        public string TextErrorToolTip
        {
            get { return _TextErrorToolTip; }
        }

        private string _errorHostnameVisibleIcon = "Hidden";
        public string ErrorHostnameVisibleIcon
        {
            get { return _errorHostnameVisibleIcon; }
            set { _errorHostnameVisibleIcon = value; OnPropertyChanged(); }
        }

        private string _controlBtnHost = IconeMap.Start;
        public string ControlBtnHost
        {
            get { return _controlBtnHost; }
            set { _controlBtnHost = value; OnPropertyChanged(); }
        }

        private string _settingOpacityControlBtn;
        public string SettingOpacityControlBtn
        {
            get { return _settingOpacityControlBtn; }
            set { _settingOpacityControlBtn = value; OnPropertyChanged(); }
        }

        private string _settingIsEnableControlBtn;
        public string SettingIsEnableControlBtn
        {
            get { return _settingIsEnableControlBtn; }
            set { _settingIsEnableControlBtn = value; OnPropertyChanged(); }
        }

        public void ErrorNameHostname()
        {
            ControlBtnHost = IconeMap.Start;
            ErrorHostnameVisibleIcon = "Visible";
            SettingIsEnableControlBtn = "False";
            SettingOpacityControlBtn = "0.5";
        }

        public void ManagementEnableGeneralControlBtn(bool obj)
        {
            if (obj)
            {
                SettingIsEnableControlBtn = "True";
                SettingOpacityControlBtn = "1";
            }
            else
            {
                SettingIsEnableControlBtn = "False";
                SettingOpacityControlBtn = "0.5";
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}