namespace EasyCar_API
{
    public class Fahrt
    {
        public long id { get; }
        public Personal personal { get; }
        public Fahrzeug fahrzeug { get; }
        public int km_start { get; }
        public int? km_ende { get; }
        public DateTime datum_start { get; }
        public DateTime? datum_ende { get; }

        public Fahrt(long id, Personal personal, Fahrzeug fahrzeug, int km_start, int? km_ende, DateTime datum_start, DateTime? datum_ende)
        {
            this.id = id;
            this.personal = personal;
            this.fahrzeug = fahrzeug;
            this.km_start = km_start;
            this.km_ende = km_ende;
            this.datum_start = datum_start;
            this.datum_ende = datum_ende;
        }
    }
}
