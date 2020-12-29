using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    class AttributeCounterField
    {
        private string name;
        private int number;
        private string whichClass;

        public string Name { get { return name; } set { name = value; } }
        public int Number { get { return number; } set { number = value; } }
        public string WhichClass { get { return whichClass; } set { whichClass = value; } }

        public AttributeCounterField(string att, string cls)
        {
            Name = att;
            WhichClass = cls;
            Number = 1;
        }

        public int checkAndIncemente(string att, string cls)
        {
            if (!(att.Equals(Name) && cls.Equals(WhichClass)))
            {
                if (att.Equals(Name)) return 1;
                return 0;
            }
            Number++;
            return 2;
        }

        public void printAttribute()
        {
            Console.WriteLine(Name + "\t" + WhichClass + "\t" + Number);
        }
    }
}
