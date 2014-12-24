using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace FckKetReg
{
    /// <summary>
    /// Allows time-based tasking for calling RequestManager functions.
    /// </summary>
    class RegistrationScheduler
    {
        private RequestManager _requestManager;

        public RegistrationScheduler(RequestManager requestManager)
        {
            _requestManager = requestManager;
        }

        public void scheduleRegistration()
        {
            AutoResetEvent autoEvent = new AutoResetEvent(false);

        }

        private bool fireSetup()
        {

            return false;
        }

        private bool fireRegistration()
        {

            return false;
        }
    }
}
