using Leaf.xNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Proxy_Checker
{
    internal class Program
    {
        static List<string> ProxyList = new List<string>();
        static List<string> GoodProxy = new List<string>();
        static void Main(string[] args)
        {
            var Url = "https://www.google.com/";

            Console.WriteLine($"[{Environment.MachineName}]~Proxy Checker | Thread: 500 | Http/s | Socks4 | Socks4A | Socks5 | Custom Url");
            Console.WriteLine($"[{Environment.MachineName}]~Load Proxy(Enter Path)");
            Console.Write($"[{Environment.MachineName}]~");

            var Path = Console.ReadLine();

            Console.WriteLine($"[{Environment.MachineName}]~1.Http/s");
            Console.WriteLine($"[{Environment.MachineName}]~2.Socks4~");
            Console.WriteLine($"[{Environment.MachineName}]~3.Socks4A");
            Console.WriteLine($"[{Environment.MachineName}]~4.Socks5");
            Console.Write($"[{Environment.MachineName}]~");

            var Type = Console.ReadLine();

            Console.WriteLine($"[{Environment.MachineName}]~1.Enter Url");
            Console.Write($"[{Environment.MachineName}]~");

            Url = Console.ReadLine();

            X:
            try
            {
                ProxyList.AddRange(File.ReadLines(Path));
            }
            catch
            {
                Console.WriteLine($"[{Environment.MachineName}]~Path Invalid Load Agen.");
                Console.Write($"[{Environment.MachineName}]~");
                Path = Console.ReadLine();
                goto X;
            }

            var Info = new ParallelOptions();
        
            if(ProxyList.Count > 1000)
                Info.MaxDegreeOfParallelism = 500;
            else if (ProxyList.Count > 500)
                Info.MaxDegreeOfParallelism = 100;
            else if (ProxyList.Count > 100)
                Info.MaxDegreeOfParallelism = 5;

            Parallel.ForEach(ProxyList,Info,Proxy =>
            {
                CheckProxy(Proxy,Type,Url);
            });

            Console.WriteLine($"[{Environment.MachineName}]~Please Wait...");
            Console.ReadKey();
        }
        static void CheckProxy(string Proxy, string Type, string Url)
        {
            try
            {
                var Request = new HttpRequest();

                Request.UserAgent = Http.RandomUserAgent();

                if (Type == "1")
                    Request.Proxy = HttpProxyClient.Parse(Proxy);
                else if (Type == "2")
                    Request.Proxy = Socks4ProxyClient.Parse(Proxy);
                else if (Type == "3")
                    Request.Proxy = Socks4AProxyClient.Parse(Proxy);
                else if (Type == "4")
                    Request.Proxy = Socks5ProxyClient.Parse(Proxy);

                var Response = Request.Get(Url);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"[{Environment.MachineName}]~{Proxy}");
                GoodProxy.Add(Proxy);

                File.WriteAllLines($"GoodProxy.txt", GoodProxy);
            }
            catch (HttpException ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[{Environment.MachineName}]~{Proxy}");
            }
        }
    }
}
