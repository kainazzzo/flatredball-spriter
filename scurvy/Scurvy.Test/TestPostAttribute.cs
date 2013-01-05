using System;

namespace Scurvy.Test
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TestPostAttribute : Attribute
    {
    }
}
