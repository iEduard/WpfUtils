using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace WpfAppLib.Updater
{
    class UpdaterViewModel : INotifyPropertyChanged
    {

        #region local variables

        private Updater updater;
        private string statusBarText;
        private Brush statusBarBackground;

        private UpdateObject updatableObject;


        // UI Text fields
        private string checkUpdatesButtonText = "Check for updates";
        private string updateButtonText = "Update";
        private string cancelButtonText = "Cancel";

        private string localPathLabelText = "Local path:";
        private string localVersionLabelText = "Local version:";
        private string remotePathLabelText = "Remote path:";
        private string remoteVersionLabelText = "Remote version:";
        private string windowTitleText = "Update";

        #endregion

        #region public variables


        /// <summary>
        /// Updatable object
        /// This object is used to bind the information to the UI
        /// </summary>
        public UpdateObject UpdatableObject 
        {
            get { return updatableObject; } 
            set{
                updatableObject = value;
                NotifyPropertyChanged("UpdatableObject");
            }
        }

        /// <summary>
        /// Label content of ths statusbart
        /// </summary>
        public string StatusBarText
        {
            get { return statusBarText; }
            set
            {
                statusBarText = value;
                NotifyPropertyChanged("StatusBarText");
            }
        }

        /// <summary>
        /// Background color of the statusbar
        /// </summary>
        public Brush StatusBarBackground
        {
            get { return statusBarBackground; }
            set
            {
                statusBarBackground = value;
                NotifyPropertyChanged("StatusBarBackground");
            }
        }


        public string CheckUpdatesButtonText 
        {
            get
            {
                return checkUpdatesButtonText;
            }
            set
            {
                checkUpdatesButtonText = value;
                NotifyPropertyChanged("CheckUpdatesButtonTex");
            }
        }

        public string UpdateButtonText 
        {
            get 
            {
                return updateButtonText;
            }
            set 
            {
                updateButtonText = value;
                NotifyPropertyChanged("UpdateButtonText");
            } 
        }

        public string CancelButtonText 
        {
            get
            {
                return cancelButtonText;
            }
            set
            {
                cancelButtonText = value;
                NotifyPropertyChanged("CancelButtonText");
            }
        }

        public string LocalPathLabelText
        {
            get
            {
                return localPathLabelText;
            }
            set
            {
                localPathLabelText = value;
                NotifyPropertyChanged("LocalPathLabelText");
            }

        }

        public string LocalVersionLabelText
        {
            get
            {
                return localVersionLabelText;
            }
            set
            {
                localVersionLabelText = value;
                NotifyPropertyChanged("LocalVersionLabelText");
            }

        }

        public string RemotePathLabelText
        {
            get
            {
                return remotePathLabelText;
            }
            set
            {
                remotePathLabelText = value;
                NotifyPropertyChanged("RemotePathLabelText");
            }

        }

        public string RemoteVersionLabelText
        {
            get
            {
                return remoteVersionLabelText;
            }
            set
            {
                remoteVersionLabelText = value;
                NotifyPropertyChanged("RemoteVersionLabelText");
            }

        }

        public string WindowTitleText
        {
            get
            {
                return windowTitleText;
            }
            set
            {
                windowTitleText = value;
                NotifyPropertyChanged("WindowTitleText");
            }
        }


        #endregion

        /// <summary>
        /// Updater view model with updater settings directly handling over
        /// </summary>
        /// <param name="updaterSettings">Updater settings with all needed information to update the currently running application</param>
        public UpdaterViewModel(updaterSettingsData updaterSettings)
        {
            // Create a new observable collection
            UpdatableObject = new UpdateObject(updaterSettings);

            StatusBarBackground = Brushes.Orange;
            StatusBarText = "Checking for available updates";

            updater = new Updater(UpdatableObject);
            updater.UpdateStateChanged += UpdaterStatusUpdate;

        }


        #region user interaction

        /// <summary>
        /// Perform an update of the selected applications
        /// </summary>
        public void Update()
        {
            updater.getUpdateAsynch();
        }

        /// <summary>
        /// Perform the update check asynchron
        /// </summary>
        public void CheckForUpdates()
        {
            updater.getVersionsAsynch();
        }

        #endregion


        /// <summary>
        /// Updater status updated function. Will be triggered by the updatable objects
        /// Method tou pdate the statusbar ui
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void UpdaterStatusUpdate(object sender, UpdateStateChangedEventArgs args)
        {

            // Call the ui thread to start invoke the ui changes
            Application.Current.Dispatcher.Invoke((Action)(() =>
            {
                StatusBarText = args.stateMsg;

                switch (args.state)
                {
                    case 0: // Finished
                        StatusBarBackground = Brushes.LightGreen;
                        break;

                    case 1: // Progress running
                        StatusBarBackground = Brushes.Orange;
                        break;
                    case 2: // Error occured
                        StatusBarBackground = Brushes.Red;
                        break;
                }

            }));



        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

    }
}
