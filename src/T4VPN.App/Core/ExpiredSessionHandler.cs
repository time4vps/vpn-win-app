using System.Threading.Tasks;
using T4VPN.Common.Threading;
using T4VPN.Common.Vpn;
using T4VPN.Core.Auth;
using T4VPN.Core.Service.Vpn;
using T4VPN.Core.Vpn;
using T4VPN.Login.ViewModels;

namespace T4VPN.Core
{
    internal class ExpiredSessionHandler : IVpnStateAware
    {
        private VpnStatus _vpnStatus;
        private readonly IScheduler _scheduler;
        private readonly IVpnServiceManager _vpnServiceManager;
        private readonly LoginViewModel _loginViewModel;
        private readonly UserAuth _userAuth;

        public ExpiredSessionHandler(
            IScheduler scheduler,
            IVpnServiceManager vpnServiceManager,
            LoginViewModel loginViewModel,
            UserAuth userAuth)
        {
            _userAuth = userAuth;
            _loginViewModel = loginViewModel;
            _vpnServiceManager = vpnServiceManager;
            _scheduler = scheduler;
        }

        public async void Execute()
        {
            await _scheduler.Schedule(async () =>
            {
                if (_vpnStatus != VpnStatus.Disconnected)
                {
                    await _vpnServiceManager.Disconnect(VpnError.Unknown);
                }

                _loginViewModel.OnSessionExpired();
                _userAuth.Logout();
            });
        }

        public Task OnVpnStateChanged(VpnStateChangedEventArgs e)
        {
            _vpnStatus = e.State.Status;

            return Task.CompletedTask;
        }
    }
}
