using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace MCTGclass
{   
    //variable die den response mit dem log zählt bei 2 mal dann den reset sperren -> reset variable wieder auf 0
    //waitone
    //nach dem waitone dann die response schicken
    public static class FightSystem
    {
        public static string Log { get; set; } = "";
        //public static bool Logrdy { get; set; } = false;
        //set
        //reset
        //wait one
        public static ManualResetEvent ResetEvent { get; } = new ManualResetEvent(false); // wie mutex aber thread übergreifend false -> zu, true -> auf
        public static Queue<User> Warteschlange = new Queue<User>();
        public static int Count_responses { get; set; } = 0;
        public static string winner { get; set; } = "";
        public static string looser { get; set; } = "";

        public static bool draw = false;

        public static void InitFight(User user)
        {
            Warteschlange.Enqueue(user);
            if(Warteschlange.Count % 2 == 0)
            {
                User player1 = Warteschlange.Dequeue();
                User player2 = Warteschlange.Dequeue();
                Startfight(player1, player2);
            }
        }

        public static void Startfight(User player1, User player2)
        {
            int count_of_rounds = 0;
            int count_cards_player1 = 0;
            int count_cards_player2 = 0;
            var rand = new Random();
            int selected_card_player1 = 0;
            int selected_card_player2 = 0;
            double card1_damage = 0;
            double card2_damage = 0;
            while (true)
            {
                count_cards_player1 = player1.Deck.Count;
                count_cards_player2 = player2.Deck.Count;
                
                count_of_rounds++;

                if (count_cards_player1 == 0 || count_cards_player2 == 0 || count_of_rounds == 100)
                {   
                    if(count_cards_player1 == 0)
                    {
                        Log += player1.Username + " doesn't have any more Cards in his Deck to play!\r\n";
                        Log += player2.Username + " won this Battle\r\nCongrats!\r\n";
                        looser = player1.Username;
                        winner = player2.Username;
                        DBManagmentFight.UpdatePlayerStats(player1.Username, "looser");
                        DBManagmentFight.UpdatePlayerStats(player2.Username, "winner");
                    }
                    else if (count_cards_player2 == 0)
                    {
                        Log += player2.Username + " doesn't have any more Cards in his Deck to play!\r\n";
                        Log += player1.Username + " won this Battle\r\nCongrats!\r\n";

                        looser = player2.Username;
                        winner = player1.Username;
                        DBManagmentFight.UpdatePlayerStats(player1.Username, "winner");
                        DBManagmentFight.UpdatePlayerStats(player2.Username, "looser");
                    }
                    else if (count_of_rounds == 100)
                    {
                        Log += "The Fight is been going on for 100 rounds now!\r\nIts a draw gerara here!";
                        draw = true;
                        DBManagmentFight.UpdatePlayerStats(player1.Username, "draw");
                        DBManagmentFight.UpdatePlayerStats(player2.Username, "draw");
                    }
                    break;
                }
               
                selected_card_player1 = rand.Next(0,count_cards_player1);
                selected_card_player2 = rand.Next(0, count_cards_player2);

                Card card1 = player1.Deck[selected_card_player1];
                Card card2 = player2.Deck[selected_card_player2];

                card1_damage = card1.Damage;
                card2_damage = card2.Damage;

                Log += "Round " + count_of_rounds + "\r\n" + card1.Name + " vs " + card2.Name + "\r\n";

                if(card1.Type != "spell" && card2.Type != "spell")
                {
                    Log += "Both Cards are monster. The elements don't have any effect!\r\n";

                    if(card1.Type == "goblin" && card2.Type == "dragon")
                    {
                        Log += player1.Username + "s " + card1.Name + " is too afraid of " + player2.Username + "s " + card2.Name + " and can't attack him\r\n";
                        card1.Damage = 0;
                    }
                    if (card2.Type == "goblin" && card1.Type == "dragon")
                    {
                        Log += player2.Username + "s " + card2.Name + " is too afraid of " + player1.Username + "s " + card1.Name + " and can't attack him\r\n";
                        card2.Damage = 0;
                    }
                    if(card1.Type == "wizzard" && card2.Type == "ork")
                    {
                        Log += player1.Username + "s " + card1.Name + " can controll " + player2.Username + "s " + card2.Name +  " and controls him\r\n";
                        card2.Damage = 0;
                    }
                    if (card2.Type == "wizzard" && card1.Type == "ork")
                    {
                        Log += player2.Username + "s " + card2.Name + " can controll " + player1.Username + "s " + card1.Name + " and controls him\r\n";
                        card1.Damage = 0;
                    }
                    if(card1.Name == "FireElves" && card2.Type == "dragon")
                    {
                        Log += player1.Username + "s " + card1.Name + " knows " + player2.Username + "s " + card2.Name + " since over a thousand years nad can evade his attacks!\r\n";
                        card2.Damage = 0;
                    }
                    if (card2.Name == "FireElves" && card1.Type == "dragon")
                    {
                        Log += player2.Username + "s " + card2.Name + " knows " + player1.Username + "s " + card1.Name + " since over a thousand years nad can evade his attacks!\r\n";
                        card1.Damage = 0;
                    }

                   
                   
                }
                else 
                {
                    if(card1.Type == "knight" && card2.Name == "WaterSpell")
                    {
                        Log += player1.Username + "s " + card1.Name + " is drowning because of " + player2.Username + "s " + card2.Name + " since his armor is too heavy!\r\n";
                        card1.Damage = 0;
                    }
                    if (card2.Type == "knight" && card1.Name == "WaterSpell")
                    {
                        Log += player2.Username + "s " + card2.Name + " is drowning because of " + player1.Username + "s " + card1.Name + " since his armor is too heavy!\r\n";
                        card2.Damage = 0;
                    }
                    if(card1.Type == "kraken" && card2.Type == "spell")
                    {
                        Log += player1.Username + "s " + card1.Name + " is immune against " + player2.Username + "s " + card2.Name + "!\r\n";
                        card2.Damage = 0;
                    }
                    if (card2.Type == "kraken" && card1.Type == "spell")
                    {
                        Log += player2.Username + "s " + card2.Name + " is immune against " + player1.Username + "s " + card1.Name + "!\r\n";
                        card1.Damage = 0;
                    }

                    if (card1.Type == "fire" && card2.Type == "normal")
                    {
                        card1.Damage = card1.Damage * 2;
                        card2.Damage = card2.Damage / 2;
                    }
                    else if (card2.Type == "fire" && card1.Type == "normal")
                    {
                        card2.Damage = card2.Damage * 2;
                        card1.Damage = card1.Damage / 2;
                    }
                    if (card1.Type == "water" && card2.Type == "fire")
                    {
                        card1.Damage = card1.Damage * 2;
                        card2.Damage = card2.Damage / 2;
                    }
                    else if (card2.Type == "water" && card1.Type == "fire")
                    {
                        card2.Damage = card2.Damage * 2;
                        card1.Damage = card1.Damage / 2;
                    }
                    if (card1.Type == "normal" && card2.Type == "water")
                    {
                        card1.Damage = card1.Damage * 2;
                        card2.Damage = card2.Damage / 2;
                    }
                    else if (card2.Type == "normal" && card1.Type == "water")
                    {
                        card2.Damage = card2.Damage * 2;
                        card1.Damage = card1.Damage / 2;
                    }

                }

                if (card1.Damage > card2.Damage)
                {
                    Log += card1.Name + " (" + card1.Damage + ")" + " does more damage than " + card2.Name + " (" + card2.Damage + ")" + "\r\n";
                    Log += player1.Username + " wins round " + count_of_rounds + "\r\n";
                    player2.Deck.Remove(card2);
                    player1.Deck.Add(card2);
                    Log += card2.Name + " has been removed from " + player2.Username + " deck and added to " + player1.Username + "s Deck\r\n";
                }
                else if (card1.Damage < card2.Damage)
                {
                    Log += card2.Name + " (" + card2.Damage + ")" + " does more damage than " + card1.Name + " (" + card1.Damage + ")" + "\r\n";
                    Log += player2.Name + " wins round " + count_of_rounds + "\r\n";
                    player1.Deck.Remove(card1);
                    player2.Deck.Add(card1);
                    Log += card1.Name + " has been removed from " + player1.Username + " deck and added to " + player2.Username + "s Deck\r\n";
                }
                else if (card1.Damage == card2.Damage)
                {
                    Log += card2.Name + " (" + card2.Damage + ")" + " does equal damage to " + card1.Name + " (" + card1.Damage + ")" + "\r\n";
                    Log += "No one wins round " + count_of_rounds + "\r\n";
                }

                
                card1.Damage = card1_damage;
                card2.Damage = card2_damage;
                
            }

            ResetEvent.Set();
        }

    }
}
