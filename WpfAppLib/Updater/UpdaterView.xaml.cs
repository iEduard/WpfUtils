using System.Windows;
using System.Windows.Media.Imaging;

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


        #region public variables

        /// <summary>
        /// Check for updates button text
        /// </summary>
        public string CheckUpdatesButtonText 
        {
            get
            {
                return viewModel.CheckUpdatesButtonText;
            }
            set
            {
                viewModel.CheckUpdatesButtonText = value;
            }
        }

        /// <summary>
        /// Textfield of the update button text
        /// </summary>
        public string UpdateButtonText
        {
            get 
            {
                return viewModel.UpdateButtonText;
            }
            set
            {
                viewModel.UpdateButtonText = value;
            }

        }

        /// <summary>
        /// Cancel Button text
        /// </summary>
        public string CancelButtonText
        {
            get
            {
                return viewModel.CancelButtonText;
            }
            set
            {
                viewModel.CancelButtonText = value;
            }
        }


        /// <summary>
        /// Local path label text
        /// </summary>
        public string LocalPathLabelText
        {
            get
            {
                return viewModel.LocalPathLabelText;
            }
            set
            {
                viewModel.LocalPathLabelText = value;
            }

        }

        /// <summary>
        /// Local version label text
        /// </summary>
        public string LocalVersionLabelText
        {
            get
            {
                return viewModel.LocalVersionLabelText;
            }
            set
            {
                viewModel.LocalVersionLabelText = value;
            }

        }

        /// <summary>
        /// Remote path label text
        /// </summary>
        public string RemotePathLabelText
        {
            get
            {
                return viewModel.RemotePathLabelText;
            }
            set
            {
                viewModel.RemotePathLabelText = value;
            }

        }

        /// <summary>
        /// Remote version label text
        /// </summary>
        public string RemoteVersionLabelText
        {
            get
            {
                return viewModel.RemoteVersionLabelText;
            }
            set
            {
                viewModel.RemoteVersionLabelText = value;
            }
        }


        #endregion

        /// <summary>
        /// Update object with settings. Used if the running application is the only application to be updated without an settings file
        /// </summary>
        /// <param name="position">position where the pop up window should be displayed</param>
        /// <param name="updaterSettings"></param>
        public UpdaterView(Point position, updaterSettingsData updaterSettings, BitmapImage windowIcon)
        {

            this.windowStartPosistion = position;

            InitializeComponent();
            viewModel = new UpdaterViewModel(updaterSettings);
            this.DataContext = viewModel;
            
            if (windowIcon != null)
            {
                this.Icon = windowIcon;
            }
            else
            {
                // Get the icon file from the resources
                //var _windowIcon = new BitmapImage();
                //using (Stream stream = Assembly.Load(this.Name).GetManifestResourceStream("WpfAppLib.Resources.info.png"))
                //{
                //    _windowIcon.BeginInit();
                //    _windowIcon.StreamSource = stream;
                //    _windowIcon.CacheOption = BitmapCacheOption.OnLoad;
                //    _windowIcon.EndInit();
                //    _windowIcon.Freeze();
                //}

            }



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
