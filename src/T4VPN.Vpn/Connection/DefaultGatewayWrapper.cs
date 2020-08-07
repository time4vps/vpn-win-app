using System;
using System.Collections.Generic;
using T4VPN.Common;
using T4VPN.Common.Logging;
using T4VPN.Common.Os.Net;
using T4VPN.Common.OS.Net.NetworkInterface;
using T4VPN.Common.Vpn;
using T4VPN.Vpn.Common;

namespace T4VPN.Vpn.Connection
{
    internal class DefaultGatewayWrapper : IVpnConnection
    {
        private readonly IVpnConnection _origin;
        private readonly INetworkInterfaces _networkInterfaces;
        private readonly string _tapAdapterDescription;
        private readonly string _tapAdapterId;
        private readonly ILogger _logger;

        public DefaultGatewayWrapper(
            ILogger logger,
            string tapAdapterId,
            string tapAdapterDescription,
            INetworkInterfaces networkInterfaces,
            IVpnConnection origin)
        {
            _logger = logger;
            _tapAdapterDescription = tapAdapterDescription;
            _tapAdapterId = tapAdapterId;
            _networkInterfaces = networkInterfaces;
            _origin = origin;
            _origin.StateChanged += Origin_StateChanged;
        }

        public event EventHandler<EventArgs<VpnState>> StateChanged;
        public InOutBytes Total => _origin.Total;

        public void Connect(IReadOnlyList<VpnHost> servers, VpnConfig config, VpnProtocol protocol,
            VpnCredentials credentials)
        {
            AddDefaultGateway();

            _origin.Connect(servers, config, protocol, credentials);
        }

        public void Disconnect(VpnError error = VpnError.None)
        {
            _origin.Disconnect(error);
        }

        public void UpdateServers(IReadOnlyList<VpnHost> servers, VpnConfig config)
        {
            _origin.UpdateServers(servers, config);
        }

        private void Origin_StateChanged(object sender, EventArgs<VpnState> e)
        {
            OnStateChanged(e.Data);
        }

        private void OnStateChanged(VpnState state)
        {
            StateChanged?.Invoke(this, new EventArgs<VpnState>(state));
        }

        private void AddDefaultGateway()
        {
            try
            {
                var localInterfaceIp = NetworkUtil.GetBestInterfaceIp(_tapAdapterId).ToString();
                var tapInterface = _networkInterfaces.Interface(_tapAdapterDescription);
                var parseResult = Guid.TryParse(tapInterface.Id, out var guid);

                if (!parseResult)
                {
                    return;
                }

                NetworkUtil.DeleteDefaultGatewayForIface(guid, localInterfaceIp);
                NetworkUtil.AddDefaultGatewayForIface(guid, localInterfaceIp);
            }
            catch (NetworkUtilException e)
            {
                _logger.Error("Add default TAP gateway failed. Error code: " + e.Code);
            }
        }
    }
}