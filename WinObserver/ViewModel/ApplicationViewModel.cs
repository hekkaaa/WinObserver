﻿using Apparat.Commands;
using Apparat.Helpers;
using Apparat.Services.Interfaces;
using Apparat.ViewModel;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace WinObserver.ViewModel
{
    public class ApplicationViewModel : INotifyPropertyChanged, IApplicationViewModel
    {
        const string VERSION_APP = "Version: 0.1.13 - Alpha | Tester build";
        private string _hostname = String.Empty;
        private string _textBlockGeneralError = String.Empty;
        private string _borderTextBox = "#FFABADB3";

        private ObservableCollection<HostViewModel> _hostsCollection;
        private readonly IAppSettingService _appSettingService;

        public ApplicationViewModel(IAppSettingService appService)
        {
            _hostsCollection = new ObservableCollection<HostViewModel>();
            // init object class  
            _appSettingService = appService;

            UpdateCollectionHistoryHostInCombobox();
        }

        public string VersionProgramm { get { return VERSION_APP; } }

        public string TextBoxHostname
        {
            get { return _hostname; }
            set
            {
                _hostname = value;
                OnPropertyChanged();
            }
        }

        public string TextBlockGeneralError
        {
            get
            {
                return _textBlockGeneralError;
            }
            set
            {
                _textBlockGeneralError = value;
                OnPropertyChanged();
            }
        }

        public string BorderTextBox
        {
            get
            {
                return _borderTextBox;
            }
            set
            {
                _borderTextBox = value;
                OnPropertyChanged();
            }
        }


        private ObservableCollection<string> _collectionRecentHost = null!;
        public ObservableCollection<string> CollectionRecentHost
        {
            get
            {
                return _collectionRecentHost;
            }
            set
            {
                _collectionRecentHost = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<HostViewModel> HostsCollection
        {
            get => _hostsCollection;
            set { _hostsCollection = value; OnPropertyChanged(); }
        }


        private DelegateCommand _addNewHost = null!;
        public DelegateCommand AddNewHost
        {
            get
            {
                return _addNewHost ??
                 (_addNewHost = new DelegateCommand((obj) =>
                 {
                     string adjustedHostname = ValidationConditionsAndCorrections.RemovingSpaces(_hostname);
                     bool result = ValidationConditionsAndCorrections.ValidationCheck(adjustedHostname);

                     if (!result)
                     {
                         ErrorValidationTextAndAnimation();
                         return;
                     }

                     HostsCollection.Add(new HostViewModel()
                     {
                         HostnameView = adjustedHostname
                     });

                     _appSettingService.AddHostInHistory(adjustedHostname);
                     UpdateCollectionHistoryHostInCombobox();
                     RemoveInfoinTextBoxPanel();
                     OnPropertyChanged();

                 }));
            }
        }

        private DelegateCommand _closeTabCommand = null!;
        public DelegateCommand CloseTabCommand
        {
            get
            {
                return _closeTabCommand
                ?? (_closeTabCommand = new DelegateCommand(
                (obj) =>
                {
                    HostViewModel? deleteObject = obj as HostViewModel;
                    if (deleteObject != null) HostsCollection.Remove(deleteObject!);
                }));
            }
        }

        private DelegateCommand _clearAllCollectionHost = null!;
        public DelegateCommand ClearAllCollectionHost
        {
            get
            {
                return _clearAllCollectionHost
                ?? (_clearAllCollectionHost = new DelegateCommand(
                (obj) =>
                {
                    _appSettingService.ClearAllCollectionHistoryHost();
                    UpdateCollectionHistoryHostInCombobox();
                }));
            }
        }

        private DelegateCommand _deleteOneItemHistoryHostname = null!;
        public DelegateCommand DeleteOneItemHistoryHostname
        {
            get
            {
                return _deleteOneItemHistoryHostname
                ?? (_deleteOneItemHistoryHostname = new DelegateCommand(
                (obj) =>
                {
                    string deleteObject = obj as string;

                    bool removeResult = _appSettingService.DeleteOneHostnameFromHistoryCollection(deleteObject);
                    if (removeResult) UpdateCollectionHistoryHostInCombobox();
                }));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }

        private void UpdateCollectionHistoryHostInCombobox()
        {
            CollectionRecentHost = _appSettingService.GetLastFiveHistoryHost();
        }


        private void RemoveInfoinTextBoxPanel()
        {
            TextBoxHostname = null!;
        }


        public void ErrorValidationTextAndAnimation()
        {
            Task.Factory.StartNew(() =>
            {
                TextBlockGeneralError = "Hostname not valid";
                BorderTextBox = "Red";

                RemoveInfoinTextBoxPanel();

                Task.Delay(5000).Wait();

                TextBlockGeneralError = string.Empty;
                BorderTextBox = "#FFABADB3";
            });
        }
    }
}
