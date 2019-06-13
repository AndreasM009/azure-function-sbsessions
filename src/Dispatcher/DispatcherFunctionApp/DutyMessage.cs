using System;
using System.Collections.Generic;
using System.Text;

namespace DispatcherFunctionApp
{
    public class DutyMessage
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string RegistrationOffice { get; set; }
        public string Text { get; set; }
    }
}
