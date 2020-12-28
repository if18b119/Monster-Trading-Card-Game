using MCTGclass;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class Tests
    {


        [Test]
        public void CreatingAdmin()  //Check if Admin is Created and if its been added to the DBManagment all_user list
        {
            Admin new_admin = new Admin("Joseph", "123", UserRole.admin);
            Assert.IsNotNull(new_admin);
            Assert.IsNotNull(DBManagment.all_user[0]);
            Assert.AreEqual("Joseph", DBManagment.all_user[0].UniqueName);
        }

        [Test]
        public void CreatingPlayer() //Check if Player is Created and if its been added to the DBManagment all_user list
        {
            Player new_player = new Player("Tarek", "123", UserRole.player);
            Assert.IsNotNull(new_player);
            Assert.IsNotNull(DBManagment.all_user[0]);
            Assert.AreEqual("Tarek", DBManagment.all_user[0].UniqueName);
        }

        [Test]
        public void AdminRemovesUser() //Test if Admin can suc. remove User
        {   

            Admin admin = new Admin("Admin", "123", UserRole.admin);
            Player player = new Player("Tarek", "123", UserRole.player);
            Assert.AreEqual(2, DBManagment.all_user.Count);

            admin.DeleteUser("Tarek");
            Assert.AreEqual(1, DBManagment.all_user.Count);
            Assert.IsFalse(DBManagment.all_user.Contains(player));            
        }

        [Test]
        public void AlreadyExistsUserOrNull() //Check if you can create 2 Users with the same unique name
        {
            Admin admin = new Admin("Tarek", "123", UserRole.admin);
            Assert.Catch<Exception>(()=>admin.AddUser("Tarek","123",UserRole.player));
            Assert.AreEqual(1, DBManagment.all_user.Count);
        }

        [Test]
        public void AddingCardsToStore() //Test if new Cards are added to the store
        {
            Card monster1 = new Monster("Elvis", CardType.monster, ElementarType.fire, MonsterType.FireElv);
            Card monster2 = new Monster("Ginger", CardType.monster, ElementarType.water, MonsterType.Dragon);
            Store.AddCard(monster1);
            Store.AddCard(monster2);
            Assert.AreEqual(2, Store.AvailableCards.Count);
        }
      

        [Test]
        public void UserBuysPackage() //Test if the player can buy a package from store, having enough Coins
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            Card monster1 = new Monster("Elvis", CardType.monster, ElementarType.fire, MonsterType.FireElv);
            Card monster2 = new Monster("Ginger", CardType.monster, ElementarType.water, MonsterType.Dragon);
            Card monster3 = new Monster("Magnum", CardType.monster, ElementarType.fire, MonsterType.Goblin);
            Card monster4 = new Monster("Krampus", CardType.monster, ElementarType.normal, MonsterType.Kraken);
            Card monster5 = new Monster("Ork", CardType.monster, ElementarType.normal, MonsterType.Ork);
            Store.AddCard(monster1);
            Store.AddCard(monster2);
            Store.AddCard(monster3);
            Store.AddCard(monster4);
            Store.AddCard(monster5);
            player1.AquirePackage();
            Assert.AreEqual(5, player1.Stack.Count);                 
        }

        [Test]
        public void UserBuysPackageNotEnoughCoins()// Test if the player can buy a package from store, not having enough Coins
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            Card monster1 = new Monster("Elvis", CardType.monster, ElementarType.fire, MonsterType.FireElv);
            Card monster2 = new Monster("Ginger", CardType.monster, ElementarType.water, MonsterType.Dragon);
            Card monster3 = new Monster("Magnum", CardType.monster, ElementarType.fire, MonsterType.Goblin);
            Card monster4 = new Monster("Krampus", CardType.monster, ElementarType.normal, MonsterType.Kraken);
            Card monster5 = new Monster("Ork", CardType.monster, ElementarType.normal, MonsterType.Ork);
            Store.AddCard(monster1);
            Store.AddCard(monster2);
            Store.AddCard(monster3);
            Store.AddCard(monster4);
            Store.AddCard(monster5);
            player1.Coins = 4;
            
            var ex = Assert.Catch<Exception>(()=> player1.AquirePackage());
            Assert.AreEqual(ex.Message, "Error -> You Don't own enough Coins!");
        }
        [Test]
        public void UserBuysCardNotEnoughCards()//Test if the player can buy a package from store, not having enough cards in store
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            Card monster1 = new Monster("Elvis", CardType.monster, ElementarType.fire, MonsterType.FireElv);
            Card monster2 = new Monster("Ginger", CardType.monster, ElementarType.water, MonsterType.Dragon);
            Store.AddCard(monster1);
            Store.AddCard(monster2);
            var ex = Assert.Catch<Exception>(() => player1.AquirePackage());
            Assert.AreEqual(ex.Message, "Error -> The store doesn't have enough Cards to sell!");
        }

        [Test]
        public void ChoosingCardIntoDeckNotInStack()//Giving INdex of a Card thats not in Stack
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            Card monster1 = new Monster("Elvis", CardType.monster, ElementarType.fire, MonsterType.FireElv);
            Card monster2 = new Monster("Ginger", CardType.monster, ElementarType.water, MonsterType.Dragon);
            player1.Stack.Add(monster1);
            player1.Stack.Add(monster2);

            var exc = Assert.Catch<Exception>(() => player1.ChoseDeck(3));
            Assert.AreEqual(exc.Message, "Error -> Out of Deck's Range!");
        }

        [Test]
        public void ChoosingCardIntoDeck()
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            Card monster1 = new Monster("Elvis", CardType.monster, ElementarType.fire, MonsterType.FireElv);
            Card monster2 = new Monster("Ginger", CardType.monster, ElementarType.water, MonsterType.Dragon);
            player1.Stack.Add(monster1);
            player1.Stack.Add(monster2);

            player1.ChoseDeck(1);
            Assert.AreEqual(player1.Deck[0].Name, "Elvis");
        }

        [Test]
        public void LogIn()//Check login
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            Assert.IsTrue(DBManagment.CheckLogIn("Tarek", "123"));
        }

        [Test]
        public void LogInWrongUsername()
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            var ex = Assert.Catch<Exception>(() => DBManagment.CheckLogIn("Marik", "123"));
            Assert.AreEqual(ex.Message, "Error -> Username doesn't exist!");
        }

        [Test]
        public void LogInWrongPwd()
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);

            var ex = Assert.Catch<Exception>(() => DBManagment.CheckLogIn("Tarek", "124"));
            Assert.AreEqual(ex.Message, "Error -> wrong Password!");
        }

        [Test]
        public void GiveNullCard() //Try giving a null card
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            Player player2 = new Player("Temp", "123", UserRole.player);
            Card monster1 = null;
            player1.Deck.Add(monster1);


            var ex = Assert.Catch<Exception>(() => player1.GiveCard(player2, player1.Deck[0]));
            Assert.AreEqual(ex.Message, "Error -> NULL Value!");
        }

        [Test]
        public void GiveNotOwnedCard() //Try give a card that not yours
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            Player player2 = new Player("Temp", "123", UserRole.player);
            Card monster1 = new Monster("Elvis", CardType.monster, ElementarType.fire, MonsterType.FireElv);

            var ex = Assert.Catch<Exception>(() => player1.GiveCard(player2, monster1));
            Assert.AreEqual(ex.Message, "Error -> You don't own the Card!");
        }

        [Test]
        public void GiveTakeCard() //User who loses a round, give his card to the enemy
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            Player player2 = new Player("Temp", "123", UserRole.player);
            Card monster1 = new Monster("Elvis", CardType.monster, ElementarType.fire, MonsterType.FireElv);
            player1.Deck.Add(monster1);

            player1.GiveCard(player2, monster1);
            Assert.AreEqual(0, player1.Deck.Count);
            Assert.AreEqual(1, player2.EDeck.Count);
            Assert.AreEqual("Elvis", player2.EDeck[0].Name);
        }

        [Test]
        public void GamePlayWithNullPlayer() 
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            Player player2 = null;
            var exc = Assert.Catch<Exception>(() => GamePlay.SetFightAttributes(player1, player2));
            Assert.AreEqual("Error -> Null Type!", exc.Message);
        }

        [Test]
        public void GamePlayWithNotEnoughCards() //one of the player doesnt have enough cards in his Deck
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            Player player2 = new Player("Marek", "123", UserRole.player);
            var exc = Assert.Catch<Exception>(() => GamePlay.SetFightAttributes(player1, player2));
            Assert.AreEqual("Error -> One of the Players doesn't have enough Cards in his Deck!", exc.Message);
        }

        [Test]
        public void GamePlayLogic()
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            Player player2 = new Player("Marek", "123", UserRole.player);
            for(int i=0;i<4;i++)
            {   
                player1.Deck[i] = new Monster("Dragonite "+ i, CardType.monster, ElementarType.water, MonsterType.Dragon);
            }
            
            for(int j=0;j<4;j++)
            {
                player2.Deck[j] = new Monster("Goblin " +j, CardType.monster, ElementarType.fire, MonsterType.Goblin);
            }

            GamePlay.SetFightAttributes(player1, player2);

            Assert.AreEqual(103, player1.Elo);
            Assert.AreEqual(95, player2.Elo);
            Assert.AreEqual(4, player1.Deck.Count);
            Assert.AreEqual(4, player2.Deck.Count);
            Assert.AreEqual(0, player1.EDeck.Count);
            Assert.AreEqual(0, player2.EDeck.Count);
        }
    }
}