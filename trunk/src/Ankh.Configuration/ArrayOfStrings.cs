using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Ankh.Configuration
{
    [Serializable]
    [XmlRoot("ArrayOfString")]
    public class ArrayOfStrings
    {
        [XmlElement("string")]
        public string[] Strings = new string[0];
    }
}
