namespace Polynomial
{
using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using PolyInterface;

	/// <summary>
	/// C# in a file implementation
	/// </summary>
	/// <description>
	/// This version writes a C# class that implements the IPolynomial
	/// interface, compiles it, reads it in, gets the interface, and then
	/// calls through the interface to evaluate the polynomial.
	/// This version is fast because calling through the interface is quick.
	/// </description>
class PolyCode: Polynomial
{
	public PolyCode(params double[] coefficients): base(coefficients)
	{
	}
	
	void WriteCode()
	{
		string timeString = polyNumber.ToString();
		polyNumber++;

        string filename = "PSF_" + timeString;
		Stream s = File.Open(filename + ".cs", FileMode.Create);
		StreamWriter t = new StreamWriter(s);

		t.WriteLine("// polynomial evaluator");
		t.Write("// Evaluating y = ");

		string[] terms = new string[coefficients.Length];
		terms[0] = coefficients[0].ToString();

		for (int i = 1; i < coefficients.Length; i++)
			terms[i] = String.Format("{0} X^{1}", coefficients[i], i);

		t.Write("{0}", String.Join(" + ", terms));
		t.WriteLine();

		t.WriteLine("");

		string className = "Poly_" + timeString;
		t.WriteLine("class {0}: PolyInterface.IPolynomial", className);
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

			// Open the file, create the instance, and cast it
			// to the assembly
		Assembly a = Assembly.LoadFrom(filename + ".dll");
		polynomial = (IPolynomial) a.CreateInstance(className);

		//File.Delete(filename + ".cs");
	}


	public override double Evaluate(double value)
	{
		if (polynomial == null)
            WriteCode();

		return(polynomial.Eval(value));
	}

	IPolynomial polynomial = null;
	static int polyNumber = 1000;
}
}