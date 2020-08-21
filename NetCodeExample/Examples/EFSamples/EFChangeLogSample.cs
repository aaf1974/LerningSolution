using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using NetCodeExample.Examples.EFSamples.DbTools;
using NetCoreLibrary;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NetCodeExample.Examples.EFSamples
{
    class EFChangeLogSample
    {
        internal static void PrintDiscinectedSample()
        {
            Repository repo = new Repository();

            var cnt = repo.Select<GCrane>().Count();
            Console.WriteLine("Toltal recod = " + cnt);
            if (cnt <= 0)
                CodeConsole.WriteLineColor("Not read data", ConsoleColor.Black, ConsoleColor.Red);
            else
                CodeConsole.WriteLineColor("Read any data for sample", ConsoleColor.Black, ConsoleColor.Green);

            repo = new Repository();
            var t1 = repo.FirstOrDefault<GCrane>(x => x.Id == 1);
            
            t1.CaretSpeed++;
            repo.Save(t1);
            CodeConsole.WriteLineColor("t1.CaretSpeed = " + t1.CaretSpeed, ConsoleColor.Black, ConsoleColor.Green);
        }

        internal static void PrintConectedSample()
        {
            Repository repo = new Repository();

            var cnt = repo.Select<GCrane>().Count();
            Console.WriteLine("Toltal recod = " + cnt);
            if (cnt <= 0)
                CodeConsole.WriteLineColor("Not read data", ConsoleColor.Black, ConsoleColor.Red);
            else
                CodeConsole.WriteLineColor("Read any data for sample", ConsoleColor.Black, ConsoleColor.Green);

            var t1 = repo.QueryableSelect<GCrane>(x => x.Id == 1).First();
            var t2 = repo.QueryableSelect<GCrane>(x => x.Id == 2).First();

            t1.InitX++;
            t2.InitY++;
            repo.SaveChanges();
            CodeConsole.WriteLineColor("t1.InitX = " + t1.InitX, ConsoleColor.Black, ConsoleColor.Green);
            CodeConsole.WriteLineColor("t2.InitY = " + t2.InitY, ConsoleColor.Black, ConsoleColor.Green);
        }

    }
}

namespace NetCodeExample.Examples.EFSamples.DbTools
{
    class SampleContext : DbContext
    {
        public SampleContext()
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server = localhost; Database = ContainerEditor; Integrated Security = SSPI; persist security info = True; ");
        }

