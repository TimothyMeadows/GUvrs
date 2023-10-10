using GUvrs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUvrs.Components.Exceptions
{
    public class GuApiException : Exception
    {
        public GuApiException(HttpErrorModel error) : base($"Gu Api Error: {error.Code}\r\n\r\n{error.Message}") { }
    }
}
