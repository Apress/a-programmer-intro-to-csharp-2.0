// 03 - C# QuickStart & Developing in C#\Properties and Indexers
// copyright 2000 Eric Gunnerson
using System;
class Circle
{
    public int Radius
    {
        get
        {
            return(radius);
        }
        set
        {
            radius = value;
            Draw();
        }
    }
    public void Draw()
    {
    }
    int radius;
}
class Test
{
    public static void Main()
    {
        Circle c = new Circle();
        c.Radius = 35;
    }
}