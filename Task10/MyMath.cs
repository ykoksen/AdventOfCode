using System;
using System.Collections.Generic;
using System.Text;

namespace Task10
{
    public static class MyMath
    {
        public static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }
    }
}
