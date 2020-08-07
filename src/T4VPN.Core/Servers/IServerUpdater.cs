using System;
using System.Threading.Tasks;

namespace T4VPN.Core.Servers
{
    public interface IServerUpdater
    {
        Task Update();

        event EventHandler ServersUpdated;
    }
}
