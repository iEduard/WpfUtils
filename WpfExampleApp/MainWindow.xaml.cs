using System;
using System.IO;
using System.Reflection;
using System.Windows;
using WpfAppLib.Infodialog;
using WpfAppLib.Updater;

namespace WpfExampleApp
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void btInfo_Click(object sender, RoutedEventArgs e)
        {
            InfoDialog _myInfoDialo = new InfoDialog(new Point(), "1.0.0.0", "Created", "Company", "Eduard Schmidt", "esc@someProvider.org", "German");
            _myInfoDialo.Show();
        }

        private void btUpdater_Click(object sender, RoutedEventArgs e)
        {

            string _settingsFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\Settings.xml";
            string _applicationFilePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //Console.WriteLine(_applicationFilePath);

            string _applicationName = System.Reflection.Assembly.GetExecutingAssembly().GetName().Name;

            Console.WriteLine(_applicationName);

            UpdaterView _myUpdateView = new UpdaterView(new Point(), _settingsFilePath, _applicationFilePath, "WPF util example");
            _myUpdateView.Show();

        }
    }
}
