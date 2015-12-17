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
    public partial class frmMain : Form
    {
        HtmlDocument doc;
        HtmlAgilityPack.HtmlDocument haDocument;
        webdbDataSetTableAdapters.TableAdapterManager manager;
        public frmMain()
        {
            InitializeComponent();
            manager = new webdbDataSetTableAdapters.TableAdapterManager();
            manager.tblSitesTableAdapter = new webdbDataSetTableAdapters.tblSitesTableAdapter();
            manager.tblSystemsTableAdapter = new webdbDataSetTableAdapters.tblSystemsTableAdapter();
            manager.tblSystemsGameDBTableAdapter = new webdbDataSetTableAdapters.tblSystemsGameDBTableAdapter();
            manager.tblSitesTableAdapter.Fill(webdbDataSet1.tblSites);
            manager.tblSystemsTableAdapter.Fill(webdbDataSet1.tblSystems);
            manager.tblSystemsGameDBTableAdapter.Fill(webdbDataSet1.tblSystemsGameDB);
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            if (txtURL.Text.Length > 0)
            {
                webBrowser1.Navigate(txtURL.Text);
            }

        }

        private void htmlDocument_click(object sender, HtmlElementEventArgs e)
        {
            HtmlElement element = this.webBrowser1.Document.GetElementFromPoint(e.ClientMousePosition);

            MessageBox.Show(getXpath(element));
        }
        private string getXpath(HtmlElement elem)
        {
            string savedId = elem.Id;
            string uniqueId = Guid.NewGuid().ToString();
            string xpath = "";
            elem.Id = uniqueId;
            haDocument = new HtmlAgilityPack.HtmlDocument();
            haDocument.LoadHtml(elem.Document.GetElementsByTagName("html")[0].OuterHtml);
            elem.Id = savedId;
            HtmlAgilityPack.HtmlNode node = haDocument.GetElementbyId(uniqueId);
            xpath = node.Name;
            HtmlAgilityPack.HtmlNode parentnode = node.ParentNode;
            while(parentnode.Id=="" && parentnode.ParentNode != null)
            {
                if (parentnode.Attributes.SingleOrDefault(x => x.Name == "class") == null)
                {
                    HtmlAgilityPack.HtmlNodeCollection siblingnodes = parentnode.SelectNodes("preceding-sibling::*");
                    if (siblingnodes != null)
                    {
                        xpath = parentnode.Name + "[" + siblingnodes.Count + "]" + "/" + xpath;
                    }
                    else
                    {
                        xpath = parentnode.Name + "/" + xpath;
                    }
                }
                else
                {
                    xpath = parentnode.Name + " [class=\"" + parentnode.Attributes["class"].Value + "\"]" + "/" + xpath;
                }
                parentnode = parentnode.ParentNode;
            }
            if(parentnode.Id!="")
            {
                xpath = parentnode.Name + " [id=\"" + parentnode.Id + "\"]" + "/" + xpath; 
            }
            return xpath;
        }
        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            doc = webBrowser1.Document;
            doc.MouseUp += new HtmlElementEventHandler(this.htmlDocument_click);
        }

        private void txtURL_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(e.KeyChar == (char)Keys.Enter)
            {
                if (txtURL.Text.Length > 0)
                {
                    webBrowser1.Navigate(txtURL.Text);
                }
            }
        }

        private void newSiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmNewSite dlgNewSite = new frmNewSite();
            if(dlgNewSite.ShowDialog()==DialogResult.OK)
            {
                webdbDataSet.tblSitesRow siterow = webdbDataSet1.tblSites.AddtblSitesRow(dlgNewSite.Name, dlgNewSite.URL);               
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            manager.tblSitesTableAdapter.Update(webdbDataSet1.tblSites);
        }

        private void addSystemToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmNewSystem dlgSystem = new frmNewSystem(ref webdbDataSet1);
            if(dlgSystem.ShowDialog()==DialogResult.OK)
            {
            }
        }
    }
}
