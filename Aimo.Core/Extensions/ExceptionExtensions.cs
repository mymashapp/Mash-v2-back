namespace Aimo.Core;

public static class ExceptionExtensions
{
    public static string GetExceptionMessage(this Exception exception)
    {
        var message = exception.Message;
        if (exception.InnerException is not null)
        {
            message += @",   InnerException :" + GetExceptionMessage(exception.InnerException);
        }

        return message;
    }
}