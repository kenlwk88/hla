using HLA.Backend.Core.DataAccess.Implementations;
using HLA.Backend.Core.Domain.Enum;
using HLA.Backend.WebServer.Application.Interfaces;
using HLA.Backend.WebServer.Domain;
using HLA.Backend.WebServer.Domain.Register;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HLA.Backend.WebServer.Application.Implementations
{
    public class RegisterServices : IRegisterServices
    {
        private readonly ILogger<RegisterServices> _logger;
        private readonly IRegisterRepo _registerRepo;
        public RegisterServices(ILogger<RegisterServices> logger, IConfiguration configuration, IRegisterRepo registerRepo)
        {
            _logger = logger;
            _registerRepo = registerRepo;
        }
        public async Task<CommonResponse> PostAsync(RegisterApiRequest request)
        {
            CommonResponse response = new();
            try
            {
                //Validate AppSource
                if (!Enum.TryParse<AppSource>(request.AppSource, true, out AppSource app))
                    return Error.Response(201).TryCast<CommonResponse>();

                int? id = await _registerRepo.SaveDeviceTokenAsync(request.AppSource, request.CustKey!, request.DeviceToken!, request.DeviceOSType);
                if(id == null || id == 0)
                    return Error.Response(801).TryCast<CommonResponse>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, nameof(PostAsync));
                return Error.Response(999).TryCast<CommonResponse>();
            }
            return response;
        }
        public async Task<CommonResponse> PutAsync(RegisterApiRequest request)
        {
            CommonResponse response = new();
            try
            {
                //Validate AppSource
                if (!Enum.TryParse<AppSource>(request.AppSource, true, out AppSource app))
                    return Error.Response(201).TryCast<CommonResponse>();

                var isDisable = await _registerRepo.DisableDeviceTokenAsync(request.AppSource, request.CustKey!, request.DeviceToken!, request.DeviceOSType);
                if (!isDisable)
                    return Error.Response(801).TryCast<CommonResponse>();

            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, nameof(PutAsync));
                return Error.Response(999).TryCast<CommonResponse>();
            }
            return response;
        }
    }
}
