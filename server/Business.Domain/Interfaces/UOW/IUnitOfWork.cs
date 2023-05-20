using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Domain.Interfaces.UOW
{
    public interface IUnitOfWork : IDisposable
    {
        Task<bool> Commit();
    }
}
