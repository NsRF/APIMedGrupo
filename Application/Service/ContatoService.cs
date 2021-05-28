using System;
using System.Collections.Generic;
using System.Linq;
using Application.Validation;
using Application.ViewModel;
using Domain.Models;
using Infraestructure.Context;

namespace Application.Service
{
    public class ContatoService : IContatoService
    {
        private readonly ApplicationDBContext _conexaoBanco;
        private static YearsOldValidation _validation;
        public ContatoService(ApplicationDBContext conexaoBanco, YearsOldValidation yearsValidation)
        {
            _conexaoBanco = conexaoBanco;
            _validation = yearsValidation;
        }
        
        public List<Contato> GetAll() => _conexaoBanco.Contatos.Where((p) => p.IsAtivo == true).ToList();

        public Contato GetPerId(int id)
        {
            var retorno = _conexaoBanco.Contatos.Where(p => p.Id == id && p.IsAtivo == true).FirstOrDefault();
            return retorno;
        }

        public (Contato, string) ActivateOrDeactivate(int id, bool activeOrDeactive)
        {
            var contatoRetorno = new ContatoError();
            
            var contato = _conexaoBanco.Contatos.Where(c => c.Id == id).FirstOrDefault();
            if (contato != null)
            {
                contato.IsAtivo = activeOrDeactive;
                contato.UpdatedAt = DateTime.Now;
                _conexaoBanco.SaveChanges();
                contatoRetorno.Contato = contato;
            }
            else
            {
                contatoRetorno.ErrMsg = "Contato não foi encontrado.";
            }

            return (contatoRetorno.Contato, contatoRetorno.ErrMsg);
        }

        public (Contato, string) Delete(int id)
        {
            var contatoRetorno = new ContatoError();
            try
            {
                var retorno = _conexaoBanco.Contatos.Where(p => p.Id == id).FirstOrDefault();
                if (retorno != null)
                {
                    _conexaoBanco.Contatos.Remove(retorno);
                    contatoRetorno.Contato = retorno;
                    _conexaoBanco.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                contatoRetorno.ErrMsg = "Erro ao incluir os dados! " + ex.Message;
            }
            return (contatoRetorno.Contato, contatoRetorno.ErrMsg);
        }
        
        public (Contato, string) Create(Contato obj)
        {
            var contatoRetorno = new ContatoError();

            obj.Idade = _validation.CalcularIdade(obj);
            var validacaoDados = _validation.ValidarDados(obj);

            if (!validacaoDados.Item1)
                contatoRetorno.ErrMsg = "Erro ao incluir dados! " + validacaoDados.Item2;
            else
            {
                try
                {
                    var contatoNovo = new Contato
                    {
                        Nome = obj.Nome,
                        Sexo = obj.Sexo,
                        DataNascimento = obj.DataNascimento,
                        Idade = obj.Idade,
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now,
                        IsAtivo = obj.IsAtivo,
                    };
                    _conexaoBanco.Add(contatoNovo);
                    _conexaoBanco.SaveChanges();
                    contatoRetorno.Contato = contatoNovo;
                }
                catch (Exception ex)
                {
                    contatoRetorno.ErrMsg = "Erro ao incluir dados! " + ex.Message;
                }
            }
            return (contatoRetorno.Contato, contatoRetorno.ErrMsg);
        }
        
        public (Contato, string) Update(Contato obj)
        {
            var objRetorno = new ContatoError();
            obj.Idade = _validation.CalcularIdade(obj);
            var validacaoDados = _validation.ValidarDados(obj);

            if (!validacaoDados.Item1)
                objRetorno.ErrMsg = "Erro ao editar os dados! " + validacaoDados.Item2;
            else
            {
                try
                {
                    var retorno = _conexaoBanco.Contatos.Where(p => p.Id == obj.Id).FirstOrDefault();
                    retorno.Nome = obj.Nome;
                    retorno.Sexo = obj.Sexo;
                    retorno.Idade = obj.Idade;
                    retorno.UpdatedAt = DateTime.Now;
                    retorno.IsAtivo = obj.IsAtivo;
                    retorno.DataNascimento = obj.DataNascimento;

                    _conexaoBanco.Update(retorno);
                    _conexaoBanco.SaveChanges();
                    objRetorno.Contato = retorno;
                }
                catch (Exception ex)
                {
                    objRetorno.ErrMsg = "Erro ao alterar os dados! " + ex.Message;
                }
            }

            return (objRetorno.Contato, objRetorno.ErrMsg);

        }
    }
}
