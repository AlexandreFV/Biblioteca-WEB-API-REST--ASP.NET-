using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Domain.Interfaces
{
    public interface IEntityBase
    {
        int Id { get; }
        bool Ativo {  get; set; }
    }
}
