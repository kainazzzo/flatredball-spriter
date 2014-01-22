using System;
using System.Linq;
using FlatRedBall;
using FlatRedBallExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.Xna.Framework;

namespace flatredball_spriter_test
{
    [TestClass]
    public class BoxTests
    {
        [TestMethod]
        public void NoExceptions()
        {
            #region xml
            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<spriter_data scml_version=""1.0"" generator=""BrashMonkey Spriter"" generator_version=""b6.1"">
    <folder id=""0"">
        <file id=""0"" name=""square.png"" width=""32"" height=""32"" pivot_x=""0"" pivot_y=""1""/>
    </folder>
    <entity id=""0"" name=""entity_000"">
        <obj_info name=""box_000"" type=""box"" w=""32"" h=""32"" pivot_x=""0"" pivot_y=""0""/>
        <obj_info name=""box_001"" type=""box"" w=""32"" h=""32"" pivot_x=""0"" pivot_y=""0""/>
        <obj_info name=""box_002"" type=""box"" w=""32"" h=""32"" pivot_x=""0"" pivot_y=""0""/>
        <obj_info name=""box_003"" type=""box"" w=""32"" h=""-32"" pivot_x=""0"" pivot_y=""0""/>
        <obj_info name=""box_004"" type=""box"" w=""54"" h=""52"" pivot_x=""0"" pivot_y=""0""/>
        <animation id=""0"" name=""NewAnimation"" length=""1000"">
            <mainline>
                <key id=""0"">
                    <object_ref id=""0"" timeline=""0"" key=""0"" z_index=""0""/>
                    <object_ref id=""1"" timeline=""1"" key=""0"" z_index=""1""/>
                    <object_ref id=""2"" timeline=""2"" key=""0"" z_index=""2""/>
                    <object_ref id=""3"" timeline=""3"" key=""0"" z_index=""3""/>
                    <object_ref id=""4"" timeline=""4"" key=""0"" z_index=""4""/>
                    <object_ref id=""5"" timeline=""5"" key=""0"" z_index=""5""/>
                </key>
                <key id=""1"" time=""500"">
                    <object_ref id=""0"" timeline=""0"" key=""0"" z_index=""0""/>
                    <object_ref id=""1"" timeline=""1"" key=""0"" z_index=""1""/>
                    <object_ref id=""2"" timeline=""2"" key=""0"" z_index=""2""/>
                    <object_ref id=""3"" timeline=""3"" key=""0"" z_index=""3""/>
                    <object_ref id=""4"" timeline=""4"" key=""0"" z_index=""4""/>
                    <object_ref id=""5"" timeline=""5"" key=""1"" z_index=""5""/>
                </key>
            </mainline>
            <timeline id=""0"" obj=""0"" name=""box_000"" object_type=""box"">
                <key id=""0"" spin=""0"">
                    <object x=""16"" y=""16"" pivot_x=""0.5"" pivot_y=""0.5"" angle=""0""/>
                </key>
            </timeline>
            <timeline id=""1"" obj=""1"" name=""box_001"" object_type=""box"">
                <key id=""0"" spin=""0"">
                    <object y=""32"" pivot_x=""0"" pivot_y=""1"" angle=""0""/>
                </key>
            </timeline>
            <timeline id=""2"" obj=""2"" name=""box_002"" object_type=""box"">
                <key id=""0"" spin=""0"">
                    <object pivot_x=""0"" pivot_y=""0"" angle=""0""/>
                </key>
            </timeline>
            <timeline id=""3"" obj=""3"" name=""box_003"" object_type=""box"">
                <key id=""0"" spin=""0"">
                    <object pivot_x=""0"" pivot_y=""1"" angle=""0""/>
                </key>
            </timeline>
            <timeline id=""4"" name=""square"">
                <key id=""0"" spin=""0"">
                    <object folder=""0"" file=""0"" y=""32"" angle=""0""/>
                </key>
            </timeline>
            <timeline id=""5"" obj=""4"" name=""box_004"" object_type=""box"">
                <key id=""0"" spin=""0"">
                    <object x=""105"" y=""119"" pivot_x=""0"" pivot_y=""1"" angle=""0""/>
                </key>
                <key id=""1"" time=""500"" spin=""0"">
                    <object x=""105"" y=""119"" pivot_x=""0"" pivot_y=""1"" angle=""0""/>
                </key>
            </timeline>
        </animation>
    </entity>
</spriter_data>
";
            #endregion

            var sos = TestSerializationUtility.DeserializeSpriterObjectSaveFromXml(xml);

            var so = sos.ToRuntime();
        }

