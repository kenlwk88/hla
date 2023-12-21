using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HLA.Backend.Core.Infra.FCM.Implementations
{
    public class FcmHla360Services : IFcmServices
    {
        private readonly ILogger<FcmHla360Services> _logger;
        public FcmHla360Services(ILogger<FcmHla360Services> logger, IConfiguration configuration)
        {
            _logger = logger;

            FirebaseApp? defaultApp = null;

            if (FirebaseApp.DefaultInstance == null)
            {
                var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
                path = path?.Replace(@"file:\", "");

                string fullPath = path + @"\ServiceAccount\" + configuration.GetSection("FCM:ServiceAccountFileName_HLA360").Value;

                defaultApp = FirebaseApp.Create(new AppOptions()
                {
                    Credential = GoogleCredential.FromFile(fullPath),
                });
            }
        }
        public async Task<string> SendToRegisterToken(Dictionary<string, string> data, string registerToken, string title, string body, bool isUseDataOnly = false)
        {
            var message = new Message();
            if (isUseDataOnly) 
            {
                message = new Message()
                {
                    Data = data,
                    Token = registerToken
                };
            }
            else 
            {
                message = new Message()
                {
                    Data = data,
                    Notification = new Notification { Body = body, Title = title },
                    Token = registerToken
                };
            }

            // Send a message to the device corresponding to the provided
            string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);

            // Response is a message ID string.
            _logger.LogInformation($"Successfully push message: {response}");
            return response;
        }
    }
}
