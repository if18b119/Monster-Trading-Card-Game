using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTGclass
{
    public class User
    {
        public string UniqueName { get; set; }
        public string Pwd { get; set; }
        public UserRole Role { get; set; }

        protected User()
        {
        }
        
            
        


    }
}
