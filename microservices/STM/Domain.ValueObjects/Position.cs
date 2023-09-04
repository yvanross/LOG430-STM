using Domain.Common.Seedwork.Interfaces;

namespace Domain.ValueObjects;

public record Position(double Latitude, double Longitude) : IValueObject<Position>
{
    public double DistanceInMeters(Position other)
    {
        const int earthRadius = 6371;

        var aPrime = new Position(ToRad(Latitude), ToRad(Longitude));

        var bPrime = new Position(ToRad(other.Latitude), ToRad(other.Longitude));

        var lat = Math.Sin((bPrime.Latitude - aPrime.Latitude) / 2);
        var lon = Math.Sin((bPrime.Longitude - aPrime.Longitude) / 2);

        var h1 = Math.Sin(lat / 2) * Math.Sin(lat / 2) +
                 Math.Cos(aPrime.Latitude) * Math.Cos(bPrime.Latitude) *
                 Math.Sin(lon / 2) * Math.Sin(lon / 2);
        var h2 = 2 * Math.Asin(Math.Min(1, Math.Sqrt(h1)));

        return h2 * earthRadius * 1000;
    }

    private static double ToRad(double degree)
    {
        return degree * (Math.PI / 180);
    }
}