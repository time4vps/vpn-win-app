#include "StdAfx.h"
#include <comdef.h>
#include "NetworkConfiguration.h"
#include "NetworkIPv6Settings.h"
#include "BestInterface.h"
#include "NetInterface.h"
#include "Route.h"

#include <string>
#include <set>
#include <algorithm>

#define EXPORT __declspec(dllexport)

const DWORD LockTimeoutMs = 5000;

extern "C" EXPORT long NetworkUtilEnableIPv6(const wchar_t* appName, const wchar_t* interfaceId)
{
    try
    {
        auto networkConfig = T4VPN::NetworkUtil::NetworkConfiguration::instance();
        auto lock = networkConfig.acquireWriteLock(LockTimeoutMs, appName);
        networkConfig.initialize();

        networkConfig.ipv6Settings().enableIPv6OnInterfacesWithId(interfaceId);
        networkConfig.applyChanges();

        lock->ReleaseWriteLock();
    }
    catch (const _com_error& error)
    {
        return error.Error();
    }

    return 0;
}

extern "C" EXPORT long NetworkUtilEnableIPv6OnAllAdapters(wchar_t* appName, const wchar_t* excludeId)
{
    std::set<std::wstring> excludeIds{};
    if (excludeId != nullptr)
    {
        excludeIds.insert(excludeId);
    }

    try
    {
        auto networkConfig = T4VPN::NetworkUtil::NetworkConfiguration::instance();
        auto lock = networkConfig.acquireWriteLock(LockTimeoutMs, appName);
        networkConfig.initialize();

        networkConfig.ipv6Settings().enableIPv6OnAllAdapters(true, excludeIds);
        networkConfig.applyChanges();

        lock->ReleaseWriteLock();
    }
    catch (const _com_error& error)
    {
        return error.Error();
    }

    return 0;
}

extern "C" EXPORT long NetworkUtilDisableIPv6OnAllAdapters(wchar_t* appName, const wchar_t* excludeId)
{
    std::set<std::wstring> excludeIds{};
    if (excludeId != nullptr)
    {
        excludeIds.insert(excludeId);
    }

    try
    {
        auto networkConfig = T4VPN::NetworkUtil::NetworkConfiguration::instance();
        auto lock = networkConfig.acquireWriteLock(LockTimeoutMs, appName);
        networkConfig.initialize();

        networkConfig.ipv6Settings().enableIPv6OnAllAdapters(false, excludeIds);
        networkConfig.applyChanges();

        lock->ReleaseWriteLock();
    }
    catch (const _com_error& error)
    {
        return error.Error();
    }

    return 0;
}

extern "C" EXPORT DWORD GetBestInterfaceIp(IN_ADDR * address, const wchar_t* excludedIfaceHwid)
{
    std::vector<std::wstring> excludedIfaceGuids{};

    try
    {
        auto networkConfig = T4VPN::NetworkUtil::NetworkConfiguration::instance();

        networkConfig.initialize();

        for (auto iface : networkConfig.getNetworkInterfacesById(excludedIfaceHwid))
        {
            excludedIfaceGuids.push_back(iface.bindName());
        }
    }
    catch (const _com_error& error)
    {
        return error.Error();
    }

    BestInterface bestInterface;
    *address = bestInterface.IpAddress(excludedIfaceGuids);

    return 0;
}

extern "C" EXPORT long NetworkUtilAddDefaultGatewayForIface(const GUID * ifaceId, wchar_t* gatewayAddr)
{
    if (!T4VPN::NetworkUtil::Route::AddDefaultGatewayForIface(ifaceId, gatewayAddr))
    {
        return 1;
    }

    return 0;
}

extern "C" EXPORT long NetworkUtilDeleteDefaultGatewayForIface(const GUID * ifaceId, wchar_t* gatewayAddr)
{
    if (!T4VPN::NetworkUtil::Route::DeleteDefaultGatewayForIface(ifaceId, gatewayAddr))
    {
        return 1;
    }

    return 0;
}