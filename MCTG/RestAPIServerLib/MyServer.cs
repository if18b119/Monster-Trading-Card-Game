using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.Threading;

namespace RestAPIServerLib
{
    public class MyServer
    {
        private int port; 
        private bool is_connected = false;
        private TcpListener listener;

        //Konstruktor, hier wird Der Port gesetzt und ein TcpListiner Objekt erstellt mit einer beliebigen IPAdresse
        //und einem Port
        public MyServer(int port_ = 10001)
        {
            this.port = port_;
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void StartListening()
        {
            try
            {
                is_connected = true;
                listener.Start(); //Server starts listetning for requests
                while (is_connected)
                {

                    Console.Write("Waiting for a connection... \r\n");
                    TcpClient client = listener.AcceptTcpClient(); // Server akzeptiert einen Request/Client und blockt
                    Console.WriteLine("Connected successfully!\r\n");
                    //ClientHandler(client);
                    Thread thread1 = new Thread(() => ClientHandler(client));
                    thread1.Start();
                    //Diconecting with the CLient
                    //client.Close();
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
            finally
            {
                EndConnection();
            }

        }

        public void EndConnection()
        {
            
                // Stop listening for new clients.
                is_connected = false;
                listener.Stop();
           
        }

        public void ClientHandler(TcpClient client)
        {
            int i;
            String data = "";
            byte[] bytes = new byte[256];
            NetworkStream stream = client.GetStream();
            while ((i = stream.Read(bytes, 0, bytes.Length)) != 0)
            {
                //Translate data bytes to a ASCII string.
                data += System.Text.Encoding.ASCII.GetString(bytes, 0, i);
                
               //Console.WriteLine(data);

                byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                if (!stream.DataAvailable)
                {
                    break;
                }
            }
            RequestKontext req = RequestKontext.GetRequest(data);
            ServerReply reply = ServerReply.HandlingRequest(req);

            using (StreamWriter writer = new StreamWriter(client.GetStream())) //using für Idisposable
            {

                writer.Write($"{reply.Protocoll} {reply.Status}\r\n");
                writer.Write($"Content-Type: {reply.ContentType}\r\n");
                writer.Write($"Content-Length: {reply.Data.Length}\r\n");
                writer.Write("\r\n");
                writer.Write($"{reply.Data}\r\n");
                writer.Write("\r\n\r\n");


            }
        }
    }
}
