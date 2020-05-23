using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace WpfAppLib.Infodialog
{
    class InfoDialogViewModel : INotifyPropertyChanged
    {


        #region private variables

        private string windowTitleText = "Infobox";


        private string versionLabelText = "Version";
        private string companyLabelText = "Company";
        private string authorLabelText = "Author";
        private string eMailLabelText = "E-Mail";
        private string infoLabelText = "Info";

        #endregion


        #region public variables

        /// <summary>
        /// Set or get the Window title
        /// </summary>
        public string WindowTitleText
        {
            get
            {
                return windowTitleText;
            }
            set
            {
                windowTitleText = value;
                NotifyPropertyChanged("WindowTitleText");
            }
        }


        public string VersionLabelText
        {
            get
            {
                return versionLabelText + ": ";
            }
            set
            {
                versionLabelText = value;
                NotifyPropertyChanged("VersionLabelText");

            }

        }

        public string CompanyLabelText
        {
            get
            {
                return companyLabelText + ": ";
            }
            set
            {
                companyLabelText = value;
                NotifyPropertyChanged("CompanyLabelText");
            }

        }


        public string AuthorLabelText
        {
            get
            {
                return authorLabelText + ": ";
            }
            set
            {
                authorLabelText = value;
                NotifyPropertyChanged("AuthorLabelText");
            }

        }


        public string EMailLabelText
        {
            get
            {
                return eMailLabelText + ": ";
            }
            set
            {
                eMailLabelText = value;
                NotifyPropertyChanged("EMailLabelText");
            }

        }

        public string InfoLabelText
        {
            get
            {
                return infoLabelText;
            }
            set
            {
                infoLabelText = value;
                NotifyPropertyChanged("InfoLabelText");
            }

        }

        #endregion


        /// <summary>
        /// Constructor
        /// </summary>
        public InfoDialogViewModel()
        {

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
