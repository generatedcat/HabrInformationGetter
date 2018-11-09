using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace HabrInformationGetter
{
    [Serializable]
    [XmlRoot("channel")]
    public class Channel
    {
        [XmlArray("ItemList"), XmlArrayItem(typeof(Item), ElementName ="item")]
        public List<Item> ItemList { get; set; }
    }
}
