using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MessageApi.Model
{
    public class DutyMessageDto
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string RegistrationOffice { get; set; }
        public string Text { get; set; }
    }
}
