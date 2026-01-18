using System;
using System.Collections.Generic;
using System.Text;

namespace Devices.Application.Common
{
    public record Error
    (
        string Code,
        string Message,
        ErrorType Type
    );
    public enum ErrorType
    {
        Validation,
        NotFound,
        Conflict,
        Unauthorized,
        Forbidden,
        Unknown
    }
}
