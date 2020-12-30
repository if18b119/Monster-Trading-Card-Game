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
        public struct Card_Type_Elemnt // mapper for extracting the type and element type out of a cards name
        {
            public string type;
            public string element;
        }

        static private Card_Type_Elemnt Get_type_element(string card_name) // method for extracting the type and element type out of a cards name
        {
            Card_Type_Elemnt return_card = new Card_Type_Elemnt();
            if (card_name.ToLower().Contains("water") == true)
            {
                return_card.element = "water";
            }
            else if (card_name.ToLower().Contains("fire") == true)
            {
                return_card.element = "fire";
            }
            else if (card_name.ToLower().Contains("normal") == true)
            {
                return_card.element = "normal";
            }
            else
            {
                return_card.element = "normal";
            }
            if (card_name.ToLower().Contains("spell"))
            {
                return_card.type = "spell";
            }
            else if (card_name.ToLower().Contains("goblin"))
            {
                return_card.type = "goblin";
            }
            else if (card_name.ToLower().Contains("dragon"))
            {
                return_card.type = "dragon";
            }
            else if (card_name.ToLower().Contains("wizzard"))
            {
                return_card.type = "wizzard";
            }
            else if (card_name.ToLower().Contains("ork"))
            {
                return_card.type = "ork";
            }
            else if (card_name.ToLower().Contains("knight"))
            {
                return_card.type = "knight";
            }
            else if (card_name.ToLower().Contains("kraken"))
            {
                return_card.type = "kraken";
            }
            else if (card_name.ToLower().Contains("elve"))
            {
                return_card.type = "elve";
            }
            else
            {
                return_card.type = "human";
            }
            return return_card;
        }

        static private bool Check_admin(string i_username)
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

        static private bool has_session(string i_username)
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

        public static int CheckLogIn(string i_name, string i_pwd)
        {
            try
            {
                int count_username = 0;
                  var con = new NpgsqlConnection(cs);
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

        public static int AddUser(string unique_name, string i_pwd, role i_role, string i_name, string i_email)
        {
            try
            {
                if (unique_name.Length >= 4 && i_pwd.Length >= 6 && i_pwd.Length <= 18 && (i_role == role.player || i_role == role.admin))
                {

                    int count_username = 0;
                      var con = new NpgsqlConnection(cs);
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


        public static int Add_cards_to_shop(string i_username, string card_id, string card_name, double damage)
        {
            try
            {
                if (has_session(i_username) == false) //check if token is valid / has session
                {
                    //throw new Exception("Not logged in / Invalid token");
                    return 1;
                }
                if (Check_admin(i_username) == false) // check if has permission (admin)
                {
                    //throw new Exception("Not authorized, please contact an Admin!");
                    return 2;
                }
                int count_cards = 0;
                  var con = new NpgsqlConnection(cs);
                con.Open();

                var sql_count = "SELECT count (*) from store where card_id = @card_id";
                  var cmd = new NpgsqlCommand(sql_count, con);
                //prepared statments
                cmd.Parameters.AddWithValue("card_id", card_id);
                cmd.Prepare();
                //
                  NpgsqlDataReader GetCount = cmd.ExecuteReader(); //curser

                GetCount.Read();
                count_cards = GetCount.GetInt32(0);

                GetCount.Close();




                if (count_cards == 0) //check if name is unique(no one else with the same name)
                {
                    Card_Type_Elemnt new_cte = Get_type_element(card_name);
                    var sql_insert = "Insert into store (card_id, name, card_type, elementar_type, damage) values (@card_id, @name, @card_type, @elementar_type, @damage)";
                      var cmd2 = new NpgsqlCommand(sql_insert, con);
                    //prepared statment
                    cmd2.Parameters.AddWithValue("card_id", card_id);
                    cmd2.Parameters.AddWithValue("name", card_name);
                    cmd2.Parameters.AddWithValue("card_type", new_cte.type);
                    cmd2.Parameters.AddWithValue("elementar_type", (ElementarType)Enum.Parse(typeof(ElementarType), new_cte.element));
                    cmd2.Parameters.AddWithValue("damage", damage);

                    cmd2.Prepare();
                    //
                    cmd2.ExecuteNonQuery();
                    return 0;
                }
                else
                {
                    //throw new Exception("Error: Card already exists!");
                    return 3;

                }


            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return 4;
            }

        }

        private static bool Check_enough_cards_store()
        {
              var con = new NpgsqlConnection(cs);
            string sql = "Select count(*) from store;";
              var cmd = new NpgsqlCommand(sql, con);
            con.Open();
              NpgsqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            int count_cards = rdr.GetInt32(0);
            rdr.Close();
            con.Close();
            if (count_cards >= 4)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static int Get_number_of_card_store()
        {
              var con = new NpgsqlConnection(cs);
            string sql = "Select count(*) from store";
              var cmd = new NpgsqlCommand(sql, con);
            con.Open();
            cmd.Prepare();

              NpgsqlDataReader rdr = cmd.ExecuteReader();
            rdr.Read();
            int count_card = rdr.GetInt32(0);
            rdr.Close();
            con.Close();
            return count_card;
        }

        private static bool Decreasing_coins_from_user(string i_username)
        {
            if (has_session(i_username) == true)
            {
                //getting the players coins
                  var con = new NpgsqlConnection(cs);
                string sql = "Select coins from game_user where username = @username";
                  var cmd = new NpgsqlCommand(sql, con);
                con.Open();
                cmd.Parameters.AddWithValue("username", i_username);
                cmd.Prepare();

                  NpgsqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                int player_coins = rdr.GetInt32(0);
                rdr.Close();

                //updating players coins#
                sql = "update game_user set coins = @coins where username = @username";
                  var cmd2 = new NpgsqlCommand(sql, con);
                cmd2.Parameters.AddWithValue("coins", player_coins - 5);
                cmd2.Parameters.AddWithValue("username", i_username);
                cmd2.Prepare();
                cmd2.ExecuteNonQuery();
                con.Close();
                return true;
            }
            else
            {
                return false;
            }
        }
        public static int Acquire_Card(string i_username)
        {
            try
            {
                if (has_session(i_username) == false) //check if token is valid / has session
                {
                    //throw new Exception("Not logged in / Invalid token");
                    return 1;
                }

                var con = new NpgsqlConnection(cs);
                string sql = "Select coins from game_user where username = @username";
                var cmd = new NpgsqlCommand(sql, con);
                con.Open();
                cmd.Parameters.AddWithValue("username", i_username);
                cmd.Prepare();

                NpgsqlDataReader rdr = cmd.ExecuteReader();
                rdr.Read();
                int player_coins = rdr.GetInt32(0);
                rdr.Close();
                if (player_coins >= 5)
                {
                    if (Check_enough_cards_store() == true)
                    {
                        //select the store table and creating random numbers that are smaller than the number of cards in store
                        int number_of_available_cards = 0;  // available cards in store
                        Random rnd = new Random();

                        int rnum = 0;
                        for (int i = 0; i < 5; i++) //5 random numbers are generated and the card with the same rownumer is added to the stack of the player (5 as a package)
                        {
                            number_of_available_cards = Get_number_of_card_store(); //getting the number of avalable cards each time after deleting one
                            rnum = rnd.Next(number_of_available_cards);


                            sql = "select row_number() OVER (ORDER BY card_id) AS row_id, st.* from store st limit 1 offset @num";
                              var cmd2 = new NpgsqlCommand(sql, con);
                            cmd2.Parameters.AddWithValue("num", rnum);
                              NpgsqlDataReader rdr2 = cmd2.ExecuteReader();

                            //the specific random card
                            rdr2.Read();
                            string card_id = rdr2.GetString(4);
                            string name = rdr2.GetString(1);
                            string card_type = rdr2.GetString(5);
                            ElementarType elementar_type = (ElementarType)Enum.Parse(typeof(ElementarType), Convert.ToString(rdr2.GetValue(2)));
                            double damage = rdr2.GetDouble(3);
                            rdr2.Close();

                            //add card to users stack
                            sql = "insert into all_user_cards (card_id, username, name, card_type, elementar_Type, damage) values (@card_id, @username, @name, @card_type, @elementar_type, @damage)";
                              var cmd3 = new NpgsqlCommand(sql, con);
                            cmd3.Parameters.AddWithValue("card_id", card_id);
                            cmd3.Parameters.AddWithValue("username", i_username);
                            cmd3.Parameters.AddWithValue("name", name);
                            cmd3.Parameters.AddWithValue("card_type", card_type);
                            cmd3.Parameters.AddWithValue("elementar_type", elementar_type);
                            cmd3.Parameters.AddWithValue("damage", damage);
                            cmd3.Prepare();
                            cmd3.ExecuteNonQuery();

                            ////////

                            //delete from store beacause card is already sold
                            sql = "Delete from store where card_id = @id";
                              var cmd4 = new NpgsqlCommand(sql, con);
                            cmd4.Parameters.AddWithValue("id", card_id);

                            cmd4.Prepare();

                            cmd4.ExecuteNonQuery();
                        }
                        if (Decreasing_coins_from_user(i_username) == true)
                        {
                            return 0;
                        }
                        else
                        {
                            //throw new Exception("Error: can't decrease users coins");
                            return 3;
                        }

                    }
                    else
                    {
                        //throw new Exception("Error: not enough Cards in Store!");
                        return 2;
                    }
                   
                }
                else
                {
                    //throw new Exception("Error: Not enough Coins!");
                    return 4;
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return 5;
            }
        }

        public static bool Has_Cards(string i_username)
        {
            int i = 0;
            using var con = new NpgsqlConnection(cs);
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
                if (has_session(i_username) == true)
                {
                    if (Has_Cards(i_username))
                    {
                        string result = "";
                        int i = 0;
                        using var con = new NpgsqlConnection(cs);
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


        public static bool Has_Deck(string i_username)
        {
            int i = 0;
            using var con = new NpgsqlConnection(cs);
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
            if (has_session(i_username))
            {
                if (Has_Deck(i_username))
                {
                    string result = "";
                    int i = 1;
                    using var con = new NpgsqlConnection(cs);
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
                    return ("Error: No deck found!");
                }
            }
            else
            {
                return ("Error: User doesn't have a session / invalid Token!");
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

        public static int Configure_Deck(string i_username, List<string> cards_id)
        {
            if (has_session(i_username))
            {
                Stack<int> has_added_deck = new Stack<int>(){ };  //here it will store the return value of each card from the method has_specific card to see if the player has the card
                if (cards_id.Count == 4)
                {
                    foreach (string card in cards_id)
                    {
                        has_added_deck.Push(Has_Specific_Card(i_username, card));

                    }

                    if (!has_added_deck.Contains(0))
                    {


                        if (!Has_Deck(i_username)) // if he doesn't have a deck, just inserting the 4 cards in the deck
                        {
                            foreach (string card in cards_id)
                            {
                                using var con = new NpgsqlConnection(cs);
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
                            using var con = new NpgsqlConnection(cs);
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
                                using var con2 = new NpgsqlConnection(cs);
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


        public static string Show_Players_Data(string i_username)
        {
            if (has_session(i_username))
            {

                string result = "";
                int i = 1;
                var cs = "Host=localhost;Port=5433;Username=tarek;Password=123456;Database=MCTG";
                using var con = new NpgsqlConnection(cs);
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

        public static bool Edit_data(string i_username, string i_name, string i_bio, string i_email)
        {
            if (has_session(i_username) == true)
            {
                var cs = "Host=localhost;Port=5433;Username=tarek;Password=123456;Database=MCTG";
                using var con = new NpgsqlConnection(cs);
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

        public static string Show_stats(string i_username)
        {
           
                if (has_session(i_username) == true)
                {
                    var cs = "Host=localhost;Port=5433;Username=tarek;Password=123456;Database=MCTG";
                    using var con = new NpgsqlConnection(cs);
                    string result = "";
                    string sql = "Select elo, wins, defeats, draws from game_user where username = @username";
                    using var cmd = new NpgsqlCommand(sql, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("username", i_username);
                    cmd.Prepare();

                    using NpgsqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        result += "Elo: " + rdr.GetInt32(0) +   " - Wins: "  + rdr.GetInt32(1) + " - Defeats: " + rdr.GetInt32(2) +" - Draws: " +  rdr.GetInt32(3);
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
