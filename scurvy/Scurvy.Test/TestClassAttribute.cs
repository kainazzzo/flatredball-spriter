using System;

namespace Scurvy.Test
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class TestClassAttribute : Attribute
    {
    }
}
