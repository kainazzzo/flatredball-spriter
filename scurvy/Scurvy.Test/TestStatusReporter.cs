namespace Scurvy.Test
{
    public abstract class TestStatusReporter
    {
        public string ClassName { get; set; }
        public string MethodName { get; set; }

        public abstract void Start();
        public abstract void Fail(string errorMessage);
        public abstract void End();
    }
}
