using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using FlatRedBall;
using FlatRedBall_Spriter;
using Scurvy.Test;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            TestRunner<Program>.SetReporter(new ConsoleReporter());
            TestRunner<Program>.RunTests((IServiceProvider)null);

            Console.ReadKey();
        }
    }

    class ConsoleReporter : TestStatusReporter
    {
        private bool failed;

        public override void Start()
        {
            failed = false;
        }

        public override void Fail(string errorMessage)
        {
            failed = true;
            var color = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Failed: {0}.{1} - {2}", this.ClassName, this.MethodName, errorMessage);
            Console.ForegroundColor = color;
        }

        public override void End()
        {
            if (!failed)
            {
                var color = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Passed! {0}.{1}", this.ClassName, this.MethodName);
                Console.ForegroundColor = color;
            }
        }
    }

    [TestClass]
    public class MyTest
    {
        [TestSetup]
        public void TestInit()
        {
            Console.WriteLine("\t---initializing---");
        }
        [TestCleanup]
        public void TestCleanup()
        {
            Console.WriteLine("\t---cleaning---");
        }

        [TestMethod]
        public void ShouldPass()
        {
            Console.WriteLine("in should pass");
        }

        [TestMethod]
        public void TestWithExitCriteria(TestContext context)
        {
            Console.WriteLine("in TestWithExitCriteria");
            context.ExitCriteria = new ExitCriteriaTest(context);
        }

        private class ExitCriteriaTest : ExitCriteria
        {
            private int i = 0;

            public ExitCriteriaTest(TestContext context)
                : base(context)
            {
            }

            public override void Update(TimeSpan elapsedTime)
            {
                Console.WriteLine("In ExitCriteriaTest: update {0}", this.i);
                i++;

                if (i >= 10)
                {
                    this.IsFinished = true;
                }
            }

            public override void Draw()
            {
                Console.WriteLine("In ExitCriteriaTest: draw {0}", this.i);
            }
        }
}
}
