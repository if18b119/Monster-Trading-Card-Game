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
    class StoreTest
    {
        /*
           Type: GET
           Options: /score
           Protocoll: HTTP/1.1
           Authorization: kienboec
           Payload:
        */
        [Test, Order(10)]
        public void CreatePackageEmptyPayload()  // No Session Created
        {
            RequestKontext req = new RequestKontext("POST", "/packages", "HTTP/1.1", "", "");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "204 No Content", "Error: No Content", "text");
            Assert.AreEqual("204 No Content", reply.Status);
            Assert.AreEqual("Error: No Content", reply.Data);
        }

        [Test, Order(11)]
        public void CreatePackageNotLoggedIn()  // No Session Created
        {
            //create admin user
            RequestKontext req = new RequestKontext("POST", "/users", "HTTP/1.1", "{\"Username\":\"testadmin\", \"Password\":\"test123\", \"Role\":\"admin\", \"Name\":\"Admin test\", \"Email\":\"testadmin@gmail.com\"}", "");
            ServerReply reply = ServerReply.HandlingRequest(req);
            //
            RequestKontext req2 = new RequestKontext("POST", "/packages", "HTTP/1.1", "[{\"Id\":\"test1\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"test2\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"test3\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"test4\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"test5\", \"Name\":\"FireSpell\", \"Damage\": 25.0}]", "testadmin");
            ServerReply reply2 = ServerReply.HandlingRequest(req2);

            Assert.IsNotNull(reply2);
            //return new ServerReply(req.Protocol, "409 Conflict", "Error: User doesnt have a session / Invalid Token!", "text");
            Assert.AreEqual("409 Conflict", reply2.Status);
            Assert.AreEqual("Error: User doesnt have a session / Invalid Token!", reply2.Data);
        }

        [Test, Order(12)]
        public void CreatePackageNotAdmin()  // NOt an admin
        {
            
            RequestKontext req2 = new RequestKontext("POST", "/packages", "HTTP/1.1", "[{\"Id\":\"test1\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"test2\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"test3\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"test4\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"test5\", \"Name\":\"FireSpell\", \"Damage\": 25.0}]", "test1");
            ServerReply reply2 = ServerReply.HandlingRequest(req2);

            Assert.IsNotNull(reply2);
            //return new ServerReply(req.Protocol, "401 Unauthorized", "Error: You are not authorized!", "text");
            Assert.AreEqual("401 Unauthorized", reply2.Status);
            Assert.AreEqual("Error: You are not authorized!", reply2.Data);
        }

        [Test, Order(13)]
        public void CreatePackage()  // successsfully add cards to store
        {
            //Log in with admin
            RequestKontext req = new RequestKontext("POST", "/sessions", "HTTP/1.1", "{\"Username\":\"testadmin\", \"Password\":\"test123\"}", "");
            ServerReply reply = ServerReply.HandlingRequest(req);
            ////
            
            RequestKontext req2 = new RequestKontext("POST", "/packages", "HTTP/1.1", "[{\"Id\":\"test1\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"test2\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"test3\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"test4\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"test5\", \"Name\":\"FireSpell\", \"Damage\": 25.0}]", "testadmin");
            ServerReply reply2 = ServerReply.HandlingRequest(req2);

            Assert.IsNotNull(reply2);
            //return new ServerReply(req.Protocol, "201 Created", "Created", "text");
            Assert.AreEqual("201 Created", reply2.Status);
            Assert.AreEqual("Created", reply2.Data);
        }

        [Test, Order(14)]
        public void CreatePackageCardsAlreadyInStore()  // should not be added in store cause already exists
        {
            

            RequestKontext req2 = new RequestKontext("POST", "/packages", "HTTP/1.1", "[{\"Id\":\"test1\", \"Name\":\"WaterGoblin\", \"Damage\": 10.0}, {\"Id\":\"test2\", \"Name\":\"Dragon\", \"Damage\": 50.0}, {\"Id\":\"test3\", \"Name\":\"WaterSpell\", \"Damage\": 20.0}, {\"Id\":\"test4\", \"Name\":\"Ork\", \"Damage\": 45.0}, {\"Id\":\"test5\", \"Name\":\"FireSpell\", \"Damage\": 25.0}]", "testadmin");
            ServerReply reply2 = ServerReply.HandlingRequest(req2);

            Assert.IsNotNull(reply2);
            //return new ServerReply(req.Protocol, "409 Conflict", "Error: One or more cards already exists in Store!", "text");
            Assert.AreEqual("409 Conflict", reply2.Status);
            Assert.AreEqual("Error: One or more cards already exists in Store!", reply2.Data);
        }


    }
}
