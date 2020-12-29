using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MED
{
    class NumericalDataAttribute : DataAttribute
    {
        private double attributeValue;

        public double AttributeValue { get { return attributeValue; } set { attributeValue = value; } }

        public NumericalDataAttribute(string setName, double setValue, string setClass = null)
        {
            Name = setName;
            AttributeValue = setValue;
            AttributeClass = setClass;
        }

        public override string getValueAsString()
        {
            return AttributeValue.ToString();
        }

        public override double getValueAsDouble()
        {
            return AttributeValue;
        }

        public override double getDistance(DataAttribute att, double minValue, double maxValue)
        {
            return (Math.Abs(att.getValueAsDouble() - AttributeValue)-minValue)/(maxValue-minValue);
        }

        public override bool isNumerical()
        {
            return true;
        }
    }
}
