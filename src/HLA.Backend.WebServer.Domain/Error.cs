using Newtonsoft.Json;

namespace HLA.Backend.WebServer.Domain
{
    public static class Error
    {
        private static readonly Dictionary<string, string> _ErrorList = new Dictionary<string, string>()
        {
            {"101" , "invalid_parameters"},
            {"201" , "invalid_appsource"},
            {"202" , "invalid_custkey"},
            {"203" , "invalid_moduletype"},
            {"801", "operation_failed"},
            {"802", "not_supported"},
            {"901", "unauthorized" },
            {"999", "system_error" }
        };
        public static object Response(int errorCode)
        {
            return new
            {
                code = errorCode,
                message = _ErrorList.GetValueOrDefault(errorCode.ToString(), "unknown_error")
            };
        }
        public static string Message(int errorCode)
        {
            return _ErrorList.GetValueOrDefault(errorCode.ToString(), "unknown_error");
        }
        public static T TryCast<T>(this object obj) where T : class
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            var json = JsonConvert.SerializeObject(obj);
            return JsonConvert.DeserializeObject<T>(json)!;
        }
    }
}
