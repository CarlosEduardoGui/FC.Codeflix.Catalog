using FC.CodeFlix.Catalog.Domain.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace FC.CodeFlix.Catalog.Api.Filters;

public class ApiGlobalExceptionFilter : IExceptionFilter
{
    private readonly IHostEnvironment _env;
    public ApiGlobalExceptionFilter(IHostEnvironment env) => _env = env;

    public void OnException(ExceptionContext context)
    {
        var details = new ProblemDetails();
        var exception = context.Exception;

        if (_env.IsDevelopment())
            details.Extensions.Add("StackTrace", exception.StackTrace);

        if (exception is EntityValidationException)
        {
            var ex = exception as EntityValidationException;
            details.Title = "One or more validation errors occurred";
            details.Status = (int)HttpStatusCode.UnprocessableEntity;
            details.Type = "UnprocessableEntity";
            details.Detail = ex!.Message;
        }
        else
        {
            details.Title = "An unexpected error occurred";
            details.Status = (int)HttpStatusCode.UnprocessableEntity;
            details.Type = "UnexpectedError";
            details.Detail = exception.Message;
        }

        context.HttpContext.Response.StatusCode = (int)details.Status;
        context.Result = new ObjectResult(details);
        context.ExceptionHandled = true;
    }
}
