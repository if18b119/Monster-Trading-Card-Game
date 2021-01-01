using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace MCTGclass
{
    public static class DBManagmentFight
    {
        public static void UpdatePlayerStats(string i_username, string result)
        {
            if (DBManagment.has_session(i_username) == true)
            {
                //fetching elo, wins, defeats, draws of player
                using var con = new NpgsqlConnection(DBManagment.cs);
                con.Open();
                string sql = "SELECT elo, wins, defeats, draws from game_user where username = @username";
                using var cmd = new NpgsqlCommand(sql, con);
                cmd.Parameters.AddWithValue("username", i_username);
                cmd.Prepare();
                using NpgsqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                int elo = rdr.GetInt32(0);
                int wins = rdr.GetInt32(1);
                int defeats = rdr.GetInt32(2);
                int draws = rdr.GetInt32(3);
                rdr.Close();
                //////////

                string sql2 = "Update game_user set elo = @elo, wins = @wins, defeats=@defeats, draws = @draws where username = @username";
                using var cmd2 = new NpgsqlCommand(sql2, con);
                cmd.Parameters.AddWithValue("username", i_username);
                if (result == "winner")
                {
                    cmd2.Parameters.AddWithValue("username", i_username);
                    cmd2.Parameters.AddWithValue("elo", elo + 3);
                    cmd2.Parameters.AddWithValue("wins", wins + 1);
                    cmd2.Parameters.AddWithValue("defeats", defeats);
                    cmd2.Parameters.AddWithValue("draws", draws);
                }
                else if (result == "looser")
                {
                    cmd2.Parameters.AddWithValue("username", i_username);
                    cmd2.Parameters.AddWithValue("elo", elo - 5);
                    cmd2.Parameters.AddWithValue("wins", wins);
                    cmd2.Parameters.AddWithValue("defeats", defeats + 1);
                    cmd2.Parameters.AddWithValue("draws", draws);
                }
                else if (result == "draw")
                {
                    cmd2.Parameters.AddWithValue("username", i_username);
                    cmd2.Parameters.AddWithValue("elo", elo);
                    cmd2.Parameters.AddWithValue("wins", wins);
                    cmd2.Parameters.AddWithValue("defeats", defeats);
                    cmd2.Parameters.AddWithValue("draws", draws + 1);
                }
                else
                {
                    return;
                }
                cmd2.Prepare();
                cmd2.ExecuteNonQuery();
                con.Close();
                return;

            }
            else
            {
                return;

            }
        }

        public static int ReadyUpForFight(string i_username)
        {
            if (DBManagment.has_session(i_username))
            {
                if (DBManagmentDeck.Has_Deck(i_username))
                {
                    string id = "";
                    string name = "";
                    string type = "";
                    string element = "";
                    double damage = 0;
                    List<Card> cards = new List<Card>();
                    using var con = new NpgsqlConnection(DBManagment.cs);
                    string sql = "Select deck.card_id, name, card_type, elementar_type, damage from deck, all_user_cards where deck.card_id = all_user_cards.card_id and deck.username = @username";
                    using var cmd = new NpgsqlCommand(sql, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("username", i_username);
                    cmd.Prepare();

                    using NpgsqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {

                        id = rdr.GetString(0);
                        name = rdr.GetString(1);
                        type = rdr.GetString(2);
                        element = Convert.ToString(rdr.GetValue(3));
                        damage = rdr.GetDouble(4);
                        Card new_card = new Card
                        {
                            ID = id,
                            Name = name,
                            Damage = damage,
                            Type = type,
                            Element = element
                        };
                        cards.Add(new_card);
                    }
                    User user = new User
                    {
                        Username = i_username,
                        Deck = cards
                    };
                    FightSystem.InitFight(user);
                    FightSystem.ResetEvent.WaitOne();
                    return 0;
                }
                else
                {
                    return 2;
                }
            }
            else
            {
                return 1;
            }
        }
    }
}
