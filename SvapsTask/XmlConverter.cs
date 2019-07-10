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
        /// Get integer (as we are working with pixels) from floating number in string
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
            string attrStrValue = node.Attributes.GetNamedItem(attrName).Value.Split(new char[] { '.' }, 2).First();
            return Convert.ToInt32(attrStrValue);
        }
    }
}
