using MCTGclass;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestAPIServerLib;
using Npgsql;

namespace MCTGNUnit
{
    public static class SessionTest
    {
        /*
            Type: GET
            Options: /score
            Protocoll: HTTP/1.1
            Authorization: kienboec
            Payload:
         */
       


        [Test, Order(5)]
        public static void LogInWrongPwd()  // No Session Created
        {
            RequestKontext req = new RequestKontext("POST", "/sessions", "HTTP/1.1", "{\"Username\":\"test1\", \"Password\":\"12345567\"}", "");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "406 Not Acceptable", "Error: Wrong Password", "text");
            Assert.AreEqual("406 Not Acceptable", reply.Status);
            Assert.AreEqual("Error: Wrong Password", reply.Data);
        }

        [Test, Order(6)]
        public static void LogIn()  // Log IN successfully / insert data in table session successfully
        {
            RequestKontext req = new RequestKontext("POST", "/sessions", "HTTP/1.1", "{\"Username\":\"test1\", \"Password\":\"test1234\"}", "");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "200 OK", "Logged IN successfully!", "text");
            Assert.AreEqual("200 OK", reply.Status);
            Assert.AreEqual("Logged IN successfully!", reply.Data);
        }

        

        [Test, Order(7)]
        public static void LogInAlreadyHasSession()  // No Session Created, already exists in table session
        {
            RequestKontext req = new RequestKontext("POST", "/sessions", "HTTP/1.1", "{\"Username\":\"test1\", \"Password\":\"test1234\"}", "");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "409 Conflict", "Error: User already logged IN / has a Session!", "text");
            Assert.AreEqual("409 Conflict", reply.Status);
            Assert.AreEqual("Error: User already logged IN / has a Session!", reply.Data);
        }

        [Test, Order(8)]
        public static void LogInUserDoesntExists()  //No such User in table game_user, must create user first
        {
            RequestKontext req = new RequestKontext("POST", "/sessions", "HTTP/1.1", "{\"Username\":\"nothere\", \"Password\":\"test1234\"}", "");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "404 Not Found", "Error: User doesn't exist!", "text");
            Assert.AreEqual("404 Not Found", reply.Status);
            Assert.AreEqual("Error: User doesn't exist!", reply.Data);
        }
        

        [Test, Order(9)]
        public static void LogInEmptyPayload()  //No such User in table game_user, must create user first
        {
            RequestKontext req = new RequestKontext("POST", "/sessions", "HTTP/1.1", "", "");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "204 No Content", "Error: No Content", "text");
            Assert.AreEqual("204 No Content", reply.Status);
            Assert.AreEqual("Error: No Content", reply.Data);
        }


    }
}
