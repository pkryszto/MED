using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    public abstract class DataAttribute
    {
        private string name;

        private string attributeClass;

        public string Name { get { return name; } set { name = value; } }

        public string AttributeClass { get { return attributeClass; } set { attributeClass = value; } }

        public abstract string getValueAsString();

        public abstract double getValueAsDouble();

        public abstract double getDistance(DataAttribute att, double minValue, double maxValue);

        public abstract bool isNumerical();
    }
}
