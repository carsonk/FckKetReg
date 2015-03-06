using FckKetReg.Models;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Forms;

namespace FckKetReg
{
    /// <summary>
    /// Allows time-based tasking for calling RequestManager functions.
    /// </summary>
    class RegistrationScheduler
    {
        IScheduler _scheduler;
        TriggerKey _currentTriggerKey;

        string _userID;
        string _password;
        string _registrationPIN;
        string _currentTermText;
        Queue<string> _CRNs;

        public BrowserWindow PreviewWindow;

        public RegistrationScheduler(string userID, string password, string registrationPIN, string currentTermText, Queue<string> CRNs)
        {
            _scheduler = StdSchedulerFactory.GetDefaultScheduler();
            _scheduler.Start();

            this._userID = userID;
            this._password = password;
            this._registrationPIN = registrationPIN;
            this._currentTermText = currentTermText;
            this._CRNs = CRNs;
        }

        ~RegistrationScheduler()
        {
            ClearTimers();
            _scheduler.Shutdown();
        }

        public void ScheduleRegistration(DateTime registrationFireTime)
        {
            ClearTimers();

            DateTimeOffset triggerStartAt = new DateTimeOffset(registrationFireTime).ToUniversalTime();

            // Creates job to be called by trigger with params to be set in Job's Execute.
            // Sets up RequestManager call.
            IJobDetail job = JobBuilder.Create<RequestManager>()
                .WithIdentity("registrationJob")
                .UsingJobData("userID", _userID)
                .UsingJobData("password", _password)
                .UsingJobData("registrationPIN", _registrationPIN)
                .UsingJobData("currentTerm", _currentTermText)
                .Build();

            // Creates a one-time simple trigger using the job. Takes UTC start time.
            ISimpleTrigger trigger = (ISimpleTrigger)TriggerBuilder.Create()
                .WithIdentity("registrationTrigger")
                .StartAt(triggerStartAt)
                .ForJob("registrationJob")
                .Build();

            // Schedules the job.
            _currentTriggerKey = trigger.Key;
            _scheduler.Context.Put("CRNs", _CRNs);
            _scheduler.Context.Put("previewWindow", PreviewWindow);
            _scheduler.ScheduleJob(job, trigger);
        }

        public void ClearTimers()
        {
            _scheduler.Clear();
        }
    }
}
