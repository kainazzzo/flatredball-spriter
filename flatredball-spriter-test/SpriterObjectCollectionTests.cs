using System;
using System.Collections.Generic;
using FlatRedBall_Spriter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace flatredball_spriter_test
{
    [TestClass]
    public class SpriterObjectCollectionTests
    {
        [TestMethod]
        public void visible_property_sets_all_spriterobjects()
        {
            var soc = new SpriterObjectCollection {SpriterEntities = new Dictionary<string, SpriterObject>
            {
                {
                    "test",
                    SpriterObjectTest.GetSimpleSpriterObject()
                },
                {
                    "test2",
                    SpriterObjectTest.GetSimpleSpriterObject()
                }
            }};

            Assert.AreNotSame(soc.SpriterEntities["test"], soc.SpriterEntities["test2"]);

            Assert.AreEqual(true, soc.SpriterEntities["test"].Visible);
            Assert.AreEqual(true, soc.SpriterEntities["test2"].Visible);

            soc.Visible = false;

            Assert.AreEqual(false, soc.SpriterEntities["test"].Visible);
            Assert.AreEqual(false, soc.SpriterEntities["test2"].Visible);
        }
    }
}
