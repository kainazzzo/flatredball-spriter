using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace flatredball_spriter_test
{
    [TestClass]
    public class MultipleEntityTests
    {
        [TestMethod]
        public void ToSpriterObjectCollectionTest()
        {
            #region xml

            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<spriter_data scml_version=""1.0"" generator=""BrashMonkey Spriter"" generator_version=""b6.1"">
    <entity id=""0"" name=""entity_000"">
        <animation id=""0"" name=""NewAnimation"" length=""1000"">
            <mainline>
                <key id=""0"">
                    <object_ref id=""0"" timeline=""0"" key=""0"" z_index=""0""/>
                </key>
            </mainline>
            <timeline id=""0"" name=""point_000"" object_type=""point"">
                <key id=""0"" spin=""0"">
                    <object x=""10"" y=""-10"" angle=""0""/>
                </key>
            </timeline>
        </animation>
    </entity>
    <entity id=""1"" name=""entity_001"">
        <animation id=""0"" name=""NewAnimation"" length=""1000"">
            <mainline>
                <key id=""0"">
                    <object_ref id=""0"" timeline=""0"" key=""0"" z_index=""0""/>
                </key>
            </mainline>
            <timeline id=""0"" name=""point_000"" object_type=""point"">
                <key id=""0"" spin=""0"">
                    <object x=""10"" y=""10"" angle=""0""/>
                </key>
            </timeline>
        </animation>
    </entity>
</spriter_data>
";

            #endregion

            var sos = TestSerializationUtility.DeserializeSpriterObjectSaveFromXml(xml);

            var soc = sos.ToRuntime();

            Assert.AreEqual(2, soc.SpriterEntities.Count);

            var so1 = soc.SpriterEntities.First();
            var so2 = soc.SpriterEntities.ElementAt(1);

            Assert.AreEqual("entity_000", so1.Key);
            Assert.AreEqual("entity_001", so2.Key);

            Assert.AreEqual(10f, so1.Value.Animations.First().Value.KeyFrames.First().Values.First().Value.RelativePosition.X);
            Assert.AreEqual(10f, so2.Value.Animations.First().Value.KeyFrames.First().Values.First().Value.RelativePosition.X);

            Assert.AreEqual(-10f, so1.Value.Animations.First().Value.KeyFrames.First().Values.First().Value.RelativePosition.Y);
            Assert.AreEqual(10f, so2.Value.Animations.First().Value.KeyFrames.First().Values.First().Value.RelativePosition.Y);

            Assert.AreEqual(1f, so1.Value.Animations.First().Value.TotalTime);
            Assert.AreEqual(1f, so2.Value.Animations.First().Value.TotalTime);
        }
    }
}
