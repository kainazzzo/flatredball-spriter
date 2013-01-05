using System;
using System.Collections.Generic;
using System.Reflection;

namespace Scurvy.Test
{
    public class TestRunner<HostType>
    {
        private TestContext context;
        private IEnumerable<MethodInfo> methods;
        private IEnumerator<MethodInfo> enumerator;
        private object currentObject;
        private Type currentType;

        public TestRunner(IServiceProvider services)
            : this(new TestContext(services))
        {
        }

        public TestRunner(TestContext context)
        {
            this.context = context;
            this.methods = FindTestMethods();
            this.enumerator = this.methods.GetEnumerator();
        }

        public void Update(TimeSpan elapsedGameTime)
        {
            TestContext testContext = this.context;
            ExitCriteria exitCriteria = testContext.ExitCriteria;
            if (exitCriteria != null)
            {
                UpdateExitCriteria(elapsedGameTime, testContext, exitCriteria);
            }
            else if (enumerator.MoveNext())
            {
                MethodInfo method = enumerator.Current;
                if (method.DeclaringType != currentType)
                {
                    currentObject = Activator.CreateInstance(method.DeclaringType);
                    currentType = method.DeclaringType;
                }

                ExecuteMethod(currentObject, method, testContext);
            }
        }

        public void Draw()
        {
            TestContext testContext = this.context;
            ExitCriteria exitCriteria = testContext.ExitCriteria;
            if (exitCriteria != null)
            {
                DrawExitCriteria(testContext, exitCriteria);
            }
        }

        public void Reporter(TestStatusReporter reporter)
        {
            TestRunner<HostType>.SetReporter(reporter);
        }

        /// <summary>Allows you to handle test statuses in a custom fashion</summary>
        public static void SetReporter(TestStatusReporter reporter)
        {
            Assert.Reporter = reporter;
        }

        /// <summary>
        /// This will run all tests that have been marked accordingly.
        /// </summary>
        /// <typeparam name="HostType">The test runner will look in the assembly of this Type.</typeparam>
        /// <param name="services">The provided service provider will be wrapped in a simple <see cref="TestContext"/>.</param>
        public static void RunTests(IServiceProvider services)
        {
            RunTests(new TestContext(services));
        }

        /// <summary>
        /// This will run all tests that have been marked accordingly.
        /// </summary>
        /// <typeparam name="HostType">The test runner will look in the assembly of this Type.</typeparam>
        /// <param name="context">The test context that will be passed to all test methods.</param>
        public static void RunTests(TestContext context)
        {
            Type currentType = null;
            object currentObject = null;

            foreach (MethodInfo method in FindTestMethods())
            {
                if (method.DeclaringType != currentType)
                {
                    currentObject = Activator.CreateInstance(method.DeclaringType);
                    currentType = method.DeclaringType;
                }
                
                ExecuteMethod(currentObject, method, context);

                if (context.ExitCriteria != null)
                {
                    ExitCriteria exit = context.ExitCriteria;
                    while (!exit.IsFinished)
                    {
                        // TODO: this blocks the main thread, should let the caller drive
                        // further updates.
                        UpdateExitCriteria(new TimeSpan(0, 0, 0, 0, 16), context, exit);
                        DrawExitCriteria(context, exit);
                    }

                    Assert.Reporter.End();
                }
            }
        }

        #region Private Methods

        private static bool UpdateExitCriteria(TimeSpan elapsedGameTime, TestContext testContext, ExitCriteria exitCriteria)
        {
            if (!exitCriteria.IsFinished)
            {
                exitCriteria.Update(elapsedGameTime);
                return true;
            }
            else
            {
                testContext.ExitCriteria = null;
                return false;
            }
        }

        private static bool DrawExitCriteria(TestContext testContext, ExitCriteria exitCriteria)
        {
            if (!exitCriteria.IsFinished)
            {
                exitCriteria.Draw();
                return true;
            }
            else
            {
                testContext.ExitCriteria = null;
                return false;
            }
        }

        private static IEnumerable<MethodInfo> FindTestMethods()
        {
            Type[] t = typeof(HostType).Assembly.GetTypes();
            foreach (Type type in t)
            {
                if (HasAttribute(type, typeof(TestClassAttribute)))
                {
                    MethodInfo[] methods = type.GetMethods();

                    MethodInfo testSetup = null, testCleanup = null, testPre = null, testPost = null;
                    List<MethodInfo> testMethods = new List<MethodInfo>(methods.Length);

                    // First, find all test, setup, and cleanup methods in this type.
                    foreach (MethodInfo method in methods)
                    {
                        if (MethodHasAttribute(method, typeof(TestPreAttribute)))
                        {
                            testPre = method;
                            continue;
                        }

                        if (MethodHasAttribute(method, typeof(TestPostAttribute)))
                        {
                            testPost = method;
                            continue;
                        }

                        if (MethodHasAttribute(method, typeof(TestMethodAttribute)))
                        {
                            testMethods.Add(method);
                            continue;
                        }

                        if (MethodHasAttribute(method, typeof(TestSetupAttribute)))
                        {
                            testSetup = method;
                            continue;
                        }

                        if (MethodHasAttribute(method, typeof(TestCleanupAttribute)))
                        {
                            testCleanup = method;
                            continue;
                        }
                    }

                    if (testSetup != null) yield return testSetup;

                    for (int i = 0; i < testMethods.Count; i++)
                    {
                        MethodInfo method = testMethods[i];

                        if (testPre != null) yield return testPre;
                        
                        if (method != null) yield return method;

                        if (testPost != null) yield return testPost;
                    }

                    if (testCleanup != null)
                    {
                        yield return testCleanup;
                    }
                }
            }
        }

        private static void ExecuteMethod(object o, MethodInfo method, TestContext context)
        {
            if (o == null || method == null)
            {
                return;
            }

            Assert.Reporter.ClassName = method.ReflectedType.Name;
            Assert.Reporter.MethodName = method.Name;
            Assert.Reporter.Start();

            try
            {
                ParameterInfo[] parameters = method.GetParameters();
                if (parameters != null && parameters.Length == 1 && parameters[0].ParameterType == typeof(TestContext))
                {
                    method.Invoke(o, new object[] { context });
                }
                else
                {
                    method.Invoke(o, null);
                }
            }
            catch (TargetInvocationException tie)
            {
                Assert.Reporter.Fail(tie.InnerException.Message);
            }
            catch (Exception ae)
            {
                Assert.Reporter.Fail(ae.Message);
            }

            if (context.ExitCriteria == null)
            {
                // only end if there is no exit criteria.
                // otherwise the runner will take care of ending.
                Assert.Reporter.End();
            }
        }

        private static bool MethodHasAttribute(MethodInfo method, Type attribute)
        {
            return method.GetCustomAttributes(attribute, true).Length > 0;
        }

        private static bool HasAttribute(Type t, Type attrType)
        {
            return t.GetCustomAttributes(attrType, true).Length > 0;
        }

        #endregion
    }
}
