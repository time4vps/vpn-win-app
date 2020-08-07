#pragma once
#include <string>
#include <vector>
#include <cstdint>

namespace ipfilter
{
    class NetInterface
    {
    public:
        NetInterface(const std::string& name, uint64_t localId);

        std::string getName() const;

        uint64_t getLocalId() const;

    private:
        std::string name;
        uint64_t localId;
    };

    std::vector<NetInterface> getNetworkInterfaces();

    std::vector<NetInterface>::iterator findNetworkInterfaceByName(
        std::vector<NetInterface>& interfaces,
        const std::string& name);
}
