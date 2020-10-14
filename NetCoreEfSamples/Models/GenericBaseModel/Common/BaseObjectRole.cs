using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreEfSamples.Models.GenericBaseModel
{
    public class BaseObjectRole
    {
        public int Id { get; set; }

        public string PropBaseValue { get; set; }

        public BaseObject BaseObject { get; set; }
    }
}
