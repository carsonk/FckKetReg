using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Controls;

namespace FckKetReg
{
    /// <summary>
    /// Allows time-based tasking for calling RequestManager functions.
    /// </summary>
    class RegistrationScheduler
    {
        // TODO: Make this class not suck.

        private TimerCallback _registrationCallback;
        private TimerCallback _loginCallback;

        private Timer _registrationTimer;
        private Timer _loginTimer;

        public RegistrationScheduler(TimerCallback registrationCallback, TimerCallback loginCallback)
        {
            _registrationCallback = registrationCallback;
            _loginCallback = loginCallback;
        }

        public void ScheduleRegistration(DateTime registrationFireTime)
        {
            
        }

        public void ClearTimers()
        {
            
        }
    }
}
