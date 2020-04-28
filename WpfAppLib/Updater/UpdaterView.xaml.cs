using System.Windows;


namespace WpfAppLib.Updater
{
    /// <summary>
    /// Interaktionslogik für Updater.xaml
    /// </summary>
    public partial class UpdaterView : Window
    {



        #region Private Variables


        /// <summary>
        /// Start Position of the Window
        /// </summary>
        private Point windowStartPosistion;

        /// <summary>
        /// The viev model
        /// </summary>
        private UpdaterViewModel viewModel;

        #endregion 

        public UpdaterView(Point position, string settingsPath, string localPath)
        {

            this.windowStartPosistion = position;

            InitializeComponent();
            viewModel = new UpdaterViewModel(settingsPath, localPath);
            this.DataContext = viewModel;

        }

        /// <summary>
        /// Updater constructor
        /// </summary>
        /// <param name="position">position where the pop up window should be displayed</param>
        /// <param name="settingsPath">path of the update configuration file</param>
        /// <param name="localPath">local path of the application</param>
        /// <param name="applicationName">name of the application called the lib. Only needed if this aopplication should also be updatable</param>
        public UpdaterView(Point position, string settingsPath, string localPath, string applicationName)
        {

            this.windowStartPosistion = position;

            InitializeComponent();
            viewModel = new UpdaterViewModel(settingsPath, localPath, applicationName);
            this.DataContext = viewModel;

        }



        #region UI events

        /// <summary>
        /// Window loaded Event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // Start the Window in the Center of the Screen
            this.Left = windowStartPosistion.X;
            this.Top = windowStartPosistion.Y;
        }

        /// <summary>
        /// Auto generating column event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void JobsDataGrid_AutoGeneratingColumn(object sender, System.Windows.Controls.DataGridAutoGeneratingColumnEventArgs args)
        {
            // Modify the header of the Name column.
            if (args.Column.Header.ToString() == "IsSelectedToDownload")
            {
                args.Column.Header = "Download";
            }

            if (args.Column.Header.ToString() == "ApplicationName")
            {
                args.Column.Header = "Application";
            }

            if (args.Column.Header.ToString() == "NewestVersionInstalled")
            {
                args.Column.Header = "Up to date";
            }

            if (args.Column.Header.ToString() == "LocalVersion")
            {
                args.Column.Header = "Local version";
            }

            if (args.Column.Header.ToString() == "LocalUrl")
            {
                args.Column.Header = "Local URL";
            }

            if (args.Column.Header.ToString() == "RemoteVersion")
            {
                args.Column.Header = "Remote version";
            }

            if (args.Column.Header.ToString() == "RemoteUrl")
            {
                args.Column.Header = "Remote URL";
            }

        }

        #endregion

        #region User Interaction

        /// <summary>
        /// Update button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.Update();
        }

        /// <summary>
        /// Cancel button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Check for updates button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckUpdatesButton_Click(object sender, RoutedEventArgs e)
        {
            viewModel.CheckForUpdates();
        }


        #endregion


    }
}
