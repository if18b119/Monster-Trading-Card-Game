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


        public static bool Add_cards_to_shop(string i_username, string card_id, string card_name, int damage)
        {
            try
            {
                if (has_session(i_username) == false) //check if token is valid / has session
                {
                    throw new Exception("Not logged in / Invalid token");
                }
                if (Check_admin(i_username) == false) // check if has permission (admin)
                {
                    throw new Exception("Not authorized, please contact an Admin!");
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
                    return true;
                }
                else
                {
                    throw new Exception("Error: Card already exists!");

                }


            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return false;
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
        public static bool Requiere_Card(string i_username)
        {
            try
            {
                if (has_session(i_username) == false) //check if token is valid / has session
                {
                    throw new Exception("Not logged in / Invalid token");
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
                if (player_coins > 5)
                {
                    if (Check_enough_cards_store() == true)
                    {
                        //select the store table and creating random numbers that are smaller than the number of cards in store
                        int number_of_available_cards = 0;  // available cards in store
                        Random rnd = new Random();

                        int rnum = 0;
                        for (int i = 0; i < 4; i++) //5 random numbers are generated and the card with the same rownumer is added to the stack of the player (5 as a package)
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
                            int damage = rdr2.GetInt32(3);
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


                    }
                    else
                    {
                        throw new Exception("Error: not enough Cards in Store!");
                    }
                    if (Decreasing_coins_from_user(i_username) == true)
                    {
                        return true;
                    }
                    else
                    {
                        throw new Exception("Error: can't decrease users coins");
                    }
                }
                else
                {
                    throw new Exception("Error: Not enough Coins!");
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
                return false;
            }
        }



        public static void Show_acquired_cards(string i_username)
        {
            try
            {
                if (has_session(i_username) == true)
                {
                    var cs = "Host=localhost;Port=5433;Username=tarek;Password=123456;Database=MCTG";
                      var con = new NpgsqlConnection(cs);
                    string sql = "Select * from all_user_cards where username = @username";
                      var cmd = new NpgsqlCommand(sql, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("username", i_username);
                    cmd.Prepare();

                      NpgsqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Console.WriteLine("Username: {0}    CardID: {1}   Name: {2}   Type: {3}   Element: {4}   Damage: {5}",
                            rdr.GetString(0), rdr.GetString(4), rdr.GetString(1), rdr.GetString(5), rdr.GetValue(2), rdr.GetInt32(3));
                    }
                }
                else
                {
                    throw new Exception("Error: User is not logged in / Invalid Token!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
        }

        public static void Show_stats(string i_username)
        {
            try
            {
                if (has_session(i_username) == true)
                {
                    var cs = "Host=localhost;Port=5433;Username=tarek;Password=123456;Database=MCTG";
                      var con = new NpgsqlConnection(cs);
                    string sql = "Select elo, wins, defeats, draws from game_user where username = @username";
                      var cmd = new NpgsqlCommand(sql, con);
                    con.Open();
                    cmd.Parameters.AddWithValue("username", i_username);
                    cmd.Prepare();

                      NpgsqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Console.WriteLine("Elo: {0}    Wins: {1}   Defeats: {2}   Draws: {3}",
                            rdr.GetInt32(0), rdr.GetInt32(1), rdr.GetInt32(2), rdr.GetInt32(3));
                    }
                }
                else
                {
                    throw new Exception("Error: User is not logged in / Invalid Token!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
        }

        public static void Show_scoreboard(string i_username)
        {
            try
            {
                if (has_session(i_username) == true)
                {
                    var cs = "Host=localhost;Port=5433;Username=tarek;Password=123456;Database=MCTG";
                      var con = new NpgsqlConnection(cs);
                    string sql = "Select * from scoreboard";
                      var cmd = new NpgsqlCommand(sql, con);
                    con.Open();
                      NpgsqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Console.WriteLine("Username: {0}   Fights: {1}   Wins: {2}   Defeats: {3}   Draws: {4}   Elo: {5} ",
                            rdr.GetString(0), rdr.GetInt32(4), rdr.GetInt32(1), rdr.GetInt32(2), rdr.GetInt32(3), rdr.GetInt32(5));
                    }
                }
                else
                {
                    throw new Exception("Error: User is not logged in / Invalid Token!");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }
        }
    }
}
