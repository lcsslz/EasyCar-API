namespace EasyCar_API
{
    public class Personal
    {
        public long id { get; }
        public string vorname { get; }
        public string nachname { get; }
        public string plz { get; }
        public string strasse { get; }
        public Rolle rolle { get; }
        public DateTime geburt { get; }

        public Personal(long id, string vorname, string nachname, string plz, string strasse, Rolle rolle, DateTime geburt)
        {
            this.id = id;
            this.vorname = vorname;
            this.nachname = nachname;
            this.plz = plz;
            this.strasse = strasse;
            this.rolle = rolle;
            this.geburt = geburt;
        }
    }
}
