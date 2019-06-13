using System;
using System.Collections.Generic;
using System.Text;

namespace DispatcherFunctionApp
{
    public class DispatchServiceOptions
    {
        public string RegistrationOfficeATopicConnectionString { get; set; }
        public string RegistrationOfficeATopicName { get; set; }
        public string RegistrationOfficeBTopicConnectionString { get; set; }
        public string RegistrationOfficeBTopicName { get; set; }
        public string RegistrationOfficeCTopicConnectionString { get; set; }
        public string RegistrationOfficeCTopicName { get; set; }
    }
}
