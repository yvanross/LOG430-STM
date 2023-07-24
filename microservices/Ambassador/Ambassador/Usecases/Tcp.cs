using ServiceMeshHelper.Clients;

namespace ServiceMeshHelper.Usecases
{
    internal class Tcp
    {
        private NodeControllerRoutingClient _nodeController = new ();

        internal Task<int> Preflight(string target)
        {
            return _nodeController.NegotiateSocketForServiceType(target);
        }
    }
}
