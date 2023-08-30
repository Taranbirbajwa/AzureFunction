using GNPMAzureFunctions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNPMAzureFunctions.Services
{
    public interface IEmailService
    {
        Task<string> SendEmailAsync(Mail objMail);
    }
}
