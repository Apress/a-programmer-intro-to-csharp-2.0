// 03 - C# QuickStart & Developing in C#\Hello, Universe
// copyright 2000 Eric Gunnerson
using System;
class Hello
{
    public static void Main(string[] args)
    {
        Console.WriteLine("Hello, Universe");
        
        // iterate over command-line arguments,
        // and print them out
        for (int arg = 0; arg < args.Length; arg++)
        Console.WriteLine("Arg {0}: {1}", arg, args[arg]);
    }
}