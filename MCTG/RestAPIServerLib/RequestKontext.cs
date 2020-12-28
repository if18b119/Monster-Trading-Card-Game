using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace RestAPIServerLib
{
    public class RequestKontext
    {
        public String Type { get; set; } = "";
        public String Options { get; set; } = "";
        public String Protocol { get; set; } = "";
        public String Body { get; set; } = "";

        public static List<HeaderInfo> HeaderInformation = new List<HeaderInfo>();
        public RequestKontext(string type, string options, string protocol, string body)
        {
            Type = type;
            Options = options;
            Protocol = protocol;
            Body = body;
        }
        //static Konstruktor, es wird automatisch aufgerufen um die Klasse zu definieren bevor ein Objekt erstellt wird
        public static RequestKontext GetRequest(String request)
        {
            String type = "";
            String options = "";
            String protocol = "";
            String body = "";

            //Sicherzustellen ob der Request nicht null oder leer ist
            try
            {
                if (request == null || request == "")
                {
                    throw new Exception("Error: Request is empty! ");

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
            /*
            OPTIONS /messages HTTP/1.1
            Host: localhost:8080
            Connection: keep-alive
            Accept: *
            Access - Control - Request - Method: GET
            Access - Control - Request - Headers: cache - control,postman - token
            Origin: https://web.postman.co
                        Sec - Fetch - Mode: cors
            Sec - Fetch - Site: cross - site
            Sec - Fetch - Dest: empty
            User - Agent: Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 85.0.4183.121 Safari / 537.36 OPR / 71.0.3770.284
            Accept - Encoding: gzip, deflate, br
            Accept - Language: de - DE,de; q = 0.9,en - US; q = 0.8,en; q = 0.7
            */


            String[] tokens = request.Split("\r\n");

            int index = 0;
            foreach (string line in tokens)
            {
                if (line.Contains(":"))
                {
                    string[] tmp1 = line.Split(':');
                    HeaderInformation.Add(new HeaderInfo(tmp1[0], tmp1[1]));
                }

                else if (line == "")
                {
                    index = Array.FindIndex(tokens, row => row == line);
                    for (int i = index + 1; i<tokens.Length; i++)
                    {
                        body += tokens[i];
                        body += "\n";
                    }
                    break;
                }

            }

            //Die erste Zeile ist in der header ist bei jedem client gleich Type /option /Protocol
            type = tokens[0].Split(' ')[0];
            options = tokens[0].Split(' ')[1];
            protocol = tokens[0].Split(' ')[2].Trim('\r');

            return new RequestKontext(type, options, protocol, body);


            //Die Informationen des Sockets/Client 


        }
    
    }
}
