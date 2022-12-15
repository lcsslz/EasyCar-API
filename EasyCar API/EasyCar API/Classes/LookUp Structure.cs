namespace EasyCar_API
{
    public abstract class LookUp
    {
        public long id { get; }
        public string bez { get; }

        public LookUp(long id, string bez)
        {
            this.id = id;
            this.bez = bez;
        }
    }
}