        [TestMethod]
        public void SimpleValueTest()
        {
            #region xml

            var xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<spriter_data scml_version=""1.0"" generator=""BrashMonkey Spriter"" generator_version=""b6.1"">
    <entity id=""0"" name=""entity_000"">
        <obj_info name=""box_000"" type=""box"" w=""32"" h=""32"" pivot_x=""0"" pivot_y=""0""/>
        <animation id=""0"" name=""NewAnimation"" length=""1000"">
            <mainline>
                <key id=""0"">
                    <object_ref id=""0"" timeline=""0"" key=""0"" z_index=""0""/>
                </key>
            </mainline>
            <timeline id=""0"" obj=""0"" name=""box_000"" object_type=""box"">
                <key id=""0"" spin=""0"">
                    <object x=""10"" y=""-10"" pivot_x=""0"" pivot_y=""1"" angle=""45"" scale_x=""0.5"" scale_y=""0.5""/>
                </key>
            </timeline>
        </animation>
    </entity>
</spriter_data>
";
            #endregion

            var sos = TestSerializationUtility.DeserializeSpriterObjectSaveFromXml(xml);

            var so = sos.ToRuntime().SpriterEntities.First().Value;

            var values = so.Animations.First().Value.KeyFrames.First().Values;

            var pivot = values.ElementAt(0).Value;
            var box = values.ElementAt(1).Value;

            Assert.IsTrue(Math.Abs(pivot.RelativePosition.X - 10) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.RelativePosition.Y - (-10)) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.RelativeRotation.Z - 45f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(box.RelativeScaleX - .5f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(box.RelativeScaleY - .5f) < Single.Epsilon);
        }

        [TestMethod]
        public void TweenTest()
        {
            #region xml

            const string xml = @"<?xml version=""1.0"" encoding=""UTF-8""?>
<spriter_data scml_version=""1.0"" generator=""BrashMonkey Spriter"" generator_version=""b6.1"">
    <entity id=""0"" name=""entity_000"">
        <obj_info name=""box_000"" type=""box"" w=""32"" h=""32"" pivot_x=""0"" pivot_y=""0""/>
        <animation id=""0"" name=""NewAnimation"" length=""1000"">
            <mainline>
                <key id=""0"">
                    <object_ref id=""0"" timeline=""0"" key=""0"" z_index=""0""/>
                </key>
                <key id=""1"" time=""500"">
                    <object_ref id=""0"" timeline=""0"" key=""1"" z_index=""0""/>
                </key>
            </mainline>
            <timeline id=""0"" obj=""0"" name=""box_000"" object_type=""box"">
                <key id=""0"" spin=""-1"">
                    <object x=""10"" y=""-10"" pivot_x=""0"" pivot_y=""1"" angle=""45"" scale_x=""0.5"" scale_y=""0.5""/>
                </key>
                <key id=""1"" time=""500"">
                    <object pivot_x=""0"" pivot_y=""1"" angle=""0""/>
                </key>
            </timeline>
        </animation>
    </entity>
</spriter_data>
";
            #endregion

            var sos = TestSerializationUtility.DeserializeSpriterObjectSaveFromXml(xml);
            var so = sos.ToRuntime().SpriterEntities.First().Value;

            so.StartAnimation();

            TimeManager.CurrentTime = .25;
            so.TimedActivity(.25f, 0.03125, .25f);

            var pivot = so.ObjectList[1];
            var box = (ScaledPolygon) so.ObjectList[0];

            Assert.IsTrue(Math.Abs(pivot.X - 5f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.Y - -5f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(pivot.RelativeRotationZ - MathHelper.ToRadians(45f/2.0f)) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(box.RelativeScaleX - .75f) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(box.RelativeScaleY - .75f) < Single.Epsilon);
            Assert.IsTrue(box.Parent == pivot);
        }
    }
}
