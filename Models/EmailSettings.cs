using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PharmacyApp.Models
{
    public class EmailSettings
    {
        public string FromEmail { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string SmtpHost { get; set; } = default!;
        public int SmtpPort { get; set; }
    }
}