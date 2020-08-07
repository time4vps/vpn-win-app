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
using System.Runtime.InteropServices;

namespace T4VPN.NetworkFilter
{
    internal class PInvoke
    {
        private const string BinaryName = "T4VPN.IpFilter.dll";

        private static string BinaryPath => $"{(Environment.Is64BitOperatingSystem ? "x64" : "x86")}\\{BinaryName}";

        static PInvoke()
        {
            var handle = LoadLibrary(BinaryPath);
            if (handle == IntPtr.Zero)
            {
                throw new DllNotFoundException($"Failed to load binary: {BinaryPath}.");
            }
        }

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string path);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterCreateDynamicSession",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CreateDynamicSession(ref IntPtr handle);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterDestroySession",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint DestroySession(IntPtr handle);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterStartTransaction",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint StartTransaction(IntPtr sessionHandle);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterAbortTransaction",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint AbortTransaction(IntPtr sessionHandle);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterCommitTransaction",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CommitTransaction(IntPtr sessionHandle);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterCreateProvider",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CreateProvider(
            IntPtr sessionHandle,
            ref DisplayData displayData,
            [In, Out] ref Guid key);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterCreateProviderContext",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CreateProviderContext(
            IntPtr sessionHandle,
            ref DisplayData displayData,
            [In] ref Guid providerKey,
            uint size,
            IntPtr data,
            [In, Out] ref Guid key);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterDestroyProviderContext",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint DestroyProviderContext(
            IntPtr sessionHandle,
            [In] ref Guid key);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterCreateCallout",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CreateCallout(
            IntPtr sessionHandle,
            ref DisplayData displayData,
            [In] ref Guid providerKey,
            uint layer,
            [In, Out] ref Guid key);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterDestroyCallout",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint DestroyCallout(
            IntPtr sessionHandle,
            [In] ref Guid key);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterCreateSublayer",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CreateSublayer(
            IntPtr sessionHandle,
            [In] ref Guid providerKey,
            ref DisplayData displayData,
            uint weight,
            [In, Out] ref Guid key);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterDestroySublayer",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint DestroySublayer(
            IntPtr sessionHandle,
            [In] ref Guid key);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterDestroyFilter",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint DestroyFilter(
            IntPtr sessionHandle,
            [In] ref Guid key);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterCreateLayerFilter",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CreateLayerFilter(
            IntPtr sessionHandle,
            [In] ref Guid providerKey,
            [In] ref Guid sublayerKey,
            ref DisplayData displayData,
            uint layer,
            uint action,
            uint weight,
            [In] ref Guid calloutKey,
            [In] ref Guid providerContextKey,
            [In, Out] ref Guid filterKey);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterCreateRemoteIPv4Filter",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CreateRemoteIPv4Filter(
            IntPtr sessionHandle,
            [In] ref Guid providerKey,
            [In] ref Guid sublayerKey,
            ref DisplayData displayData,
            uint layer,
            uint action,
            uint weight,
            [In] ref Guid calloutKey,
            [In] ref Guid providerContextKey,
            [MarshalAs(UnmanagedType.LPStr)] string addr,
            [In, Out] ref Guid filterKey);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterCreateAppFilter",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CreateAppFilter(
            IntPtr sessionHandle,
            [In] ref Guid providerKey,
            [In] ref Guid sublayerKey,
            ref DisplayData displayData,
            uint layer,
            uint action,
            uint weight,
            [In] ref Guid calloutKey,
            [In] ref Guid providerContextKey,
            [MarshalAs(UnmanagedType.LPWStr)] string appPath,
            [In, Out] ref Guid filterKey);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterCreateRemoteTCPPortFilter",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CreateRemoteTCPPortFilter(
            IntPtr sessionHandle,
            [In] ref Guid providerKey,
            [In] ref Guid sublayerKey,
            ref DisplayData displayData,
            uint layer,
            uint action,
            uint weight,
            uint port,
            [In, Out] ref Guid filterKey);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterCreateRemoteUDPPortFilter",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CreateRemoteUDPPortFilter(
            IntPtr sessionHandle,
            [In] ref Guid providerKey,
            [In] ref Guid sublayerKey,
            ref DisplayData displayData,
            uint layer,
            uint action,
            uint weight,
            uint port,
            [In, Out] ref Guid filterKey);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterCreateRemoteNetworkIPv4Filter",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CreateRemoteNetworkIPv4Filter(
            IntPtr sessionHandle,
            [In] ref Guid providerKey,
            [In] ref Guid sublayerKey,
            ref DisplayData displayData,
            uint layer,
            uint action,
            uint weight,
            [In] ref Guid calloutKey,
            [In] ref Guid providerContextKey,
            ref NetworkAddress addr,
            [In, Out] ref Guid filterKey);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterCreateNetInterfaceFilter",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CreateNetInterfaceFilter(
            IntPtr sessionHandle,
            [In] ref Guid providerKey,
            [In] ref Guid sublayerKey,
            ref DisplayData displayData,
            uint layer,
            uint action,
            uint weight,
            [MarshalAs(UnmanagedType.LPStr)] string interfaceId,
            [In, Out] ref Guid filterKey);

        [DllImport(
            BinaryName,
            EntryPoint = "IPFilterCreateLoopbackFilter",
            CallingConvention = CallingConvention.Cdecl)]
        public static extern uint CreateLoopbackFilter(
            IntPtr sessionHandle,
            [In] ref Guid providerKey,
            [In] ref Guid sublayerKey,
            ref DisplayData displayData,
            uint layer,
            uint action,
            uint weight,
            [In, Out] ref Guid filterKey);
    }
}
