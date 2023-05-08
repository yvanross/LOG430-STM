namespace ApplicationLogic.Services;

public class DefiniteIntegralService
{
    private const int NumDivisions = 1000;

    public static double TrapezoidalRule(double lowerBound, double upperBound, Func<double, double> function)
    {
        double stepSize = (upperBound - lowerBound) / NumDivisions;
        double integral = 0.0;
        double currentX = lowerBound;

        for (int i = 0; i < NumDivisions; i++)
        {
            integral += 0.5 * stepSize * (function(currentX) + function(currentX + stepSize));
            currentX += stepSize;
        }

        return integral;
    }
}