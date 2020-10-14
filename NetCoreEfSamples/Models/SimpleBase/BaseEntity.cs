namespace NetCoreEfSamples.Models.SimpleBase
{
    public class BaseEntity
    {
        public int Id { get; set; }

        public string Name { get; set; }


        public virtual NavigPropertyBase navigPropertyBase { get; set; }


    }
}
