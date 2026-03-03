using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DTOS.Livro.LivroDTO;

namespace Sistema.Application.Interfaces.Services
{
    public interface ILivroService : ICrudService<LivroResponseDTO, LivroCreateDTO, LivroEditDTO>
    {
    }
}
