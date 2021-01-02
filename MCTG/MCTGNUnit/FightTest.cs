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
    class FightTest
    {
        /*
        Type: GET
        Options: /score
        Protocoll: HTTP/1.1
        Authorization: kienboec
        Payload:
         */
        public User player1 = new User();
        public User player2 = new User();
        [SetUp]

        public void InitPlayer()
        {
            Card card1 = new Card
            {
                Name = "WaterGoblin",
                Type = "goblin",
                Element = "water",
                Damage = 100
            };

            Card card2 = new Card
            {
                Name = "FireDragon",
                Type = "dragon",
                Element = "fire",
                Damage = 100
            };

            Card card3 = new Card
            {
                Name = "FireSpell",
                Type = "spell",
                Element = "fire",
                Damage = 100
            };

            Card card4 = new Card
            {
                Name = "NormalSpell",
                Type = "spell",
                Element = "normal",
                Damage = 100
            };

            Card card5 = new Card
            {
                Name = "FireElve",
                Type = "elve",
                Element = "fire",
                Damage = 10
            };

            Card card6 = new Card
            {
                Name = "FireTroll",
                Type = "troll",
                Element = "fire",
                Damage = 10
            };

            Card card7 = new Card
            {
                Name = "NormalOrk",
                Type = "ork",
                Element = "normal",
                Damage = 10
            };

            Card card8 = new Card
            {
                Name = "WaterGoblin",
                Type = "goblin",
                Element = "water",
                Damage = 10
            };

            this.player1 = new User
            {
                Username = "player1",
                Deck = new List<Card>() { card1, card2, card3, card4}
            };

            this.player2 = new User
            {
                Username = "player2",
                Deck = new List<Card>() { card5, card6, card7, card8 }
            };
        }

        [Test, Order(38)]
        public void BattleInvalidToken()
        {
            RequestKontext req = new RequestKontext("POST", "/battles", "HTTP/1.1", "", "asd");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "401 Unauthorized", "Error: Not logged in or invalid token", "text");
            Assert.AreEqual("401 Unauthorized", reply.Status);
            Assert.AreEqual("Error: Not logged in or invalid token", reply.Data);
        }

        [Test, Order(39)]
        public void BattleWithNoDeck()
        {
            RequestKontext req = new RequestKontext("POST", "/battles", "HTTP/1.1", "", "test1");
            ServerReply reply = ServerReply.HandlingRequest(req);

            Assert.IsNotNull(reply);
            //return new ServerReply(req.Protocol, "409 Conflict", "Error: User doesnt own a deck!", "text");
            Assert.AreEqual("409 Conflict", reply.Status);
            Assert.AreEqual("Error: User doesnt own a deck!", reply.Data);
        }

        [Test, Order(40)]
        public void Battle()
        {
            FightSystem.Startfight(player1, player2);
            //return new ServerReply(req.Protocol, "409 Conflict", "Error: User doesnt own a deck!", "text");
            Assert.AreEqual("player1", FightSystem.winner);
            Assert.AreEqual("player2", FightSystem.looser);
        }



    }
}
