using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using System.Linq;
using TesteApiWeb.Class;
using TesteApiWeb.Context;
using TesteApiWeb.DTOS;
using TesteApiWeb.Models;

namespace TesteApiWeb.Services
{
    public class LivroService : ServicePersonalizado<Livro>
    {

        private readonly AppDBContextSistema _context;

        public LivroService(AppDBContextSistema context) { _context = context; }

        public ServiceResult<IEnumerable<LivroDTOGetGetId>> ListarLivros()
        {
            var todosLivros = _context.Livros.Include(l => l.Categorias).AsNoTracking().
                Select(l => new LivroDTOGetGetId
                {
                    LivroId = l.LivroId,
                    Nome = l.Nome,
                    Quantidade = l.Quantidade,
                    Categorias = l.Categorias!.Select(c => new CategoriaDTO
                    {
                        CategoriaId = c.CategoriaId,
                        Nome = c.Nome,
                    }).ToList()
                })
                .ToList();

            if (!todosLivros.Any())
                return Result<IEnumerable<LivroDTOGetGetId>>(false, NaoEncontrado, null, ResultType.NotFound);

            return Result<IEnumerable<LivroDTOGetGetId>>(true, EncontradasSucesso, todosLivros, ResultType.Sucesso);

        }

        public ServiceResult<LivroDTOCreateEdit> CriarLivro(LivroDTOCreateEdit livroDTO)
        {
            var nomeFormatado = PadronizarNome(livroDTO.Nome);

            if (_context.Livros.AsNoTracking().Any(l => l.Nome == nomeFormatado))
                return Result<LivroDTOCreateEdit>(false, JaExisteEsseNome, null, ResultType.Conflito);

            var idsCategorias = livroDTO.CategoriasIds ?? new List<int>();

            var categoriasSelecionadas = _context.Categorias.Where(c => idsCategorias.Contains(c.CategoriaId)).ToList();

            if (idsCategorias.Count != categoriasSelecionadas.Count)
                return Result<LivroDTOCreateEdit>(false, "Uma ou mais categorias informadas não foram encontradas.", null, ResultType.NotFound);

            var novoLivroEntity = new Livro
            {
                Nome = nomeFormatado,
                Quantidade = livroDTO.Quantidade,
                Categorias = categoriasSelecionadas,
            };

            _context.Livros.Add(novoLivroEntity);
            _context.SaveChanges();

            var livroDTOBanco = EntityToEditDTO(novoLivroEntity);

            return Result<LivroDTOCreateEdit>(true, AdicionadoSucesso, livroDTOBanco, ResultType.Criado);

        }

        public ServiceResult<LivroDTOCreateEdit> EditarLivro(int id, LivroDTOCreateEdit livroDTO)
        {
            var livroProcurado = _context.Livros.Include(l => l.Categorias).FirstOrDefault(l => l.LivroId == id);

            if (livroProcurado == null)
                return Result<LivroDTOCreateEdit>(false, NaoEncontrado, null, ResultType.NotFound);

            if(livroProcurado.LivroId != livroDTO.LivroId)
                return Result<LivroDTOCreateEdit>(false, IdDiferente, null, ResultType.Invalido);

            livroProcurado.Nome = PadronizarNome(livroDTO.Nome);
            livroProcurado.Quantidade = livroDTO.Quantidade;

            var ids = livroDTO.CategoriasIds ?? new List<int>();
            var novasCategorias = _context.Categorias
                .Where(c => ids.Contains(c.CategoriaId))
                .ToList();

            livroProcurado.Categorias = novasCategorias;
            _context.SaveChanges();

            var livroDTOResult = EntityToEditDTO(livroProcurado);

            return Result(true, AtualizadoSucesso, livroDTOResult, ResultType.Sucesso);

        }

        public ServiceResult<LivroDTOGetGetId> ProcurarLivroPorId(int id)
        {
            var livroBanco = _context.Livros
                    .Include(l => l.Categorias)
                    .AsNoTracking()
                    .FirstOrDefault(l => l.LivroId == id);

            if (livroBanco == null)
                return Result<LivroDTOGetGetId>(false, NaoEncontrado, null, ResultType.NotFound);


            var livroDTO = EntityToGetDTO(livroBanco);

            return Result<LivroDTOGetGetId>(true, EncontradasSucesso, livroDTO, ResultType.Sucesso);

        }

        public ServiceResult<bool> ApagarLivro(int id)
        {
            var livroProcurado = _context.Livros.Include(l => l.Categorias).FirstOrDefault(l => l.LivroId == id);

            if (livroProcurado == null)
                return Result<bool>(false, NaoEncontrado, false, ResultType.NotFound);

            if (livroProcurado.Categorias != null && livroProcurado.Categorias.Any())
                return Result<bool>(false, RegistrosVinculados, false, ResultType.Conflito);

            _context.Livros.Remove(livroProcurado);
            _context.SaveChanges();

            return Result<bool>(true, ExcluidoSucesso, true, ResultType.Sucesso);
        }
        private LivroDTOCreateEdit EntityToEditDTO(Livro livro)
        {
            return new LivroDTOCreateEdit
            {
                Nome = livro.Nome,
                Quantidade = livro.Quantidade,
                CategoriasIds = livro.Categorias?.Select(c => c.CategoriaId).ToList() ?? new List<int>()
            };
        }

        private LivroDTOGetGetId EntityToGetDTO(Livro livro)
        {
            return new LivroDTOGetGetId
            {
                LivroId = livro.LivroId,
                Nome = livro.Nome,
                Quantidade = livro.Quantidade,
                Categorias = livro.Categorias?.Select(c => new CategoriaDTO
                {
                    CategoriaId = c.CategoriaId,
                    Nome = c.Nome
                }).ToList() ?? new List<CategoriaDTO>()
            };
        }
    }
}
