using Newtonsoft.Json;

namespace T4VPN.Core.Servers.Contracts
{
    public class GuestHoleServerContract
    {
        [JsonProperty("host")]
        public string Host { get; set; }

        [JsonProperty("ip")]
        public string Ip { get; set; }
    }
}