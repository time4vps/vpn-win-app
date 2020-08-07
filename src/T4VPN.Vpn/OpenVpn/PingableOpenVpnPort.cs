/*
 * Copyright (c) 2020 Time4VPS
 *
 * This file is part of T4VPN.
 *
 * T4VPN is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * T4VPN is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with T4VPN.  If not, see <https://www.gnu.org/licenses/>.
 */

using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using T4VPN.Common.Threading;
using T4VPN.Common.Vpn;
using T4VPN.Vpn.Common;

namespace T4VPN.Vpn.OpenVpn
{
    internal class PingableOpenVpnPort
    {
        private readonly byte[] _staticKey;

        public PingableOpenVpnPort(byte[] staticKey)
        {
            _staticKey = staticKey;
        }

        public async Task<bool> Alive(VpnEndpoint vpnEndpoint, Task timeoutTask)
        {
            var packet = new OpenVpnHandshake(_staticKey);
            var endpoint = new IPEndPoint(IPAddress.Parse(vpnEndpoint.Server.Ip), vpnEndpoint.Port);

            using var socket = new Socket(
                AddressFamily.InterNetwork,
                MapSocketType(vpnEndpoint.Protocol),
                MapProtocolType(vpnEndpoint.Protocol));
            try
            {
                await SafeSocketAction(socket.ConnectAsync(endpoint)).WithTimeout(timeoutTask);

                var bytes = packet.Bytes(vpnEndpoint.Protocol == VpnProtocol.OpenVpnTcp);
                await SafeSocketAction(socket.SendAsync(new ArraySegment<byte>(bytes), SocketFlags.None)).WithTimeout(timeoutTask);

                var answer = new byte[1024];
                var received = await SafeSocketFunc(socket.ReceiveAsync(new ArraySegment<byte>(answer), SocketFlags.None)).WithTimeout(timeoutTask);

                return received > 0;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                socket.Close();
            }
        }

        private ProtocolType MapProtocolType(VpnProtocol protocol)
        {
            if (protocol == VpnProtocol.OpenVpnTcp)
            {
                return ProtocolType.Tcp;
            }

            return ProtocolType.Udp;
        }

        private SocketType MapSocketType(VpnProtocol protocol)
        {
            if (protocol == VpnProtocol.OpenVpnUdp)
            {
                return SocketType.Dgram;
            }

            return SocketType.Stream;
        }

        private static Task SafeSocketAction(Task task)
        {
            task.ContinueWith(t =>
                {
                    switch (t.Exception?.InnerException)
                    {
                        case null:
                        case SocketException _:
                        case ObjectDisposedException _:
                            return;
                        default:
                            throw t.Exception;
                    }
                },
                TaskContinuationOptions.OnlyOnFaulted);

            return task;
        }

        private static Task<TResult> SafeSocketFunc<TResult>(Task<TResult> task)
        {
            task.ContinueWith(t =>
                {
                    switch (t.Exception?.InnerException)
                    {
                        case null:
                        case SocketException _:
                        case ObjectDisposedException _:
                            return;
                        default:
                            throw t.Exception;
                    }
                },
                TaskContinuationOptions.OnlyOnFaulted);

            return task;
        }
    }
}
