using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace MCTGclass
{
    public static class DBManagmentUserData
    {
        public static bool Edit_data(string i_username, string i_name, string i_bio, string i_email)
        {
            if (DBManagment.has_session(i_username) == true)
            {
                using var con = new NpgsqlConnection(DBManagment.cs);
                con.Open();
                string sql = "Update game_user set name = @name, bio = @bio, email=@email where username = @username";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", i_username);
                cmd.Parameters.AddWithValue("name", i_name);
                cmd.Parameters.AddWithValue("bio", i_bio);
                cmd.Parameters.AddWithValue("email", i_email);

                cmd.Prepare();
                cmd.ExecuteNonQuery();
                con.Close();
                return true;
            }
            else
            {
                return false;

            }
        }
        public static string Show_Players_Data(string i_username)
        {
            if (DBManagment.has_session(i_username))
            {

                string result = "";
                int i = 1;
                using var con = new NpgsqlConnection(DBManagment.cs);
                string sql = "Select username, name, email, bio from game_user where username = @username";
                using var cmd = new NpgsqlCommand(sql, con);
                con.Open();
                cmd.Parameters.AddWithValue("username", i_username);
                cmd.Prepare();

                using NpgsqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    result += i + ") Username: " + rdr.GetString(0) + " - Name: " + rdr.GetString(1) + " - E-Mail: " +
                       rdr.GetString(2) + " - Bio: " + rdr.GetString(3) + "\r\n";
                    i++;
                }
                return result;
            }
            else
            {
                return ("Error: User doesn't have a session / invalid Token!");
            }
        }
    }
}
