using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTGclass
{
    public class Player : User
    {
        public List<Card> Stack { get; set; } = new List<Card>();
        public List<Card> Deck { get; set; } = new List<Card>(4);

        public List<Card> EDeck { get; set;} = new List<Card>();
        public int Coins { set; get; }
        public int Elo { set; get; }

        public Player(string name, string pwd, UserRole role)
        {
            UniqueName = name;
            Pwd = pwd;
            Role = role;
            Coins = 20;
            Elo = 100;

            DBManagment.AddUser(this);
        }

        public void SellCard(Card sell_card)
        {
            if (sell_card!=null)
            {
                if(Stack.Any(x=> x.ID==sell_card.ID)) //checking if the card exists in users stack
                {
                    if(!Deck.Any(x=> x.ID ==sell_card.ID)) //the card can't be in users deck
                    {
                        Stack.Remove(sell_card);
                        Deck.Remove(sell_card);
                        Coins += 1;
                    }
                    else
                    {
                        throw new Exception("Error -> Card is not allowed to be in the Deck!");
                    }
                }
                else
                {
                    throw new Exception("Error -> You dont own the Card!");
                }
            }
            else
            {
                throw new Exception("Error -> Got a Null Exception!");
            }
        }


        public void AquirePackage()
        {
            List<Card> new_package;
            if (Coins>5)
            {
                new_package= Store.SellPackage();
                for(int i=0; i<new_package.Count(); i++)
                {
                    Stack.Add(new_package[i]);

                }
            }
            else
            {
                throw new Exception("Error -> You Don't own enough Coins!");
            }
            
        }

        public void PrintStack()
        {
            for(int i=0; i<Stack.Count(); i++)
            {
                Console.WriteLine($"{i}.    ID: {Stack[i].ID}   Name: {Stack[i].Name}    Type: {Stack[i].Type}    Elementartype: {Stack[i].EType}   Damage: {Stack[i].Damage}");
            }
        }

        public void ChoseDeck(int index)
        {   
            if(index>0 && index<=Stack.Count())
            {
                index = index - 1;
                Deck.Add(Stack[index]);
            }
            else
            {

                throw new Exception("Error -> Out of Deck's Range!");
            }
            

        }

        public void TakeCard(Card card)
        {
            if(card==null)
            {
                throw new Exception("Error ->  NULL Value!");
            }
            else
            {
                EDeck.Add(card);
            }
        }

        public void GiveCard(Player enemy, Card card)
        {
            if (card == null || enemy == null)
            {
                throw new Exception("Error -> NULL Value!");
            }
            else
            {
                if(Deck.Any(x=> x.ID==card.ID))
                {
                    Deck.Remove(card);
                    enemy.TakeCard(card);
                }
                else
                {
                    throw new Exception("Error -> You don't own the Card!");
                }
            }
            
        }



    }
}
