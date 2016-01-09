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
        #region "Members"

        HtmlDocument doc;
        HtmlAgilityPack.HtmlDocument haDocument;
        webdbDataSetTableAdapters.TableAdapterManager manager;

        #endregion

        #region "Constructor"

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

        #endregion

        #region "Methods"

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
            /*xpath = node.Name;
            HtmlAgilityPack.HtmlNode parentnode = node.ParentNode;
            while (parentnode.Id == "" && parentnode.ParentNode != null)
            {
                if (parentnode.Attributes.SingleOrDefault(x => x.Name == "class") == null)
                {
                    HtmlAgilityPack.HtmlNodeCollection siblingnodes = parentnode.SelectNodes("preceding-sibling::*");
                    if (siblingnodes != null)
                    {
                        xpath = parentnode.Name + "[" + (siblingnodes.Count+1) + "]" + "/" + xpath;
                    }
                    else
                    {
                xpath = parentnode.Name + "/" + xpath;
                    }
                }
                else
                {
                    xpath = parentnode.Name + @"[class=""" + parentnode.Attributes["class"].Value + @"""]/" + xpath;
                }
                parentnode = parentnode.ParentNode;
            }
            if (parentnode.Id != "")
            {
                xpath = @"//*" + @"[@id=""" + parentnode.Id + @"""]/" + xpath;
            }
            */
            xpath = node.XPath;
            return xpath;
        }

        private DataTable getTable(HtmlElement elem)
        {
            DataTable dt = new DataTable();
            haDocument = new HtmlAgilityPack.HtmlDocument();
            haDocument.LoadHtml(elem.Document.GetElementsByTagName("html")[0].OuterHtml);
            string xpath = getXpath(elem);
            HtmlAgilityPack.HtmlNode mainnode = haDocument.DocumentNode.SelectSingleNode(xpath);
            HtmlAgilityPack.HtmlNode tablenode = mainnode;
            while (tablenode.Name != "table" && tablenode.ParentNode != null)
                tablenode = tablenode.ParentNode;
            if(tablenode.ParentNode == null)
            {
                return null;
            }
            else
            {
                HtmlAgilityPack.HtmlNodeCollection headernodes = tablenode.SelectNodes("./thead/tr/*|*/*/th");
                HtmlAgilityPack.HtmlNodeCollection rownodes = tablenode.SelectNodes("./tbody/tr[td]");
                for(int positionheader = 0;positionheader< headernodes.Count; positionheader++)
                {
                    //scan table for links
                    bool islink = false;
                    foreach (HtmlAgilityPack.HtmlNode rownode in rownodes)
                    {
                        if (rownode.SelectNodes("./td")[positionheader].SelectSingleNode(".//a") != null)
                        {
                            islink = true;
                        }
                    }
                    if(islink)
                    {
                        dt.Columns.Add(headernodes[positionheader].InnerText + "_Text", typeof(string));
                        dt.Columns.Add(headernodes[positionheader].InnerText + "_URL", typeof(string));
                    }
                    else
                    {
                        dt.Columns.Add(headernodes[positionheader].InnerText, typeof(string));
                    }
                }
                if (rownodes != null)
                {
                    foreach (HtmlAgilityPack.HtmlNode rownode in rownodes)
                    {
                        HtmlAgilityPack.HtmlNodeCollection colnodes = rownode.SelectNodes("./td");
                        if (colnodes != null)
                        {
                            DataRow dr = dt.NewRow();
                            int idxcol = 0;
                            foreach (HtmlAgilityPack.HtmlNode colnode in colnodes)
                            {
                                if (colnode.SelectSingleNode(".//a") == null)
                                {
                                    dr[idxcol++] = colnode.InnerText;
                                }
                                else
                                {
                                    dr[idxcol++] = colnode.SelectSingleNode(".//a").InnerText;
                                    dr[idxcol++] = colnode.SelectSingleNode(".//a").Attributes["href"].Value;
                                }
                            }
                            dt.Rows.Add(dr);
                        }
                    }
                }
                return dt;
            }
        }

        #endregion

        #region "Event Handlers"

        private void btnGo_Click(object sender, EventArgs e)
        {
            if (txtURL.Text.Length > 0)
            {
                webBrowser1.AllowNavigation = true;
                webBrowser1.Navigate(txtURL.Text);
            }

        }

        private void htmlDocument_click(object sender, HtmlElementEventArgs e)
        {
            HtmlElement element = this.webBrowser1.Document.GetElementFromPoint(e.ClientMousePosition);

            dgvResults.DataSource = getTable(element);
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            doc = webBrowser1.Document;
            doc.MouseUp += new HtmlElementEventHandler(this.htmlDocument_click);
            webBrowser1.AllowNavigation = false;
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

        #endregion

    }
}
