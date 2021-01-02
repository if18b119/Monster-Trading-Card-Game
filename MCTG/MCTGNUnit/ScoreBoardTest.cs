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
    class ScoreBoardTest
    {
        /*
         Type: GET
         Options: /score
         Protocoll: HTTP/1.1
         Authorization: kienboec
         Payload:
      */
        [Test, Order(36)]
        public void ShowScoreBoardInvalidToken()
        {
            RequestKontext req = new RequestKontext("GET", "/stats", "HTTP/1.1", "", "asd");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "401 Unauthorized", "Error: Not logged in or invalid token", "text");
            Assert.AreEqual("401 Unauthorized", reply.Status);
            Assert.AreEqual("Error: User is not logged in / Invalid Token!", reply.Data);
        }

        [Test, Order(37)]
        public void ShowScoreBoard()
        {
            RequestKontext req = new RequestKontext("GET", "/stats", "HTTP/1.1", "", "testadmin");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "200 OK", "reply", "text");
            Assert.AreEqual("200 OK", reply.Status);

        }
    }
}
