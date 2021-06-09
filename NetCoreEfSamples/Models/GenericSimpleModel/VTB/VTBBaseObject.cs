using NetCoreEfSamples.Models.GenericSimpleModel.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreEfSamples.Models.GenericSimpleModel.VTB
{
    class VTBBaseObject : BaseObject<VTBBaseObjectRole, VTBBaseObjectGeoObject>
    {
        public BaseObject<VTBBaseObjectRole, VTBBaseObjectGeoObject> BaseObject { get; set; }
    }
}
