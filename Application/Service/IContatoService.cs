using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Service
{
    public interface IContatoService
    {
        List<Contato> GetAll();

        Contato GetPerId(int id);
        (Contato, string) ActivateOrDeactivate(int id, bool activeOrDeactive);

        (Contato, string) Delete(int id);

        (Contato, string) Create(Contato contato);

        (Contato, string) Update(Contato contato);

    }
}
