using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GNPMAzureFunctions.Models
{
    public record NotificationReciever(string agreementNumber, string accountNumber, string companyName, string status, string createdBy, string salesPerson);
    public record Mail(string fromMailAddress, string toMailAddress, string ccMailAddress, string bccMailAddress, string replyToMailAddress, string subject, string contentType, string encoding, string bodycontentTransferEncoding, string mailbody);
}
