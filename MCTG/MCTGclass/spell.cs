using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTGclass
{
    class Spell: Card
    {   

        public override int Attack()
        {
            return Damage;
        }
    }
}
