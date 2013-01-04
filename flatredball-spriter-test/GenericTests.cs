using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using FlatRedBall_Spriter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace flatredball_spriter_test
{
    [TestClass]
    public class GenericTests
    {
        [TestMethod]
        public void TestDeserialize()
        {
            var sos = SpriterObjectSave.FromFile(@"C:\Users\Domenic\Documents\GitHub\flatredball-spriter\spriterfiles\monsterexample\mytest.scml");
        }
    }
}
