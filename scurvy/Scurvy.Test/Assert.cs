namespace Scurvy.Test
{
    public static class Assert
    {
        public static TestStatusReporter Reporter { get; internal set; }

        static Assert()
        {
            Reporter = new DefaultReporter();
        }

        public static void IsTrue(bool condition)
        {
            IsTrue(condition, string.Empty);
        }

        public static void IsTrue(bool condition, string description)
        {
            if (!condition)
            {
                Reporter.Fail(description);
            }
        }

        public static void AreEqual<T>(T first, T second, string description)
        {
            IsTrue(first != null ? first.Equals(second) : false, string.Format("Expected Value <{0}> but observed <{1}>. Description: [{2}]", first, second, description));
        }
    }
}
