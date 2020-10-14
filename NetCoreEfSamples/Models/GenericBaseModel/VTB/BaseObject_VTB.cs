using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreEfSamples.Models.GenericBaseModel
{

    public interface IBaseObjectVTB :
       IBaseObject<BaseObjectRole_VTB, BaseObjectGeoObject_VTB>
    {

    }


    class BaseObject_VTB : BaseObject, IBaseObjectVTB
    {
        public string VtbBaseObjectValue { get; set; }
        public BaseObject_VTB()
        {
        }
        public new  BaseObjectRole_VTB BaseObjectRole { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public new BaseObjectGeoObject_VTB BaseObjectGeoObject { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
