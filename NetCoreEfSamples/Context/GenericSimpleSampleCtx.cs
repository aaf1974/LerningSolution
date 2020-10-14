using Microsoft.EntityFrameworkCore;
using NetCoreEfSamples.Models.GenericSimpleModel.VTB;
using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreEfSamples.Context
{
    //Add-Migration Sample_GenericSimple -Context GenericSimpleSampleCtx
    //update-database -Context GenericSimpleSampleCtx
    class GenericSimpleSampleCtx : BaseSampleContext
    {
        public DbSet<VTBBaseObject> BaseObjects { get; set; }

        public DbSet<VTBBaseObjectRole> BaseObjectRoles { get; set; }

        public DbSet<VTBBaseObjectGeoObject> BaseObjectGeoObjects { get; set; }
    }
}
