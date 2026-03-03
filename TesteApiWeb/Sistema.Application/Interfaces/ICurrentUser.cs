using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Application.Interfaces
{
    public interface ICurrentUser
    {
        string userId { get; }
        bool IsAdmin { get; }
    }
}
