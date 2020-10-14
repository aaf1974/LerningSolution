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
        private readonly GenericSimpleSampleCtx _simpleGeneric;

        public SampleUsing(GenericSampleCtx genericSampleCtx, 
            GenericSimpleSampleCtx simpleGeneric)
        {
            _genericSampleCtx = genericSampleCtx;
            _simpleGeneric = simpleGeneric;
        }

        void Sample()
        {
            //Models.GenericBaseModel.BaseObjectRole_VTB n = _genericSampleCtx.BaseObject.FirstOrDefault().BaseObjectRole;

            //_genericSampleCtx.BaseObjectRoles.Where(x => x.BaseObject.Id == 5);

            var c = _genericSampleCtx.BaseObject.First().BaseObjectGeoObject.BaseObjects.First().Id;
        }
    }
}
