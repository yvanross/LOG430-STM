namespace ApplicationLogic.Interfaces;

public interface IAuthorizationService
{
    Task<string?> Authorize(string username, string secret);

    Task Register(string name, string secret, string teamName);
}