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

        public static void AddUser(User new_user)
        {
            if(new_user!=null)
            {
                if (new_user.Role == UserRole.admin)
                {
                    if (!all_user.Any(u => u.UniqueName == new_user.UniqueName)) //check if name is unique(no one else with the same name)
                    {
                        all_user.Add(new_user);

                    }
                    else
                    {
                        new_user = null;  //deleting the user if it already exists
                        throw new Exception("Error -> Username already in use!");

                    }

                }
                else if (new_user.Role == UserRole.player)
                {
                    if (!all_user.Any(x => x.UniqueName == new_user.UniqueName))//Check if  name is unique
                    {
                        all_user.Add(new_user);
                    }
                    else
                    {
                        new_user=null; //deleting the user if it already exists
                        throw new Exception("Error -> Username already in use!");
                    }

                }
                else
                {
                    throw new Exception("Error -> Unknown Role");
                }
            }

            else
            {
                throw new Exception("Error -> Null type!");
            }
            

        }

        public static void DeleteUser(User user)
        {
            if(user!=null)
            {
                if(all_user.Any(u => u.UniqueName == user.UniqueName))
                {

                    all_user.Remove(user);
                    user = null;

                }
                else
                {
                    throw new Exception("Error -> User doesn't exist!");
                }
            }
            else
            {
                throw new Exception("Error -> Null Type!");
            }
        }

        public static void PrintAllUser()
        {
            for (int i=0;i<all_user.Count(); i++)
            {
                Console.WriteLine($"Name: {all_user[i].UniqueName}");
            }
        }


    }
}
