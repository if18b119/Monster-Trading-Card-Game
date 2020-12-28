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
           /* Console.WriteLine($"Type: {req.Type}");
            Console.WriteLine($"Request: {req.Options}");
            Console.WriteLine($"Protocoll: {req.Protocol}");

            foreach (HeaderInfo tmp in RequestKontext.HeaderInformation)
            {
                Console.WriteLine($"{tmp.key}:{tmp.value}");
            }*/
            ///
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
            if (frag[1] == "messages" && frag.Length == 2)
            {
                string name;
                string[] messages = Directory.GetFiles(pfad); //geting all messages (with full path)
                //Converting String array into String
                StringBuilder tmp = new StringBuilder();
                foreach (string value in messages)
                {
                   
                    name = value.Replace(pfad,"");
                    tmp.Append("Name: ");
                    tmp.Append(name);
                    tmp.Append("\n");
                    tmp.Append("Nachricht:\n");
                    //getting the message out of the txt file
                    using (var streamReader = new StreamReader(value, Encoding.UTF8))
                    {
                        tmp.Append(streamReader.ReadToEnd());
                    }
                    tmp.Append("\n\n");

                }
                string _return = tmp.ToString();
                return new ServerReply(req.Protocol, "200 OK", _return, "text");
            }
            else if (frag[1] == "messages" && frag.Length == 3 && frag[2]!="") // frag[0]="", frag[1]="messages", frag[3]=integer
            {
                if (Convert.ToInt32(frag[2]) <= Directory.GetFiles(pfad).Length && frag[2] != "" && Convert.ToInt32(frag[2]) > 0)//if the index doesn't pass the number of messages
                {
                    string file_on_index;
                    int index = Convert.ToInt32(frag[2]) - 1;
                    StringBuilder tmp = new StringBuilder();
                    file_on_index = Convert.ToString(Directory.GetFiles(pfad).GetValue(index));
                    tmp.Append("Name: ");
                    tmp.Append(file_on_index.Remove(0, pfad.Length));
                    tmp.Append("\n");
                    tmp.Append("Nachricht:\n");
                    using (var streamReader = new StreamReader(file_on_index, Encoding.UTF8))
                    {
                        tmp.Append(streamReader.ReadToEnd());
                    }
                    

                    string _return = Convert.ToString(tmp);

                    return new ServerReply(req.Protocol, "200 OK", _return, "text");
                }

                else
                {
                    return OuttaRange(req);
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
            if (frag[1] == "messages" && frag.Length == 3)
            {
                if (Convert.ToInt32(frag[2]) <= Directory.GetFiles(pfad).Length && Convert.ToInt32(frag[2]) > 0 && frag[2] != "")
                {
                    if (req.Body[0] != '\n')
                    {
                        string file_on_index;
                        int index = Convert.ToInt32(frag[2]) - 1;
                        file_on_index = Convert.ToString(Directory.GetFiles(pfad).GetValue(index));
                        File.WriteAllText(file_on_index, req.Body);
                        return new ServerReply(req.Protocol, "200 OK", "", "text");
                    }

                    else
                    {
                        return BadRequest(req);
                    }

                }
                else
                {
                    return OuttaRange(req);
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
            if (frag[1] == "messages" && frag.Length == 3)
            {
                if (Convert.ToInt32(frag[2]) <= Directory.GetFiles(pfad).Length && Convert.ToInt32(frag[2]) > 0 && frag[2] != "")//if the index doesn't pass the number of messages
                {
                    int index = Convert.ToInt32(frag[2]) - 1;
                    string file_on_index = Convert.ToString(Directory.GetFiles(pfad).GetValue(index));
                    File.Delete(file_on_index);
                    return new ServerReply(req.Protocol, "200 OK", "", "text");
                }
                else
                {
                    return OuttaRange(req);
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

            return new ServerReply(req.Protocol, "400 Bad Request", "", "text");            
        }

        public static ServerReply OuttaRange(RequestKontext req)
        {
            return new ServerReply(req.Protocol, "416 Range Not Satisfiable", "", "text");
        }
    }
}
