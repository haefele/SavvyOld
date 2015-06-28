using System;
using Anotar.Custom;

namespace Savvy.Services.Exceptions
{
    public class ExceptionHandler : IExceptionHandler
    {
        public void Handle(Exception exception)
        {
            LogTo.Error(exception, "Error occured.");
        }
    }
}