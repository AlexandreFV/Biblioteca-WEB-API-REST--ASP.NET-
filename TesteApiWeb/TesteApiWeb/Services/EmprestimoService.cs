using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Biblioteca_WEB_API_REST_ASP.Class;
using Biblioteca_WEB_API_REST_ASP.Context;
using Biblioteca_WEB_API_REST_ASP.Models;
using static DTOS.Categoria.CategoriaDTO;
using static DTOS.Emprestimo.EmprestimoDTO;
using static DTOS.Livro.LivroDTO;

namespace Biblioteca_WEB_API_REST_ASP.Services
{
    public class EmprestimoService : ServicePersonalizado<Emprestimo>
{
        private readonly AppDBContextSistema _context;

        public EmprestimoService(AppDBContextSistema context)
        {
            _context = context;
        }

        //public async Task <ServiceResult<IEnumerable<EmprestimoDTOResponse>>> listarEmprestimosAsync(ClaimsPrincipal userClaim)
        //{

        //    var isAdmin = userClaim.IsInRole("Admin");

        //    var emprestimos = await AplicarFiltroVisualizacao(_context.Emprestimos.AsNoTracking(), isAdmin)
        //        .Include(e => e.Livro)
        //        .Select(e => Mapear(e))
        //        .ToListAsync();

        //    return Result<IEnumerable<EmprestimoDTOResponse>>(true, EncontradasSucesso, emprestimos, ResultType.Sucesso);
        //}
        
        //public async Task<ServiceResult<EmprestimoDTOResponse>> criarEmprestimoAsync(EmprestimoDTOCreate emprestimoCreateDTO)
        //{

        //}

        private IQueryable<Emprestimo> AplicarFiltroVisualizacao(IQueryable<Emprestimo> query, bool isAdmin)
        {

            if (!isAdmin)
                query = query.Where(e => e.Ativo);

            return query;
        }

        private EmprestimoDTOResponse Mapear(Emprestimo l)
        {
            var LivroMapeado = new EmprestimoDTOResponse
            {
                
            };

            return LivroMapeado;
        }

    }
}
