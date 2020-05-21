using System.IO;
using System.Reflection;
using System.Windows;
using WpfAppLib.Infodialog;
using WpfAppLib.Updater;
using WpfAppLib.MultiUpdater;

namespace WpfExampleApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Applicatoion info example
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btInfo_Click(object sender, RoutedEventArgs e)
        {
            InfoDialog _myInfoDialo = new InfoDialog(new Point(), "1.0.0.0", "Created", "Company", "Eduard Schmidt", "esc@someProvider.org", "German");
            _myInfoDialo.Show();
        }

        /// <summary>
        /// Application update example
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btUpdater_Click(object sender, RoutedEventArgs e)
        {

            //Create new update settings object
            // Fill in the settings you need to reference to your application update path.
            WpfAppLib.Updater.updaterSettingsData _updaterSettings = new WpfAppLib.Updater.updaterSettingsData
            {
                appFileName = "WpfExampleApp.exe",
                appLocalPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                appName = Assembly.GetExecutingAssembly().GetName().Name,
                appServerPath = @"U:\tmp\WpfExampleApp\bin\Debug\",
                settingsFileName = "Settings.xml",
                settingsLocalPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                settingsServerPath = @"U:\tmp\WpfExampleApp\bin\Debug\"
            };

            // Create the updater object
            UpdaterView _myUpdateView = new UpdaterView(new Point(), _updaterSettings);

            // Show the updater object
            _myUpdateView.Show();

        }

        /// <summary>
        /// Application multi updater example
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btMultiUpdater_Click(object sender, RoutedEventArgs e)
        {
            // Setup some strings  for the update mechanism
            string _settingsFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Settings.xml";
            string _applicationFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string _applicationName = Assembly.GetExecutingAssembly().GetName().Name;

            // Create a new multi updater view
            MultiUpdaterView _myUpdateView = new MultiUpdaterView(new Point(), _settingsFilePath, _applicationFilePath, "WPF util example");

            // Show the multi updater view
            _myUpdateView.Show();
        }
    }
}
