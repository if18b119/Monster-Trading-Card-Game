using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTGclass
{
    class User
    {
        protected string Unique_name { get; set; }
        protected string Pwd { get; set; }
        protected int Id { get; set; }
        protected Role role { get; set; }

        protected User()
        {

        }


    }
}
