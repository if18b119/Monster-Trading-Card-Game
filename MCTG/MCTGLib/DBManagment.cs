using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTGclass
{
    public static class DBManagment
    {
        public static readonly List <User> all_user = new List<User>();
        public static bool CheckLogIn(string name, int pwd)
        {
            return true;
        }

        public static bool AddUser(string name, string pwd,  UserRole role)
        {
            if(role == UserRole.admin)
            {   
                if(!all_user.Any(u => u.UniqueName == name)) //check if name is unique(no one else with the same name)
                {
                    User new_admin = new Admin(name, pwd, UserRole.admin);
                    all_user.Add(new_admin);
                    return true;
                }
                else
                {
                    Console.WriteLine("Username existiert bereits!");
                    return false;
                }
                
            }
            else if(role ==UserRole.player)
            {
                if (all_user.Find(x => x.UniqueName == name) == null)//Check if  name is unique
                {
                    User new_player = new Player(name, pwd, UserRole.player);
                    all_user.Add(new_player);
                    return true;
                }
                else
                {
                    return false;
                }
                   
            }
            else
            {
                return false;
            }

        }


    }
}
