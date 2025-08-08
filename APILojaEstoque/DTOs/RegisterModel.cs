using System.ComponentModel.DataAnnotations;

namespace APILojaEstoque.DTOs
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "O campo 'Username' é obrigatório.")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "O campo 'Password' é obrigatório.")]
        public string? Password { get; set; }

        [EmailAddress][Required(ErrorMessage = "O campo 'Email' é obrigatório.")]

        public string? Email { get; set; }
    }
}
