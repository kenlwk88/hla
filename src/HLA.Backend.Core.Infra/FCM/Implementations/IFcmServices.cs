using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Backend.Core.Infra.FCM.Implementations
{
    public interface IFcmServices
    {
        Task<string> SendToRegisterToken(Dictionary<string, string> data, string registerToken, string title, string body, bool isUseDataOnly = false);
    }
}
