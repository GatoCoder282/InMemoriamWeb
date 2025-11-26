using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InMemoriam.Core.Entities;

namespace InMemoriam.Core.Interfaces
{
    public interface ITokenService
    {
        string GenerateToken(User user);
    }
}
