namespace Polynomial
{
using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using PolyInterface;
using System.CodeDom;
using System.CodeDom.Compiler;
using Microsoft.CSharp;

	/// <summary>
	/// C# in a file implementation, using the CodeDOM to create the
	/// C# code...
	/// </summary>
	/// <description>
	/// This version writes a C# class that implements the IPolynomial
	/// interface, compiles it, reads it in, gets the interface, and then
	/// calls through the interface to evaluate the polynomial.
	/// This version is fast because calling through the interface is quick.
	/// </description>
class PolyCodeDom: Polynomial
{
	public PolyCodeDom(params double[] coefficients): base(coefficients)
	{
	}
	
	void WriteCode()
	{
		string timeString = polyNumber.ToString();
		polyNumber++;

        string filename = "PSCD_" + timeString;
		Stream s = File.Open(filename + ".cs", FileMode.Create);
		StreamWriter t = new StreamWriter(s);

			// Generate code in C#
		CSharpCodeProvider provider = new CSharpCodeProvider();
		ICodeGenerator cg = provider.CreateGenerator(t);
		CodeGeneratorOptions op = new CodeGeneratorOptions();

			// Generate the comments at the beginning of the function
		CodeCommentStatement comment = new CodeCommentStatement("Polynomial evaluator");
		cg.GenerateCodeFromStatement(comment, t, op);

		string[] terms = new string[coefficients.Length];
		terms[0] = coefficients[0].ToString();

		for (int i = 1; i < coefficients.Length; i++)
			terms[i] = String.Format("{0} X^{1}", coefficients[i], i);

		comment = new CodeCommentStatement("Evaluating Y = " + String.Join(" + ", terms));
		cg.GenerateCodeFromStatement(comment, t, op);

			// The class is named with a unique name
		string className = "Poly_" + timeString;
		CodeTypeDeclaration polyClass = new CodeTypeDeclaration(className);
			// The class implements IPolynomial
		polyClass.BaseTypes.Add("PolyInterface.IPolynomial");

			// Set up the Eval function
		CodeParameterDeclarationExpression param1 = 
			new CodeParameterDeclarationExpression("double", "x");
		CodeMemberMethod eval = new CodeMemberMethod();
		eval.Name = "Eval";
		eval.Parameters.Add(param1);

			// workaround for bug below...
		eval.ReturnType = new CodeTypeReference("public double");
			// BUG: This doesn't generate "public", it just leaves
			// the attribute off of the member...
		eval.Attributes |= MemberAttributes.Public;

			// Create the expression to do the evaluation of the
			// polynomail. To do this, we chain together binary 
			// operators to get the desired expression
			// a0 + x * (a1 + x * (a2 + x * (a3)));
			//
			// This is very much like building a parse tree for 
			// an expression.
		
		CodeBinaryOperatorExpression plus = new CodeBinaryOperatorExpression();
		plus.Left = new CodePrimitiveExpression(coefficients[0]);
		plus.Operator = CodeBinaryOperatorType.Add;

		CodeBinaryOperatorExpression current = plus;

		for (int i = 1; i < coefficients.Length; i++)
		{
			CodeBinaryOperatorExpression multiply = new CodeBinaryOperatorExpression();
			current.Right = multiply;
			multiply.Left = new CodeSnippetExpression("x");
			multiply.Operator = CodeBinaryOperatorType.Multiply;

			CodeBinaryOperatorExpression add = new CodeBinaryOperatorExpression();
			multiply.Right = add;
			add.Operator = CodeBinaryOperatorType.Add;
			add.Left = new CodePrimitiveExpression(coefficients[i]);
			current = add;
		}
		current.Right = new CodePrimitiveExpression(0.0);

			// return the expression...
		eval.Statements.Add(new CodeMethodReturnStatement(plus));
		polyClass.Members.Add(eval);
		cg.GenerateCodeFromType(polyClass, t, op);

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