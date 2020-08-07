using System;
using System.Threading.Tasks;
using T4VPN.Common.Vpn;
using T4VPN.Core.Profiles;
using T4VPN.Core.Vpn;

namespace T4VPN.Core.Service.Vpn
{
    public interface IVpnManager
    {
        Task Connect(Profile profile);
        Task Reconnect();
        Task Disconnect(VpnError vpnError = VpnError.None);
        Task GetState();
        void OnVpnStateChanged(VpnStateChangedEventArgs e);
        event EventHandler<VpnStateChangedEventArgs> VpnStateChanged;
    }
}
