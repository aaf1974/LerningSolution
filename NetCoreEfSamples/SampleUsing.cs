using NetCoreEfSamples.Context;
using NetCoreEfSamples.Models.GenericBaseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NetCoreEfSamples
{
    class SampleUsing
    {
        private GenericSampleCtx _genericSampleCtx;

        public SampleUsing(GenericSampleCtx genericSampleCtx)
        {
            _genericSampleCtx = genericSampleCtx;
        }

        void Sample()
        {
            //Models.GenericBaseModel.BaseObjectRole_VTB n = _genericSampleCtx.BaseObject.FirstOrDefault().BaseObjectRole;
            
            _genericSampleCtx.BaseObjectRoles.Where(x => x.BaseObject.Id == 5);
        }
    }
}
