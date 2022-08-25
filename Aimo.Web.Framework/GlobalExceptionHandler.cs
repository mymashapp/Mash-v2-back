using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

public class GlobalExceptionHandler : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        switch (context.Exception)
        {
            case ArgumentNullException e when e.Message.Contains("[NullGuard]"):
                context.Result = new BadRequestObjectResult(e.Message);
                context.ExceptionHandled = true;
                break;
        }
    }
}