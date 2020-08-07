#ifndef T4VPN_NETWORK_UTIL_IPADDRESS_H
#define T4VPN_NETWORK_UTIL_IPADDRESS_H

#include <Windows.h>

class IpAddress
{
private:
    PCWSTR address;
    UINT32 pIPv4Address;
public:
    IpAddress(PCWSTR address);
    UINT32 IPv4();
    UINT32 ReversedIPv4();
    ~IpAddress();
};

#endif
