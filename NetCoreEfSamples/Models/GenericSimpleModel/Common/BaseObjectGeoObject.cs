using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace NetCoreEfSamples.Models.GenericSimpleModel.Common
{
    public class BaseObjectGeoObject
    {
        public int Id { get; set; }

        public string BaseObjectGeoObjectTitle { get; set; }


        [NotMapped]
        public ICollection<IBaseObject> BaseObjects { get; set; }
    }
}