        public DbSet<GCrane> GCranes { get; set; }
    }

    interface IEntity
    {
        int Id { get; set; }
    }

    public class GCrane : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(20, ErrorMessage = "Макс. длина поля CraneName =20")]
        public string CraneName { get; set; }
        public int InitX { get; set; }
        public int InitY { get; set; }
        public int Length { get; set; }
        public decimal FullUpSpeed { get; set; }
        public decimal EmptyUpSpeed { get; set; }
        public decimal FullDownSpeed { get; set; }
        public decimal EmptyDownSpeed { get; set; }
        public decimal CaretSpeed { get; set; }
        public decimal CraneMoveSpeed { get; set; }
        public decimal ZeizeTime { get; set; }
        public decimal ReleaseTime { get; set; }

    }


    class Repository
    {
        private SampleContext _context;

        public Repository()
        {
            _context = new SampleContext();
        }



        public int SaveChanges()
        {
            var changes = _context.ChangeTracker.Entries().Where(x => 
                (x.State == EntityState.Added) ||
                (x.State == EntityState.Deleted) ||
                (x.State == EntityState.Modified))
            .ToList();

            foreach (var c in changes)
            {
                var diference = GetChanges(c);
                PrintDiference(diference);
            }




            return _context.SaveChanges();
        }

        List<ChangesStruct> GetChanges(EntityEntry entityObject)
        {
            var res = entityObject.Entity.GetType().GetTypeInfo().DeclaredProperties
                .Select(x => new ChangesStruct
                {
                    PropertyName = x.Name,
                    NewValue = entityObject.Property(x.Name).CurrentValue.ToString(),
                    OldValue = entityObject.Property(x.Name).OriginalValue.ToString(),
                    //NewValue = entityObject.CurrentValues.GetValue<object>(x.Name).ToString(),
                    //OldValue = entityObject.OriginalValues.GetValue<object>(x.Name).ToString(),
                })
                .Where(x => x.OldValue != x.NewValue)
                .ToList();

            return res;
        }

        void PrintDiference(List<ChangesStruct> diference)
        {
            foreach (var d in diference)
            {
                CodeConsole.WriteLineColor($"Изменилось свойство {d.PropertyName}, old = {d.OldValue}, new = {d.NewValue}", ConsoleColor.Black, ConsoleColor.Gray);
            }
        }

        #region Save

        /// <summary>
        /// Сохранение объекта в БД
        /// </summary>
        public TEntity Save<TEntity>(TEntity current)
            where TEntity : class, IEntity
        {
            TEntity exist = _context.Set<TEntity>()
                //.AsQueryable()
                .AsTracking(QueryTrackingBehavior.TrackAll)
                .FirstOrDefault(x => x.Id == current.Id);

            if (exist != null)
            {
                _context.Entry(exist).State = EntityState.Detached;
                _context.Add(current);
                _context.Entry(current).State = EntityState.Modified;

                /*                var existsJson = JsonConvert.SerializeObject(exist);
                                var currentJson = JsonConvert.SerializeObject(current);

                                var existD = JsonConvert.DeserializeObject<Dictionary<string, string>>(existsJson);
                                var currentD = JsonConvert.DeserializeObject<Dictionary<string, string>>(currentJson);
                                var diference = existD
                                    .Join(currentD,
                                            x => x.Key,
                                            y => y.Key,
                                            (x, y) => new { x.Key, oldValue = x.Value, newValueV = y.Value })
                                    .Where(x => x.oldValue != x.newValueV)
                                    .ToList();*/
                List<ChangesStruct> diference = GetChanges(exist, current);
                PrintDiference(diference);
/*                foreach (var d in diference )
                {
                    CodeConsole.WriteLineColor($"Изменилось свойство {d.PropertyName}, old = {d.OldValue}, new = {d.NewValue}" , ConsoleColor.Black, ConsoleColor.Gray);
                }
*/
                /*                //exist = entity;
                                //_context.Update(exist);

                                var changeInfo = _context.ChangeTracker.Entries()
                                    .Where(t => t.State == EntityState.Modified)
                                    .Select(t => new {
                                    OProperties = t.OriginalValues.Properties,
                                    CProperties = t.CurrentValues.Properties
                                //Original = t.OriginalValues.PropertyNames.ToDictionary(pn => pn, pn => t.OriginalValues[pn]),
                                //Current = t.CurrentValues.PropertyNames.ToDictionary(pn => pn, pn => t.CurrentValues[pn]),
                            });

                                Dictionary<string, System.Tuple<object, object>> modified =
                                  _context.Entry(entity)
                                    .Properties.Where(p => p.IsModified)
                                    .Where(p => p.OriginalValue.ToString() != p.CurrentValue.ToString())
                                    .ToDictionary(p => p.Metadata.Name, p => new System.Tuple<object, object>(p.OriginalValue, p.CurrentValue));*/

                SaveChanges();
                return current;
            }
            else
            {
                _context.Add(current);
                //_context.SaveChanges();
                SaveChanges();
                return current;
            }
        }

        List<ChangesStruct> GetChanges(object original, object current)
        {
            Dictionary<string, string> buildPropertyDict(object entityObject)
            {
                var jString = JsonConvert.SerializeObject(entityObject);
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(jString);
            }

            var res = buildPropertyDict(current)
                .Join(buildPropertyDict(original),
                    cur => cur.Key,
                    orig => orig.Key,
                    (cur, orig) => new ChangesStruct { PropertyName = cur.Key, NewValue = cur.Value, OldValue = orig.Value }
                )
                .Where(x => x.OldValue != x.NewValue)
                .ToList();

            return res;
        }

        class ChangesStruct
        {
            public string PropertyName { get; set; }

            public string OldValue { get; set; }

            public string NewValue { get; set; }
        }

        #endregion

        #region Select Sync
        /// <summary>
        /// Получение данных из БД с инклудами
        /// </summary>
        public TEntity[] Select<TEntity>(params Expression<Func<TEntity, object>>[] includes)
            where TEntity : class
        {
            return Select(x => true, includes);
        }

        /// <summary>
        /// Получение данных из БД по условию с инклудами
        /// </summary>
        public TEntity[] Select<TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
            where TEntity : class
        {
            return QueryableSelect(predicate, includes)
                .AsNoTracking()
                .ToArray();
        }


        /// <summary>
        /// Поиск в БД первой записи соответсвующей условию или null с инклудами
        /// </summary>
        public TEntity FirstOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
            where TEntity : class
        {
            return QueryableSelect(predicate, includes)
                .AsNoTracking()
                .FirstOrDefault();
        }

        /// <summary>
        /// Поиск в БД первой записи соответсвующей условию с инклудами
        /// </summary>
        public TEntity First<TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
            where TEntity : class
        {
            return QueryableSelect(predicate, includes)
                .AsNoTracking()
                .First();
        }

        /// <summary>
        /// Поиск в БД единственной записи соответсвующей условию с инклудами. Ошибка в случае если найдено больше записей соотвествующие условию
        /// </summary>
        public TEntity SingleOrDefault<TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
            where TEntity : class
        {
            return QueryableSelect(predicate, includes)
                .AsNoTracking()
                .SingleOrDefault();
        }

        #endregion

        #region Queriable
        /// <summary>
        /// Получение данных из БД с инклудами
        /// </summary>
        public IQueryable<TEntity> QueryableSelect<TEntity>(params Expression<Func<TEntity, object>>[] includes)
            where TEntity : class
        {
            return QueryableSelect(pred => true, includes);
        }

        /// <summary>
        /// Получение данных по условию из БД с инклудами
        /// </summary>
        public IQueryable<TEntity> QueryableSelect<TEntity>(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] includes)
            where TEntity : class
        {
            IQueryable<TEntity> result = _context.Set<TEntity>().Where(predicate);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    result = result.Include(include);
                }
            }
            return result;
        }
        #endregion
    }

    public class AuditLog
    {
        [Key]
        public Guid AuditLogID { get; set; }

        [Required]
        [MaxLength(50)]
        public string UserID { get; set; }

        [Required]
        public DateTime EventDateUTC { get; set; }

        [Required]
        [MaxLength(1)]
        public string EventType { get; set; }

        [Required]
        [MaxLength(100)]
        public string TableName { get; set; }

        [Required]
        [MaxLength(100)]
        public string RecordID { get; set; }

        [Required]
        [MaxLength(100)]
        public string ColumnName { get; set; }

        public string OriginalValue { get; set; }

        public string NewValue { get; set; }
    }

    /*
    class Logger1
    {
        //https://stackoverflow.com/questions/17904631/how-can-i-log-all-entities-change-during-savechanges-using-ef-code-first

        List<AuditLog> AuditLogs = new List<AuditLog>();
        private SampleContext _context;

        public Logger1(SampleContext context)
        {
            _context = new SampleContext();
        }

        public int LoggingChanges()
        {
            int objectsCount;

            List<EntityEntry> newEntities = new List<EntityEntry>();

            // Get all Added/Deleted/Modified entities (not Unmodified or Detached)
            foreach (var entry in _context.ChangeTracker.Entries().Where
                (x => (x.State == EntityState.Added) ||
                    (x.State == EntityState.Deleted) ||
                    (x.State == EntityState.Modified)))
            {
                if (entry.State == EntityState.Added)
                {
                    newEntities.Add(entry);
                }
                else
                {
                    // For each changed record, get the audit record entries and add them
                    foreach (AuditLog changeDescription in GetAuditRecordsForEntity(entry))
                    {
                        this.AuditLogs.Add(changeDescription);
                    }
                }
            }

            // Default save changes call to actually save changes to the database
            objectsCount = _context.SaveChanges();

            // We don't have recordId for insert statements that's why we need to call this method again.
            foreach (var entry in newEntities)
            {
                // For each changed record, get the audit record entries and add them
                foreach (AuditLog changeDescription in GetAuditRecordsForEntity(entry, true))
                {
                    this.AuditLogs.Add(changeDescription);
                }

                // TODO: Think about performance here. We are calling db twice for one insertion.
                objectsCount += _context.SaveChanges();
            }

            return objectsCount;

            //return _context.SaveChanges();
        }

        /// <summary>
        /// Helper method to create record description for Audit table based on operation done on dbEntity
        /// - Insert, Delete, Update
        /// </summary>
        /// <param name="dbEntity"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private List<AuditLog> GetAuditRecordsForEntity(EntityEntry dbEntity, bool insertSpecial = false)
        {
            List<AuditLog> changesCollection = new List<AuditLog>();

            DateTime changeTime = DateTime.Now;

            // Get Entity Type Name.
            string tableName1 = dbEntity.GetTableName();

            // http://stackoverflow.com/questions/2281972/how-to-get-a-list-of-properties-with-a-given-attribute
            // Get primary key value (If we have more than one key column, this will need to be adjusted)
            string primaryKeyName = dbEntity.GetAuditRecordKeyName();

            int primaryKeyId = 0;
            object primaryKeyValue;

            if (dbEntity.State == EntityState.Added || insertSpecial)
            {
                primaryKeyValue = dbEntity.GetPropertyValue(primaryKeyName, true);

                if (primaryKeyValue != null)
                {
                    Int32.TryParse(primaryKeyValue.ToString(), out primaryKeyId);
                }

                // For Inserts, just add the whole record
                // If the dbEntity implements IDescribableEntity,
                // use the description from Describe(), otherwise use ToString()
                changesCollection.Add(new AuditLog()
                {
                    //UserID = userId,
                    EventDateUTC = changeTime,
                    EventType = ModelConstants.UPDATE_TYPE_ADD,
                    TableName = tableName1,
                    RecordID = primaryKeyId,  // Again, adjust this if you have a multi-column key
                    ColumnName = "ALL",    // To show all column names have been changed
                    NewValue = (dbEntity.CurrentValues.ToObject() is IAuditableEntity) ?
                                            (dbEntity.CurrentValues.ToObject() as IAuditableEntity).Describe() :
                                            dbEntity.CurrentValues.ToObject().ToString()
                }
                    );
            }

            else if (dbEntity.State == EntityState.Deleted)
            {
                primaryKeyValue = dbEntity.GetPropertyValue(primaryKeyName);

                if (primaryKeyValue != null)
                {
                    Int32.TryParse(primaryKeyValue.ToString(), out primaryKeyId);
                }

                // With deletes use whole record and get description from Describe() or ToString()
                changesCollection.Add(new AuditLog()
                {
                    UserId = userId,
                    EventDate = changeTime,
                    EventType = ModelConstants.UPDATE_TYPE_DELETE,
                    TableName = tableName1,
                    RecordId = primaryKeyId,
                    ColumnName = "ALL",
                    OriginalValue = (dbEntity.OriginalValues.ToObject() is IAuditableEntity) ?
                                        (dbEntity.OriginalValues.ToObject() as IAuditableEntity).Describe() :
                                        dbEntity.OriginalValues.ToObject().ToString()
                });
            }

            else if (dbEntity.State == EntityState.Modified)
            {
                primaryKeyValue = dbEntity.GetPropertyValue(primaryKeyName);

                if (primaryKeyValue != null)
                {
                    Int32.TryParse(primaryKeyValue.ToString(), out primaryKeyId);
                }

                foreach (string propertyName in dbEntity.OriginalValues.PropertyNames)
                {
                    // For updates, we only want to capture the columns that actually changed
                    if (!object.Equals(dbEntity.OriginalValues.GetValue<object>(propertyName),
                            dbEntity.CurrentValues.GetValue<object>(propertyName)))
                    {
                        changesCollection.Add(new AuditLog()
                        {
                            //UserID = userId,
                            EventDateUTC = changeTime,
                            EventType = ModelConstants.UPDATE_TYPE_MODIFY,
                            TableName = tableName1,
                            RecordID = primaryKeyId,
                            ColumnName = propertyName,
                            OriginalValue = dbEntity.OriginalValues.GetValue<object>(propertyName) == null ? null : dbEntity.OriginalValues.GetValue<object>(propertyName).ToString(),
                            NewValue = dbEntity.CurrentValues.GetValue<object>(propertyName) == null ? null : dbEntity.CurrentValues.GetValue<object>(propertyName).ToString()
                        }
                            );
                    }
                }
            }


            // Otherwise, don't do anything, we don't care about Unchanged or Detached entities
            return changesCollection;
        }
    }
    */
}
