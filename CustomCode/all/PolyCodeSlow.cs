namespace Polynomial
{
using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;

	/// <summary>
	/// C# in a file implementation, slow version
	/// </summary>
	/// <description>
	/// This version writes a C# class that implements the IPolynomial
	/// interface, compiles it, reads it in, and the uses reflection to
	/// call the evaluation function. 
	/// This version is slow, because calling a method through reflection
	/// is slow. 
	/// </description>
class PolyCodeSlow: Polynomial
{
	public PolyCodeSlow(params double[] coefficients): base(coefficients)
	{
	}
	
	void WriteCode()
	{
		string timeString = polyNumber.ToString();
		polyNumber++;

        string filename = "PS_" + timeString;
		Stream s = File.Open(filename + ".cs", FileMode.Create);
		StreamWriter t = new StreamWriter(s);

		t.WriteLine("// polynomial evaluator");
		t.Write("// Evaluating y = ");


		string[] terms = new string[coefficients.Length];
		terms[0] = coefficients[0].ToString();

		for (int i = 1; i < coefficients.Length; i++)
			terms[i] = String.Format("{0} X^{1}", coefficients[1], i);

		t.Write("{0}", String.Join(" + ", terms));
		t.WriteLine();

		t.WriteLine("");

		string className = "Poly_" + timeString;
		t.WriteLine("class {0}", className);
		t.WriteLine("{");
		t.WriteLine("public double Eval(double value)");
		t.WriteLine("{");
		t.WriteLine("	return(");
		t.WriteLine("		{0}", coefficients[0]);

		string	closing = "";
		for (int i = 1; i < coefficients.Length; i++)
		{
			t.WriteLine("		+ value * ({0} ", coefficients[i]);
			closing += ")";
		}
		t.Write("\t{0}", closing);
		t.WriteLine(");");
		
		t.WriteLine("}");
		t.WriteLine("}");
		t.Close();
		s.Close();

			// Build the file

		ProcessStartInfo psi = new ProcessStartInfo();
		psi.FileName = "cmd.exe";
		psi.Arguments = String.Format("/c csc /optimize+ /r:polynomial.exe /target:library {0}.cs > compile.out", filename);
		psi.WindowStyle = ProcessWindowStyle.Minimized;

		Process proc = Process.Start(psi);
		proc.WaitForExit();

			// Open the file, and get a pointer to the method info
		Assembly a = Assembly.LoadFrom(filename + ".dll");
		
		func = a.CreateInstance(className);

		invokeType = a.GetType(className);

		File.Delete(filename + ".cs");
	}


	public override double Evaluate(double value)
	{
		if (invokeType == null)
            WriteCode();

		object[] args = new Object[] {value};
		object retValue = 
			invokeType.InvokeMember(	"Eval", 
								BindingFlags.Default | BindingFlags.InvokeMethod,
								null,
								func, 
								args);
		return((double) retValue);
	}

	object func = null;
	Type invokeType = null;

	static int polyNumber = 0;	// which number we're using...
}
}
#if junk
		object o = ass.CreateInstance("Samples.TypeResolve.Second.YourType");
		Console.WriteLine("Instance created");
		Type t = o.GetType();
		Console.WriteLine("Type: {0}", t);

		foreach (MemberInfo m in t.GetMembers())
		{
			//Console.WriteLine("Member: {0}", m);
		}

		object result;
		object[] fargs = new object[] {3.3f, 5.5f};
		result = t.InvokeMember("SimpleMethod", BindingFlags.Default | BindingFlags.InvokeMethod, null, o , 
fargs);

#endif