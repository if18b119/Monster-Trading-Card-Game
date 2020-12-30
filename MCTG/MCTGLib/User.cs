using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTGclass
{
    public class User
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
        public role Role { get; set; } = role.player;
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
        public string Bio { get; set; } = "";

    }
}
