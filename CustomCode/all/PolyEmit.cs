namespace Polynomial
{
using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using PolyInterface;

	/// <summary>
	/// PolyEmit - Use Reflection.Emit.
	/// </summary>
	/// <description>
	/// This version uses Reflection.Emit to write a custom evaluator in
	/// IL. This gives the advantages of PolyCode, but without the overhead
	/// of having to write and compile the file.
	/// The IL algorithm is the same one the the C# compiler emits in the
	/// PolyCode example.
	/// </description>
class PolyEmit: Polynomial
{
	Type	theType = null;
	object	theObject = null;
	IPolynomial poly = null;
	
	public PolyEmit(params double[] coefficients): base(coefficients)
	{
	}
		/// <summary>
		/// Create an assembly that will evaluate the polynomial.
		/// </summary>
	private Assembly EmitAssembly()
	{
			//
			// Create an assembly name
			//
		AssemblyName assemblyName = new AssemblyName();
		assemblyName.Name = "PolynomialAssembly";

			//
			// Create a new assembly with one module
			//
        AssemblyBuilder newAssembly = Thread.GetDomain().DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
		ModuleBuilder newModule = newAssembly.DefineDynamicModule("Evaluate");

			//		
		    //  Define a public class named "PolyEvaluate" in the assembly.
			//			
        TypeBuilder myType = 
			newModule.DefineType("PolyEvaluate", TypeAttributes.Public);

			//
			// Mark the class as implementing IPolynomial. This is
			// the first step in that process.
			//
		myType.AddInterfaceImplementation(typeof(IPolynomial));

			// Add a constructor
		ConstructorBuilder constructor = 
			myType.DefineDefaultConstructor(MethodAttributes.Private);

			//
			// Define a method on the type to call. We pass an
			// array that defines the types of the parameters, 
			// the type of the return type, the name of the method,
			// and the method attributes.
			//
		Type[] paramTypes = new Type[] {typeof(double)};
		Type returnType = typeof(double);
		MethodBuilder simpleMethod = 
			myType.DefineMethod("Eval", 
								MethodAttributes.Public | MethodAttributes.Virtual, 
								returnType, 
								paramTypes);

			//
			// From the method, get an ILGenerator. This is used to
			// emit the IL that we want.
			//
		ILGenerator il = simpleMethod.GetILGenerator();

			//
			// Emit the IL. This is a hand-coded version of what
			// you'd get if you compiled the code example and then ran
			// ILDASM on the output.
			//

			//
			// This first section repeated loads the coefficients
			// x value on the stack for evaluation. 
			//
		for (int index = 0; index < coefficients.Length - 1;index++)
		{
			il.Emit(OpCodes.Ldc_R8, coefficients[index]);
			il.Emit(OpCodes.Ldarg_1);
		}

			// load the last coefficient
		il.Emit(OpCodes.Ldc_R8, coefficients[coefficients.Length - 1]);

			// Emit the remainder of the code. This is a repeated
			// section of multiplying the terms together and
			// accumulating them. 
		for (int loop = 0; loop < coefficients.Length - 1; loop++)
		{
			il.Emit(OpCodes.Mul);
			il.Emit(OpCodes.Add);
		}

			// return the value
		il.Emit(OpCodes.Ret);

			//
			// Finish the process.
			// Create the type.
			//
		//myType.CreateType();

			//
			// Hook up the interface member to the member function
			// that implements that member.
			// 1) Get the interface member.
			// 2) Get the type of the new class that was created
			// 3) Get the member of the class that does the evaluation
			// 4) Hook that member to the interface member.
			//
		MethodInfo methodInterfaceEval = typeof(IPolynomial).GetMethod("Eval");

	//	Type t2 = newModule.GetType("PolyEvaluate");
	//	MethodInfo methodImplementEval = t2.GetMethod("Eval");

		//methodImplementEval = simpleMethod;
		myType.DefineMethodOverride(simpleMethod, methodInterfaceEval);
		myType.CreateType();

      	return newAssembly;
	}

	public void Setup()
	{
			// Create the assembly, create an instance of the 
			// evalution class, and save away an interface
			// reference to it. 
		Assembly ass = EmitAssembly();

		theObject = ass.CreateInstance("PolyEvaluate");
		theType = theObject.GetType();

		poly = (IPolynomial) theObject;
	}

	public override double Evaluate(double value)
    {
		if (theType == null)
		{
			Setup();
		}

		return(poly.Eval(value));
    }
}
}