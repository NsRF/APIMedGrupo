using System;
namespace Domain.Models
{
    public class Contato
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public DateTime DataNascimento { get; set; }
        public string Sexo { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool? IsAtivo { get; set; }
        public int Idade { get; set; }
    }
}
