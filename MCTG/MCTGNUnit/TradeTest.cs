using MCTGclass;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestAPIServerLib;
using System.Threading;

namespace MCTGNUnit
{
    class TradeTest
    {
        /*
       Type: GET
       Options: /score
       Protocoll: HTTP/1.1
       Authorization: kienboec
       Payload:
        */
      

        [Test, Order(41)]
        public void CheckDealsInvalidToken()
        {
            RequestKontext req = new RequestKontext("GET", "/tradings", "HTTP/1.1", "", "asdw");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "401 Unauthorized", "User doesn't have a session / invalid Token!", "text");
            Assert.AreEqual("401 Unauthorized", reply.Status);
            Assert.AreEqual("User doesn't have a session / invalid Token!", reply.Data);
        }

        [Test, Order(42)]
        public void CheckDeals()
        {
            RequestKontext req = new RequestKontext("GET", "/tradings", "HTTP/1.1", "", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "401 Unauthorized", "User doesn't have a session / invalid Token!", "text");
            Assert.AreEqual("200 OK", reply.Status);
        }

        [Test, Order(43)]
        public void CreateDealWithCardInDeck() //should fail
        {
            RequestKontext req = new RequestKontext("POST", "/tradings", "HTTP/1.1", "{\"Id\": \"1\", \"CardToTrade\": \"test1\", \"Type\": \"spell\", \"MinimumDamage\": 15}", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "409 Conflict", "Error: User is not allowed to have the card in his deck!", "text");
            Assert.AreEqual("409 Conflict", reply.Status);
            Assert.AreEqual("Error: User is not allowed to have the card in his deck!", reply.Data);
        }

        [Test, Order(44)]
        public void CreateDealWithNotObtainedCard() //should fail
        {
            RequestKontext req = new RequestKontext("POST", "/tradings", "HTTP/1.1", "{\"Id\": \"1\", \"CardToTrade\": \"asddwasd-sdgsd\", \"Type\": \"spell\", \"MinimumDamage\": 15}", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "409 Conflict", "Error: User doesnt own this card!", "text");
            Assert.AreEqual("409 Conflict", reply.Status);
            Assert.AreEqual("Error: User doesnt own this card!", reply.Data);
        }

        [Test, Order(45)]
        public void CreateDealWithInvalidToken() //should fail
        {
            RequestKontext req = new RequestKontext("POST", "/tradings", "HTTP/1.1", "{\"Id\": \"1\", \"CardToTrade\": \"test5\", \"Type\": \"spell\", \"MinimumDamage\": 15}", "asdf");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "401 Unauthorized", "Error: Not logged in or invalid token", "text");
            Assert.AreEqual("401 Unauthorized", reply.Status);
            Assert.AreEqual("Error: Not logged in or invalid token", reply.Data);
        }

        [Test, Order(45)]
        public void CreateDeal() 
        {
            RequestKontext req = new RequestKontext("POST", "/tradings", "HTTP/1.1", "{\"Id\": \"1\", \"CardToTrade\": \"test5\", \"Type\": \"spell\", \"MinimumDamage\": 15}", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
         
            Assert.AreEqual("201 Created", reply.Status);
            Assert.AreEqual("Treade Created", reply.Data);
        }

        [Test, Order(46)]
        public void TryTradeWithYourself()
        {
            RequestKontext req = new RequestKontext("POST", "/tradings/1", "HTTP/1.1", "\"test3\"", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "409 Conflict", "Error: Can't trade with yourself!", "text");
            Assert.AreEqual("409 Conflict", reply.Status);
            Assert.AreEqual("Error: Can't trade with yourself!", reply.Data);
        }

        [Test, Order(47)]
        public void AddPackages()
        {
            RequestKontext req2 = new RequestKontext("POST", "/packages", "HTTP/1.1", "[{\"Id\":\"test6\", \"Name\":\"WaterGoblin\", \"Damage\": 40.0}, {\"Id\":\"test7\", \"Name\":\"Dragon\", \"Damage\": 100.0}, {\"Id\":\"test8\", \"Name\":\"WaterSpell\", \"Damage\": 40.0}, {\"Id\":\"test9\", \"Name\":\"Ork\", \"Damage\": 90.0}, {\"Id\":\"test10\", \"Name\":\"FireSpell\", \"Damage\": 50.0}]", "testadmin");
            ServerReply reply2 = ServerReply.HandlingRequest(req2);
            Assert.AreEqual("201 Created", reply2.Status);
        }

        [Test, Order(48)]
        public void BuyPackage()
        {
            RequestKontext req = new RequestKontext("POST", "/transactions/packages", "HTTP/1.1", "", "test1");
            ServerReply reply = ServerReply.HandlingRequest(req);
            Assert.AreEqual("200 OK", reply.Status);
        }

        [Test, Order(49)]
        public void TryTradeWithWrongCard()
        {
            RequestKontext req = new RequestKontext("POST", "/tradings/1", "HTTP/1.1", "\"test6\"", "test1");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "409 Conflict", "Error: This Card doesn't match the required Card!", "text");
            Assert.AreEqual("409 Conflict", reply.Status);
            Assert.AreEqual("Error: This Card doesn't match the required Card!", reply.Data);
        }

        [Test, Order(50)]
        public void TryTradeNonExisitingTrade()
        {
            RequestKontext req = new RequestKontext("POST", "/tradings/1111", "HTTP/1.1", "\"test6\"", "test1");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            // return new ServerReply(req.Protocol, "409 Conflict", "Error: Trade doesn't exists!", "text");
            Assert.AreEqual("409 Conflict", reply.Status);
            Assert.AreEqual("Error: Trade doesn't exists!", reply.Data);
        }

        [Test, Order(51)]
        public void TryTradeNotObtainedCard()
        {
            RequestKontext req = new RequestKontext("POST", "/tradings/1", "HTTP/1.1", "\"test1\"", "test1");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            // return new ServerReply(req.Protocol, "409 Conflict", "Error: User doesnt own this card!", "text");
            Assert.AreEqual("409 Conflict", reply.Status);
            Assert.AreEqual("Error: User doesnt own this card!", reply.Data);
        }

        [Test, Order(52)]
        public void TryTradeInvalidToken()
        {
            RequestKontext req = new RequestKontext("POST", "/tradings/1", "HTTP/1.1", "\"test10\"", "teasdst1");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            // return new ServerReply(req.Protocol, "401 Unauthorized", "Error: Not logged in or invalid token", "text");
            Assert.AreEqual("401 Unauthorized", reply.Status);
            Assert.AreEqual("Error: Not logged in or invalid token", reply.Data);
        }

        [Test, Order(53)]
        public void Trade()
        {
            RequestKontext req = new RequestKontext("POST", "/tradings/1", "HTTP/1.1", "\"test10\"", "test1");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "200 OK", "Treaded successfully!", "text");
            Assert.AreEqual("200 OK", reply.Status);
            Assert.AreEqual("Treaded successfully!", reply.Data);

        }

        [Test, Order(54)]
        public static void LogOut()  //No such User in table game_user, must create user first
        {
            RequestKontext req = new RequestKontext("POST", "/signout", "HTTP/1.1", "", "test1");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "404 Not Found", "Error: User doesn't exist!", "text");
            Assert.AreEqual("200 OK", reply.Status);

        }


    }
}
