using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Integration.Innov8tif.WebServer.Application
{
    public interface IEkycServices
    {
        Task<dynamic> PostJourneyId(dynamic request);
        Task<dynamic> PostOkayId(dynamic request);
        Task<dynamic> PostOkayFace(object data);
        Task<dynamic> PostOkayDoc(dynamic request);
        Task<dynamic> GetScoreCard(object data);
    }
}
