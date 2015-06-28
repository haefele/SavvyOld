using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Savvy.Services.Exceptions
{
    public interface IExceptionHandler
    {
        void Handle(Exception exception);
    }
}
