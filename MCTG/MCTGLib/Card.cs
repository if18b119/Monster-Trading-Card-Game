using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTGclass
{
    public abstract class Card
    {
        public int ID { get; set; }
        public int Damage { get; set; }
        public string Name { get; set; }
        public CardType Type { get; set; }
        public ElementarType EType { get; set; }
        abstract public int Attack();

    }
}
