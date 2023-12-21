using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Backend.Core.Domain.Kafka
{
    public class NotificationModel
    {
        public List<int> FcmIds { get; set; } = new List<int>();
    }
}
