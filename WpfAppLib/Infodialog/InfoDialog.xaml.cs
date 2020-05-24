using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WpfAppLib.Infodialog
{

    /// <summary>
    /// Interaktionslogik für InfoDialog.xaml
    /// </summary>
    public partial class InfoDialog : Window
    {

        #region private variables

        private Point windowStartPosistion; // Start Position of the Window
        private string VersionNumber;
        private InfoDialogViewModel viewModel;


        #endregion


        #region public variables


        /// <summary>
        /// Set or get the Window title
        /// </summary>
        public string WindowTitleText
        {
            get
            {
                return viewModel.WindowTitleText;
            }
            set
            {
                viewModel.WindowTitleText = value;
            }
        }


        public string VersionLabelText
        {
            get
            {
                return viewModel.VersionLabelText;
            }
            set
            {
                viewModel.VersionLabelText = value;
            }

        }

        public string CompanyLabelText
        {
            get
            {
                return viewModel.CompanyLabelText;
            }
            set
            {
                viewModel.CompanyLabelText = value;
            }

        }

        public string AuthorLabelText
        {
            get
            {
                return viewModel.AuthorLabelText;
            }
            set
            {
                viewModel.AuthorLabelText = value;
            }

        }

        public string EMailLabelText
        {
            get
            {
                return viewModel.EMailLabelText;
            }
            set
            {
                viewModel.EMailLabelText = value;
            }

        }

        public string InfoLabelText
        {
            get
            {
                return viewModel.InfoLabelText;
            }
            set
            {
                viewModel.InfoLabelText = value;
            }

        }

        #endregion



        /// <summary>
        /// Create a new infodialog object
        /// </summary>
        /// <param name="position">Postion of the infodialog to be shown first</param>
        /// <param name="versionNumber">Version number to be shown</param>
        /// <param name="historyText">History tet in an scrollable textbox </param>
        /// <param name="companyName">Company name to be shown</param>
        /// <param name="author">Author of the programm to be shown</param>
        /// <param name="eMailAdress">Mail address to be shown</param>
        /// <param name="language">currently unuesed</param>
        public InfoDialog(Point position, string versionNumber, string historyText, string companyName, string author, string eMailAdress)
        {
            windowStartPosistion = position;
            viewModel = new InfoDialogViewModel();


            InitializeComponent();
            this.DataContext = viewModel;

            VersionNumber = versionNumber;

            // Init the labels
            lVersion.Content = VersionNumber;
            lCompany.Content = companyName;
            lAuthor.Content = author;
            lemailAdress.Content = eMailAdress;

            // Define the Text in the Info Field
            tbVersionHistory.AppendText(historyText);

            // Set the Textbox to read only
            tbVersionHistory.IsReadOnly = true;

        }



        /// <summary>
        /// Create a new infodialog object
        /// </summary>
        /// <param name="position">Postion of the infodialog to be shown first</param>
        /// <param name="versionNumber">Version number to be shown</param>
        /// <param name="historyText">History tet in an scrollable textbox </param>
        /// <param name="companyName">Company name to be shown</param>
        /// <param name="author">Author of the programm to be shown</param>
        /// <param name="eMailAdress">Mail address to be shown</param>
        /// <param name="language">currently unuesed</param>
        public InfoDialog(Point position, string versionNumber, string historyText, string companyName, string author, string eMailAdress, BitmapImage windowIcon)
        {
            windowStartPosistion = position;
            viewModel = new InfoDialogViewModel();


            InitializeComponent();
            this.DataContext = viewModel;

            VersionNumber = versionNumber;

            // Init the labels
            lVersion.Content = VersionNumber;
            lCompany.Content = companyName;
            lAuthor.Content = author;
            lemailAdress.Content = eMailAdress;

            // Define the Text in the Info Field
            tbVersionHistory.AppendText(historyText);

            // Set the Textbox to read only
            tbVersionHistory.IsReadOnly = true;

            // Change the icon if teh window icon is transmitted            
            if (windowIcon != null)
            {
                this.Icon = windowIcon;
            }
        }


        /// <summary>
        /// Close the Info Dialog
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bClose_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Open up the Default Email Client to send a Bug Report
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void emailLink_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Diagnostics.Process proc = new System.Diagnostics.Process();
                proc.StartInfo.FileName = "mailto:ing.eduard.schmidt@gmail.com?subject=Connecty Support&body=Connecty Version=" + VersionNumber;
                proc.Start();
            }
            catch
            {
                // The EMail link could not be opend..
            }


        }

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

    
    }
}
