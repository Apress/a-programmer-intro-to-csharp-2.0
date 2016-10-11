namespace Polynomial
{
	using System;
	/// <summary>
	/// The simplest polynomial implementation
	/// </summary>
	/// <description>
	/// This implementation loops through the coefficients and evaluates each
	/// term of the polynomial.
	/// </description>
	class PolySimple: Polynomial
	{
		public PolySimple(params double[] coefficients): base(coefficients)
		{
		}

		public override double Evaluate(double value)
		{
			double retval = coefficients[0];
	
			double f = value;

			for (int i = 1; i < coefficients.Length; i++)
			{
				retval += coefficients[i] * f;
				f *= value;
			}
			return(retval);
		}
	}
}
