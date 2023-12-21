using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Backend.Core.DataAccess.Models
{
    public class Notifiaction
    {
        public Int64 ID { get; set; }
        public string? CustKey { get; set; }
        public string? AppSource { get; set; }
        public string? DeviceToken { get; set; }
        public string? Title { get; set; }
        public string? PushMessage { get; set; }
        public bool IsProcess { get; set; }
        public bool IsPush { get; set; }
        public DateTime? IsPushDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Payload { get; set; }
    }
}
