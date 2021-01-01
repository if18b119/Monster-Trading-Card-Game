using System;
using System.Collections.Generic;
using System.Text;
using  Npgsql;

namespace MCTGclass
{
    public static class DBManagmentShowCards
    {
        public static bool Has_Cards(string i_username)
        {
            int i = 0;
            using var con = new NpgsqlConnection(DBManagment.cs);
            string sql = "Select count (*) from all_user_cards where username = @username";
            using var cmd = new NpgsqlCommand(sql, con);
            con.Open();
            cmd.Parameters.AddWithValue("username", i_username);
            cmd.Prepare();

            using NpgsqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            i = rdr.GetInt32(0);
            if (i > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static string Show_acquired_cards(string i_username)
        {
            try
            {
                if (DBManagment.has_session(i_username) == true)
                {
                    if (Has_Cards(i_username))
                    {
                        string result = "";
                        int i = 0;
                        using var con = new NpgsqlConnection(DBManagment.cs);
                        string sql = "Select * from all_user_cards where username = @username";
                        using var cmd = new NpgsqlCommand(sql, con);
                        con.Open();
                        cmd.Parameters.AddWithValue("username", i_username);
                        cmd.Prepare();

                        using NpgsqlDataReader rdr = cmd.ExecuteReader();
                        while (rdr.Read())
                        {
                            result += i + ") CardID: " + rdr.GetString(4) + " Name: " + rdr.GetString(1) + "Type: " +
                               rdr.GetString(5) + " Element: " + rdr.GetValue(2) + " Damage: " + rdr.GetDouble(3) + "\n";
                            i++;
                        }
                        return result;
                    }
                    else
                    {
                        return "User doesn't have any Cards!";
                    }

                }

                else
                {
                    return "Error: User is not logged in / Invalid Token!";

                }

            }
            catch (Exception e)
            {
                return "Exception caught: " + e;
            }
        }
    }
}
