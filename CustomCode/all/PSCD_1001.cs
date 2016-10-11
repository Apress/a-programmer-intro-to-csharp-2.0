// Polynomial evaluator
// Evaluating Y = 5.5 + 7 X^1 + 15 X^2 + 30 X^3 + 500 X^4 + 100 X^5 + 1 X^6
public class Poly_1001 : PolyInterface.IPolynomial {
    
    public double Eval(double x) {
        return (5.5 
                    + (x 
                    * (7 
                    + (x 
                    * (15 
                    + (x 
                    * (30 
                    + (x 
                    * (500 
                    + (x 
                    * (100 
                    + (x 
                    * (1 + 0)))))))))))));
    }
}
