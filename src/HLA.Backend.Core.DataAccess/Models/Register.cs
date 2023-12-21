using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Backend.Core.DataAccess.Models
{
    public class Register
    {
        public Int64 ID { get; set; }
        public DateTime? RecordCreated { get; set; }
        public string? CustKey { get; set; }
        public string? DeviceToken { get; set; }
        public bool? IsActive { get; set; }
        public string? AppSource { get; set; }
        public int? DeviceOSType { get; set; } //1=Android, 2=IOS
        public DateTime? IsActiveUpdated { get; set; }
        public bool? IsInvalidDeviceToken { get; set; }
        public DateTime? IsInvalidDTDate { get; set; }
    }
}
