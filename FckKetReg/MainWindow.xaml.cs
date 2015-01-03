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
                rm = new RequestManager(creds.username, creds.password, creds.regPin, creds.term);
            }
            catch(ArgumentException) { LogToOutput("[-] Error: Invalid input."); return; }


            if (rm.AttemptLogin())
            {
                rm.AccessRegistrationPage();
            }
            _browserWindow.SetPage(rm.CurrentHTML);
            LogToOutput(rm.Output);
        }

        private void ScheduleRegistration(object sender, RoutedEventArgs e)
        {
            LogToOutput("[-] Scheduling registration.");
            try
            {
                InputtedCreds creds = GetInputtedCreds();
                _requestManager = new RequestManager(creds.username, creds.password, creds.regPin, creds.term);
            } catch (ArgumentException exception)
            {
                LogToOutput("[-] Error: Invalid argument passed. (" + exception.Message + ")");
                return;
            }
            
            DateTime inputTime = Convert.ToDateTime(scheduledTime.Text);

            // TODO: Use reflection here to loop through CRNs text boxes.
            _requestManager.CRNs.Enqueue(crn1.Text);
            _requestManager.CRNs.Enqueue(crn2.Text);
            _requestManager.CRNs.Enqueue(crn3.Text);
            _requestManager.CRNs.Enqueue(crn4.Text);
            _requestManager.CRNs.Enqueue(crn5.Text);
            _requestManager.CRNs.Enqueue(crn6.Text);
            _requestManager.CRNs.Enqueue(crn7.Text);
            _requestManager.CRNs.Enqueue(crn8.Text);
            _requestManager.CRNs.Enqueue(crn9.Text);
            _requestManager.CRNs.Enqueue(crn10.Text);

            _registrationScheduler = new RegistrationScheduler(RegistrationCallback, LoginCallback);
            _registrationScheduler.ScheduleRegistration(inputTime);
        }

        public void RegistrationCallback(Object stateInfo)
        {
            _requestManager.AddClasses();
            LogToOutput(_requestManager.Output, true);
            _browserWindow.SetPage(_requestManager.CurrentHTML);
        }

        public void LoginCallback(Object stateInfo)
        {
            if(_requestManager.AttemptLogin())
            {
                if(_requestManager.AccessRegistrationPage() == false)
                {
                    _registrationScheduler.ClearTimers();
                }
            }

            LogToOutput(_requestManager.Output, true);
            _browserWindow.SetPage(_requestManager.CurrentHTML);
        }

        private InputtedCreds GetInputtedCreds()
        {
            InputtedCreds ofTheJedi = new InputtedCreds();
            ofTheJedi.username = userID.Text;
            ofTheJedi.password = password.Password;
            ofTheJedi.regPin = regPin.Text;
            ofTheJedi.term = new Term(term.Text);
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
            public Term term;
        }
    }
}
