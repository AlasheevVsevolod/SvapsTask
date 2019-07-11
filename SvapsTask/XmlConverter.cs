using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SvapsTask
{
    public static class XmlConverter
    {
        /// <summary>
        /// Get int from int number in string
        /// </summary>
        /// <param name="node">Node which contains given attribute</param>
        /// <param name="attrName">Attribute which contains number</param>
        /// <returns></returns>
        public static int GetIntValueFromXmlAttr(XmlNode node, string attrName)
        {
            if (node.Attributes.GetNamedItem(attrName) == null)
            {
                Console.WriteLine($"Error occured during processing {attrName} attribute in {node.Name} node");
                throw new NullReferenceException($"{attrName} attribute in {node.Name} node");
            }

            if (node.Attributes.GetNamedItem(attrName).Value.Contains("."))
            {
                Console.WriteLine($"Error occured during processing {attrName} attribute in {node.Name} node." +
                                  $" For attributes that can contain double value use GetDoubleValueFromXmlAttr()" +
                                  $" method");
                throw new ArithmeticException($"{attrName} attribute in {node.Name} node");
            }
            string attrStrValue = node.Attributes.GetNamedItem(attrName).Value;
            return Convert.ToInt32(attrStrValue);
        }

        /// <summary>
        /// Get double (as we are working with pixels) from double number in string
        /// </summary>
        /// <param name="node">Node which contains given attribute</param>
        /// <param name="attrName">Attribute which contains number</param>
        /// <returns></returns>
        public static double GetDoubleValueFromXmlAttr(XmlNode node, string attrName)
        {
            if (node.Attributes.GetNamedItem(attrName) == null)
            {
                Console.WriteLine($"Error occured during processing {attrName} attribute in {node.Name} node");
                throw new NullReferenceException($"{attrName} attribute in {node.Name} node");
            }
            string attrStrValue = node.Attributes.GetNamedItem(attrName).Value;
            return XmlConvert.ToDouble(attrStrValue);
        }
    }
}
