using Dapper;
using HLA.Backend.Core.DataAccess.Models;
using Microsoft.Extensions.Logging;
using System.Data;

namespace HLA.Backend.Core.DataAccess.Implementations
{
    public interface IRegisterRepo
    {
        Task<IEnumerable<Register>> GetRegisterUsersAsync(string custKey, string appSource);
        Task UpdateInvalidDeviceTokenAsync(string deviceToken);
        Task<int?> SaveDeviceTokenAsync(string appSource, string custKey, string deviceToken, int deviceOsType);
        Task<bool> DisableDeviceTokenAsync(string appSource, string custKey, string deviceToken, int deviceOsType);
    }
    public class RegisterRepo : IRegisterRepo
    {
        private readonly ILogger<RegisterRepo> _logger;
        private readonly DbContext _dbContext;
        public RegisterRepo(ILogger<RegisterRepo> logger, DbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Register>> GetRegisterUsersAsync(string custKey, string appSource) 
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CustKey", custKey, dbType: DbType.String);
                parameters.Add("@AppSource", appSource, dbType: DbType.String);
                var result = await _dbContext.QueryStoredProcedureAsync<Register>("CGL_GETREGISTER", parameters);
                return result;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task UpdateInvalidDeviceTokenAsync(string deviceToken)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@DeviceToken", deviceToken, dbType: DbType.String);
                parameters.Add("@IsInvalidDeviceToken", true, dbType: DbType.Boolean);
                parameters.Add("@IsInvalidDTDate", DateTime.Now, dbType: DbType.DateTime);
                await _dbContext.ExecuteSPAsync("CGL_UPDATEINVALIDDEVICE", parameters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<int?> SaveDeviceTokenAsync(string appSource, string custKey, string deviceToken, int deviceOsType)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@RecordCreated", DateTime.Now, dbType: DbType.DateTime);
                parameters.Add("@CustKey", custKey, dbType: DbType.String);
                parameters.Add("@DeviceToken", deviceToken, dbType: DbType.String);
                parameters.Add("@AppSource", appSource, dbType: DbType.String);
                parameters.Add("@DeviceOSType", deviceOsType, dbType: DbType.Int32);
                var result = await _dbContext.ExecuteStoredProcedureAsync("CGL_SAVEREG", parameters);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<bool> DisableDeviceTokenAsync(string appSource, string custKey, string deviceToken, int deviceOsType)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@IsActiveUpdated", DateTime.Now, dbType: DbType.DateTime);
                parameters.Add("@CustKey", custKey, dbType: DbType.String);
                parameters.Add("@DeviceToken", deviceToken, dbType: DbType.String);
                parameters.Add("@AppSource", appSource, dbType: DbType.String);
                parameters.Add("@DeviceOSType", deviceOsType, dbType: DbType.Int32);
                var result = await _dbContext.ExecuteStoredProcedureAsync("CGL_UPDATEREG", parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    }
}
