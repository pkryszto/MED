using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    class StringDataAttribute : DataAttribute
    {
        private string attributeValue;

        public string AttributeValue { get { return attributeValue; } set { attributeValue = value; } }

        public StringDataAttribute(string setName, string setValue, string setClass = null)
        {
            Name = setName;
            AttributeValue = setValue;
            AttributeClass = setClass;
        }

        public override string getValueAsString()
        {
            return AttributeValue;
        }

        public override double getDistance(DataAttribute att, double minValue, double maxValue)
        {
            if (att.getValueAsString().Equals(this.getValueAsString())) return 0;
            else return 1;
        }

        public override double getValueAsDouble()
        {
            return 0;
        }

        public override bool isNumerical()
        {
            return false;
        }
    }
}
