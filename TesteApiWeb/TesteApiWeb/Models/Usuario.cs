using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace TesteApiWeb.Models
{
    public class Usuario : IdentityUser
    {
        public string Nome { get; set; } = string.Empty;
        public bool Ativo { get; set; } = true;

    }
}
