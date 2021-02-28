using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Shared.EntityFrameworkCore.Interfaces
{
    public interface IEntityChangeHandler
    {
        void Handle(DbContext context);

        Task HandleAsync(DbContext context);
    }
}