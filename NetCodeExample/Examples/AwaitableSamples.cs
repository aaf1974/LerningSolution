using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NetCodeExample.Examples
{
    //https://habr.com/ru/post/509082/
    class AwaitableSamples
    {
        void DoSomethingClassic()
        {
            DoSomethingAsync().ContinueWith((task1) => {
                if (task1.IsCompletedSuccessfully)
                {
                    DoSomethingElse1Async(task1.Result).ContinueWith((task2) => {
                        if (task2.IsCompletedSuccessfully)
                        {
                            DoSomethingElse2Async(task2.Result).ContinueWith((task3) => {
                                //TODO: Do something
                            });
                        }
                    });
                }
            });
        }


        private Task<int> DoSomethingAsync() => throw new NotImplementedException();
        private Task<int> DoSomethingElse1Async(int i) => throw new NotImplementedException();
        private Task<int> DoSomethingElse2Async(int i) => throw new NotImplementedException();


        async Task DoSomethingBeautiful()
        {
            var res1 = await DoSomethingAsync();
            var res2 = await DoSomethingElse1Async(res1);
            await DoSomethingElse2Async(res2);
        }
    }

    
}
