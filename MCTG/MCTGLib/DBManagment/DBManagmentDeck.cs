using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace MCTGclass
{
    public static class DBManagmentDeck
    {
        public static bool Has_Deck(string i_username)
        {
            int i = 0;
            using var con = new NpgsqlConnection(DBManagment.cs);
            string sql = "Select count(*) from deck where username = @username";
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

        public static string Show_Deck(string i_username)
        {
            if (DBManagment.has_session(i_username))
            {
                if (Has_Deck(i_username))
                {
                    string result = "";
                    int i = 1;
                    using var con = new NpgsqlConnection(DBManagment.cs);
                    string sql = "Select deck.card_id, name, card_type, elementar_type, damage from deck, all_user_cards where deck.card_id = all_user_cards.card_id and deck.username = @username";
                    using var cmd = new NpgsqlCommand(sql, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("username", i_username);
                    cmd.Prepare();

                    using NpgsqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        result += i + ") CardID: " + rdr.GetString(0) + " Name: " + rdr.GetString(1) + "Type: " +
                           rdr.GetString(2) + " Element: " + rdr.GetValue(3) + " Damage: " + rdr.GetDouble(4) + "\r\n";
                        i++;
                    }
                    return result;
                }
                else
                {
                    return ("No deck found!");
                }
            }
            else
            {
                return ("Error: User doesn't have a session / invalid Token!");
            }
        }

        public static int Configure_Deck(string i_username, List<string> cards_id)
        {
            if (DBManagment.has_session(i_username))
            {
                Stack<int> has_added_deck = new Stack<int>() { };  //here it will store the return value of each card from the method has_specific card to see if the player has the card
                if (cards_id.Count == 4)
                {
                    foreach (string card in cards_id)
                    {
                        has_added_deck.Push(DBManagment.Has_Specific_Card(i_username, card));

                    }

                    if (!has_added_deck.Contains(0))
                    {


                        if (!Has_Deck(i_username)) // if he doesn't have a deck, just inserting the 4 cards in the deck
                        {
                            foreach (string card in cards_id)
                            {
                                using var con = new NpgsqlConnection(DBManagment.cs);
                                con.Open();
                                var sql_insert = "Insert into deck (card_id, username) values (@card_id, @username)";
                                using var cmd = new NpgsqlCommand(sql_insert, con);
                                //prepared statment
                                cmd.Parameters.AddWithValue("card_id", card);
                                cmd.Parameters.AddWithValue("username", i_username);
                                cmd.Prepare();
                                //
                                cmd.ExecuteNonQuery();
                                con.Close();

                            }

                            return 0;
                        }
                        else  // if he has a deck, the old one is getting deleted and the new ones are getting inserted
                        {
                            //Delete the old cards
                            using var con = new NpgsqlConnection(DBManagment.cs);
                            con.Open();
                            var sql_delete = "delete from deck where username = @username";
                            using var cmd2 = new NpgsqlCommand(sql_delete, con);
                            //prepared statment
                            cmd2.Parameters.AddWithValue("username", i_username);
                            cmd2.Prepare();
                            //
                            cmd2.ExecuteNonQuery();
                            con.Close();
                            //////////////////////////
                            //insert the new cards
                            foreach (string card in cards_id)
                            {
                                using var con2 = new NpgsqlConnection(DBManagment.cs);
                                con.Open();
                                var sql_insert2 = "Insert into deck (card_id, username) values (@card_id, @username)";
                                using var cmd3 = new NpgsqlCommand(sql_insert2, con);
                                //prepared statment
                                cmd3.Parameters.AddWithValue("card_id", card);
                                cmd3.Parameters.AddWithValue("username", i_username);
                                cmd3.Prepare();
                                //
                                cmd3.ExecuteNonQuery();
                                con.Close();

                            }
                            return 0;
                        }
                    }
                    else
                    {
                        //one or more cards is not obtained by the user
                        return 3;
                    }
                }
                else
                {
                    //must be 4 Cards to create or replace deck!
                    return 2;
                }

            }
            else
            {
                // NO session / invalid Token
                return 1;
            }
        }
    }
}
