#ifndef T4VPN_NETWORK_UTIL_ASSERTION_H
#define T4VPN_NETWORK_UTIL_ASSERTION_H

#include <Windows.h>

namespace T4VPN
{
    namespace NetworkUtil
    {
        void assertSuccess(HRESULT result);
    }
}

#endif // T4VPN_NETWORKUTIL_ASSERTION_H
