using System;
using System.Net.NetworkInformation;
using System.Linq;

namespace AuthGateway.AuthEngine.Logic.Helpers
{
    public class MACAddress
    {
        public static string Get()
        {
            PhysicalAddress macAddress = (
                from nic in NetworkInterface.GetAllNetworkInterfaces()
                where nic.OperationalStatus == OperationalStatus.Up
                select nic.GetPhysicalAddress()
                ).FirstOrDefault();

            byte[] bytes = macAddress.GetAddressBytes();
            string MAC = "";
            for (int i = 0; i < bytes.Length; i++) {

                MAC += bytes[i].ToString("X2");

                if (i != bytes.Length - 1) {
                    MAC += ":";
                }
            }

            return MAC;
        }
    }
}
