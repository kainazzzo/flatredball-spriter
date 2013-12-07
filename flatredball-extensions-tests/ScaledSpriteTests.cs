using System;
using FlatRedBallExtensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace flatredball_extensions_tests
{
    [TestClass]
    public class ScaledSpriteTests
    {
        [TestMethod]
        public void ParentScaleAffectsSpriteScale()
        {
            var spo = new ScaledPositionedObject
            {
                ScaleX = .5f,
                ScaleY = .25f
            };

            var sprite = new ScaledSprite
            {
                RelativeScaleX = .5f,
                RelativeScaleY = 1.0f
            };

            sprite.AttachTo(spo, true);

            sprite.UpdateDependencies(0);

            Assert.IsTrue(Math.Abs(sprite.Width - .25f * ScaledSprite.DefaultTextureWidth) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(sprite.Height - .25f * ScaledSprite.DefaultTextureHeight) < Single.Epsilon);
        }

        [TestMethod]
        public void DetachedScaledSpriteWorksNormally()
        {
            var sprite = new ScaledSprite
            {
                ScaleX = .5f,
                ScaleY = .5f
            };

            sprite.UpdateDependencies(0);

            Assert.IsTrue(Math.Abs(sprite.Width - .5f * ScaledSprite.DefaultTextureWidth) < Single.Epsilon);
            Assert.IsTrue(Math.Abs(sprite.Height - .5f * ScaledSprite.DefaultTextureHeight) < Single.Epsilon);
        }
    }
}
