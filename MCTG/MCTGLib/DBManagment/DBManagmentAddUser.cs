using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace MCTGclass
{
    public static class DBManagmentAddUser
    {
        public static int AddUser(string unique_name, string i_pwd, role i_role, string i_name, string i_email)
        {
            try
            {
                if (unique_name.Length >= 4 && i_pwd.Length >= 6 && i_pwd.Length <= 18 && (i_role == role.player || i_role == role.admin))
                {

                    int count_username = 0;
                    var con = new NpgsqlConnection(DBManagment.cs);
                    con.Open();
                    //check if user already exists
                    var sql_count = "SELECT count (*) from game_user where username = @username";
                    var cmd = new NpgsqlCommand(sql_count, con);
                    //prepared statments
                    cmd.Parameters.AddWithValue("username", unique_name);
                    cmd.Prepare();
                    //
                    NpgsqlDataReader GetCount = cmd.ExecuteReader(); //curser

                    GetCount.Read();
                    count_username = GetCount.GetInt32(0);

                    GetCount.Close();
                    if (count_username == 0) //check if name is unique(no one else with the same name)
                    {
                        var sql_insert = "Insert into game_user (username, pwd, name, email, role) values (@username,@pwd,@name,@email, @role)";
                        var cmd2 = new NpgsqlCommand(sql_insert, con);
                        //prepared statment
                        cmd2.Parameters.AddWithValue("username", unique_name);
                        cmd2.Parameters.AddWithValue("pwd", i_pwd);
                        cmd2.Parameters.AddWithValue("name", i_name);
                        cmd2.Parameters.AddWithValue("email", i_email);
                        cmd2.Parameters.AddWithValue("role", i_role);

                        cmd2.Prepare();
                        //
                        cmd2.ExecuteNonQuery();
                        return 0;
                    }
                    else
                    {
                        //throw new Exception("Error: Username already in use!");
                        return 1;
                    }

                }
                else
                {
                    //throw new Exception("Error: Username min 4 chars / pwd between 6 - 18 chars / role player or admin / ");
                    return 2;
                }
            }
            catch (Exception e)
            {
                // Console.WriteLine("{0} Exception caught.", e);
                return 3;
            }

        }
    }
}
