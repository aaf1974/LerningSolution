using Microsoft.EntityFrameworkCore;
using NetCoreEfSamples.Models.GenericBaseModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreEfSamples.Context
{
    //Add-Migration Sample_GenericSample -Context GenericSampleCtx
    //update-database -Context GenericSampleCtx
    class GenericSampleCtx : BaseSampleContext
    {
        public DbSet<BaseObject_VTB> BaseObject { get; set; }

        public DbSet<BaseObjectRole_VTB> BaseObjectRoles { get; set; }

        public DbSet<BaseObjectGeoObject_VTB> BaseObjectGeoObjects { get; set; }
    }
}
