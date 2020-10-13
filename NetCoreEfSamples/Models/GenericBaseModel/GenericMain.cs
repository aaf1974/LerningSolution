using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreEfSamples.Models.GenericBaseModel
{
    public class GenericMain<T>
        where T: GenericPropBase
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public T NavProp { get; set; }

        public int NavPropId { get; set; }
    }
}
