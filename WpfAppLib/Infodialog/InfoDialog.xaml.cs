﻿using System;
using System.Windows;

namespace WpfAppLib.Infodialog
{

    /// <summary>
    /// Interaktionslogik für InfoDialog.xaml
    /// </summary>
    public partial class InfoDialog : Window
    {

        /// <summary>
        /// Start Position of the Window
        /// </summary>
        private Point windowStartPosistion;

        private string VersionNumber;

        /// <summary>
        /// Info Dialog Constructor
        /// </summary>
        public InfoDialog(Point position, string versionNumber, string historyText, string companyName, string author, string eMailAdress, string language)
        {
            windowStartPosistion = position;
            InitializeComponent();

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
