using Business.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Domain.Interfaces.Application
{
    public interface ITokenApplication
    {
        Task<User> GenerateJWT(User u, IdentityConfig c);
    }
}
