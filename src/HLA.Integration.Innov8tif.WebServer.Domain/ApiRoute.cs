using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Integration.Innov8tif.WebServer.Domain
{
    public static class ApiRoute
    {
        public const string JourneyId = "/api/ekyc/journeyid";
        public const string OkayId = "/api/ekyc/okayid";
        public const string OkayFace = "/api/ekyc/okayface/v1-1";
        public const string OkayDoc = "/api/ekyc/okaydoc";
        public const string ScoreCard = "/api/ekyc/scorecard";
    }
}
