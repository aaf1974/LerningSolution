using NetCodeExample.Examples;
using NetCodeExample.Examples.DiRegExample;
using NetCodeExample.Examples.EFSamples;
using System;

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

                case SampleEnum.Check_SQL_ARITHABORT:
                    CheckSQL_ARITHABORT.PrintSample();
                    break;

                case SampleEnum.EFDisconectedRepoChangeLogSample:
                    EFChangeLogSample.PrintDiscinectedSample();
                    break;

                case SampleEnum.EFConnectedRepoChangeLogSample:
                    EFChangeLogSample.PrintConectedSample();
                    break;

                case SampleEnum.Двойная_Регистрация_DI:
                    TwiceRegSample.PrintConectedSample();
                    break;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}
