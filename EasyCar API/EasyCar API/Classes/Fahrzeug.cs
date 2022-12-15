namespace EasyCar_API
{
    public class Fahrzeug
    {
        public long id { get; }
        public Marke marke { get; }
        public Modell modell { get; }
        public Zustand zustand { get; }
        public int km_stand { get; }
        public string nummernschild { get; }
        public DateTime tuev_bis { get; }

        public Fahrzeug(long id, Marke marke, Modell modell, Zustand zustand, int km_stand, string nummernschild, DateTime tuev_bis)
        {
            this.id = id;
            this.marke = marke;
            this.modell = modell;
            this.zustand = zustand;
            this.km_stand = km_stand;
            this.nummernschild = nummernschild;
            this.tuev_bis = tuev_bis;
        }
    }
}
