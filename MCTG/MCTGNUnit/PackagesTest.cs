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
    class PackagesTest
    {
        /*
          Type: GET
          Options: /score
          Protocoll: HTTP/1.1
          Authorization: kienboec
          Payload:
       */
        [Test, Order(15)]
        public void AcquirePackageNotLoggedIn()  // should not aqcuire package cause not logged in
        {


            RequestKontext req = new RequestKontext("POST", "/transactions/packages", "HTTP/1.1", "", "aws");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "401 Unauthorized", "Error: Not logged in or invalid token", "text");
            Assert.AreEqual("401 Unauthorized", reply.Status);
            Assert.AreEqual("Error: Not logged in or invalid token", reply.Data);
        }

        [Test, Order(16)]
        public void AcquirePackage()  // successfully bought package , added to players stack
        {         
            RequestKontext req = new RequestKontext("POST", "/transactions/packages", "HTTP/1.1", "", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);
            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "200 OK", "Cards Acquired", "text");
            Assert.AreEqual("200 OK", reply.Status);
            Assert.AreEqual("Cards Acquired", reply.Data);
        }

        [Test, Order(17)]
        public void AcquirePackageNotEnoughCards()  // not enough cards in store to buy
        {
            RequestKontext req = new RequestKontext("POST", "/transactions/packages", "HTTP/1.1", "", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "409 Conflict", "Error: Not enough cards in Store!", "text");
            Assert.AreEqual("409 Conflict", reply.Status);
            Assert.AreEqual("Error: Not enough cards in Store!", reply.Data);
        }

        [Test, Order(18)]
        public void AcquirePackageNotEnoughCoins()  // User doesnt have enough coins to buy package
        {
            DBManagmentPackages.Decreasing_coins_from_user("testadmin");
            DBManagmentPackages.Decreasing_coins_from_user("testadmin");
            DBManagmentPackages.Decreasing_coins_from_user("testadmin");

            RequestKontext req = new RequestKontext("POST", "/transactions/packages", "HTTP/1.1", "", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "409 Conflict", "Error: User doesnt have enough money!", "text");
            Assert.AreEqual("409 Conflict", reply.Status);
            Assert.AreEqual("Error: User doesnt have enough money!", reply.Data);
        }

        [Test, Order(19)]
        public void ShowCards() //show all acquired cards
        {
            RequestKontext req = new RequestKontext("GET", "/cards", "HTTP/1.1", "", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);
            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "200 OK", result, "text");
            Assert.AreEqual("200 OK", reply.Status);
        }

        [Test, Order(20)]
        public void ShowCardsInvalidToken() //show all acquired cards
        {
            RequestKontext req = new RequestKontext("GET", "/cards", "HTTP/1.1", "", "");
            ServerReply reply = ServerReply.HandlingRequest(req);
            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "401 Unauthorized", "Error: Not logged in or invalid token", "text");
            Assert.AreEqual("401 Unauthorized", reply.Status);
            Assert.AreEqual("Error: Not logged in or invalid token", reply.Data);
        }
    }
}
