using System.Collections.Generic;
using System.Diagnostics;
using Scurvy.Test;

namespace spritertestgame
{
    public class XnaTestReporter : TestStatusReporter
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
            var status = string.Format("Failed - {0}.{1}: {2}", this.ClassName, this.MethodName, errorMessage);
            Debug.WriteLine(status);
            this.Statuses.Add(status);
        }

        public override void End()
        {
            if (!failed)
                this.Statuses.Add(string.Format("Passed - {0}.{1}", this.ClassName, this.MethodName));
        }
    }
}
