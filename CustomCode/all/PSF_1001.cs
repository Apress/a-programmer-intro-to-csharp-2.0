// polynomial evaluator
// Evaluating y = 5.5 + 7 X^1 + 15 X^2 + 30 X^3 + 500 X^4 + 100 X^5 + 1 X^6

class Poly_1001: PolyInterface.IPolynomial
{
public double Eval(double value)
{
	return(
		5.5
		+ value * (7 
		+ value * (15 
		+ value * (30 
		+ value * (500 
		+ value * (100 
		+ value * (1 
	)))))));
}
}
