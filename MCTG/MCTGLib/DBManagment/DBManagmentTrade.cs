using System;
using System.Collections.Generic;
using System.Text;
using Npgsql;

namespace MCTGclass
{
    public static class DBManagmentTrade
    {
        public static bool Count_Trading()
        {
            int i = 0;
            using var con = new NpgsqlConnection(DBManagment.cs);
            string sql = "Select count(*) from trading_erea_offer";
            using var cmd = new NpgsqlCommand(sql, con);
            con.Open();
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
        public static string Show_tradings(string i_username)
        {
            if (DBManagment.has_session(i_username))
            {
                if (Count_Trading())
                {
                    string result = "";
                    int i = 1;
                    using var con = new NpgsqlConnection(DBManagment.cs);
                    string sql = "Select trading_erea_offer.username, trading_erea_offer.offer_id, all_user_cards.name, elementar_type, damage, all_user_cards.card_type, min_damage, trading_erea_req.card_type FROM all_user_cards, trading_erea_offer, trading_erea_req where trading_erea_offer.card_id = all_user_cards.card_id and trading_erea_offer.username not in (@username)";
                    using var cmd = new NpgsqlCommand(sql, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("username", i_username);
                    cmd.Prepare();

                    using NpgsqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        result += i + ") TradeID: " + rdr.GetInt32(1) + "    -      Owner: " + rdr.GetString(0) + "\r\n" + "Card to give:\r\n" +
                                    "Name: " + rdr.GetString(2) + "   -   Type: " + rdr.GetString(5) + "   -   Element: " + rdr.GetValue(3) + "   -   Damage: " + rdr.GetDouble(4)
                                    + "\r\n" + "Requested Card: \r\n" + " MinDamage: " + rdr.GetDouble(6) + "   -   Type: " + rdr.GetString(7) + "\r\n";
                        i++;
                    }
                    return result;
                }
                else
                {
                    return ("No tradings available");
                }

            }
            else
            {
                return ("User doesn't have a session / invalid Token!");
            }
        }

        public static bool CheckIfCardNotInDeck(string i_username, string id)
        {
            int count = 0;
            var con = new NpgsqlConnection(DBManagment.cs);
            con.Open();
            //check if user already exists
            var sql_count = "SELECT count (*) from deck where username = @username and card_id = @card_id";
            var cmd = new NpgsqlCommand(sql_count, con);
            //prepared statments
            cmd.Parameters.AddWithValue("username", i_username);
            cmd.Parameters.AddWithValue("card_id", id);
            cmd.Prepare();
            //
            NpgsqlDataReader GetCount = cmd.ExecuteReader(); //curser

            GetCount.Read();
            count = GetCount.GetInt32(0);
            if (count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int Create_Trading_Deal(string i_username, TreadingDeal td)
        {
            if (DBManagment.has_session(i_username))
            {
                if (DBManagment.Has_Specific_Card(i_username, td.CardToTrade) == 1) //check if user owns the card
                {
                    if(CheckIfCardNotInDeck(i_username, td.CardToTrade)==true)
                    {
                        //insert into trading_erea_offer
                        using var con = new NpgsqlConnection(DBManagment.cs);
                        con.Open();
                        string sql = "Insert into trading_erea_offer (offer_id, username, card_id) values (@id, @username, @card_id)";
                        using var cmd = new NpgsqlCommand(sql, con);
                        cmd.Parameters.AddWithValue("username", i_username);
                        cmd.Parameters.AddWithValue("id", td.ID);
                        cmd.Parameters.AddWithValue("card_id", td.CardToTrade);
                        cmd.Prepare();
                        cmd.ExecuteNonQuery();
                        con.Close();


                        //insert into trading_erea_req
                        con.Open();
                        sql = "Insert into trading_erea_req (offer_id, min_damage, card_type) values (@id, @min_damage, @card_type)";
                        using var cmd2 = new NpgsqlCommand(sql, con);
                        cmd2.Parameters.AddWithValue("id", td.ID);
                        cmd2.Parameters.AddWithValue("min_damage", td.MinimumDamage);
                        cmd2.Parameters.AddWithValue("card_type", td.Type);
                        cmd2.Prepare();
                        cmd2.ExecuteNonQuery();
                        con.Close();

                        return 0;
                    }
                    else
                    {
                        // card is in users deck
                        return 3;
                    }
                }
                else
                {
                    //User doesnt own the card he wanna sell
                    return 2;
                }

            }
            else
            {
                //invalid token, doesnt have session
                return 1;
            }
        }

        public static int Has_deal(string i_username, int offer_id)
        {
            int i = 0;
            using var con = new NpgsqlConnection(DBManagment.cs);
            string sql = "Select count(*) from trading_erea_offer where username = @username and offer_id = @offer_id";
            using var cmd = new NpgsqlCommand(sql, con);
            con.Open();
            cmd.Parameters.AddWithValue("username", i_username);
            cmd.Parameters.AddWithValue("offer_id", offer_id);
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

        public static int Delete_Trade(string i_username, int offer_id)
        {
            if (DBManagment.has_session(i_username))
            {
                if (Has_deal(i_username, offer_id) == 1)
                {
                    using var con = new NpgsqlConnection(DBManagment.cs);
                    con.Open();
                    string sql = "DELETE FROM trading_erea_offer where offer_id = @offer_id";
                    using var cmd = new NpgsqlCommand(sql, con);
                    cmd.Parameters.AddWithValue("offer_id", offer_id);
                    cmd.Prepare();
                    cmd.ExecuteNonQuery();
                    con.Close();
                    return 0;
                }
                else
                {
                    //User doesnt own the card he wanna delete from offer
                    return 2;
                }
            }
            else
            {
                //invalid token, doesnt have session
                return 1;
            }
        }

        public static int Check_trade_exists(int trading_id)
        {
            int i = 0;
            using var con = new NpgsqlConnection(DBManagment.cs);
            string sql = "Select count(*) from trading_erea_offer where offer_id = @card_id";
            using var cmd = new NpgsqlCommand(sql, con);
            con.Open();
            cmd.Parameters.AddWithValue("card_id", trading_id);
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


        public static int Trade(string i_username, int trading_id, string card_id)
        {
            if (DBManagment.has_session(i_username))
            {
                if (DBManagment.Has_Specific_Card(i_username, card_id) == 1)
                {
                    if (Check_trade_exists(trading_id) == 1)
                    {
                        //fetching min damage and type from request
                        double req_min_damage = 0;
                        string req_card_type = "";
                        using var con = new NpgsqlConnection(DBManagment.cs);
                        string sql = "Select min_damage, card_type from trading_erea_req where offer_id = @offer_id";
                        using var cmd = new NpgsqlCommand(sql, con);
                        con.Open();
                        cmd.Parameters.AddWithValue("offer_id", trading_id);
                        cmd.Prepare();

                        using NpgsqlDataReader rdr = cmd.ExecuteReader();
                        rdr.Read();
                        req_min_damage = rdr.GetDouble(0);
                        req_card_type = rdr.GetString(1);
                        rdr.Close();
                        ////////////////

                        //Fetching the data from the offered card
                        double offered_card_damage = 0;
                        string offered_card_type = "";
                        string offered_card_id = "";
                        string owner = "";
                        sql = "Select card_type, damage, trading_erea_offer.username, trading_erea_offer.card_id from all_user_cards, trading_erea_offer where trading_erea_offer.offer_id = @offer_id";
                        using var cmd2 = new NpgsqlCommand(sql, con);
                        cmd2.Parameters.AddWithValue("card_id", card_id);
                        cmd2.Parameters.AddWithValue("offer_id", trading_id);

                        cmd2.Prepare();
                        using NpgsqlDataReader rdr2 = cmd2.ExecuteReader();
                        rdr2.Read();
                        offered_card_damage = rdr2.GetDouble(1);
                        offered_card_type = rdr2.GetString(0);
                        owner = rdr2.GetString(2);
                        offered_card_id = rdr2.GetString(3);
                        rdr2.Close();

                        //fetching the card that is given to trade
                        double given_card_damage = 0;
                        string given_card_type = "";
                        sql = "Select damage, card_type from all_user_cards WHERE card_id = @card_id";
                        using var cmd3 = new NpgsqlCommand(sql, con);
                        cmd3.Parameters.AddWithValue("card_id", card_id);
                        cmd3.Prepare();
                        using NpgsqlDataReader rdr3 = cmd3.ExecuteReader();
                        rdr3.Read();
                        given_card_damage = rdr3.GetDouble(0);
                        given_card_type = rdr3.GetString(1);
                        rdr3.Close();
                        if (owner != i_username)
                        {
                            if (given_card_damage > req_min_damage && given_card_type == req_card_type)
                            {
                                sql = "UPDATE all_user_cards set username = @username where card_id = @card_id";
                                using var cmd4 = new NpgsqlCommand(sql, con);
                                cmd4.Parameters.AddWithValue("card_id", card_id);
                                cmd4.Parameters.AddWithValue("username", owner);
                                cmd4.Prepare();
                                cmd4.ExecuteNonQuery();
                                Delete_Trade(owner, trading_id);

                                sql = "UPDATE all_user_cards set username = @username where card_id = @card_id";
                                using var cmd5 = new NpgsqlCommand(sql, con);
                                cmd5.Parameters.AddWithValue("card_id", offered_card_id);
                                cmd5.Parameters.AddWithValue("username", i_username);
                                cmd5.Prepare();
                                cmd5.ExecuteNonQuery();
                                Delete_Trade(owner, trading_id);
                                return 0;

                            }
                            else
                            {
                                return 5;
                                //doesn't have the correct card
                            }
                        }
                        else
                        {
                            //can't trade with yourself
                            return 4;
                        }

                    }
                    else
                    {
                        //trade doesnt exists
                        return 3;
                    }
                }
                else
                {
                    //You dont own the card
                    return 2;
                }
            }
            else
            {
                //doesnt have session / invalid token
                return 1;
            }
        }
    }
}
