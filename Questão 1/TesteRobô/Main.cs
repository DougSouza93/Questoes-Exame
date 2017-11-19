using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RobotTest
{
    public partial class TestForm : Form
    {
        List<SearchResult> SearchResultsData = new List<SearchResult>();
        string bingUrl = "http://www.bing.com/";

        public TestForm()
        {
            InitializeComponent();
        }

        private void TestBrowser_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if(TestBrowser.Document.Url.Equals("http://www.bing.com/"))
            {
                SearchForABSCard();
            }
            else if(TestBrowser.Document.Url.ToString().Contains("http://www.bing.com/search?q="))
            {
                var resultsElements = TestBrowser.Document.GetElementById("b_results").Children;

                var newSearchResult = new SearchResult(); // A variavel precisou sair do escopo que ela estava, para poder instanciar na chamada do procedimento

                foreach (HtmlElement result in resultsElements)
                {

                    if (result.OuterHtml.Contains("b_algo"))
                    {
                        var title = result.Children[0].InnerText.Trim();
                        
                        if (title == "DM Sistemas | DM Sistemas") //Title estava rodando sem sentido, colocado o if para que fosse um marco onde ele deveria parar
                        {                            
                            newSearchResult.Title = title;                            
                            newSearchResult.Url = result.Children[1].InnerText.Trim().Substring(0, 21);
                            newSearchResult.Description = result.Children[1].InnerText.Substring(21);   
                            /* Em URL e Description as informações estavam juntas na mesma children, usei um contador para
                             * poder separar os campos, limitando a do URL e o restante das informações indo para o Descritpion
                             */                    
                        }
                    }
                }

                WriteDataInCsv(newSearchResult);
            }
        }

        public void SearchForABSCard()
        {
            var searchBox = TestBrowser.Document.GetElementById("sb_form_q");
            searchBox.InnerText = "ABSCard Gestão de Benefícios";

            var searchButton = TestBrowser.Document.GetElementById("sb_form_go");
            searchButton.InvokeMember("click");
        }

        public void WriteDataInCsv(SearchResult result)
        {
            // Não precisava de foreach aqui, uma vez que esta como paramentro é um Objeto, não precisava rodar o Objeto
            var sw = new StreamWriter(Application.StartupPath + "\\SearchResults.csv");
            {
                sw.WriteLine(result.Title);
                sw.WriteLine(result.Url);
                sw.WriteLine(result.Description);
                sw.Close();
            }
            sw.Dispose();

            Process.Start("notepad.exe", Application.StartupPath + "\\SearchResults.csv");
            Close();
        }
    }
}
