using System;
using NetTools;
using System.Net;
using System.Collections.Frozen;

namespace run
{
    internal class Program
    {
        static IPAddressRange RangeA { get; set; }
        static IPAddressRange RangeB { get; set; }

        static void Main(string[] args)
        {
            // rangeA.Begin is "192.168.0.0", and rangeA.End is "192.168.0.255".
            RangeA = new IPAddressRange();
            var rangeA = IPAddressRange.Parse("172.22.0.0/18");
            Console.WriteLine("Parse: " + rangeA);
            Console.WriteLine(" Range: " + RangeA.Contains(IPAddress.Parse("172.28.0.0"))); // is True.
            Console.WriteLine(" Range not: " + RangeA.Contains(IPAddress.Parse("172.22.62.1"))); // is False.
            Console.WriteLine(" CIDR: " + RangeA.ToCidrString()); // is 192.168.0.0/24
            RangeB = new IPAddressRange();
            // rangeB.Begin is "192.168.0.10", and rangeB.End is "192.168.10.20".
            var rangeB1 = IPAddressRange.Parse("172.28.88.0/24");
            Console.WriteLine("Parse: " + rangeB1);
            Console.WriteLine(" Range: " + RangeB.Contains(IPAddress.Parse("172.22.0.0"))); // is True.
            Console.WriteLine(" Range not: " + RangeB.Contains(IPAddress.Parse("172.28.87.9"))); // is False.
            Console.WriteLine(" CIDR: " + RangeB.ToCidrString()); // is 192.168.0.0/24

            Console.WriteLine("Probe completed.");
            Console.ReadLine();
        }
    }
