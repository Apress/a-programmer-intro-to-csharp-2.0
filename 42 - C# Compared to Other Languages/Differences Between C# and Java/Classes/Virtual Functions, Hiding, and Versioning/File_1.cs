// 39 - C# Compared to Other Languages\Differences Between C# and Java\Classes\Virtual Functions, Hiding, and Versioning
// copyright 2000 Eric Gunnerson
public class B
{
} 
public class D: B
{
    public void Process(object o) {}
}
class Test
{
    public static void Main()
    {
        D d = new D();
        d.Process(15);    // make call
    }
}