using System;
using System.IO;
using System.Runtime.Remoting;
using System.Text;
using System.Xml.Serialization;
using FlatRedBall_Spriter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace flatredball_spriter_test
{
    [TestClass]
    public class absolute_value_tests
    {
        [TestMethod]
        public void test_deserialize()
        {
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<spriter_data>
    <entity>
        <animation>
            <mainline>
                <key>
                    <object_ref id=""0"" parent=""0"" name=""head90"" folder=""0"" file=""0"" abs_x=""200"" abs_y=""201"" abs_pivot_x=""2"" abs_pivot_y=""3"" abs_angle=""4"" abs_scale_x=""5"" abs_scale_y=""6"" abs_a=""7"" timeline=""0"" key=""0"" z_index=""0""/>
                </key>
            </mainline>
        </animation>
    </entity>
</spriter_data>
";
            var sos = TestSerializationUtility.DeserializeFromXml<SpriterObjectSave>(xml);
            
            var @object = sos.Entity[0].Animation[0].Mainline.Keys[0].ObjectRef[0];

            Assert.AreEqual(200, @object.AbsoluteX);
            Assert.AreEqual(201, @object.AbsoluteY);
            Assert.AreEqual(2, @object.AbsolutePivotX);
            Assert.AreEqual(3, @object.AbsolutePivotY);
            Assert.AreEqual(4, @object.AbsoluteAngle);
            Assert.AreEqual(5, @object.AbsoluteScaleX);
            Assert.AreEqual(6, @object.AbsoluteScaleY);
            Assert.AreEqual(7, @object.AbsoluteAlpha);
        }

        [TestMethod]
        public void absolute_values_are_used_if_available()
        {
            #region xml
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<spriter_data scml_version=""1.0"" generator=""BrashMonkey Spriter"" generator_version=""b5"">
    <folder id=""0"">
        <file id=""0"" name=""head90.png"" width=""116"" height=""128"" pivot_x=""0"" pivot_y=""1""/>
    </folder>
    <entity id=""0"" name=""entity_000"">
        <animation id=""0"" name=""NewAnimation"" length=""1000"">
            <mainline>
                <key id=""0"">
                    <bone_ref id=""0"" timeline=""1"" key=""0""/>
                    <bone_ref id=""1"" timeline=""3"" key=""0""/>
                    <object_ref id=""0"" parent=""0"" name=""head90"" folder=""0"" file=""0"" abs_x=""200"" abs_y=""200"" abs_pivot_x=""0"" abs_pivot_y=""1"" abs_angle=""0"" abs_scale_x=""1"" abs_scale_y=""1"" abs_a=""1"" timeline=""0"" key=""0"" z_index=""0""/>
                    <object_ref id=""1"" parent=""1"" name=""head90_000"" folder=""0"" file=""0"" abs_x=""200"" abs_y=""200"" abs_pivot_x=""0"" abs_pivot_y=""1"" abs_angle=""360"" abs_scale_x=""1"" abs_scale_y=""1"" abs_a=""1"" timeline=""2"" key=""0"" z_index=""1""/>
                </key>
            </mainline>
            <timeline id=""0"" name=""head90"">
                <key id=""0"" spin=""0"">
                    <object folder=""0"" file=""0"" x=""100"" y=""100""/>
                </key>
            </timeline>
            <timeline id=""1"" name=""bone_000"" object_type=""bone"">
                <key id=""0"" spin=""0"">
                    <bone x=""100"" y=""100"" angle=""0""/>
                </key>
            </timeline>
            <timeline id=""2"" name=""head90_000"">
                <key id=""0"" spin=""0"">
                    <object folder=""0"" file=""0"" x=""66.666667"" y=""100"" scale_x=""0.666667""/>
                </key>
            </timeline>
            <timeline id=""3"" name=""bone_001"" object_type=""bone"">
                <key id=""0"" spin=""0"">
                    <bone x=""100"" y=""100"" scale_x=""1.5""/>
                </key>
            </timeline>
        </animation>
    </entity>
</spriter_data>
";
            #endregion

            var sos = TestSerializationUtility.DeserializeFromXml<SpriterObjectSave>(xml);
            
        }
    }
}