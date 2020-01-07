using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.DependencyInjection;

namespace WebApplication1.Filters
{
    public class ExceptionFilter : IExceptionFilter
    {
        IHostingEnvironment _hostingEnvironment;
        IModelMetadataProvider _modelMetadataProvider;

        public ExceptionFilter(IHostingEnvironment hostingEnvironment)
        {
            this._hostingEnvironment = hostingEnvironment;
        }

        public void OnException(ExceptionContext context)
        {
            if (!_hostingEnvironment.IsDevelopment())
            {
                return;
            }
            if (_modelMetadataProvider == null)
                _modelMetadataProvider = context.HttpContext.RequestServices.GetRequiredService<IModelMetadataProvider>();

            context.ExceptionHandled = true;
            var result = new ViewResult { ViewName = "Error"};
            result.ViewData = new ViewDataDictionary(_modelMetadataProvider, context.ModelState);
            result.ViewData.Add("Exception", context.Exception);
            result.ViewData.Add("Name", context.HttpContext.Session.GetString("Name"));
            result.ViewData.Add("Id", context.HttpContext.Session.GetString("Id"));
            context.Result = result;

            //web api response
            //HttpStatusCode status = HttpStatusCode.InternalServerError;
            //String message = String.Empty;

            //var exceptionType = context.Exception.GetType();
            //if (exceptionType == typeof(UnauthorizedAccessException))
            //{
            //    message = "Unauthorized Access";
            //    status = HttpStatusCode.Unauthorized;
            //}
            //else if (exceptionType == typeof(NotImplementedException))
            //{
            //    message = "A server error occurred.";
            //    status = HttpStatusCode.NotImplemented;
            //}
            //else
            //{
            //    message = context.Exception.Message;
            //    status = HttpStatusCode.ExpectationFailed;
            //}
            //context.ExceptionHandled = true;
            //context.HttpContext.Response.StatusCode = (int)status;
            //context.HttpContext.Response.ContentType = "application/json";
            //string Error = $"{message} {context.Exception.StackTrace}";

            //context.Result = new RedirectResult($"/Error?message={message}");
        }
    }
}
