using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTGclass
{
    public class Player : User
    {
        public List<Card> Stack { get; }
        public List<Card> Deck { get; } = new List<Card>(4);

        public List<Card> Edeck { get; } = new List<Card>(4);

        private int Coins { set; get; }
        private int Elo { set; get; }

        public Player(string name, string pwd, UserRole role)
        {
            UniqueName = name;
            Pwd = pwd;
            Role = role;
            Coins = 20;
            Elo = 100;

        }

        public bool SellCard(Card sell_card)
        {
            if (sell_card!=null)
            {
                if(Stack.Find(x=> x.ID==sell_card.ID)!=null) //checking if the card exists in users stack
                {
                    if(Deck.Find(x=> x.ID ==sell_card.ID)==null) //the card can't be in users deck
                    {
                        Stack.Remove(sell_card);
                        Deck.Remove(sell_card);
                        Coins += 1;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        public bool AquirePackage()
        {
            List<Card> new_package;
            if (Coins>5)
            {
                new_package= Store.SellPackage();
                for(int i=0; i<new_package.Count(); i++)
                {
                    Stack.Add(new_package[i]);

                }
                return true;
            }
            else
            {
                Console.WriteLine("You dont have enough coins!");
                return false;
            }
            
        }

        public void PrintStack()
        {
            for(int i=0; i<Stack.Count(); i++)
            {
                Console.WriteLine($"{i}.    ID: {Stack[i].ID}   Name: {Stack[i].Name}    Type: {Stack[i].Type}    Elementartype: {Stack[i].EType}   Damage: {Stack[i].Damage}");
            }
        }

        public bool ChoseDeck(int index)
        {   
            if(index<0 && index<=Stack.Count())
            {
                index = index - 1;
                Deck.Add(Stack[index]);
                return true;
            }
            else
            {   
                
                return false;
            }
            

        }




    }
}
