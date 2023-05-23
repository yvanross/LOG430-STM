namespace ApplicationLogic.Services;

public class DefiniteIntegralService
{
    private const int NumDivisions = 1000;

    public static double TrapezoidalRule(double lowerBound, double upperBound, Func<double, double, double> function, int parameter)
    {
        double stepSize = (upperBound - lowerBound) / NumDivisions;
        double integral = 0.0;
        double currentX = lowerBound;

        for (int i = 0; i < NumDivisions; i++)
        {
            integral += 0.5 * stepSize * (function(currentX, parameter) + function(currentX + stepSize, parameter));
            currentX += stepSize;
        }

        return integral;
    }
}