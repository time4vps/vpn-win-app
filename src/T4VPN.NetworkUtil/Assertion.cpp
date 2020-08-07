#include "StdAfx.h"
#include "Assertion.h"
#include <comdef.h>

namespace T4VPN
{
    namespace NetworkUtil
    {
        void assertSuccess(HRESULT result)
        {
            if (result != S_OK)
            {
                throw _com_error(result);
            }
        }
    }
}
