using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Application.Service;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace APIContatoNasser.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContatoController : Controller
    {
        private readonly IContatoService _contatoService;
        
        public ContatoController(IContatoService contatoService)
        {
            _contatoService = contatoService;
        }
        
        /// <summary>
        /// Retorna todos os contatos existentes no banco.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public ActionResult<List<Contato>> GetAll() => _contatoService.GetAll();

        /// <summary>
        /// Retorna um contato específico, caso o mesmo exista no banco.
        /// </summary>
        /// <param name="id">Id do contato.</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public ActionResult GetPerId(int id)
        {
            var result = _contatoService.GetPerId(id);
            if (result != null)
                return Ok(result);
            
            return NotFound("Registro não encontrado ou inativo.");
        }

        /// <summary>
        /// Ativa ou desativa um contato.
        /// </summary>
        /// <param name="id">Id do contato.</param>
        /// <param name="isActive">Parametro para desativar ou ativar contato.</param>
        /// <returns></returns>
        [HttpGet("changeState/{id}")]
        public ActionResult ChangeState(int id, [FromQuery(Name = "isActive")] bool isActive)
        {
            var result = _contatoService.ActivateOrDeactivate(id, isActive);
            if (result.Item1 != null && String.IsNullOrEmpty(result.Item2))
                return Ok("Estado do contato foi alterado com sucesso.");

            return NotFound(result.Item2);
        }

        /// <summary>
        /// Cria um novo contato.
        /// </summary>
        /// <param name="bodyContato">Body enviado por meio da requisição com os dados do contato.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Post([FromBody] Contato bodyContato)
        {
            var model = _contatoService.Create(bodyContato);

            if (model.Item1 != null && String.IsNullOrEmpty(model.Item2))
                return Ok(model.Item1);
            
            return BadRequest(model.Item2);
        }
        
        /// <summary>
        /// Edita um contato existente
        /// </summary>
        /// <param name="bodyContato">Body enviado por meio da requisição com os dados do contato.</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public ActionResult Put([FromBody] Contato bodyContato, int id)
        {
            bodyContato.Id = id;
            var model = _contatoService.Update(bodyContato);
            if (model.Item1 != null && String.IsNullOrEmpty(model.Item2))
                return Ok(model.Item1);
            
            return BadRequest("Registro não encontrado!");
        }
        
        /// <summary>
        /// Deleta um contato existente.
        /// </summary>
        /// <param name="id">Id do contato.</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public ActionResult Delete(int id)
        {
            var model = _contatoService.Delete(id);

            if (model.Item1 != null && String.IsNullOrEmpty(model.Item2))
                return Ok("Registro excluído com sucesso!");
            
            return BadRequest("Registro não encontrado!" + model.Item2);
        }
    }
}