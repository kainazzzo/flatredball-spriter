using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using FlatRedBall_Spriter;

namespace flatredball_spriter_test
{
    public static class TestSerializationUtility
    {
        public static T DeserializeFromXml<T>(string xml)
        {
            using (var ms = new MemoryStream(Encoding.UTF8.GetBytes(xml)))
            {
                return (T) new XmlSerializer(typeof (T)).Deserialize(ms);
            }
        }
    }
}
