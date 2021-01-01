using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace MCTGclass
{
    public static class DBManagmentLogIn
    {
        public static int CheckLogIn(string i_name, string i_pwd)
        {
            try
            {
                int count_username = 0;
                var con = new NpgsqlConnection(DBManagment.cs);
                con.Open();

                var sql = "SELECT count (*) from game_user where username = @username";
                var cmd = new NpgsqlCommand(sql, con);

                cmd.Parameters.AddWithValue("username", i_name);
                cmd.Prepare();

                NpgsqlDataReader rdr = cmd.ExecuteReader();

                rdr.Read();
                count_username = rdr.GetInt32(0);

                rdr.Close();

                if (count_username != 0)
                {
                    //Check if User already has a Session / is logged in
                    sql = "Select count(*) from session where username = @username";
                    var cmd2 = new NpgsqlCommand(sql, con);
                    cmd2.Parameters.AddWithValue("username", i_name);
                    cmd2.Prepare();

                    NpgsqlDataReader rdr2 = cmd2.ExecuteReader();
                    rdr2.Read();
                    int count_session = rdr2.GetInt32(0);
                    rdr2.Close();

                    if (count_session > 0)
                    {
                        //throw new Exception("Error: User already logged in");
                        return 1;
                    }
                    ///////////////////////


                    //Check if pwd is correct
                    sql = "SELECT pwd from game_user where username = @username";
                    var cmd3 = new NpgsqlCommand(sql, con);

                    cmd3.Parameters.AddWithValue("username", i_name);
                    cmd3.Prepare();

                    NpgsqlDataReader rdr3 = cmd3.ExecuteReader();
                    rdr3.Read();
                    string pwd = rdr3.GetString(0);
                    rdr3.Close();

                    if (pwd == i_pwd)
                    {
                        //creating a session by inserting into session table
                        var sql_insert = "Insert into session (username) values (@username)";
                        var cmd4 = new NpgsqlCommand(sql_insert, con);
                        //prepared statment
                        cmd4.Parameters.AddWithValue("username", i_name);
                        cmd4.Prepare();
                        //
                        cmd4.ExecuteNonQuery();
                        con.Close();
                        return 0;
                    }
                    else
                    {
                        con.Close();
                        //throw new Exception("Error -> wrong Password!");
                        return 2;
                    }
                }
                else
                {
                    con.Close();
                    //throw new Exception("Error -> Username doesn't exist!");
                    return 3;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);

                return 4;
            }

        }
    }
}
