using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreEfSamples.Models.GenericBaseModel
{
    //public abstract class IBaseObject<T1, T2>
    //{
    //    int _id { get; set; }
    //    public int Id { get => _id; set => _id = value; }
    //}
    public interface IBaseObject<T1, T2>
    {
        public int Id { get; set; }

        public string BaseObjectTitle { get; set; }

        public T1 BaseObjectRole { get; set; }

        public int BaseObjectRoleId { get; set; }

        public T2 BaseObjectGeoObject { get; set; }

        public int BaseObjectGeoObjectId { get; set; }
    }

    public interface IBaseObject : 
        IBaseObject<BaseObjectRole, BaseObjectGeoObject>
    { 
        
    }

    public class BaseObject : IBaseObject
    {
        public int Id { get; set; }

        public string BaseObjectTitle { get; set; }

        public BaseObjectRole BaseObjectRole { get; set; }

        public int BaseObjectRoleId { get; set; }

        public BaseObjectGeoObject BaseObjectGeoObject { get; set; }

        public int BaseObjectGeoObjectId { get; set; }
    }
}
