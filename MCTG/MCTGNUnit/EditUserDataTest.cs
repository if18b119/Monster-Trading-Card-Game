using MCTGclass;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestAPIServerLib;

namespace MCTGNUnit
{
    class EditUserDataTest
    {
        /*
         Type: GET
         Options: /score
         Protocoll: HTTP/1.1
         Authorization: kienboec
         Payload:
      */
        [Test, Order(28)]
        public void ShowPlayerDataInvalidToken()  //invalid token
        {
            RequestKontext req = new RequestKontext("GET", "/users/asdwa", "HTTP/1.1", "",  "asdwa");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "401 Unauthorized", ""Error: User doesn't have a session / invalid Token!"", "text");
            Assert.AreEqual("401 Unauthorized", reply.Status);
            Assert.AreEqual("Error: User doesn't have a session / invalid Token!", reply.Data);
        }

        [Test, Order(29)]
        public void ShowPlayerDataTokenNotSameAsOption()  //invalid token
        {
            RequestKontext req = new RequestKontext("GET", "/users/asdwa", "HTTP/1.1", "", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "400 Bad Request", "Bad Request", "text");
            Assert.AreEqual("400 Bad Request", reply.Status);
            Assert.AreEqual("Bad Request", reply.Data);
        }

        [Test, Order(30)]
        public void ShowPlayerData()  //Ok
        {
            RequestKontext req = new RequestKontext("GET", "/users/testadmin", "HTTP/1.1", "", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "200 OK", result, "text");
            Assert.AreEqual("200 OK", reply.Status);
        }

        [Test, Order(31)]
        public void EditPlayerData()  //Ok
        {
            RequestKontext req = new RequestKontext("PUT", "/users/testadmin", "HTTP/1.1", "{\"Name\": \"Test Admin\",  \"Bio\": \"me playin...\", \"Email\": \"t.admin@gmail.com\"}", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "200 OK", result, "text");
            Assert.AreEqual("200 OK", reply.Status);
        }

        [Test, Order(32)]
        public void EditPlayerDataOfSomeoneElse()  //NOt possible
        {
            RequestKontext req = new RequestKontext("PUT", "/users/testadmin", "HTTP/1.1", "{\"Name\": \"Test Admin\",  \"Bio\": \"me playin...\", \"Email\": \"t.admin@gmail.com\"}", "test1");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "401 Unauthorized", "Error: You can't change the data of someone else!", "text");
            Assert.AreEqual("401 Unauthorized", reply.Status);
            Assert.AreEqual("Error: You can't change the data of someone else!", reply.Data);
        }

        [Test, Order(33)]
        public void EditPlayerInvalidToken()  //INvalid Token
        {
            RequestKontext req = new RequestKontext("PUT", "/users/asd", "HTTP/1.1", "{\"Name\": \"Test Admin\",  \"Bio\": \"me playin...\", \"Email\": \"t.admin@gmail.com\"}", "asd");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "401 Unauthorized", "Error: Not logged in or invalid token", "text");
            Assert.AreEqual("401 Unauthorized", reply.Status);
            Assert.AreEqual("Error: Not logged in or invalid token", reply.Data);
        }

    }
}
