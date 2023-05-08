namespace ApplicationLogic.Interfaces;

public interface IAuthorizationService
{
    Task<string?> Authorize(string username, string secret);

    Task Register(string name, string secret, string teamName, string group);

    Task<string[]> GetVisibleAccounts(string jwt);

    Task Remove(string nodeName);
}