using System;
using System.Diagnostics;

namespace Scurvy.Test
{
    internal class DefaultReporter : TestStatusReporter
    {
        private bool failed;

        public override void Start()
        {
            this.failed = false;
        }

        public override void Fail(string errorMessage)
        {
            this.failed = true;
            Debug.WriteLine(string.Format("Failed: {1}.{2} - {0}", errorMessage, this.ClassName, this.MethodName));
        }

        public override void End()
        {
            if (!failed)
            {
                Debug.WriteLine(string.Format("Passed: {0}.{1}", this.ClassName, this.MethodName));
            }
        }
    }
}
