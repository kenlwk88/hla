using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HLA.Backend.Core.Domain.Auth
{
    public class AuthConfig
    {      
        public List<Clients> Auth { get; set; } = new List<Clients>();
    }
    public class Clients 
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
    }
}
