#ifndef T4VPN_NETWORK_UTIL_NETWORK_CONFIGURATION_H
#define T4VPN_NETWORK_UTIL_NETWORK_CONFIGURATION_H

#include <Windows.h>
#include <Netcfgx.h>
#include <atlbase.h>

#include "NetworkIPv6Settings.h"
#include "NetInterface.h"

#include <string>
#include <vector>

namespace T4VPN
{
    namespace NetworkUtil
    {
        class NetworkConfiguration
        {
        public:
            virtual ~NetworkConfiguration();

            static NetworkConfiguration instance();

            void initialize();

            void applyChanges();

            CComPtr<INetCfgLock> acquireWriteLock(
                DWORD timeoutInMs,
                const std::wstring& appName);

            NetworkIPv6Settings ipv6Settings();

            const std::vector<NetInterface> getNetworkInterfacesById(const std::wstring& id);

        protected:
            NetworkConfiguration(CComPtr<INetCfg> config);

        private:
            CComPtr<INetCfg> config;
        };
    }
}

#endif // T4VPN_NETWORK_UTIL_NETWORK_CONFIGURATION_H
