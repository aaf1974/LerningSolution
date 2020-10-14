using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreEfSamples.Models.GenericSimpleModel.Common
{
    public class BaseObject<T1, T2>
        where T1 : BaseObjectRole
        where T2: BaseObjectGeoObject
    {
        public int Id { get; set; }

        public string BaseObjectTitle { get; set; }

        public T1 BaseObjectRole { get; set; }

        public int BaseObjectRoleId { get; set; }

        public T2 BaseObjectGeoObject { get; set; }

        public int BaseObjectGeoObjectId { get; set; }
    }
}
