namespace NetCodeExample.Examples.EfSoftDelete
{
    class SampleEntity : IDeletable
    {
        public bool IsDeleted { get; set; }
    }
}
