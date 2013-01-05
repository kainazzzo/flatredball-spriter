using System;

namespace Scurvy.Test
{
    public abstract class ExitCriteria
    {
        public bool IsFinished;
        public readonly TestContext Context;

        public ExitCriteria(TestContext context)
        {
            this.Context = context;
        }

        public abstract void Update(TimeSpan elapsedTime);

        public abstract void Draw();
    }
}
