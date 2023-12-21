using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Backend.Core.DataAccess.Models
{
    public class NotificationDetail
    {
        public Int64 ID { get; set; }
        public string? AppSource { get; set; }
        public string? ModuleType { get; set; }
        public string? CustKey { get; set; }
        public string? Title { get; set; }
        public string? ShortMessage { get; set; }
        public string? DetailMessage { get; set; }
        public bool IsRead { get; set; }
        public DateTime? IsReadDate { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Data { get; set; }
    }
}
