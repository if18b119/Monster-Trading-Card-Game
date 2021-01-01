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
    public class CreateUserTest
    {
        /*
            Type: GET
            Options: /score
            Protocoll: HTTP/1.1
            Authorization: kienboec
            Payload:
         */
        

        [Test, Order(1)]
        public void CreateUser()  //Add successfully
        {
            RequestKontext req = new RequestKontext("POST", "/users", "HTTP/1.1", "{\"Username\":\"test1\", \"Password\":\"test1234\"}", "");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "201 Created", "Created", "text");
            Assert.AreEqual("201 Created", reply.Status);
            Assert.AreEqual("Created", reply.Data);

        }

        [Test, Order(2)]
        public void CreateAlreadyExistingUser() //should fail to create in database
        {
            RequestKontext req = new RequestKontext("POST", "/users", "HTTP/1.1", "{\"Username\":\"test1\", \"Password\":\"test1234\"}", "");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "406 Not Acceptable", "Error: Username already in use!", "text");
            Assert.AreEqual("406 Not Acceptable", reply.Status);
            Assert.AreEqual("Error: Username already in use!", reply.Data);
        }

        [Test, Order(3)]
        public void CreateUserWithInvalidCredential() //should fail to create in database
        {
            RequestKontext req = new RequestKontext("POST", "/users", "HTTP/1.1", "{\"Username\":\"12\", \"Password\":\"\"}", "");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "416 Requested Range Not Satisfiable", "Error: Username min 4 chars, pwd between 6 - 18 chars!", "text");

            Assert.AreEqual("416 Requested Range Not Satisfiable", reply.Status);
            Assert.AreEqual("Error: Username min 4 chars, pwd between 6 - 18 chars!", reply.Data);
        }


        [Test, Order(4)]
        public void CreateUserWithEmptyPayload() //should fail to create in database
        {
            RequestKontext req = new RequestKontext("POST", "/users", "HTTP/1.1", "", "");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "204 No Content", "Error: No Content", "text");

            Assert.AreEqual("204 No Content", reply.Status);
            Assert.AreEqual("Error: No Content", reply.Data);
        }



    }
}