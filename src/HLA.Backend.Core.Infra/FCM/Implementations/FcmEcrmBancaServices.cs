using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Backend.Core.Infra.FCM.Implementations
{
    public class FcmEcrmBancaServices : IFcmServices
    {
        private readonly ILogger<FcmEcrmBancaServices> _logger;
        private readonly string _appName = "ECRM_BANCA";
        public FcmEcrmBancaServices(ILogger<FcmEcrmBancaServices> logger, IConfiguration configuration)
        {
            _logger = logger;

            FirebaseApp? defaultApp = null;

            if (FirebaseApp.GetInstance(_appName) == null)
            {
                var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                path = path?.Replace(@"file:\", "");

                string fullPath = path + @"\ServiceAccount\" + configuration.GetSection("FCM:ServiceAccountFileName_ECRMBANCA").Value;

                defaultApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(fullPath),
                }, _appName);
            }
        }
        public async Task<string> SendToRegisterToken(Dictionary<string, string> data, string registerToken, string title, string body, bool isUseDataOnly = false)
        {
            var message = new Message()
            {
                Data = data,
                Notification = new Notification { Body = body, Title = title },
                Token = registerToken
            };

            // Send a message to the device corresponding to the provided
            var fireBaseApp = FirebaseApp.GetInstance(_appName);
            string response = await FirebaseMessaging.GetMessaging(fireBaseApp).SendAsync(message);

            // Response is a message ID string.
            _logger.LogInformation($"Successfully push message: {response}");
            return response;
        }
    }
}
