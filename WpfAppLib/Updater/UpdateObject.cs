﻿using CopyAndCompare;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace WpfAppLib.Updater
{
    public class UpdateObject : INotifyPropertyChanged
    {

        #region Private variables

        // Private Variables
        private bool isSelectedToDownload;
        private bool newestVersionInstalled;
        private int downloadState = 0;
        private updaterSettingsData settings;
        private string remoteVersion;
        private string localVersion;

        #endregion


        #region Public variables

        /// <summary>
        /// Select to update. If True this application will be updated from the Server.
        /// </summary>
        public bool IsSelectedToDownload
        {
            get { return isSelectedToDownload; }
            set
            {
                isSelectedToDownload = value;
                NotifyPropertyChanged("IsSelectedToDownload");
            }
        }

        /// <summary>
        /// Name of the Application to update / check for updates
        /// </summary>
        public string ApplicationName
        {
            get { return settings.appName; }
            set
            {
                settings.appName = value;
                NotifyPropertyChanged("ApplicationName");
            }
        }

        /// <summary>
        /// True = Installed Version is the newest available
        /// False = New Version ofthe Software available on the Server
        /// </summary>
        public bool NewestVersionInstalled
        {
            get { return newestVersionInstalled; }
            private set
            {
                newestVersionInstalled = value;
                NotifyPropertyChanged("NewestVersionInstalled");
            }
        }

        /// <summary>
        /// Local version information
        /// </summary>
        public string LocalVersion
        {
            get { return localVersion; }
            set
            {
                localVersion = value;
                NotifyPropertyChanged("LocalVersion");
            }
        }

        /// <summary>
        /// Url for the local application
        /// </summary>
        public string LocalUrl
        {
            get { return pathShortener(settings.appLocalPath); }
            private set
            {
                settings.appLocalPath = value;
                NotifyPropertyChanged("LocalUrl");

            }
        }

        /// <summary>
        /// Local version information
        /// </summary>
        public string RemoteVersion
        {
            get { return remoteVersion; }
            set
            {
                remoteVersion = value;
                NotifyPropertyChanged("RemoteVersion");
            }
        }

        /// <summary>
        /// RemoteUrl
        /// </summary>
        public string RemoteUrl
        {
            get { return pathShortener(settings.appServerPath); }
            private set
            {
                settings.appServerPath = value;
                NotifyPropertyChanged("RemoteUrl");
            }
        }

         #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="selection"></param>
        public UpdateObject(updaterSettingsData settings)
        {
            this.settings = settings;          


            // Init the States
            this.IsSelectedToDownload = false;
            this.NewestVersionInstalled = false;
            this.downloadState = 0;

        }

        /// <summary>
        /// Compare the local with the remote file.
        /// Compared by the file information of the two files. 
        /// All version parts are used to compare. Major, minor, revision, build
        /// </summary>
        /// <returns>Returns true if the remote fileversion is newer than the local version.</returns>
        public bool compareFiles()
        {
            bool retval = false;

            if (!File.Exists(this.settings.appLocalPath + this.settings.appFileName))
            {
                Console.WriteLine("Local file not found: " + this.settings.appLocalPath + this.settings.appFileName);
                LocalVersion = "nul";
                retval = true;
                if (!File.Exists(this.settings.appServerPath + this.settings.appFileName))
                {
                    Console.WriteLine("Server file not found: " + this.settings.appServerPath + this.settings.appFileName);
                    RemoteVersion = "nul";
                }
                else
                {
                    FileVersionInfo updateFileVersion = FileVersionInfo.GetVersionInfo(this.settings.appServerPath + this.settings.appFileName);
                    RemoteVersion = updateFileVersion.FileVersion.ToString();
                }
            }
            else if (!File.Exists(this.settings.appServerPath + this.settings.appFileName))
            {
                Console.WriteLine("Server file not found: " + this.settings.appServerPath + this.settings.appFileName);
                RemoteVersion = "nul";
            }
            else
            {

                try
                {
                    FileVersionInfo localFileVersion = FileVersionInfo.GetVersionInfo(this.settings.appLocalPath + this.settings.appFileName);
                    LocalVersion = localFileVersion.FileVersion.ToString();

                }
                catch
                {
                    LocalVersion = "nul";
                    // Do nothing
                }

                try
                {
                    FileVersionInfo updateFileVersion = FileVersionInfo.GetVersionInfo(this.settings.appServerPath + this.settings.appFileName);
                    RemoteVersion = updateFileVersion.FileVersion.ToString();
                }
                catch
                {
                    RemoteVersion = "nul";
                    // Do nothing
                }


                try
                {
                    // Get the file version for the notepad.
                    // Use either of the two following commands.
                    FileVersionInfo localFileVersion = FileVersionInfo.GetVersionInfo(this.settings.appLocalPath + this.settings.appFileName);
                    FileVersionInfo updateFileVersion = FileVersionInfo.GetVersionInfo(this.settings.appServerPath + this.settings.appFileName);

                    

                    if (compareFileVersion(localFileVersion, updateFileVersion))
                    {
                        retval = true;
                        Console.WriteLine("New Version available for: " + updateFileVersion.FileDescription);
                        Console.WriteLine("\t -Server version: " + updateFileVersion.FileVersion);
                        Console.WriteLine("\t -Local version: " + localFileVersion.FileVersion);
                    }
                    else
                    {
                        retval = false;
                        Console.WriteLine("Current Version installed for: " + updateFileVersion.FileDescription + ": " + localFileVersion.FileVersion);
                        this.NewestVersionInstalled = true;
                    }

                }
                catch
                {
                    retval = false;
                }
            }

            this.IsSelectedToDownload = retval;
            return retval;
        }

        /// <summary>
        /// Compare the different versions Major, Minor,Revision, Build and check if the remote version is newer than the ĺocal one.
        /// </summary>
        /// <param name="localFile">local ´file informations</param>
        /// <param name="remoteFile">remote file informations</param>
        /// <returns>Returns true if the online version is newer</returns>
        private bool compareFileVersion(FileVersionInfo localFile, FileVersionInfo remoteFile)
        {
            // Init the return valiue
            bool retval = false;

            if (remoteFile.FileMajorPart > localFile.FileMajorPart)
            {
                retval = true;
            }
            else if (remoteFile.FileMajorPart < localFile.FileMajorPart)
            {
                retval = false;
            }
            else if (remoteFile.FileMinorPart > localFile.FileMinorPart)
            {
                retval = true;
            }
            else if (remoteFile.FileMinorPart < localFile.FileMinorPart)
            {
                retval = false;
            }
            else if (remoteFile.FileBuildPart > localFile.FileBuildPart)
            {
                retval = true;
            }
            else if (remoteFile.FileBuildPart < localFile.FileBuildPart)
            {
                retval = false;
            }
            else if (remoteFile.FilePrivatePart > localFile.FilePrivatePart)
            {
                retval = true;
            }
            else if (remoteFile.FilePrivatePart < localFile.FilePrivatePart)
            {
                retval = false;
            }

            return retval;

        }

        /// <summary>
        /// Update the running application
        /// </summary>
        /// <returns>Returns true if the update (download) perfomed successfully</returns>
        public bool performUpdate()
        {

            bool _retVal = false;

            if (!this.IsSelectedToDownload)
            {
                return _retVal;
            }

            Console.WriteLine("Performing Update for " + this.settings.appName + "...");

            // Check if the Destination Directory allready exists
            // If not create it
            if (!Directory.Exists(this.settings.appLocalPath))
            {
                Directory.CreateDirectory(this.settings.appLocalPath);
            }


            // Da wir den Instally selber nicht so einfach ersetzen können da er ja bereits läuft müssen wir ihm einen besonderen Namen geben mit dem versionszusatz
            string _fileNameAddon = this.settings.appFileName.Replace(".exe", "") + "_" + FileVersionInfo.GetVersionInfo(this.settings.appServerPath + this.settings.appFileName).FileVersion + ".exe";

            // perform the update of the execution file
            CopyMaster copyFiles = new CopyMaster();
            _retVal = copyFiles.copyFile(this.settings.appServerPath + this.settings.appFileName, this.settings.appLocalPath + _fileNameAddon, true);

            // Update the settings File
            // Check if the Destination Directory allready exists
            // If not create it
            if (!Directory.Exists(this.settings.settingsLocalPath))
            {
                Directory.CreateDirectory(this.settings.settingsLocalPath);
            }

            if (this.settings.settingsFileName != "" && _retVal)
            {
                // perform the update of the execution file               
                bool _var = copyFiles.copyFile(this.settings.settingsServerPath + this.settings.settingsFileName, this.settings.settingsLocalPath + this.settings.settingsFileName, true);
            }

            Console.WriteLine("... Update done");

            return _retVal;
        }

        /// <summary>
        /// Function to shorten the path to visualize.
        /// A path of: C:\Users\Edu\GitRepos\WpfUtils\WpfExampleApp\bin\Debug
        /// Will be shortened to: C:\Users\...\bin\Debug
        /// </summary>
        /// <param name="path">the path to shorten</param>
        /// <returns>The shortened path</returns>
        private string pathShortener(string path)
        {
            // Define some variables to use in this function
            bool _firstPartFound = false;
            bool _secondPartFound = false;
            string _shortenedPath = "";
            string[] _splits = path.Split('\\');
            StringBuilder _sb = new StringBuilder();
            int _partCount = 0;
            bool _lastOneWasNotEmpty = false;
            int _firstPartCount = 0;
            int _secondPartCount = 0;
            string _secondPart = "";

            // Add the first part
            for (_firstPartCount = 0; _firstPartCount <= (_splits.Length - 1); _firstPartCount++)
            {

                if (_splits[_firstPartCount] == "")
                {
                    _sb.Append("\\");
                    _lastOneWasNotEmpty = false;
                }
                else
                {
                    if (_lastOneWasNotEmpty)
                    {
                        _sb.Append("\\");
                    }
                    _sb.Append(_splits[_firstPartCount]);
                    _partCount++;
                    _lastOneWasNotEmpty = true;
                }

                // Check if we allready recived all Data.
                if (_partCount >= 2)
                {
                    _firstPartFound = true;
                    break;
                }
            }

            // Only search for the second part if the fir
            if (_firstPartFound)
            {
                _sb.Append("\\...\\");


                // Add the second part
                for (int _i = _splits.Length - 1; _i > _firstPartCount; _i--)
                {

                    if (_splits[_i] == "")
                    {
                        _secondPart = string.Concat("\\", _secondPart);
                        _lastOneWasNotEmpty = false;
                    }
                    else
                    {
                        if (_lastOneWasNotEmpty)
                        {
                            _secondPart = string.Concat("\\", _secondPart);
                        }
                        _secondPart = string.Concat(_splits[_i], _secondPart);
                        _secondPartCount++;
                        _lastOneWasNotEmpty = true;
                    }

                    // Check if we allready recived all Data.
                    if (_secondPartCount >= 2)
                    {
                        _secondPartFound = true;
                        break;
                    }

                }
                _sb.Append(_secondPart);

            }

            // If both parts are found we will create the shortened path string. Otherwise we will return the input path 
            if (_firstPartFound && _secondPartFound)
            {
                _shortenedPath = _sb.ToString();
            }
            else
            {
                _shortenedPath = path;
            }


            return _shortenedPath;

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
