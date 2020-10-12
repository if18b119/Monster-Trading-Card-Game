using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTGclass
{
    public class Admin : User
    {   
        
        public Admin(string unique_name, string pwd, UserRole role)
        {


            UniqueName = unique_name;
            Pwd = pwd;
            Role = role;

            DBManagment.AddUser(this);
        }

        //noch zu implementieren
        public void AddUser(string unique_name, string pwd, UserRole role)
        {
            
            if(role==UserRole.admin)
            {
                 Admin new_admin = new Admin(unique_name, pwd, role);
                DBManagment.AddUser(new_admin);
            }
            else if(role==UserRole.player)
            {
                Player new_player = new Player(unique_name, pwd, role);
                DBManagment.AddUser(new_player);
            }
            else
            {
                throw new Exception("Error -> User Role not defiened");
            }
             
        }

        public void DeleteUser(string name)
        {
             
            DBManagment.DeleteUser(DBManagment.all_user.Find(x => x.UniqueName == name));
        }

    }
}
