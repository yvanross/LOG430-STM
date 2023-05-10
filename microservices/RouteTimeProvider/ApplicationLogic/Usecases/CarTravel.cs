using Entities.DomainInterfaces;

namespace ApplicationLogic.Usecases
{
    public class CarTravel
    {
        public async Task<int> GetTravelTimeInSeconds(string startingCoordinates, string destinationCoordinates, IRouteTimeProvider routeTimeProvider)
        {
            return await routeTimeProvider.GetTravelTimeInSeconds(RemoveWhiteSpaces(startingCoordinates), RemoveWhiteSpaces(destinationCoordinates));

            string RemoveWhiteSpaces(string s) => s.Replace(" ", "");
        }
    }
}
