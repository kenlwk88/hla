using Dapper;
using HLA.Backend.Core.DataAccess.Models;
using Microsoft.Extensions.Logging;
using System.Data;

namespace HLA.Backend.Core.DataAccess.Implementations
{
    public interface INotificationRepo 
    {
        Task<IEnumerable<Notifiaction>> GetUnpushNotificationAsync(int? FcmId);
        Task<bool> UpdateIsProcessPushNotificationAsync(int Id, bool isProcess);
        Task<bool> UpdateIsPushNotificationAsync(int Id, bool isPush);
        Task<int?> InsertPushNotificationAsync(string custKey, string appSource, string deviceToken, string title, string message, string payload);
        Task<int?> InsertErrorForPushNotificationAsync(int Id, string deviceToken, string message);
        Task<int?> InsertNotificationAsync(string appSource, string moduleType, string custKey, string title, string shortMessage, string detailMessage, string data);
        Task<bool> UpdateIsReadNotificationAsync(string appSource, string moduleType, string custKey, int id);
        Task<IEnumerable<NotificationDetail>> GetNotificationsAsync(string appSource, string moduleType, string custKey, int id, int? days);
        Task<IEnumerable<BulkNotifiaction>> GetBulkNotificationAsync();
        Task<bool> SaveBulkNotificationLogAsync(int Id, bool isError, string errorMessage);
    }
    public class NotificationRepo : INotificationRepo
    {
        private readonly ILogger<NotificationRepo> _logger;
        private readonly DbContext _dbContext;
        public NotificationRepo(ILogger<NotificationRepo> logger, DbContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        public async Task<IEnumerable<Notifiaction>> GetUnpushNotificationAsync(int? FcmId) 
        {
            try
            {
                var parameters = new DynamicParameters();
                if(FcmId != null)
                    parameters.Add("@FcmId", FcmId, dbType: DbType.Int32);
                var result = await _dbContext.QueryStoredProcedureAsync<Notifiaction>("INAPP_UNPUSHNOTIFICATION", parameters);
                return result;
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<bool> UpdateIsProcessPushNotificationAsync(int Id, bool isProcess)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ID", Id, dbType: DbType.Int32);
                parameters.Add("@IsProcess", isProcess, dbType: DbType.Boolean);
                var result = await _dbContext.ExecuteStoredProcedureAsync("INAPP_UPDATE_ISPROCESS_PUSHNOTIFICATION", parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<bool> UpdateIsPushNotificationAsync(int Id, bool isPush)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ID", Id, dbType: DbType.Int32);
                parameters.Add("@IsPush", isPush, dbType: DbType.Boolean);
                var result = await _dbContext.ExecuteStoredProcedureAsync("INAPP_UPDATE_ISPUSH_PUSHNOTIFICATION", parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<int?> InsertPushNotificationAsync(string custKey, string appSource, string deviceToken, string title, string message ,string payload)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@CustKey", custKey, dbType: DbType.String);
                parameters.Add("@AppSource", appSource, dbType: DbType.String);
                parameters.Add("@DeviceToken", deviceToken, dbType: DbType.String);
                parameters.Add("@Title", title, dbType: DbType.String);
                parameters.Add("@Message", message, dbType: DbType.String);
                if (!string.IsNullOrEmpty(payload))
                    parameters.Add("@Payload", payload, dbType: DbType.String);
                var result = await _dbContext.ExecuteStoredProcedureAsync("INAPP_SAVENOTIFICATION", parameters);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<int?> InsertErrorForPushNotificationAsync(int Id, string deviceToken, string message)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@FcmID", Id, dbType: DbType.Int32);
                parameters.Add("@DeviceToken", deviceToken, dbType: DbType.String);
                parameters.Add("@Message", message, dbType: DbType.String);
                var result = await _dbContext.ExecuteStoredProcedureAsync("INAPP_SAVENOTIFICATIONERROR", parameters);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<int?> InsertNotificationAsync(string appSource, string moduleType, string custKey, string title, string shortMessage, string detailMessage, string data)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@AppSource", appSource, dbType: DbType.String);
                parameters.Add("@ModuleType", moduleType, dbType: DbType.String);
                parameters.Add("@CustKey", custKey, dbType: DbType.String);
                parameters.Add("@Title", title, dbType: DbType.String);
                parameters.Add("@ShortMessage", shortMessage, dbType: DbType.String);
                parameters.Add("@DetailMessage", detailMessage, dbType: DbType.String);
                parameters.Add("@Data", data, dbType: DbType.String);
                var result = await _dbContext.ExecuteStoredProcedureAsync("INAPP_SAVENOTIFICATIONLIST", parameters);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<bool> UpdateIsReadNotificationAsync(string appSource, string moduleType, string custKey, int id)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@AppSource", appSource, dbType: DbType.String);
                parameters.Add("@ModuleType", moduleType, dbType: DbType.String);
                parameters.Add("@CustKey", custKey, dbType: DbType.String);
                parameters.Add("@Id", id, dbType: DbType.Int32);
                var result = await _dbContext.ExecuteStoredProcedureAsync("INAPP_UPDATE_ISREAD_NOTIFICATIONLIST", parameters);
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<IEnumerable<NotificationDetail>> GetNotificationsAsync(string appSource, string moduleType, string custKey, int id, int? days)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@AppSource", appSource, dbType: DbType.String);
                parameters.Add("@ModuleType", moduleType, dbType: DbType.String);
                parameters.Add("@CustKey", custKey, dbType: DbType.String);
                parameters.Add("@Id", id, dbType: DbType.Int64);
                parameters.Add("@day", days, dbType: DbType.Int32);
                var result = await _dbContext.QueryStoredProcedureAsync<NotificationDetail>("INAPP_GETNOTIFICATIONLIST", parameters);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<IEnumerable<BulkNotifiaction>> GetBulkNotificationAsync()
        {
            try
            {
                var parameters = new DynamicParameters();
                var result = await _dbContext.QueryStoredProcedureAsync<BulkNotifiaction>("INAPP_GETBUIKNOTIFICATION", parameters);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
        public async Task<bool> SaveBulkNotificationLogAsync(int Id, bool isError, string errorMessage)
        {
            try
            {
                var parameters = new DynamicParameters();
                parameters.Add("@ID", Id, dbType: DbType.Int32);
                parameters.Add("@IsError", isError, dbType: DbType.Boolean);
                parameters.Add("@ErrorMessage", string.IsNullOrEmpty(errorMessage) ? null : errorMessage , dbType: DbType.String);
                var result = await _dbContext.ExecuteStoredProcedureAsync("INAPP_SAVEBULKNOTIFICATIONLOG", parameters);
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
