using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreEfSamples.Models.GenericBaseModel
{
    public class BaseObjectGeoObject
        // where T1 : BaseObjectRole
        //where T2 : BaseObjectGeoObject<T1, T2>
    {
        public int Id { get; set; }

        public string BaseObjectGeoObject_Title { get; set; }

        //public ICollection<BaseObject<T1, T2>> BaseObjects { get; set; }

        public ICollection<BaseObject> BaseObjects { get; set; }
    }
}
