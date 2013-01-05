using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Scurvy.Test;

namespace TestableGame
{
    class XnaTestReporter : TestStatusReporter
    {
        private bool failed;
        public List<string> Statuses = new List<string>();

        public override void Start()
        {
            failed = false;
        }

        public override void Fail(string errorMessage)
        {
            failed = true;
            this.Statuses.Add(string.Format("Failed - {0}.{1}: {2}", this.ClassName, this.MethodName, errorMessage));
        }

        public override void End()
        {
            if (!failed)
                this.Statuses.Add(string.Format("Passed - {0}.{1}", this.ClassName, this.MethodName));
        }
    }
}
