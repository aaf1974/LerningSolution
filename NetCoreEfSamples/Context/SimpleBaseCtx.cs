using Microsoft.EntityFrameworkCore;
using NetCoreEfSamples.Models.SimpleBase;

namespace NetCoreEfSamples.Context
{

    //Add-Migration Sample_SimpleBase -Context SimpleBaseCtx
    //Removing migration '20201013102027_Sample_SimpleBase'

    /// <summary>
    /// 
    /// </summary>
    public class SimpleBaseCtx : BaseSampleContext
    {
        public DbSet<ChildEntity> childEntities { get; set; }
    }
}
