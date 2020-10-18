using System;
using System.Collections.Generic;
using System.Text;

namespace MCTGclass
{
    public static class GamePlay
    {
        
        public static void SetFightAttributes(Player player1, Player player2)
        {
            Random rnd = new Random();           
            List<Card> player1_deck = new List<Card>();
            List<Card> player2_deck = new List<Card>();
            int deck_index_player1 = 0;
            int deck_index_player2 = 0;
            int who_starts = 0;

            if (player1 != null && player2 != null)
            {   
                if(player1.Deck.Count <4 || player2.Deck.Count <4)
                {
                    throw new Exception("Error -> One of the Players doesn't have enough Cards in his Deck!");
                }
                else
                {
                    who_starts = rnd.Next(1, 2);

                    if (who_starts == 1)
                    {
                        Console.WriteLine($"{player1.UniqueName} goes first!");
                    }

                    else if (who_starts == 2)
                    {
                        Console.WriteLine($"{player2.UniqueName} goes first!");
                    }

                    //THE FIGHT
                    for (int i = 0; i < 101; i++)
                    {
                        if (player1.Deck.Count == 0)
                        {
                            Console.WriteLine($"{player1.UniqueName} won the Game!");
                            player1.Deck.AddRange(player2.EDeck);
                            player2.EDeck.Clear();
                            player1.EDeck.Clear();
                            player1.Elo += 3;
                            player2.Elo -= 5;
                            return;
                        }

                        if (player2.Deck.Count == 0)
                        {
                            Console.WriteLine($"{player2.UniqueName} won the Game!");
                            player2.Deck.AddRange(player1.EDeck);
                            player1.EDeck.Clear();
                            player2.EDeck.Clear();
                            player2.Elo += 3;
                            player1.Elo -= 5;
                            return;
                        }

                        if (i == 100)
                        {
                            Console.WriteLine("The Round is been going for too long!");
                            player1.Deck.AddRange(player2.EDeck);
                            player2.Deck.AddRange(player1.EDeck);
                            player1.EDeck.Clear();
                            player2.EDeck.Clear();
                            return;
                        }
                        player1_deck.Clear();
                        player1_deck = player1.Deck;
                        player1_deck.AddRange(player1.EDeck);
                        player2_deck.Clear();
                        player2_deck = player2.Deck;
                        player2_deck.AddRange(player2.EDeck);

                        //Attacking
                        switch (who_starts)
                        {
                            case 1:
                                deck_index_player1 = rnd.Next(0, player1.Deck.Count - 1);
                                deck_index_player2 = rnd.Next(0, player2.Deck.Count - 1);
                                CompareCards(player1, player2, player1.Deck[deck_index_player1], player2.Deck[deck_index_player2]);
                                who_starts = 2;
                                break;
                            case 2:
                                deck_index_player2 = rnd.Next(0, player2.Deck.Count - 1);
                                deck_index_player1 = rnd.Next(0, player1.Deck.Count - 1);
                                CompareCards(player2, player1, player2.Deck[deck_index_player2], player1.Deck[deck_index_player1]);
                                who_starts = 1;
                                break;

                            default:
                                throw new Exception("Error -> Something went wrong Choosing the Cards!");

                        }
                    }
                }
                                             
            }
            else
            {
                throw new Exception("Error -> Null Type!");
            }           
        }
        public static void CompareCards(Player attacker, Player defender, Card attacking_card, Card defending_card)
        {
            if(attacking_card.Attack()>defending_card.Attack())
            {
                defender.GiveCard(attacker, defending_card);
                return;
            }

            else if(attacking_card.Attack() < defending_card.Attack())
            {
                attacker.GiveCard(defender, attacking_card);
                return;
            }

            else if(attacking_card.Attack() == defending_card.Attack())
            {
                attacker.GiveCard(defender, attacking_card);
            }
        }
    }

   
}
