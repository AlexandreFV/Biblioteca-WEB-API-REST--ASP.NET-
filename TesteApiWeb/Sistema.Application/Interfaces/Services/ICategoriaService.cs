using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DTOS.Categoria.CategoriaDTO;

namespace Sistema.Application.Interfaces.Services
{
    public interface ICategoriaService : ICrudService<CategoriaResponseDTO, CategoriaCreateDTO, CategoriaUpdateDTO>
    {
    }
}
