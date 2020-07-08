using System;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleTool2
{
    class Program
    {
        static void Main(string[] args)
        {
            TaskMethod("Task 0");
            var t1 = new Task(()=> { TaskMethod("Task 1"); });
            var t2 = new Task(() => { TaskMethod("Task 2"); });
            t2.Start();
            t1.Start();

            Task.Run(()=> TaskMethod("Task 3"));
            Task.Factory.StartNew(() => TaskMethod("Task 4"));
            Task.Factory.StartNew(() => TaskMethod("Task 5"),TaskCreationOptions.LongRunning);
            Console.ReadLine();

        }

       static  void TaskMethod(string name)
        {
            Console.WriteLine("task {0} is running on a thread id {1}, is thread pool thread:{2}",name,Thread.CurrentThread.ManagedThreadId,Thread.CurrentThread.IsThreadPoolThread);
        }
    }
}