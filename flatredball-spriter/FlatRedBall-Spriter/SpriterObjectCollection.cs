using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FlatRedBall.Graphics;

namespace FlatRedBall_Spriter
{
    public class SpriterObjectCollection
    {
        public IDictionary<string, SpriterObject> SpriterEntities { get; set; }

        public void AddToManagers()
        {
            AddToManagers(null);
        }

        public void AddToManagers(Layer layer)
        {
            if (SpriterEntities == null) return;

            foreach (var spriterEntity in SpriterEntities)
            {
                if (spriterEntity.Value != null) spriterEntity.Value.AddToManagers(layer);
            }
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

            foreach (var spriterEntity in SpriterEntities)
            {
                if (spriterEntity.Value != null) spriterEntity.Value.Destroy();
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
                soc.SpriterEntities.Add(spriterEntity.Key,
                    spriterEntity.Value != null ? spriterEntity.Value.Clone() : null);
            }

            return soc;
        }
    }
}
