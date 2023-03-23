using ApplicationLogic.Interfaces;
using Entities.BusinessObjects;

namespace ApplicationLogic.Usecases
{
    public class SubscriptionUC
    {
        private readonly IRepositoryWrite _repositoryWrite;
        
        public SubscriptionUC(IRepositoryWrite repositoryWrite)
        {
            _repositoryWrite = repositoryWrite;
        }

        public void Subscribe(string id, string address, string port)
        {
            var newNode = new Node()
            {
                Name = id,
                Address = address,
                Port = port
            };

            _repositoryWrite.AddOrUpdateNode(newNode);
        }
    }
}
