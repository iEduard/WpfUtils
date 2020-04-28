using System;
using System.Collections.ObjectModel;
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

        #endregion

        #region public variables

        /// <summary>
        /// Observable List for the Data Grid and the Simulation Interface.
        /// </summary>
        public ObservableCollection<UpdateObject> UpdatableObjects { get; set; }


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


        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settingsPath"></param>
        public UpdaterViewModel(string settingsPath, string localPath)
        {
            // Create a new observable collection
            UpdatableObjects = new ObservableCollection<UpdateObject>();

            StatusBarBackground = Brushes.Orange;
            StatusBarText = "Checking for available updates";

            updater = new Updater(settingsPath, localPath, UpdatableObjects);
            updater.UpdateStateChanged += UpdaterStatusUpdate;
        }

        /// <summary>
        /// Constructor 
        /// </summary>
        /// <param name="settingsPath">own settings path of the updater</param>
        /// <param name="localPath">local path of the running application</param>
        /// <param name="applicationName">own application name</param>
        public UpdaterViewModel(string settingsPath, string localPath, string applicationName)
        {
            // Create a new observable collection
            UpdatableObjects = new ObservableCollection<UpdateObject>();

            StatusBarBackground = Brushes.Orange;
            StatusBarText = "Checking for available updates";

            updater = new Updater(settingsPath, localPath, UpdatableObjects, applicationName);
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
        /// 
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
