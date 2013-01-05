using System;

namespace Scurvy.Test
{
    public class TestContext
    {
        public readonly IServiceProvider Services;
        public ExitCriteria ExitCriteria;

        public TestContext(IServiceProvider services)
        {
            this.Services = services;
        }
    }
}
