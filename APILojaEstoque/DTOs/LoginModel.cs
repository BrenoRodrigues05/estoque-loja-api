using System.ComponentModel.DataAnnotations;

namespace APILojaEstoque.DTOs
{
    public class LoginModel
    {
        [Required(ErrorMessage = "O campo 'Username' é obrigatório.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "O campo 'Password' é obrigatório.")]
        public string? Password { get; set; }
    }
}
