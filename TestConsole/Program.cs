using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTool2
{
    class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                Task.Run(Producer);
                Thread.Sleep(200);
            }
        }

        static async Task Producer()
        {
             await Process();
        }

        static  void Producer1()
        {
             var result =Process().Result;
        }

        static async Task<bool> Process()
        {
            await Task.Run(() =>
            {
                Thread.Sleep(1000);
            });

            Console.WriteLine("Ended - " + DateTime.Now.ToLongTimeString());
            return true;
        }
    }
}