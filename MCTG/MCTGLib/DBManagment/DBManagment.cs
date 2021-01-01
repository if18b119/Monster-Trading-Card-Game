using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Npgsql;



namespace MCTGclass
{
    public static class DBManagment
    {

        public static string cs = "Host=localhost;Port=5433;Username=tarek;Password=123456;Database=MCTG";
       
        static public bool Check_admin(string i_username)
        {
              var con = new NpgsqlConnection(cs);
            con.Open();
            string sql = "Select role from game_user where username = @username";
              var cmd = new NpgsqlCommand(sql, con);
            cmd.Parameters.AddWithValue("username", i_username);
            cmd.Prepare();

              NpgsqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            string role = Convert.ToString(rdr.GetValue(0));
            rdr.Close();
            con.Close();
            if (role.ToLower() == "admin")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static public bool has_session(string i_username)
        {
            var con = new NpgsqlConnection(cs);
            string sql = "Select count(*) from session where username = @username";
              var cmd = new NpgsqlCommand(sql, con);
            con.Open();
            cmd.Parameters.AddWithValue("username", i_username);
            cmd.Prepare();

              NpgsqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            int count_session = rdr.GetInt32(0);
            rdr.Close();
            con.Close();

            if (count_session == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

            

        public static string Show_scoreboard(string i_username)
        {
           
           if (has_session(i_username) == true)
           {
                var con = new NpgsqlConnection(cs);
                string result = "";
                string sql = "Select * from scoreboard";
                var cmd = new NpgsqlCommand(sql, con);
                con.Open();
                NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result += "Username: " + rdr.GetString(0) +  " - Fights: " + rdr.GetInt32(4) + " - Wins: " + rdr.GetInt32(1) +
                            " - Defeats: " + rdr.GetInt32(2) + " - Draws: " + rdr.GetInt32(3) +  " - Elo: " + rdr.GetInt32(5) + "\r\n";
                }
                return result;
           }
           else
           {
                return "Error: User is not logged in / Invalid Token!";
           }           
           
        }


        public static int Has_Specific_Card(string i_username, string card_id)
        {
            int i = 0;
            using var con = new NpgsqlConnection(cs);
            string sql = "Select count(*) from all_user_cards where username = @username and card_id = @card_id";
            using var cmd = new NpgsqlCommand(sql, con);
            con.Open();
            cmd.Parameters.AddWithValue("username", i_username);
            cmd.Parameters.AddWithValue("card_id", card_id);
            cmd.Prepare();

            using NpgsqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            i = rdr.GetInt32(0);
            if (i > 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

    }
}
