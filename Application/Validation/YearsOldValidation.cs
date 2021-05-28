using System;
using Application.ViewModel;
using Domain.Models;

namespace Application.Validation
{
    public class YearsOldValidation
    {
        public int CalcularIdade(Contato obj)
        {
            var dataNascimento = obj.DataNascimento;
            int idade = DateTime.Now.Year - dataNascimento.Year;
            if (DateTime.Now.DayOfYear < dataNascimento.DayOfYear)
            {
                idade = idade - 1;
            }
            return idade;
        }
        
        public (bool, string) ValidarDados(Contato obj)
        {
            if (obj.DataNascimento > DateTime.Now)
                return (false, "Data de nascimento é maior do que a data atual.");

            if (obj.Idade < 18)
                return (false, "O contato deve ser maior de idade.");

            return (true, null);
        }
    }
}