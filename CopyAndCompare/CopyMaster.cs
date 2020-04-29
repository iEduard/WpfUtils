using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace CopyAndCompare
{
    #region Events

    public class CopyStateChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Transfer rate in Mb/s
        /// </summary>
        public int transferRate { get; set; }

        /// <summary>
        /// Current File to copy
        /// </summary>
        public string currentFile { get; set; }

        /// <summary>
        /// Overall state for the complete copy requested
        /// </summary>
        public string overallStatePercentage { get; set; }
    }

    public delegate void CopyStateChangedEventHandler(object sender, CopyStateChangedEventArgs e);


    public class CopyFinishedEventArgs : EventArgs
    {
        /// <summary>
        /// Time elapsed for the complete process
        /// </summary>
        public long timeElapsed { get; set; }

        /// <summary>
        /// Summary flag
        /// FALSE = At least one error ocurred. The Files not copyed or copyed with a error are listet in the "notCopyedFiles" List<string>
        /// TRUE = Every requested directory or files where copyed with no error.
        /// </summary>
        public bool allFilesCopyedAndNoError { get; set; }

        /// <summary>
        /// List of all files that are not correct copyed due to errors. No entrys if the bool flag for no errors is set to false.
        /// </summary>
        public List<string> notCopyedFiles { get; set; }

        /// <summary>
        /// Source Dir for the compare operation
        /// </summary>
        public string sourceDir { get; set; }

        /// <summary>
        /// Destionation Dir for the compare operation
        /// </summary>
        public string destinationDir { get; set; }

    }

    public delegate void CopyFinishedEventHandler(object sender, CopyFinishedEventArgs e);

    #endregion

    public class CopyMaster
    {

        #region Public variables

        /// <summary>
        /// Source directory for the copy cmd
        /// </summary>
        public string SourceDirectory
        {
            get
            {
                return sourceDir;
            }
        }

        /// <summary>
        /// Destination directory for the copy cmd
        /// </summary>
        public string DestinationDir
        {
            get
            {
                return destinationDir;
            }
        }


        /// <summary>
        /// Source filename and dir for the copy cmd
        /// </summary>
        public string SourceFileName
        {
            get
            {
                return sourceFile;
            }
        }

        /// <summary>
        /// Destination filename and dir for the copy cmd
        /// </summary>
        public string DestinationFileName
        {
            get
            {
                return destinationFile;
            }
        }

        /// <summary>
        /// Defines in wich way the functions will write to the console output
        /// 0 = Write nothing to Console (default)
        /// 1 = Write ony Started and Finished to the console
        /// 2 = Write everything to the console
        /// 3 = Write Everything to the State Changed event
        /// </summary>
        public int debugLevel { get; set; }


        #endregion


        #region Public events


        public event CopyStateChangedEventHandler CopyStateChanged;
        public event CopyFinishedEventHandler CopyFinished;

        #endregion


        #region Local variables

        /// <summary>
        /// Source directory for the copy cmd
        /// </summary>
        private string sourceDir;

        /// <summary>
        /// Destination directory for the copy cmd
        /// </summary>
        private string destinationDir;

        /// <summary>
        /// Thread for the directory copy work
        /// </summary>
        private Thread copyDirectoryThread;

        /// <summary>
        /// TRUE => Stops the thread. Will be initialized with false at the beginning.
        /// </summary>
        private bool stopCopyDirectoryThread = false;

        /// <summary>
        /// Source Filename + dir for the copy cmd
        /// </summary>
        private string sourceFile;

        /// <summary>
        /// Destination Filename + dir for the copy cmd
        /// </summary>
        private string destinationFile;

        /// <summary>
        /// Thread for the file copy work
        /// </summary>
        private Thread copyFileThread;

        /// <summary>
        /// Currently not used.
        /// </summary>
        private bool stopCopyFileThread = false;

        /// <summary>
        /// 
        /// </summary>
        private CopyFinishedEventArgs finishedEventArgs = new CopyFinishedEventArgs();

        /// <summary>
        /// 
        /// </summary>
        private CopyStateChangedEventArgs changedEventArgs = new CopyStateChangedEventArgs();

        #endregion

        /// <summary>
        /// Empty c onstructor
        /// </summary>
        public CopyMaster()
        {

        }

        #region Directory copy

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="destinationDirectory"></param>
        public bool CopyDirectoryAsynch(string sourceDirectory, string destinationDirectory)
        {
            finishedEventArgs.destinationDir = destinationDirectory;
            finishedEventArgs.sourceDir = sourceDirectory;

            bool _retVal = false;
            if (CheckThreadIsAlreadyRunning())
            {
                return _retVal;
            }

            // Set the directorys 
            this.destinationDir = destinationDirectory;
            this.sourceDir = sourceDirectory;


            // Check if Data is valid
            if (Directory.Exists(sourceDir))
            {
                // Start the Copying thread
                copyDirectoryThread = new Thread(CopyDirectoryThread);
                copyDirectoryThread.Start();
                _retVal = true;
            }
            else
            {
                finishedEventArgs.allFilesCopyedAndNoError = false;
                finishedEventArgs.notCopyedFiles = new List<string>();
                finishedEventArgs.notCopyedFiles.Add("Source Directory not found. No wile was copyed");
                finishedEventArgs.timeElapsed = 0;

                // Hit the event that we have finished without copying anything
                CopyFinished.Invoke(this, finishedEventArgs);
            }

            return _retVal;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="destinationDirectory"></param>
        /// <returns></returns>
        public bool CopyDirectorySynchron(string sourceDirectory, string destinationDirectory)
        {
            finishedEventArgs.destinationDir = destinationDirectory;
            finishedEventArgs.sourceDir = sourceDirectory;

            bool _retVal = false;
            if (CheckThreadIsAlreadyRunning())
            {
                return _retVal;
            }

            // Set the directorys 
            this.destinationDir = destinationDirectory;
            this.sourceDir = sourceDirectory;


            // Check if Data is valid
            if (Directory.Exists(sourceDir))
            {
                // Start the Copying thread
                CopyDirectoryThread();
                _retVal = true;
            }
            else
            {
                finishedEventArgs.allFilesCopyedAndNoError = false;
                finishedEventArgs.notCopyedFiles = new List<string>();
                finishedEventArgs.notCopyedFiles.Add("Source Directory not found. No wile was copyed");
                finishedEventArgs.timeElapsed = 0;

                // Hit the event that we have finished without copying anything
                CopyFinished.Invoke(this, finishedEventArgs);
            }

            return _retVal;
        }


        /// <summary>
        /// Copy function. Attention!
        /// </summary>
        private void CopyDirectoryThread()
        {
            // Reset the stop bit
            stopCopyDirectoryThread = false;

            // Get the overal amount of data
            //DirectoryInfo dir = new DirectoryInfo(sourceDir);

            // Start the elapsed time measurements
            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();

            // get the size of the source directory
            //GetDirectoryMegaByteSize(this.sourceDir, true);

            // Perform the Copy
            finishedEventArgs.allFilesCopyedAndNoError = true;
            copyDirectory(this.sourceDir, this.destinationDir, true);

            // get the size of the destination directory
            //GetDirectoryMegaByteSize(this.destinationDir, true);

            // After we are done send teh finished Event
            _stopwatch.Stop();
            finishedEventArgs.timeElapsed = _stopwatch.ElapsedMilliseconds;

            if (stopCopyDirectoryThread)
            {
                finishedEventArgs.allFilesCopyedAndNoError = false;
            }

            // Hit the event that we have finished without copying anything
            CopyFinished.Invoke(this, finishedEventArgs);

        }


        /// <summary>
        /// Function to copy folders
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="destinationDirectory"></param>
        /// <param name="copySubDirs"></param>
        public void copyDirectory(string sourceDirectory, string destinationDirectory, bool copySubDirs)
        {
            // Check if we need to stop
            if (stopCopyDirectoryThread)
            {
                return;
            }

            // Get the subdirectories for the specified directory.
            DirectoryInfo _dir = new DirectoryInfo(sourceDirectory);
            DirectoryInfo[] _dirs;


            if (!_dir.Exists)
            {
                finishedEventArgs.notCopyedFiles.Add("Source directory does not exist or could not be found: " + sourceDirectory);
                return;
            }

            try
            {
                _dirs = _dir.GetDirectories();
            }
            catch
            {
                finishedEventArgs.notCopyedFiles.Add("Source directory does not exist or could not be found: " + sourceDirectory);
                return;
            }


            // If the destination directory doesn't exist, create it.
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = _dir.GetFiles();
            foreach (FileInfo _file in files)
            {
                StringBuilder _msg = new StringBuilder();
                string _copyPath = Path.Combine(destinationDirectory, _file.Name);

                _msg.Append("Copying File: " + _file.Name + " ... ");
                WriteDebugMsg(_msg.ToString(), 2);

                copyFile(Path.Combine(_file.DirectoryName, _file.Name), _copyPath, true);
                
                _msg = new StringBuilder("Done");
                WriteLineDebugMsg(_msg.ToString(), 2);

                // Check if we need to stop
                if (stopCopyDirectoryThread)
                {
                    return;
                }

            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo _subdir in _dirs)
                {
                    string temppath = Path.Combine(destinationDirectory, _subdir.Name);
                    copyDirectory(_subdir.FullName, temppath, copySubDirs);

                    // Check if we need to stop
                    if (stopCopyDirectoryThread)
                    {
                        return;
                    }
                }
            }
        }


        /// <summary>
        /// Function to stop the copy process
        /// </summary>
        public void stopCopyDirectory()
        {
            this.stopCopyDirectoryThread = true;
        }

        #endregion

        #region File Copy

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        public void CopyFileAsynch(string source, string destination)
        {
            if (CheckThreadIsAlreadyRunning())
            {
                return;
            }

            this.sourceFile = source;
            this.destinationFile = destination;

            finishedEventArgs.destinationDir = destination;
            finishedEventArgs.sourceDir = source;

            // Check if Data is valid
            if (File.Exists(this.sourceFile))
            {
                // Start the Copying thread
                copyFileThread = new Thread(CopyFileThread);
                copyFileThread.Start();

            }
            else
            {

                finishedEventArgs.allFilesCopyedAndNoError = false;
                finishedEventArgs.notCopyedFiles = new List<string>();
                finishedEventArgs.notCopyedFiles.Add("Source Directory not found. No wile was copyed");
                finishedEventArgs.timeElapsed = 0;

                // Hit the event that we have finished without copying anything
                CopyFinished.Invoke(this, finishedEventArgs);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        private void CopyFileThread()
        {
            // Reset the stop bit
            stopCopyFileThread = false;

            // Start measuring the leapsed time
            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();

            finishedEventArgs.allFilesCopyedAndNoError = true;  // Preset the Args with true. Will be set to false if an error occured
            copyFile(this.sourceFile, this.destinationFile, true);

            // Safe the elapsed Time to the event argument
            _stopwatch.Stop();
            finishedEventArgs.timeElapsed = _stopwatch.ElapsedMilliseconds;
            CopyFinished.Invoke(this, finishedEventArgs);   // Hit the event that we have finished without copying anything

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourcefileName"></param>
        /// <param name="destinationFileName"></param>
        /// <param name="overwrite"></param>
        public bool copyFile(string sourcefileName, string destinationFileName, bool overwrite)
        {
            bool _retVal = false;

            try
            {
                File.Copy(sourcefileName, destinationFileName, true);
                _retVal = true;
            }
            catch (Exception err)
            {
                StringBuilder _msg = new StringBuilder();
                _msg.Append("Bei dem kopieren ist ein fehler aufgetreten.");
                _msg.Append(err);
                WriteLineDebugMsg(_msg.ToString(), 2);

                finishedEventArgs.allFilesCopyedAndNoError = false;
                finishedEventArgs.notCopyedFiles = new List<string>();
                finishedEventArgs.notCopyedFiles.Add("File: " + sourcefileName + " was not copyed");
            }

            return _retVal;
        }

        #endregion

        #region Directory size functions

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="printResults"></param>
        /// <returns></returns>
        public float GetDirectoryMegaByteSize(string directory, bool printResults)
        {

            float _directorySize = GetDirectoryMegaByteSize(directory);

            // Print out the Result if requested
            if (printResults)
            {
                WriteLineDebugMsg("Directory size of: " + directory + " is: " + _directorySize.ToString() + " MB", 1);
            }

            return _directorySize;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="directory"></param>
        /// <returns></returns>
        public static float GetDirectoryMegaByteSize(string directory)
        {
            return (float)GetDirectoryByteSize(directory) / (1024 * 1024);
        }

        /// <summary>
        /// Returns the Directory Size in Bytes
        /// </summary>
        /// <param name="directory"></param>
        /// <returns>returns the Directory Size in Byte</returns>
        public static long GetDirectoryByteSize(string directory)
        {
            long _directorySize = 0;

            try
            {

                // Get the subdirectories for the specified directory.
                DirectoryInfo _dir = new DirectoryInfo(directory);
                DirectoryInfo[] _dirs;

                try
                {
                    _dirs = _dir.GetDirectories();
                }
                catch
                {
                    throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + directory);
                }

                // Get array of all file names.
                string[] _fileCollection = Directory.GetFiles(directory, "*.*");

                // Calculate total bytes of all files in a loop.
                foreach (string _file in _fileCollection)
                {
                    // Use FileInfo to get length of each file.
                    FileInfo _fileInfo = new FileInfo(_file);
                    _directorySize += _fileInfo.Length;
                }


                // If copying subdirectories, copy them and their contents to new location.
                if (true)
                {
                    foreach (DirectoryInfo _subdir in _dirs)
                    {
                        _directorySize += GetDirectoryByteSize(Path.Combine(directory, _subdir.Name));
                    }
                }


            }
            catch
            {
                // Do nothing
            }
            // Return total size in Mb
            return _directorySize;
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckThreadIsAlreadyRunning()
        {
            bool _retVal = false;

            if (copyDirectoryThread != null)
            {
                if (copyDirectoryThread.IsAlive)
                {
                    _retVal = true;
                }
            }

            if (copyFileThread != null)
            {
                if (copyFileThread.IsAlive)
                {
                    _retVal = true;
                }
            }

            return _retVal;
        }

        #region Debug msg Outputter

        /// <summary>
        /// Write the debug msg to the Console according to the current debug level
        /// </summary>
        /// <param name="msg">Debug msg</param>
        /// <param name="debugLvl">Debug lvl</param>
        private void WriteDebugMsg(string msg, int debugLvl)
        {
            if (debugLevel >= debugLvl && debugLevel < 3)
            {
                Console.Write(msg);
            }
            else if (debugLevel == 3)
            {
                CopyStateChanged.Invoke(this, new CopyStateChangedEventArgs { currentFile = msg, overallStatePercentage = "0", transferRate = 0 });
            }
        }


        /// <summary>
        /// Write the debug msg to the Console according to the current debug level
        /// </summary>
        /// <param name="msg">Debug msg</param>
        /// <param name="debugLvl">Debug lvl</param>
        private void WriteLineDebugMsg(string msg, int debugLvl)
        {
            if (debugLevel >= debugLvl && debugLevel < 3)
            {
                Console.WriteLine(msg);
            }
            else if (debugLevel == 3)
            {
                CopyStateChanged.Invoke(this, new CopyStateChangedEventArgs { currentFile = msg + "\n", overallStatePercentage = "0", transferRate = 0 });
            }
        }

        #endregion


    }

}
