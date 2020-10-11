using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTGclass
{
    class Admin : User
    {   
        
        public Admin(string unique_name, string pwd, UserRole role)
        {
            UniqueName = unique_name;
            Pwd = pwd;
            Role = role;
        }

        //noch zu implementieren
        public bool AddUser(string unique_name, string pwd, UserRole role)
        {
            return DBManagment.AddUser(unique_name, pwd, role);
        }



    }
}
