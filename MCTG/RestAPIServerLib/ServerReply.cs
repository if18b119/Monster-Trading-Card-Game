using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using MCTGclass;
using Newtonsoft.Json;
namespace RestAPIServerLib
{
    public class ServerReply
    {
        public String Protocoll { get; set; }
        public String Status { get; set; }
        public String Data { get; set; }
        public String ContentType { get; set; }

        private static String pfad;
        




        public ServerReply(string protocoll, string status, string data, string content_type)
        {
            
            Protocoll = protocoll;
            Status = status;
            Data = data;
            ContentType = content_type;

        }

        public static ServerReply HandlingRequest(RequestKontext req)
        {
            pfad = "C:\\Users\\titto\\Desktop\\3.Semester\\Software Engineering\\RestServer\\Restful API Ue1\\Messages\\";
            if (req == null)
            {
                return BadRequest(req);
            }

            //Header Informationen vom  client werden ausgegeben
             Console.WriteLine($"Type: {req.Type}");
            Console.WriteLine($"Request: {req.Options}");
            Console.WriteLine($"Protocoll: {req.Protocol}");
            /*
           foreach (HeaderInfo tmp in RequestKontext.HeaderInformation)
            {
                Console.WriteLine($"{tmp.key}:{tmp.value}");
            }*/
            if (req.Type == "GET")
            {
                return GET(req);
            }

            else if (req.Type == "POST")
            {
                return POST(req);

            }
            else if (req.Type == "DELETE")
            {
                return DELETE(req);
            }
            else if (req.Type == "PUT")
            {
                return PUT(req);
            }
            else
            {
                return new ServerReply(req.Protocol, "405 Method Not Allowed", "", "text"); 
            }
        }

