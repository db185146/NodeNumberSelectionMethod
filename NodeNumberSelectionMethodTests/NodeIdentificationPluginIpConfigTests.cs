using System.Collections.Generic;
using System.Net;
using NodeNumberSelectionMethod;
using Xunit;

namespace NodeNumberSelectionMethodTests
{
    public class NodeIdentificationPluginIpConfigTests
    {
        [Fact]
        private static void SelectNodeNumberUsingIp_ShouldUseIpv6AddressIfNoIpv4AddressesExist()
        {
            var result = NodeIdentificationPluginIpConfig.SelectNodeNumberUsingIp(new List<IPAddress>()
            {
                IPAddress.Parse("fe80::acae:9741:4c5a:1100%25"),
            });

            Assert.Equal(4352, result);
        }

        [Fact]
        private static void SelectNodeNumberUsingIp_ShouldUseFirstIpv4Address()
        {
            var result = NodeIdentificationPluginIpConfig.SelectNodeNumberUsingIp(new List<IPAddress>()
            {
                IPAddress.Parse("fe80::acae:9741:4c5a:1100%25"),
                IPAddress.Parse("153.87.46.2")
            });

            Assert.Equal(2, result);
        }

        [Fact]
        private static void SelectNodeNumberUsingIp_ShouldNotEvaluateLoopbackAddress()
        {
            var result = NodeIdentificationPluginIpConfig.SelectNodeNumberUsingIp(new List<IPAddress>()
            {
                IPAddress.Loopback,
                IPAddress.Parse("fe80::acae:9741:4c5a:1100%25"),
                IPAddress.Parse("153.87.46.2")
            });

            Assert.Equal(2, result);
        }

        [Fact]
        private static void SelectNodeNumberUsingIp_ShouldSortIpv4ByFirstTwoParts()
        {
            var result = NodeIdentificationPluginIpConfig.SelectNodeNumberUsingIp(new List<IPAddress>()
            {
                IPAddress.Parse("153.88.46.3"),
                IPAddress.Parse("154.87.46.4"),
                IPAddress.Parse("152.88.46.2")
            });

            Assert.Equal(2, result);
        }

        [Fact]
        private static void SelectNodeNumberUsingIp_ShouldSortIpv6ByFirstAndThirdNybbles()
        {
            var result = NodeIdentificationPluginIpConfig.SelectNodeNumberUsingIp(new List<IPAddress>()
            {
                IPAddress.Loopback,
                IPAddress.Parse("fe80::acae:9741:4c5a:1100%25"),
                IPAddress.Parse("2001:DB8:5000:AB00:2300:34:A4:801"),
            });

            Assert.Equal(2049, result);
        }

        [Fact]
        private static void SelectNodeNumberUsingIp_ShouldReturnNegativeOneWhenThereAreNoValidIps()
        {
            var result = NodeIdentificationPluginIpConfig.SelectNodeNumberUsingIp(new List<IPAddress>()
            {
                IPAddress.Loopback
            });

            Assert.Equal(-1, result);
        }
    }
}
