using Application.Interfaces;

namespace Application.Usecases
{
    public class CarTravel
    {
        private readonly IRouteTimeProvider _routeTimeProvider;

        public CarTravel(IRouteTimeProvider routeTimeProvider)
        {
            _routeTimeProvider = routeTimeProvider;
        }

        public async Task<int> GetTravelTimeInSeconds(string startingCoordinates, string destinationCoordinates)
        {
            return await _routeTimeProvider.GetTravelTimeInSeconds(RemoveWhiteSpaces(startingCoordinates), RemoveWhiteSpaces(destinationCoordinates));

            string RemoveWhiteSpaces(string s) => s.Replace(" ", "");
        }
    }
}
