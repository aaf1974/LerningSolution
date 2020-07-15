using NetCodeExample.Examples;
using NetCoreLibrary;
using System;
using System.Diagnostics;

namespace NetCodeExample
{
    class SampleRunner
    {
        internal static void RunSample(SampleEnum sample)
        {
             switch(sample)
            {
                case SampleEnum.Deconstruct:
                    DeconstructorSampleUsing.PrintSample();
                    break;

                case SampleEnum.DictionaryDeconstruct:
                    DeconstructorDictSample.PrintSample();
                    break;

                case SampleEnum.Switch:
                    SwitchSample.PrintSample();
                    break;

                case SampleEnum.INstanceWitoutConstructorCall:
                    INstanceWitoutConstructorCall.PrintSample();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
