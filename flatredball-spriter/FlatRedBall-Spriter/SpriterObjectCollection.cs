using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall;
using FlatRedBall.Graphics;
using FlatRedBallExtensions;

namespace FlatRedBall_Spriter
{
    public class SpriterObjectCollection : ScaledPositionedObject
    {
        public IDictionary<string, SpriterObject> SpriterEntities { get; set; }

        public void AddToManagers()
        {
            AddToManagers(null);
        }

        public void AddToManagers(Layer layer)
        {
            if (SpriterEntities == null) return;

            foreach (var spriterEntity in SpriterEntities.Where(spriterEntity => spriterEntity.Value != null))
            {
                spriterEntity.Value.AddToManagers(layer);
            }

            SpriteManager.AddPositionedObject(this);
        }

        public SpriterObject FindByName(string name = "")
        {
            if (SpriterEntities == null) return null;

            if (SpriterEntities.ContainsKey(name))
            {
                return SpriterEntities[name];
            }

            return null;
        }

        public void Destroy()
        {
            if (SpriterEntities == null) return;

            foreach (var spriterEntity in SpriterEntities.Where(spriterEntity => spriterEntity.Value != null))
            {
                spriterEntity.Value.Destroy();
            }
        }

        public SpriterObjectCollection Clone()
        {
            var soc = new SpriterObjectCollection();

            if (SpriterEntities == null)
            {
                return soc;
            }

            soc.SpriterEntities = new Dictionary<string, SpriterObject>();
            foreach (var spriterEntity in SpriterEntities)
            {
                if (spriterEntity.Value != null)
                {
                    var so = spriterEntity.Value.Clone();
                    so.AttachTo(soc, false);

                    soc.SpriterEntities.Add(spriterEntity.Key,
                        so);
                }
            }


            return soc;
        }

        public void StartAnimation(string name = null)
        {
            if (SpriterEntities == null) return;

            foreach (var spriterEntity in SpriterEntities.Where(spriterEntity => spriterEntity.Value != null))
            {
                if (name == null)
                {
                    spriterEntity.Value.StartAnimation();
                }
                else
                {
                    spriterEntity.Value.StartAnimation(name);
                }
            }
        }
    }
}
