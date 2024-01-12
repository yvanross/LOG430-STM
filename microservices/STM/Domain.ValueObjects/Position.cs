using Domain.Common.Seedwork.Interfaces;

namespace Domain.ValueObjects;

public record Position(double Latitude, double Longitude) : IValueObject<Position>
{
    public double DistanceInMeters(Position other)
    {
        const int earthRadius = 6371; // Radius of the Earth in kilometers

        double latDifference = ToRad(other.Latitude - Latitude);
        double lonDifference = ToRad(other.Longitude - Longitude);

        double a = Math.Sin(latDifference / 2) * Math.Sin(latDifference / 2) +
                   Math.Cos(ToRad(Latitude)) * Math.Cos(ToRad(other.Latitude)) *
                   Math.Sin(lonDifference / 2) * Math.Sin(lonDifference / 2);
        double c = 2 * Math.Asin(Math.Sqrt(a));

        var distance = earthRadius * c * 1000; // Convert to meters

        return distance;
    }

    private static double ToRad(double degree)
    {
        return degree * (Math.PI / 180);
    }

}