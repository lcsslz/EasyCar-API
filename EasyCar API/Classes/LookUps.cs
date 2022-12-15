namespace EasyCar_API
{
    public class Rolle : LookUp
    {
        public Rolle(long id, string bez) : base(id, bez)
        {
        }
    }

    public class Marke : LookUp
    {
        public Marke(long id, string bez) : base(id, bez)
        {
        }
    }

    public class Typ : LookUp
    {
        public Typ(long id, string bez) : base(id, bez)
        {
        }
    }

    public class Zustand : LookUp
    {
        public Zustand(long id, string bez) : base(id, bez)
        {
        }
    }

    public class Modell : LookUp
    {
        public long jahr { get; }
        public Typ typ { get; }

        public Modell(long id, string bez, int jahr, Typ typ) : base(id, bez)
        {
            this.jahr = jahr;
            this.typ = typ;
        }
    }
}
