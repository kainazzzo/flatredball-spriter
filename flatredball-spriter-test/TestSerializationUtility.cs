using System.IO;
using System.Text;
using System.Xml.Serialization;
using FlatRedBall_Spriter;
using Telerik.JustMock;

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

        public static SpriterObjectSave DeserializeSpriterObjectSaveFromXml(string xml)
        {
            var sos = DeserializeFromXml<SpriterObjectSave>(xml);
            sos.TextureLoader = Mock.Create<ITextureLoader>();
            sos.Directory = "C:\\";
            sos.FileName = "test.scml";
            return sos;
        }
    }
}
