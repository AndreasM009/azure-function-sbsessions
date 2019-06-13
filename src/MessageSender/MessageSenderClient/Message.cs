﻿using System;
using System.Collections.Generic;
using System.Text;

namespace MessageSenderClient
{
    public class Message
    {
        public Guid CustomerId { get; set; }
        public Guid Id { get; set; }
        public string BlobUri { get; set; }
        public string RegistrationOffice { get; set; }
    }
}