        public static ServerReply GET(RequestKontext req)
        {   
            if(req.Options.Contains("/")==false)    //Get - localhost:123123/messages/1
            {
                return new ServerReply(req.Protocol, "404 Not Found", "", "text");
            }
            string[] frag = req.Options.Split('/'); //---/messages


            if (frag[1] == "cards" && frag.Length == 2)
            {
                string username = req.Authorization;
                string result = DBManagment.Show_acquired_cards(username);
                if (result == "Error: User is not logged in / Invalid Token!")
                {
                    return new ServerReply(req.Protocol, "401 Unauthorized", "Error: Not logged in or invalid token", "text");
                }
                else if (result.Contains("Exception caught: "))
                {
                    return BadRequest(req);
                }
                else
                {
                    return new ServerReply(req.Protocol, "200 OK", result, "text");
                }
            }


            else if (frag[1] == "deck" && frag.Length == 2) 
            {
                string result = DBManagment.Show_Deck(req.Authorization);
                if (result == "No deck found!")//if the index doesn't pass the number of messages
                {                  
                    return new ServerReply(req.Protocol, "200 OK", result, "text");
                }
                else if (result == "Error: User doesn't have a session / invalid Token!")
                {
                    return new ServerReply(req.Protocol, "401 Unauthorized", result, "text");
                }
                else
                {
                    return new ServerReply(req.Protocol, "200 OK", result, "text");
                }
            }


            else if (frag[1] == "users" && frag.Length == 3 && frag[2] != "")
            {
                if(frag[2] == req.Authorization)
                {
                    string result = DBManagment.Show_Players_Data(req.Authorization);
                    if (result == "Error: User doesn't have a session / invalid Token!" )
                    {
                        return new ServerReply(req.Protocol, "401 Unauthorized", result, "text");
                    }
                    else
                    {
                        return new ServerReply(req.Protocol, "200 OK", result, "text");
                    }
                }
                else
                {
                    return BadRequest(req);
                }
            }

            else if (frag[1] == "stats" && frag.Length == 2)
            {
                string result = DBManagment.Show_stats(req.Authorization);
                if(result == "Error: User is not logged in / Invalid Token!")
                {
                    return new ServerReply(req.Protocol, "401 Unauthorized", result, "text");
                }
                else
                {
                    return new ServerReply(req.Protocol, "200 OK", result, "text");
                }
            }

            else if (frag[1] == "score" && frag.Length == 2)
            {
                string result = DBManagment.Show_scoreboard(req.Authorization);
                if(result == "Error: User is not logged in / Invalid Token!")
                {
                    return new ServerReply(req.Protocol, "401 Unauthorized", result, "text");

                }
                else
                {
                    return new ServerReply(req.Protocol, "200 OK", result, "text");
                }
            }

            else if (frag[1] == "tradings" && frag.Length == 2)
            {
                string result = DBManagment.Show_tradings(req.Authorization);
                if(result == "User doesn't have a session / invalid Token!")
                {
                    return new ServerReply(req.Protocol, "401 Unauthorized", result, "text");
                }
                else
                {
                    return new ServerReply(req.Protocol, "200 OK", result, "text");
                }
            }

            else
            {
                return BadRequest(req);
            }
        }
        public static ServerReply POST(RequestKontext req)
        {
            if (req.Options.Contains("/") == false)
            {
                return BadRequest(req);
            }
            string[] frag = req.Options.Split('/');
            if (frag[1] == "users" && frag.Length == 2)
            {
                if (req.Body == "")
                {
                    return new ServerReply(req.Protocol, "204 No Content", "Error: No Content", "text");
                }
                else
                {
                    User new_user = JsonConvert.DeserializeObject<User>(req.Body);
                    int response = DBManagment.AddUser(new_user.Username, new_user.Password, new_user.Role, new_user.Name, new_user.Email);
                    if (response == 0)
                    {
                        return new ServerReply(req.Protocol, "201 Created", "Created", "text");
                    }
                    else if (response == 1)
                    {
                        return new ServerReply(req.Protocol, "406 Not Acceptable", "Error: Username already in use!", "text");
                    }
                    else if (response == 2)
                    {
                        return new ServerReply(req.Protocol, "416 Requested Range Not Satisfiable", "Error: Username min 4 chars, pwd between 6 - 18 chars!", "text");
                    }
                    else
                    {
                        return BadRequest(req);
                    }

                }
            }


            else if (frag[1] == "sessions" && frag.Length == 2)
            {
                if (req.Body == "")
                {
                    return new ServerReply(req.Protocol, "204 No Content", "Error: No Content", "text");
                }
                else
                {
                   User new_user = JsonConvert.DeserializeObject<User>(req.Body);
                   int response = DBManagment.CheckLogIn(new_user.Username, new_user.Password);
                   if(response == 0)
                    {
                        return new ServerReply(req.Protocol, "200 OK", "Logged IN successfully!", "text");
                    }
                   else if (response == 1)
                    {
                        return new ServerReply(req.Protocol, "409 Conflict", "Error: User already logged IN / has a Session!", "text");

                    }
                   else if (response == 2)
                    {
                        return new ServerReply(req.Protocol, "406 Not Acceptable", "Error: Wrong Password", "text");
                    }
                   else if (response == 3)
                    {
                        return new ServerReply(req.Protocol, "404 Not Found", "Error: User doesn't exist!", "text");
                    }
                    else
                    {
                        return BadRequest(req);
                    }
                }

            }

            else if (frag[1] == "packages" && frag.Length == 2)
            {
                if (req.Body == "")
                {
                    return new ServerReply(req.Protocol, "204 No Content", "Error: No Content", "text");
                }
                else
                {
                    string username = req.Authorization; //username from the req object (req got it from the header information)
                    Stack<int> response = new Stack<int>(4); 
                    List<Card> new_cards = JsonConvert.DeserializeObject<List<Card>>(req.Body); //getting the cards from the req body
                    foreach (Card card in new_cards)
                    {
                         response.Push(DBManagment.Add_cards_to_shop(username, card.ID, card.Name,Convert.ToDouble(card.Damage)));
                    }
                    if(!response.Contains(1) && !response.Contains(2) && !response.Contains(3) && !response.Contains(4))
                    {
                        return new ServerReply(req.Protocol, "201 Created", "Created", "text");
                    }
                    if(response.Contains(1))
                    {
                        return new ServerReply(req.Protocol, "409 Conflict", "Error: User already logged IN / has a Session!", "text");
                    }
                    else if(response.Contains(2))
                    {
                        return new ServerReply(req.Protocol, "401 Unauthorized", "Error: You are not authorized!", "text");
                    }
                    else if (response.Contains(3))
                    {
                        return new ServerReply(req.Protocol, "409 Conflict", "Error: One of the cards already exists!", "text");
                    }
                    else
                    {
                        return BadRequest(req);
                    }
                    
                }
            }

            
            else if (frag[1] == "transactions" && frag.Length == 3 && frag[2] == "packages")
            {
                
                
                    string username = req.Authorization;
                    int result = DBManagment.Acquire_Card(username);
                    if (result == 0)
                    {
                        return new ServerReply(req.Protocol, "200 OK", "Cards Acquired", "text");
                    }
                   
                    else if (result == 1)
                    {
                        return new ServerReply(req.Protocol, "401 Unauthorized", "Error: Not logged in or invalid token", "text");
                    }

                    else if (result == 2)
                    {
                        return new ServerReply(req.Protocol, "409 Conflict", "Error: Not enough cards in Store!", "text");
                    }

                    else if (result == 4)
                    {
                        return new ServerReply(req.Protocol, "409 Conflict", "Error: User doesnt have enough money!", "text");

                    }
                    else
                    {
                        return BadRequest(req);
                    }                                                     
            }
            else if (frag[1] == "tradings" && frag.Length == 2)
            {   
                TreadingDeal td = JsonConvert.DeserializeObject<TreadingDeal>(req.Body);
                int result = DBManagment.Create_Trading_Deal(req.Authorization, td);
                if(result == 0)
                {
                    return new ServerReply(req.Protocol, "200 OK", "Treade Created", "text");
                }
                else if (result == 1)
                {
                    return new ServerReply(req.Protocol, "401 Unauthorized", "Error: Not logged in or invalid token", "text");
                }
                else if (result == 2)
                {
                    return new ServerReply(req.Protocol, "409 Conflict", "Error: User doesnt own this card!", "text");
                }
                else
                {
                    return BadRequest(req);
                }
            }


            else
            {
                return BadRequest(req);
            }
        }

