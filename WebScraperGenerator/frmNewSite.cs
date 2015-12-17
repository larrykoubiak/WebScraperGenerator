using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebScraperGenerator
{
    public partial class frmNewSite : Form
    {
        private string strName;
        private string strURL;

        public string URL
        {
            get { return strURL; }
            set { strURL = value; }
        }

        public string Name
        {
            get { return strName; }
            set { strName = value; }
        }

        public frmNewSite()
        {
            InitializeComponent();
        }

        private void txtSiteName_TextChanged(object sender, EventArgs e)
        {
            strName = txtSiteName.Text;
        }

        private void txtSiteURL_TextChanged(object sender, EventArgs e)
        {
            strURL = txtSiteURL.Text;
        }
    }
}
