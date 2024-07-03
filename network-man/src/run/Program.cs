using System;
using NetTools;
using System.Net;

namespace run
{
    internal class Program
    {
        static IPAddressRange RangeA { get;  set; }
        static IPAddressRange RangeB { get;  set; }

        static void Main(string[] args)
        {
            // rangeA.Begin is "192.168.0.0", and rangeA.End is "192.168.0.255".
            RangeA = new IPAddressRange();
            var rangeA = IPAddressRange.Parse("192.168.0.0/255.255.255.0");
            Console.WriteLine("Parse: " + rangeA);
            Console.WriteLine(" Range: " + RangeA.Contains(IPAddress.Parse("192.168.0.34"))); // is True.
            Console.WriteLine(" Range not: " + RangeA.Contains(IPAddress.Parse("192.168.10.1"))); // is False.
            Console.WriteLine(" CIDR: " + RangeA.ToCidrString()); // is 192.168.0.0/24
            RangeB = new IPAddressRange();
            // rangeB.Begin is "192.168.0.10", and rangeB.End is "192.168.10.20".
            var rangeB1 = IPAddressRange.Parse("192.168.0.10 - 192.168.10.20");
            Console.WriteLine("Parse: " + rangeB1);
            Console.WriteLine(" Range: " + RangeB.Contains(IPAddress.Parse("192.168.3.45"))); // is True.
            Console.WriteLine(" Range not: " + RangeB.Contains(IPAddress.Parse("192.168.0.9"))); // is False.
            Console.WriteLine(" CIDR: " + RangeB.ToCidrString()); // is 192.168.0.0/24

            Console.WriteLine("Probe completed.");
        }
    }
}
