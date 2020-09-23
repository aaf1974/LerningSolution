using NetCoreLibrary;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NetCodeExample.Examples
{
    class AsyncFluentBuilderSample
    {
        internal static void RunSample()
        {
            List<(string, string)> outed = new List<(string, string)>
            {
                (string.Empty, "пример использования:"),
                (@"..\..\..\Examples\AsyncFluentBuilder.cs", nameof(SampleUsing))
            };

            CodeConsole.OutCode2Console(outed);

            var builder = SampleUsing();
            //builder.Task.Wait();
            //builder.Task.RunSynchronously();
        }

        static AsyncFluentBuilder SampleUsing()
        {
            AsyncFluentBuilder builder = new AsyncFluentBuilder().DoA().DoB(true).DoB(false);

            return builder;
        }
    }


    //https://stackoverflow.com/questions/32112418/how-to-design-fluent-async-operations
    //https://stackoverflow.com/questions/25302178/using-async-tasks-with-the-builder-pattern
    class AsyncFluentBuilder
    {
        public Task Task { get; private set; }
        public AsyncFluentBuilder()
        {
            // The entry point for the async work.
            // Spin up a completed task to start with so that we dont have to do null checks    
            this.Task = Task.FromResult<int>(0);
        }


        /// Does A and returns the `this` current fluent instance.
        public AsyncFluentBuilder DoA()
        {
            QueueWork(DoAInternal);
            return this;
        }

        /// Does B and returns the `this` current fluent instance.
        public AsyncFluentBuilder DoB(bool flag)
        {
            QueueWork(() => DoBInternal(flag));
            return this;
        }

        /// Synchronously perform the work for method A.
        private async void DoAInternal()
        {
            //Thread.Sleep(5000);
            await Task.Delay(5000);
            Console.WriteLine("Execute A method");
        }

        int i = 2;
        /// Synchronously perform the work for method B.
        private async void DoBInternal(bool flag)
        {
            await Task.Delay(500 * i);
            //Thread.Sleep(500*i);
            i--;
            Console.WriteLine($"Execute B method with {flag}");
        }


        private void QueueWork(Action work)
        {
            // queue up the work
            this.Task = this.Task.ContinueWith<AsyncFluentBuilder>(task =>
            {
                work();
                return this;
            }, TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}
