using FckKetReg.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FckKetReg
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void CheckLogin(object sender, RoutedEventArgs e)
        {
            string inputtedUsername = userID.Text;
            string inputtedPassword = password.Password;
            RequestManager rm = new RequestManager(inputtedUsername, inputtedPassword);
            rm.CurrentTerm = new Term(term.Text);
            rm.RegistrationPIN = regPin.Text;

            if (rm.AttemptLogin())
            {
                rm.AccessRegistrationPage();
            }

            output.Text = rm.Output;
        }
    }
}
