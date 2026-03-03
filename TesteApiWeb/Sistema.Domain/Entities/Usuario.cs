using Microsoft.AspNetCore.Identity;
using Sistema.Domain.Interfaces;
using System.ComponentModel.DataAnnotations;

namespace Biblioteca_WEB_API_REST_ASP.Models
{
    public class Usuario : IdentityUser
    {
        public string Nome { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;

    }
}
