using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RidgeRacer3DTableTool.Models
{
    class ErrorInfo
    {
        public bool result = false;
        public string message = "";
    }

    class UserStateProgressChanged
    {
        public int maximum;
        public ErrorInfo errorInfo;
    }
}
