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
    public partial class frmNewSystem : Form
    {
        webdbDataSet dataset;
        private int myVar;

        public int MyProperty
        {
            get { return myVar; }
            set { myVar = value; }
        }     
        public frmNewSystem()
        {
            InitializeComponent();
        }
        public frmNewSystem(ref webdbDataSet ds)
        {
            dataset = ds;
        }
    }
}
