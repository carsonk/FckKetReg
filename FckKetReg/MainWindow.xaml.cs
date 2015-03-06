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
using Xceed.Wpf.Toolkit;

namespace FckKetReg
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private BrowserWindow _browserWindow;
        private RegistrationScheduler _registrationScheduler;
        private RequestManager _requestManager;

        public MainWindow()
        {
            InitializeComponent();
            _browserWindow = new BrowserWindow();
            _browserWindow.Show();
            String browserInitValue = "<html><body></body></html>";
            _browserWindow.SetPage(browserInitValue);
        }

        private void CheckLogin(object sender, RoutedEventArgs e)
        {
            RequestManager rm;
            try
            {
                InputtedCreds creds = GetInputtedCreds();
                rm = new RequestManager();
                rm.SetCreds(creds.username, creds.password, creds.regPin, creds.term, creds.CRNs);
            }
            catch(ArgumentException) { LogToOutput("[-] Error: Invalid input."); return; }

            if (rm.AttemptLogin())
            {
                rm.AccessRegistrationPage();
            }

            _browserWindow.SetPage(rm.CurrentHTML);
            LogToOutput(rm.Output);
        }

        private void FireRegistration(object sender, RoutedEventArgs e)
        {
            RequestManager rm;
            try
            {
                InputtedCreds creds = GetInputtedCreds();
                rm = new RequestManager();
                rm.SetCreds(creds.username, creds.password, creds.regPin, creds.term, creds.CRNs);
            }
            catch (ArgumentException) { LogToOutput("[-] Error: Invalid input."); return; }

            bool accessRegistrationSuccess = false;

            if (rm.AttemptLogin())
            {
                accessRegistrationSuccess = rm.AccessRegistrationPage();
            }

            if(accessRegistrationSuccess)
            {
                rm.AddClasses();
            }

            _browserWindow.SetPage(rm.CurrentHTML);
            LogToOutput(rm.Output);
        }

        private void ScheduleRegistration(object sender, RoutedEventArgs e)
        {
            LogToOutput("[-] Scheduling registration.");
            InputtedCreds creds = GetInputtedCreds();

            try
            {
                _requestManager = new RequestManager();
                _requestManager.SetCreds(creds.username, creds.password, creds.regPin, creds.term, creds.CRNs);
            } catch (ArgumentException exception)
            {
                LogToOutput("[-] Error: Invalid argument passed. (" + exception.Message + ")");
                return;
            }
            
            DateTime inputTime = Convert.ToDateTime(scheduledTime.Text);

            _registrationScheduler = new RegistrationScheduler(creds.username, creds.password, creds.regPin, creds.term, creds.CRNs);
            _registrationScheduler.PreviewWindow = _browserWindow;
            _registrationScheduler.ScheduleRegistration(inputTime);
        }

        private InputtedCreds GetInputtedCreds()
        {
            InputtedCreds ofTheJedi = new InputtedCreds();
            ofTheJedi.username = userID.Text;
            ofTheJedi.password = password.Password;
            ofTheJedi.regPin = regPin.Text;
            ofTheJedi.term = term.Text;

            ofTheJedi.CRNs = new Queue<string>();
            ofTheJedi.CRNs.Enqueue(crn1.Text);
            ofTheJedi.CRNs.Enqueue(crn2.Text);
            ofTheJedi.CRNs.Enqueue(crn3.Text);
            ofTheJedi.CRNs.Enqueue(crn4.Text);
            ofTheJedi.CRNs.Enqueue(crn5.Text);
            ofTheJedi.CRNs.Enqueue(crn6.Text);
            ofTheJedi.CRNs.Enqueue(crn7.Text);
            ofTheJedi.CRNs.Enqueue(crn8.Text);
            ofTheJedi.CRNs.Enqueue(crn9.Text);
            ofTheJedi.CRNs.Enqueue(crn10.Text);

            return ofTheJedi; // Lol.
        }
       
        private void LogToOutput(String addToOutput, bool clear = false)
        {
            if(clear)
            {
                output.Text = addToOutput + "\n";
            }
            else
            {
                output.Text += addToOutput + "\n";
            }
        }

        private struct InputtedCreds
        {
            public string username;
            public string password;
            public string regPin;
            public string term;
            public Queue<string> CRNs;
        }
    }
}
