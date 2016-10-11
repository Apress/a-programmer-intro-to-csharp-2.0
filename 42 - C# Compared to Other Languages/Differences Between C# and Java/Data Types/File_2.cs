// 39 - C# Compared to Other Languages\Differences Between C# and Java\Data Types
// copyright 2000 Eric Gunnerson
using System;
class Test
{
    public static void Main()
    {
        int v = 55;
        object o = v;        // box v into o
        Console.WriteLine("Value is: {0}", o);
        int v2 = (int) o;    // unbox back to an int
    }
}