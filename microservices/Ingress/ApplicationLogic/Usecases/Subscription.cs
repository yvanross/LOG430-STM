using ApplicationLogic.Interfaces;
using Entities.BusinessObjects;
using Polly;

namespace ApplicationLogic.Usecases
{
    public class Subscription
    {
        private readonly IRepositoryWrite _repositoryWrite;

        private readonly IAuthorizationService _authorizationService;

        public Subscription(IRepositoryWrite repositoryWrite, IAuthorizationService authorizationService)
        {
            _repositoryWrite = repositoryWrite;
            _authorizationService = authorizationService;
        }

        public async Task<string?> Authorize(string username, string secret)
        {
            return await _authorizationService.Authorize(username, secret);
        }

        public async Task Subscribe(string teamName, string username, string address, string port, string secret, string version)
        {
            var retryPolicy = Policy
                .Handle<Exception>()
                .RetryAsync(1, (_, _) => _authorizationService.Register(username, secret, teamName));

            await retryPolicy.ExecuteAsync(async () => await _authorizationService.Authorize(username, secret) is not null);

            var newNode = new Node()
            {
                Name = $"{teamName}-{username}",
                Address = address,
                Port = port,
                Version = version
            };

            _repositoryWrite.AddOrUpdateNode(newNode);
        }
    }
}
