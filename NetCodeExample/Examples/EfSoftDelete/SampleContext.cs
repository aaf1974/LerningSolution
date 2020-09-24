using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;

namespace NetCodeExample.Examples.EfSoftDelete
{
    class SampleContext : DbContext
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ApplaySoftDeleteFilter(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }


        internal static void ApplaySoftDeleteFilter(ModelBuilder modelBuilder)
        {
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IDeletable).IsAssignableFrom(entityType.ClrType))
                {
                    modelBuilder.SetSoftDeleteFilter(entityType.ClrType);
                }
            }
        }

        #region Save Changes override
        //https://spin.atomicobject.com/2019/01/29/entity-framework-core-soft-delete/#:~:text=Soft%20Delete%20in%20EF%20Core&text=Adding%20a%20soft%20delete%20query,incorporate%20your%20soft%20delete%20functionality.&text=The%20above%20code%20intercepts%20any,the%20isDeleted%20column%20to%20true%20.
        public override int SaveChanges()
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
        {
            UpdateSoftDeleteStatuses();
            return base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        private void UpdateSoftDeleteStatuses()
        {
            foreach (var entry in ChangeTracker.Entries())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.CurrentValues[nameof(IDeletable.IsDeleted)] = false;
                        break;
                    case EntityState.Deleted:
                        entry.State = EntityState.Modified;
                        entry.CurrentValues[nameof(IDeletable.IsDeleted)] = true;
                        break;
                }
            }
        }
        #endregion

    }
}
