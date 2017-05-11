using API.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Net;

namespace API.Filters
{

    public class UserNotifyExceptionFilter : ExceptionFilterAttribute
    {
        public ILogger Logger { get; set; }

        public UserNotifyExceptionFilter(ILogger logger)
        {
            this.Logger = logger;
        }

        public override void OnException(ExceptionContext context)
        {
            Logger.Error(context.Exception.Message);
            if (context.Exception is InvalidInputException)
            {
                context.Result = new ContentResult()
                {
                    Content = context.Exception.Message,
                    StatusCode = (int)HttpStatusCode.BadRequest
                };
            } else if (context.Exception is AuthorizationException)
            {
                context.Result = new ContentResult()
                {
                    Content = context.Exception.Message,
                    StatusCode = (int)HttpStatusCode.Unauthorized
                };
            }
        }
    }
}