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
    class DeckTest
    {

        /*
          Type: GET
          Options: /score
          Protocoll: HTTP/1.1
          Authorization: kienboec
          Payload:
       */

        [Test, Order(21)]
        public void ShowDeckNoCards() //shows no cards cause user doesnt have any
        {
            RequestKontext req = new RequestKontext("GET", "/deck", "HTTP/1.1", "", "test1");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "200 OK", "Error: No deck found!", "text");
            Assert.AreEqual("200 OK", reply.Status);
            Assert.AreEqual("No deck found!", reply.Data);
        }

        [Test, Order(22)]
        public void ShowDeck() //show all acquired cards
        {
            RequestKontext req = new RequestKontext("GET", "/deck", "HTTP/1.1", "", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "200 OK", "result", "text");
            Assert.AreEqual("200 OK", reply.Status);
        }

        [Test, Order(23)]
        public void ShowDeckInvalidToken()  //user doesnt exist
        {
            RequestKontext req = new RequestKontext("GET", "/deck", "HTTP/1.1", "", "asdwa");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "Error: User doesn't have a session / invalid Token!", result, "text");
            Assert.AreEqual("401 Unauthorized", reply.Status);
            Assert.AreEqual("Error: User doesn't have a session / invalid Token!", reply.Data);
        }


        [Test, Order(24)]
        public void ConfigureDeckInvalidToken()  //invalid token
        {
            RequestKontext req = new RequestKontext("PUT", "/deck", "HTTP/1.1", "[\"test1\", \"test2\", \"test3\", \"test4\"]", "asdwa");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "401 Unauthorized", "Error: Not logged in or invalid token", "text");
            Assert.AreEqual("401 Unauthorized", reply.Status);
            Assert.AreEqual("Error: Not logged in or invalid token", reply.Data);
        }

        [Test, Order(25)]
        public void ConfigureDeckNot4Cards()   //in order to configure deck one must choose 4 cards
        {
            RequestKontext req = new RequestKontext("PUT", "/deck", "HTTP/1.1", "[\"test1\", \"test2\", \"test3\"]", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "416 Range Not Satisfiable", "Error: Must be 4 Cards to create or edit deck!", "text");
            Assert.AreEqual("416 Range Not Satisfiable", reply.Status);
            Assert.AreEqual("Error: Must be 4 Cards to create or edit deck!", reply.Data);
        }

        [Test, Order(26)]
        public void ConfigureDeckAddNotObtainedCard()  //should not go through cause user doesnt own one or more cards that he wanna put in his deck
        {
            RequestKontext req = new RequestKontext("PUT", "/deck", "HTTP/1.1", "[\"test1\", \"test2\", \"test3\", \"test4444\"]", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "401 Unauthorized", "Error: One or more Cards are not obtained by the User!", "text");
            Assert.AreEqual("401 Unauthorized", reply.Status);
            Assert.AreEqual("Error: One or more Cards are not obtained by the User!", reply.Data);
        }

        [Test, Order(27)]
        public void ConfigureDeck()  //should be added to table deck
        {
            RequestKontext req = new RequestKontext("PUT", "/deck", "HTTP/1.1", "[\"test1\", \"test2\", \"test3\", \"test4\"]", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "200 OK", "Deck configured!", "text");
            Assert.AreEqual("200 OK", reply.Status);
            Assert.AreEqual("Deck configured!", reply.Data);
        }
    }
}
