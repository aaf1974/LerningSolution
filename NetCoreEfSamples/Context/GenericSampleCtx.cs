﻿using Microsoft.EntityFrameworkCore;
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
        public DbSet<MainSample> mainSamples { get; set; }

        public DbSet<GenericPropChild> genericPropChildren { get; set; }
    }
}