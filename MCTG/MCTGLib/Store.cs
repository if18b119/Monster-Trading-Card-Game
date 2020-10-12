using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTGclass
{
    public static class Store
    {
        static public List<Card> AvailableCards { get; set; }
        static public bool AddCard(Card card)
        {   
            if(card!=null)
            {
                if(AvailableCards.Find(x=> x.ID ==card.ID)==null) //check if the card is not already in the store
                {
                    AvailableCards.Add(card);
                    return true;
                }
                else
                {
                    Console.WriteLine("Card already exists in store!");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Card has a Null Value!");
                return false;
            }
        }

        static public bool RemoveCard(Card card)
        {
            if(card!=null)
            {
                if(AvailableCards.Find(x=> x.ID == card.ID)!=null) //check if the card really exists in store
                {
                    AvailableCards.Remove(card);
                    return true;
                }
                else
                {
                    Console.WriteLine("Card doesn't exist in the store!");
                    return false;
                }
            }
            else
            {
                Console.WriteLine("Card has a Null Value!");
                return false;
            }
        }

        static public List<Card> SellPackage()
        {
            int number_of_cards = AvailableCards.Count();
            List<Card> package = new List<Card>(5);
            Random rnd = new Random();
            int rn = 0;

            if (number_of_cards>5)
            {   
                for(int i=0; i<5; i++)
                {
                    rn = rnd.Next(0, number_of_cards - 1);
                    package.Add(AvailableCards[rn]);
                    Store.RemoveCard(AvailableCards[rn]);
                }
                return package;
            }
            else
            {
                Console.WriteLine("The store doesn't have enough Cards to sell!");
                return null;
            }
        }
    }
}
