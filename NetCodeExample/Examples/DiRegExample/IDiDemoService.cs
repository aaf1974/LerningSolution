using System;

namespace NetCodeExample.Examples.DiRegExample
{
    //https://stackoverflow.com/questions/39174989/how-to-register-multiple-implementations-of-the-same-interface-in-asp-net-core
    public interface IDiDemoService
    {
        void DoTheThing();
    }


    internal class DiDemoServiceC : IDiDemoService
    {
        public void DoTheThing()
        {
            throw new NotImplementedException();
        }
    }

    internal class DiDemoServiceB : IDiDemoService
    {
        public void DoTheThing()
        {
            throw new NotImplementedException();
        }
    }

    internal class DiDemoServiceA : IDiDemoService
    {
        public void DoTheThing()
        {
            throw new NotImplementedException();
        }
    }
}
