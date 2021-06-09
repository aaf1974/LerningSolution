using NetCoreLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NetCodeExample.Examples
{
    class SemaphoreSlimSample
    {
        internal static void PrintSample()
        {
            Sample s = new Sample();
            s.Run();
        }

        class Sample
        {
            private readonly SemaphoreSlim semaphore = new SemaphoreSlim(3);
            internal void Run()
            {
                var tasks = Enumerable.Range(0, 20).
                    Select(x => WorkINternal(x))
                    ;

                Task.Run(
                    async () =>
                    {
                        await Task.WhenAll(tasks);
                    }
                    );
            }

            async Task WorkINternal(int i)
            {
                await semaphore.WaitAsync();

                CodeConsole.WriteLineColor(
                    $"i = {i}, semaphore = {semaphore.CurrentCount}, taskCount = {ThreadPool.ThreadCount}, taskPending = {ThreadPool.PendingWorkItemCount}", 
                    ConsoleColor.Black, ConsoleColor.Green);

                await Task.Delay(5000);

                CodeConsole.WriteLineColor(
                    $"end task i = {i}, semaphore = {semaphore.CurrentCount}, taskCount = {ThreadPool.ThreadCount}, taskPending = {ThreadPool.PendingWorkItemCount}",
                    ConsoleColor.Black, ConsoleColor.Blue);

                semaphore.Release();
            }
        }
    }
}
