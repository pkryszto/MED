using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    class AttributeCounter
    {
        private List<AttributeCounterField> attributes;
        private int diffValues;

        public List<AttributeCounterField> Attributes { get { return attributes; } set { attributes = value; } }
        public int DiffValues { get { return diffValues; } set { diffValues = value; } }

        public AttributeCounter()
        {
            attributes = new List<AttributeCounterField>();
            DiffValues = 0;
        }

        public void addAttribute(string name, string attributeClass)
        {
            int result=0;
            bool diffUp = true;
            foreach (var v in Attributes)
            {
                result = v.checkAndIncemente(name, attributeClass);
                if (result == 2) return;
                if (result == 1) diffUp = false;
            }
            AttributeCounterField toAdd = new AttributeCounterField(name, attributeClass);
            Attributes.Add(toAdd);
            if (diffUp) DiffValues++;
        }

        public void printAttribute(string header)
        {
            Console.WriteLine(header + "\t("+DiffValues+" different values)");
            foreach (var v in Attributes) v.printAttribute();
        }

        public int returnNumberForOneClass(string att, string cls)
        {
            var toReturn = Attributes.Find(x => x.Name.Equals(att) && x.WhichClass.Equals(cls));
            if (toReturn == null) return 0;
            return toReturn.Number;
        }

        public int returnNumberForAllClasses(string att)
        {
            var toReturn = Attributes.FindAll(x => x.Name.Equals(att));
            if (toReturn == null) return 0;
            int result = 0;
            foreach (var v in toReturn) result += v.Number;
            return result;
        }

    }
}
