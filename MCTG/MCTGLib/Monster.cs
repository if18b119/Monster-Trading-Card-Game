using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MCTGclass
{
    public class Monster : Card
    {
       public MonsterType Mtype { get; set; }
        
       public Monster(string name, CardType type, ElementarType e_type, MonsterType m_type)
        {
            Name = name;
            if(m_type==MonsterType.Dragon)
            {
                Damage = 70;
            }
            else if(m_type == MonsterType.FireElv)
            {
                Damage = 30;
            }
            else if (m_type == MonsterType.Goblin)
            {
                Damage = 20;
            }
            else if (m_type == MonsterType.Kraken)
            {
                Damage = 40;
            }
            else if (m_type == MonsterType.Ork)
            {
                Damage = 50;
            }
            else if (m_type == MonsterType.Wizzard)
            {
                Damage = 50;
            }
            Random rnd = new Random();
            ID = HelperClass.GetTimeStamp(DateTime.Now)+rnd.Next(0,1000);
            Type = type;
            EType = e_type;
        }
       public override  int Attack ()
       {
          return Damage;
       }


    }
}
