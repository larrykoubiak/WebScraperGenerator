using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebScraperGenerator.Data_Classes
{
    class WebSite
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

    }
}
