using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace WpfAppLib.CopyAndCompare
{
    #region Events

    public class CompareFinishedEventArgs : EventArgs
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
        public bool allFilesComparedAndNoDifferences { get; set; }

        /// <summary>
        /// List of all not found Files in the Destination dir
        /// </summary>
        public List<string> notFoundFiles { get; set; }

        /// <summary>
        /// List of all Files that should not exist
        /// </summary>
        public List<string> foundFilesThatShouldNotExist { get; set; }

        /// <summary>
        /// List of all not found Directory in the Destination Dir
        /// </summary>
        public List<string> notFoundDirectorys { get; set; }

        /// <summary>
        /// Found Directorys that should not exist in the Destination Dir
        /// </summary>
        public List<string> foundDirectorysThatShouldNotExist { get; set; }

        /// <summary>
        /// List of all Files that are not the same (Version, Size, Hash?)
        /// </summary>
        public List<string> differentFiles { get; set; }

        /// <summary>
        /// Source Dir for the compare operation
        /// </summary>
        public string sourceDir { get; set; }

        /// <summary>
        /// Destionation Dir for the compare operation
        /// </summary>
        public string destinationDir { get; set; }
    }

    public delegate void CompareFinishedEventHandler(object sender, CompareFinishedEventArgs e);

    #endregion

    public class Compare
    {

        #region Public Variables

        public string sourceDir { get; private set; }
        public string destinationDir { get; private set; }

        /// <summary>
        /// Defines in wich way the functions will write to the console output
        /// 0 = Write nothing to Console (default)
        /// 1 = Write ony Started and Finished to the console
        /// 2 = Write everything to the console
        /// </summary>
        public int debugLevel { get; set; }

        #endregion

        #region Threads

        private Thread compareDirectoryThread;

        #endregion

        #region Events

        public event CompareFinishedEventHandler CompareFinished;

        #endregion

        #region Local Variables

        /// <summary>
        /// 
        /// </summary>
        private CompareFinishedEventArgs finishedEventArgs = new CompareFinishedEventArgs();

        #endregion

        /// <summary>
        /// Constructor
        /// </summary>
        public Compare()
        {
            this.debugLevel = 0;
        }

        #region Directory compare

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="destinationDirectory"></param>
        public void CompareDirectoryAsynch(string sourceDirectory, string destinationDirectory)
        {
            finishedEventArgs.sourceDir = sourceDirectory;
            finishedEventArgs.destinationDir = destinationDirectory;

            if (CheckThreadIsAlreadyRunning())
            {
                return;
            }

            // Set the directorys 
            this.destinationDir = destinationDirectory;
            this.sourceDir = sourceDirectory;


            // Check if Data is valid
            if (Directory.Exists(sourceDir))
            {
                // Start the Copying thread
                compareDirectoryThread = new Thread(CompareDirectoryThread);
                compareDirectoryThread.Start();

            }
            else
            {
                finishedEventArgs.allFilesComparedAndNoDifferences = false;
                finishedEventArgs.notFoundDirectorys = new List<string>();
                finishedEventArgs.notFoundDirectorys.Add(sourceDir);
                finishedEventArgs.timeElapsed = 0;

                // Hit the event that we have finished without copying anything
                CompareFinished.Invoke(this, finishedEventArgs);
            }
        }

        /// <summary>
        /// Synchronous call of the Working method
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="destinationDirectory"></param>
        public void CompareDirectorySynch(string sourceDirectory, string destinationDirectory)
        {
            finishedEventArgs.sourceDir = sourceDirectory;
            finishedEventArgs.destinationDir = destinationDirectory;


            // Set the directorys 
            this.destinationDir = destinationDirectory;
            this.sourceDir = sourceDirectory;


            // Check if Data is valid
            if (Directory.Exists(sourceDir))
            {
                // Start the Copying thread
                CompareDirectoryThread();

            }
            else
            {
                finishedEventArgs.allFilesComparedAndNoDifferences = false;
                finishedEventArgs.notFoundDirectorys = new List<string>();
                finishedEventArgs.notFoundDirectorys.Add(sourceDir);
                finishedEventArgs.timeElapsed = 0;

                // Hit the event that we have finished without copying anything
                CompareFinished.Invoke(this, finishedEventArgs);
            }
        }

        /// <summary>
        /// Copy function. Attention!
        /// </summary>
        private void CompareDirectoryThread()
        {

            // Get the overal amount of data
            //DirectoryInfo dir = new DirectoryInfo(sourceDir);

            // Start the elapsed time measurements
            Stopwatch _stopwatch = new Stopwatch();
            _stopwatch.Start();

            // get the size of the source directory
            //GetDirectoryMegaByteSize(this.sourceDir, true);

            // Preset the Finished Arguments
            finishedEventArgs.allFilesComparedAndNoDifferences = true;
            finishedEventArgs.differentFiles = new List<string>();
            finishedEventArgs.foundDirectorysThatShouldNotExist = new List<string>();
            finishedEventArgs.foundFilesThatShouldNotExist = new List<string>();
            finishedEventArgs.notFoundDirectorys = new List<string>();
            finishedEventArgs.notFoundFiles = new List<string>();

            // Perform the Copy
            CompareDirectory(this.sourceDir, this.destinationDir, true);

            // get the size of the destination directory
            //GetDirectoryMegaByteSize(this.destinationDir, true);

            // After we are done send teh finished Event
            _stopwatch.Stop();
            finishedEventArgs.timeElapsed = _stopwatch.ElapsedMilliseconds;

            // Hit the event that we have finished without copying anything
            CompareFinished.Invoke(this, finishedEventArgs);

        }


        /// <summary>
        /// Function to copy folders
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="destinationDirectory"></param>
        /// <param name="copySubDirs"></param>
        public void CompareDirectory(string sourceDirectory, string destinationDirectory, bool compareSubDirs)
        {

            // Get the subdirectories for the specified directory.
            DirectoryInfo _srcDir = new DirectoryInfo(sourceDirectory);
            DirectoryInfo[] _srcDirs;

            // Get the subdirectories for the specified directory.
            DirectoryInfo _dstDir = new DirectoryInfo(destinationDirectory);


            if (!_srcDir.Exists)
            {
                finishedEventArgs.notFoundDirectorys.Add(sourceDirectory);
                finishedEventArgs.allFilesComparedAndNoDifferences = false;
                return;
            }
            else if (!_dstDir.Exists)
            {
                finishedEventArgs.notFoundDirectorys.Add(destinationDir);
                finishedEventArgs.allFilesComparedAndNoDifferences = false;
                return;
            }

            try
            {
                _srcDirs = _srcDir.GetDirectories();
            }
            catch
            {
                finishedEventArgs.notFoundDirectorys.Add("Source directory does not exist or could not be found: " + sourceDirectory);
                finishedEventArgs.allFilesComparedAndNoDifferences = false;
                return;
            }



            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = _srcDir.GetFiles();
            foreach (FileInfo _file in files)
            {

                string _comparePath = Path.Combine(destinationDirectory, _file.Name);

                // Check File Exists
                FileInfo _dstFile = new FileInfo(_comparePath);

                WriteLineDebugMsg("Compareing File: " + _file.Name + " ... ", 2);


                if (!_dstFile.Exists)
                {
                    if (finishedEventArgs.notFoundFiles != null)
                    {
                        finishedEventArgs.notFoundFiles.Add("File not Found: " + _dstFile.FullName);
                        finishedEventArgs.allFilesComparedAndNoDifferences = false;
                    }
                }

                WriteLineDebugMsg("Done", 2);

            }

            // If copying subdirectories, copy them and their contents to new location.
            if (compareSubDirs)
            {
                foreach (DirectoryInfo _subdir in _srcDirs)
                {
                    string temppath = Path.Combine(destinationDirectory, _subdir.Name);
                    CompareDirectory(_subdir.FullName, temppath, compareSubDirs);
                }
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public bool CheckThreadIsAlreadyRunning()
        {
            bool _retVal = false;

            if (compareDirectoryThread != null)
            {
                if (compareDirectoryThread.IsAlive)
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
            if (debugLevel >= debugLevel)
            {
                Console.Write(msg);
            }
        }


        /// <summary>
        /// Write the debug msg to the Console according to the current debug level
        /// </summary>
        /// <param name="msg">Debug msg</param>
        /// <param name="debugLvl">Debug lvl</param>
        private void WriteLineDebugMsg(string msg, int debugLvl)
        {
            if (debugLevel >= debugLvl)
            {
                Console.WriteLine(msg);
            }
        }

        #endregion


    }
}

