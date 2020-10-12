using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTGclass
{
    public static class Store
    {
        static readonly public List<Card> AvailableCards = new List<Card>();
        static public void AddCard(Card card)
        {  

            if(card!=null)
            {
                if(!AvailableCards.Any(x=> x.ID ==card.ID)) //check if the card is not already in the store
                {
                    AvailableCards.Add(card);
                }
                else
                {
                    throw new Exception("Error -> Card already exists in store!");
                    
                }
            }
            else
            {
                throw  new NullReferenceException("Error -> Card has a Null Value!");
            }
        }

        static public void RemoveCard(Card card)
        {
            if(card!=null)
            {
                if(AvailableCards.Find(x=> x.ID == card.ID)!=null) //check if the card really exists in store
                {
                    AvailableCards.Remove(card);
                }
                else
                {
                    throw new Exception("Error -> Card doesn't exist in the store!");
                }
            }
            else
            {
                throw new Exception("Error -> Card has a Null Value!");
            }
        }

        static public List<Card> SellPackage()
        {
            int number_of_cards = AvailableCards.Count;
            List<Card> package = new List<Card>(5);
            Random rnd = new Random();
            int rn = 0;

            if (number_of_cards>=5)
            {   
                for(int i=0; i<5; i++)
                {
                    rn = rnd.Next(0, number_of_cards - 1);
                    package.Add(AvailableCards[rn]);
                    Store.RemoveCard(AvailableCards[rn]);
                    number_of_cards--;
                }
                return package;
            }
            else
            {
                throw new Exception("Error -> The store doesn't have enough Cards to sell!");
                
            }
        }
    }
}
