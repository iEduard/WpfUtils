using System;
using System.Collections.ObjectModel;
using System.Threading;

namespace WpfAppLib.Updater
{
    #region Events

    public class UpdateStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// State of the creation 
        /// 0 = Done
        /// 1 = Progress running
        /// 2 = Error occured 
        /// </summary>
        public int state { get; set; }

        /// <summary>
        /// Status bar msg for the user
        /// </summary>
        public string stateMsg { get; set; }
    }

    public delegate void UpdateStateChangedEventHandler(object sender, UpdateStateChangedEventArgs e);

    #endregion


    class Updater
    {

        #region private variables
        /// <summary>
        /// Thread to check for the updates
        /// </summary>
        private Thread getVersionsThread;

        /// <summary>
        /// Thread to get the updates
        /// </summary>
        private Thread getUpdateThread;


        private UpdateStateChangedEventArgs updateStateChangedEventArgs = new UpdateStateChangedEventArgs();


        #endregion

        #region public variables

        /// <summary>
        /// Observable collection of updatable applications. This variable is binded with the UI
        /// </summary>
        public UpdateObject UpdatableObject { get; set; }

        #endregion

        #region events
        public event UpdateStateChangedEventHandler UpdateStateChanged;
        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settingsPath"></param>
        /// <param name="updatableObjects"></param>
        public Updater(string settingsPath, string localPath, UpdateObject updatableObject)
        {

            //this.UpdatableObject = updatableObject;

            //foreach (var _settings in UpdaterSettings.load(settingsPath, localPath))
            //{
            //    UpdatableObjects.Add(new UpdateObject(_settings));
            //}


            //// Check for Updates
            //getVersionsAsynch();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="settingsPath">file path of the settings</param>
        /// <param name="localPath">local file path</param>
        /// <param name="updatableObjects">collection of updatable files</param>
        /// <param name="applicationName">application name of the current running application</param>
        public Updater(string settingsPath, string localPath, ObservableCollection<UpdateObject> updatableObjects, string applicationName)
        {
            //this.OwnApplicationName = applicationName;
            //this.UpdatableObjects = updatableObjects;

            //foreach (var _settings in UpdaterSettings.load(settingsPath, localPath))
            //{
            //    UpdatableObjects.Add(new UpdateObject(_settings));
            //}

            //// Check for Updates
            //getVersionsAsynch();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="updatableObject">collection of updatable files</param>
        public Updater(UpdateObject updatableObject)
        {
            this.UpdatableObject = updatableObject;

            // Check for Updates
            getVersionsAsynch();
        }


        #region get update information

        /// <summary>
        /// Function to start the check for update thread
        /// </summary>
        public void getVersionsAsynch()
        {
            getVersionsThread = new Thread(getVersions);
            getVersionsThread.Start();
        }


        /// <summary>
        /// Function to get the update information for the current
        /// </summary>
        /// <returns></returns>
        private void getVersions()
        {
            UpdateStateChanged.Invoke(this, new UpdateStateChangedEventArgs { stateMsg = "Check for updates: " + this.UpdatableObject.ApplicationName , state = 1 });
            this.UpdatableObject.compareFiles();

            Thread.Sleep(200);
            UpdateStateChanged.Invoke(this, new UpdateStateChangedEventArgs { stateMsg = "Check for updates done", state = 0 });
        }

        #endregion

        #region Model Events

        #endregion

        #region get update 

        /// <summary>
        /// Get the update by using a thread
        /// </summary>
        public void getUpdateAsynch()
        {
            getUpdateThread = new Thread(getUpdate);
            getUpdateThread.Start();
        }

        /// <summary>
        /// get the update
        /// </summary>
        private void getUpdate()
        {
            this.UpdatableObject.performUpdate();
            UpdateStateChanged.Invoke(this, new UpdateStateChangedEventArgs { stateMsg = "Download done. Saved under: "+ UpdatableObject.PathShortener(UpdatableObject.DownloadFileName), state = 0 });
        }

        #endregion

    }
}
