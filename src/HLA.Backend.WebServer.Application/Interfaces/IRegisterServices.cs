using HLA.Backend.WebServer.Domain.Register;
using HLA.Backend.WebServer.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Backend.WebServer.Application.Interfaces
{
    public interface IRegisterServices
    {
        Task<CommonResponse> PostAsync(RegisterApiRequest request);
        Task<CommonResponse> PutAsync(RegisterApiRequest request);
    }
}