        public static ServerReply PUT(RequestKontext req)
        {
            if (req.Options.Contains("/") == false)
            {
                return BadRequest(req);
            }
            string[] frag = req.Options.Split('/');

            if (frag[1] == "deck" && frag.Length == 2)
            {
                List <string> deck_cards = JsonConvert.DeserializeObject<List<string>>(req.Body);
                foreach (string s in deck_cards)
                {
                    Console.WriteLine(s);
                }
                int result = DBManagment.Configure_Deck(req.Authorization, deck_cards);
                if (result == 0)
                {
                    return new ServerReply(req.Protocol, "200 OK", "Deck configured!", "text");
                }
                else if (result == 2)
                {
                    return new ServerReply(req.Protocol, "416 Range Not Satisfiable", "Error: Must be 4 Cards to create or edit deck!", "text");
                }
                else if(result ==1)
                {
                    return new ServerReply(req.Protocol, "401 Unauthorized", "Error: Not logged in or invalid token", "text");
                }
                else if(result == 3)
                {
                    return new ServerReply(req.Protocol, "401 Unauthorized", "Error: One or more Cards are not obtained by the User!", "text");
                }
                else
                {
                    return BadRequest(req);
                }
            }
            else if (frag[1] == "users" && frag.Length == 3 && frag[2] != "")
            {
                if (frag[2] == req.Authorization)
                {
                    User new_data = JsonConvert.DeserializeObject<User>(req.Body);
                    if(DBManagment.Edit_data(req.Authorization, new_data.Name, new_data.Bio, new_data.Email))
                    {
                        return new ServerReply(req.Protocol, "200 OK", "Data configured!", "text");
                    }
                    else
                    {
                        return new ServerReply(req.Protocol, "401 Unauthorized", "Error: Not logged in or invalid token", "text");
                    }
                }
                else
                {
                    return BadRequest(req);
                }
            }
            else
            {
                return BadRequest(req);
            }
        }

        public static ServerReply DELETE(RequestKontext req)
        {
            if (req.Options.Contains("/") == false)
            {
                return BadRequest(req);
            }
            string[] frag = req.Options.Split('/');
           if (frag[1] == "tradings" && frag.Length == 3 && frag[2] != "")
            {
                string card_id = frag[2];
                int result = DBManagment.Delete_Trade(req.Authorization, card_id);
                if(result==0)
                {
                    return new ServerReply(req.Protocol, "200 OK", "Trade deleted", "text");
                }
                else if(result == 1)
                {
                    return new ServerReply(req.Protocol, "401 Unauthorized", "Error: Not logged in or invalid token", "text");
                }
                else if(result == 2)
                {
                    return new ServerReply(req.Protocol, "409 Conflict", "Error: User doesnt own this card!", "text");

                }
                else
                {
                    return BadRequest(req);
                }
            }
            else
            {
                return BadRequest(req);
            }

        }

        public static ServerReply BadRequest(RequestKontext req)
        {   
            if(req == null)
            {
                throw new Exception("Error: NULL!");
            }

            return new ServerReply(req.Protocol, "400 Bad Request", "Bad Request", "text");            
        }

        public static ServerReply OuttaRange(RequestKontext req)
        {
            return new ServerReply(req.Protocol, "416 Range Not Satisfiable", "", "text");
        }
    }
}
