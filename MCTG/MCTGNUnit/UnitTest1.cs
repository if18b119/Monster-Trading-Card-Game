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
        public void AdminRemovesUser()
        {   

            Admin admin = new Admin("Admin", "123", UserRole.admin);
            Player player = new Player("Tarek", "123", UserRole.player);
            Assert.AreEqual(2, DBManagment.all_user.Count);

            admin.DeleteUser("Tarek");
            Assert.AreEqual(1, DBManagment.all_user.Count);
            Assert.IsFalse(DBManagment.all_user.Contains(player));            
        }

        [Test]
        public void AlreadyExistsUserOrNull()
        {
            Admin admin = new Admin("Tarek", "123", UserRole.admin);
            Assert.Catch<Exception>(()=>admin.AddUser("Tarek","123",UserRole.player));
            Assert.AreEqual(1, DBManagment.all_user.Count);
        }

        [Test]
        public void AddingCardsToStore()
        {
            Card monster1 = new Monster("Elvis", CardType.monster, ElementarType.fire, MonsterType.FireElv);
            Card monster2 = new Monster("Ginger", CardType.monster, ElementarType.water, MonsterType.Dragon);
            Store.AddCard(monster1);
            Store.AddCard(monster2);
            Assert.AreEqual(2, Store.AvailableCards.Count);
        }
      

        [Test]
        public void UserBuysPackage()
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
        public void UserBuysPackageNotEnoughCoins()
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
            
            Assert.Catch<Exception>(()=> player1.AquirePackage());
        }
        [Test]
        public void UserBuysCardNotEnoughCards()
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            Card monster1 = new Monster("Elvis", CardType.monster, ElementarType.fire, MonsterType.FireElv);
            Card monster2 = new Monster("Ginger", CardType.monster, ElementarType.water, MonsterType.Dragon);
            Store.AddCard(monster1);
            Store.AddCard(monster2);
            Assert.Catch<Exception>(() => player1.AquirePackage());
            
        }

        [Test]
        public void LogIn()
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            Assert.IsTrue(DBManagment.CheckLogIn("Tarek", "123"));
        }

        [Test]
        public void LogInWrongUsername()
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            Assert.Catch<Exception>(() => DBManagment.CheckLogIn("Marik", "123"));
        }
        [Test]
        public void LogInWrongPwd()
        {
            Player player1 = new Player("Tarek", "123", UserRole.player);
            //Assert.AreEqual(false, DBManagment.CheckLogIn("Tarek", "124"));
            Assert.Catch<Exception>(() => DBManagment.CheckLogIn("Tarek", "124"));
        }

    }
}