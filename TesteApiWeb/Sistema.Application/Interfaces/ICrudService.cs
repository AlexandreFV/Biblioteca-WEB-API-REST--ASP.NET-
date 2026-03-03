using Biblioteca_WEB_API_REST_ASP.Class;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sistema.Application.Interfaces
{
    public interface ICrudService<TResponseDto, TCreateDto, TEditDto>
    {

        Task<ServiceResult<IEnumerable<TResponseDto>>> listarAsync();
        Task<ServiceResult<TResponseDto>> buscarPorId(int id);
        Task<ServiceResult<bool>> softDeletePorId(int id);
        Task<ServiceResult<bool>> editarAsync (int id, TEditDto TDtoEdit);
        Task<ServiceResult<TResponseDto>> adicionarAsync(TCreateDto TCreteDTO);
    }
}
