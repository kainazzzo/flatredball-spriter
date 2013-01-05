using System;
using System.Collections.Generic;
using System.Text;

namespace Scurvy.Test
{
    [AttributeUsage( 
        AttributeTargets.Method, 
        AllowMultiple=false, 
        Inherited=true )]
    public class TestMethodAttribute : Attribute
    {

    }
}
