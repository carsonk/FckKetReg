using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace FckKetReg
{
    /// <summary>
    /// Interaction logic for BrowserWindow.xaml
    /// </summary>
    public partial class BrowserWindow : Window
    {
        public BrowserWindow()
        {
            InitializeComponent();

            // Hack to prevent script errors from showing.
            // May not always work, as it kind of abuses a bug.
            try {
                dynamic activeX = CurrentPageBrowser.GetType().InvokeMember("ActiveXInstance",
                        BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.NonPublic,
                        null, this.CurrentPageBrowser, new object[] { });
                activeX.Silent = true;
            } catch
            {
                // Failed.
            }
        }

        public void SetPage(String htmlText)
        {
            htmlText = htmlText.Replace("<script", "<notatag");
            CurrentPageBrowser.NavigateToString(htmlText);
        }
    }
}
