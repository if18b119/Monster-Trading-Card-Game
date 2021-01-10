using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace MCTGclass
{
    public static class DBManagmentStats
    {
        public static string Show_stats(string i_username)
        {

            if (DBManagment.has_session(i_username) == true)
            {
                using var con = new NpgsqlConnection(DBManagment.cs);
                string result = "";
                string sql = "Select elo, wins, defeats, draws from game_user where username = @username";
                using var cmd = new NpgsqlCommand(sql, con);
                con.Open();
                cmd.Parameters.AddWithValue("username", i_username);
                cmd.Prepare();

                using NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result += "Elo: " + rdr.GetInt32(0) + " - Wins: " + rdr.GetInt32(1) + " - Defeats: " + rdr.GetInt32(2) + " - Draws: " + rdr.GetInt32(3);
                }
                return result;
            }
            else
            {
                return "Error: User is not logged in / Invalid Token!";
            }

        }
    }
}
