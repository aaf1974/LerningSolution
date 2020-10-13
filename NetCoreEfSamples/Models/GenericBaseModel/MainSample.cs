using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreEfSamples.Models.GenericBaseModel
{
    class MainSample : GenericMain<GenericPropChild>
    {
        public string AnyVal { get; set; }
    }
}
