using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using System.Dynamic;
using System.Net;
using HLA.Backend.WebServer.Domain;

namespace HLA.Backend.WebServer.Filter.Validation
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// </remarks>
    /// <returns></returns>
    public class ValidationFilterAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// </summary>
        /// <remarks>
        /// </remarks>
        /// <returns></returns>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (context.ModelState.ErrorCount > 0)
            {
                var modelType = context.ActionDescriptor.Parameters
                    .FirstOrDefault(p => p.BindingInfo!.BindingSource!.Id.Equals("Body", StringComparison.InvariantCultureIgnoreCase))?.ParameterType; //Get model type  

                var expandoObj = new ExpandoObject();
                var expandoObjCollection = expandoObj as ICollection<KeyValuePair<string, object>>;

                var dictionary = context.ModelState.ToDictionary(k => k.Key, v => v.Value)
                    .Where(v => v.Value?.ValidationState == ModelValidationState.Invalid)
                    .ToDictionary( k =>
                    {
                        if (modelType != null)
                        {
                            var property = modelType.GetProperties().First(p => p.Name.Equals(k.Key, StringComparison.InvariantCultureIgnoreCase));
                            if (property != null)
                            {
                                var displayName = property.GetCustomAttributes(typeof(JsonPropertyAttribute), true).Cast<JsonPropertyAttribute>().SingleOrDefault()?.PropertyName;
                                return displayName ?? property.Name;
                            }
                        }
                        return k.Key; //Nothing found, return original vaidation key  
                    },
                    v => v.Value?.Errors.Select(e => e.ErrorMessage).ToList() as Object); //Box String collection  
                foreach (var keyValuePair in dictionary)
                {
                    expandoObjCollection.Add(keyValuePair!);
                }
                context.HttpContext.Response.ContentType = "application/json";
                context.HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                CommonResponse result = Error.Response(101).TryCast<CommonResponse>();
                List<string> errors = new List<string>();
                foreach (var error in expandoObj) 
                {
                    var list = ToList(ToDynamic(error.Value!));
                    errors.AddRange(list);
                }
                result.Data = errors;
                context.Result = new JsonResult(result);
            }
            base.OnActionExecuting(context);
        }
        private List<string> ToList(object obj) 
        {
            return JsonConvert.DeserializeObject<List<string>>(JsonConvert.SerializeObject(obj))!;
        }
        private dynamic ToDynamic(object obj)
        {
            return JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(obj))!;
        }
    }
}
