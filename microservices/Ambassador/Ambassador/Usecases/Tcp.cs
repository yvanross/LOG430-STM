using ServiceMeshHelper.Services;

namespace ServiceMeshHelper.Usecases
{
    internal class Tcp
    {
        private NodeControllerRoutingClient _nodeController = new ();

        internal Task<int> Preflight(string target)
        {
            return _nodeController.NegotiateSocket(target);
        }
    }
}
