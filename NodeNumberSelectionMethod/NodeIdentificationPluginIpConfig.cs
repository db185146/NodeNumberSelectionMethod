using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace NodeNumberSelectionMethod
{
    public class NodeIdentificationPluginIpConfig
    {
        public static int SelectNodeNumberUsingIp(IReadOnlyCollection<IPAddress> ipAddresses)
        {
            int GetNybbleAsInt(byte[] bytes, int startNdx) =>
                (bytes[startNdx] << 8) | bytes[startNdx + 1];

            // Get all non-loopback IP addresses
            var nonLoopbackIpAddresses = ipAddresses
                .Where(x => !IPAddress.IsLoopback(x))
                .ToList();

            Console.WriteLine("found the following nonloopback addresses:");
            foreach (var address in nonLoopbackIpAddresses)
            {
                Console.WriteLine(address);
            }

            // The IPv4 would be our first choice if it is available.
            // Sort any IPv4 addresses and take the first one.	
            // Sorting is done by the first two groups (byte 0, then byte 1).
            var pick1 = nonLoopbackIpAddresses
                .Where(x => x.AddressFamily == AddressFamily.InterNetwork)
                .Select(x => x.GetAddressBytes())
                .Where(x => x.Length == 4)
                .Select(x => new { bytes = x, rankA = x[0], rankB = x[1] })
                .OrderBy(x => x.rankA)
                .ThenBy(x => x.rankB)
                .Select(x => x.bytes)
                .FirstOrDefault();

            if (pick1 != null)
            {
                Console.WriteLine($"Node number {pick1[3]} determined from ipv4 address {pick1[0]}.{pick1[1]}.{pick1[2]}.{pick1[3]}");
                return pick1[3];
            }

            // IPv6 would be our second choice.
            // Sort any IPv6 addresses and take the first one.	
            // Sorting is done by the first two groups (bytes 0 & 1, then bytes 2 & 3).
            var pick2 = nonLoopbackIpAddresses
                .Where(x => x.AddressFamily == AddressFamily.InterNetworkV6)
                .Select(x => x.GetAddressBytes())
                .Where(x => x.Length == 16)
                .Select(x => new { bytes = x, rankA = GetNybbleAsInt(x, 0), rankB = GetNybbleAsInt(x, 2) })
                .OrderBy(x => x.rankA)
                .ThenBy(x => x.rankB)
                .Select(x => x.bytes)
                .FirstOrDefault();

            if (pick2 == null)
            {
                Console.WriteLine("unable to determine node number from IP.  returning -1");
                return -1;
            }

            Console.WriteLine($"Node number {GetNybbleAsInt(pick2, 14)} determined from ipv6 address");
            return GetNybbleAsInt(pick2, 14);
        }
    }
}
