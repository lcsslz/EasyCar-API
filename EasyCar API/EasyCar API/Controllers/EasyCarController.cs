using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace EasyCar_API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EasyCarController : ControllerBase
    {
        private readonly ILogger<EasyCarController> _logger;

        public EasyCarController(ILogger<EasyCarController> logger)
        {
            _logger = logger;
        }

        [HttpGet("PrüfeAnmeldung")]
        public Response PruefeAnmeldung(string nutzername, string passwort)
        {
            string qry;
            Response response;
            SqlCommand command;
            DataTable dt;

            qry = "SELECT personal.id AS personal_id, vorname, nachname, plz, strasse, geburt, rolle.id AS rolle_id, bez AS rolle_bez " +
                  "FROM personal " +
                  "LEFT JOIN rolle ON rolle.id = personal.rolle_id " +
                  "WHERE nutzername = @nutzername AND passwort = @passwort";
            command = new SqlCommand(qry);
            command.Parameters.AddWithValue("@nutzername", nutzername);
            command.Parameters.AddWithValue("@passwort", passwort);

            dt = SQL.GetData(command);

            if (dt.Rows.Count == 0)
            {
                response = new Response("Keinen Mitarbeiter mit diesen Anmeldedaten gefunden.");
            }
            else if (dt.Rows.Count > 1)
            {
                response = new Response("Mehr als einen Mitarbeiter mit diesen Anmeldedaten gefunden.");
            }
            else
            {
                long personal_id = dt.Rows[0].Field<long>("personal_id");
                string vorname = dt.Rows[0].Field<string>("vorname");
                string nachname = dt.Rows[0].Field<string>("nachname");
                string plz = dt.Rows[0].Field<string>("plz");
                string strasse = dt.Rows[0].Field<string>("strasse");
                DateTime geburt = dt.Rows[0].Field<DateTime>("geburt");

                long rolle_id = dt.Rows[0].Field<long>("rolle_id");
                string rolle_bez = dt.Rows[0].Field<string>("rolle_bez");

                Rolle rolle = new Rolle(rolle_id, rolle_bez);
                Personal personal = new Personal(personal_id, vorname, nachname, plz, strasse, rolle, geburt);

                response = new Response(personal);
            }
            return response;
        }

        [HttpGet("GetFahrzeuge")]
        public Response GetFahrzeuge(int aktuell_gebucht, long personal_id, long marke_id, long modell_id, long typ_id, long zustand_id)
        {
            string qry;
            Response response;
            SqlCommand command;
            DataTable dt;

            qry = "SELECT fahrzeug.id AS fahrzeug_id, km_stand, nummernschild, tuev_bis, marke.id AS marke_id, marke.bez AS marke_bez, modell.id AS modell_id, modell.bez AS modell_bez, modell.jahr AS modell_jahr, " +
                         "typ.id AS typ_id, typ.bez AS typ_bez, zustand.id AS zustand_id, zustand.bez AS zustand_bez " +
                  "FROM fahrzeug " +
                    "LEFT JOIN marke ON fahrzeug.marke_id = marke.id " +
                    "LEFT JOIN modell ON fahrzeug.modell_id = modell.id " +
                    "LEFT JOIN typ ON modell.typ_id = typ.id " +
                    "LEFT JOIN zustand ON fahrzeug.zustand_id = zustand.id " +
                  "WHERE " +
                    "(@aktuell_gebucht = -1 OR " +
                        "(@aktuell_gebucht = 0 AND fahrzeug.id NOT IN (SELECT DISTINCT fahrzeug_id FROM fahrt WHERE (GETDATE() BETWEEN datum_start AND datum_ende OR datum_start < GETDATE() AND datum_ende IS NULL)) " +
                        "OR @aktuell_gebucht = 1 AND fahrzeug.id IN (SELECT DISTINCT fahrzeug_id FROM fahrt WHERE (GETDATE() BETWEEN datum_start AND datum_ende OR datum_start < GETDATE() AND datum_ende IS NULL)))) " +
                    "AND (@personal_id = -1 OR fahrzeug.id IN (SELECT DISTINCT fahrzeug_id FROM fahrt WHERE personal_id = @personal_id)) " +
                    "AND (@marke_id = -1 OR marke.id = @marke_id) " +
                    "AND (@modell_id = -1 OR modell.id = @modell_id) " +
                    "AND (@typ_id = -1 OR typ.id = @typ_id) " +
                    "AND (@zustand_id = -1 OR zustand.id = @zustand_id)";

            command = new SqlCommand(qry);
            command.Parameters.AddWithValue("@aktuell_gebucht", aktuell_gebucht);
            command.Parameters.AddWithValue("@personal_id", personal_id);
            command.Parameters.AddWithValue("@marke_id", marke_id);
            command.Parameters.AddWithValue("@modell_id", modell_id);
            command.Parameters.AddWithValue("@typ_id", typ_id);
            command.Parameters.AddWithValue("@zustand_id", zustand_id);

            dt = SQL.GetData(command);

            if (aktuell_gebucht > 1 || aktuell_gebucht < -1)
            {
                response = new Response("aktuell_gebucht muss -1 (nicht beachten), 0 (aktuell frei) oder 1 (aktuell gebucht) sein.");
            }
            else if (dt.Rows.Count == 0)
            {
                response = new Response("Keine Fahrzeuge gefunden.");
            }
            else
            {
                List<Fahrzeug> fahrzeuge = new List<Fahrzeug>();
                foreach (DataRow row in dt.Rows)
                {
                    long fahrzeug_id = row.Field<long>("fahrzeug_id");
                    int km_stand = row.Field<int>("km_stand");
                    string nummernschild = row.Field<string>("nummernschild");
                    DateTime tuev_bis = row.Field<DateTime>("tuev_bis");

                    marke_id = dt.Rows[0].Field<long>("marke_id");
                    string marke_bez = row.Field<string>("marke_bez");

                    modell_id = row.Field<long>("modell_id");
                    string modell_bez = row.Field<string>("modell_bez");
                    int modell_jahr = row.Field<int>("modell_jahr");

                    typ_id = row.Field<long>("typ_id");
                    string typ_bez = row.Field<string>("typ_bez");

                    zustand_id = row.Field<long>("zustand_id");
                    string zustand_bez = row.Field<string>("zustand_bez");

                    Typ typ = new Typ(typ_id, typ_bez);
                    Modell modell = new Modell(modell_id, modell_bez, modell_jahr, typ);
                    Marke marke = new Marke(marke_id, marke_bez);
                    Zustand zustand = new Zustand(zustand_id, zustand_bez);

                    Fahrzeug fahrzeug = new Fahrzeug(fahrzeug_id, marke, modell, zustand, km_stand, nummernschild, tuev_bis);
                    fahrzeuge.Add(fahrzeug);
                }
                response = new Response(fahrzeuge);
            }
            return response;
        }

        [HttpGet("GetFahrten")]
        public Response GetFahrten(int aktuell_gebucht, long personal_id, long fahrzeug_id)
        {
            string qry;
            Response response;
            SqlCommand command;
            DataTable dt;

            qry = "SELECT fahrt.id AS fahrt_id, km_start, km_ende, datum_start, datum_ende, personal.id AS personal_id, vorname, nachname, plz, strasse, geburt, rolle.id AS rolle_id, rolle.bez AS rolle_bez, " +
                    "fahrzeug.id AS fahrzeug_id, km_stand, nummernschild, tuev_bis, marke.id AS marke_id, marke.bez AS marke_bez, modell.id AS modell_id, modell.bez AS modell_bez, jahr, " +
                    "typ.id AS typ_id, typ.bez AS typ_bez, zustand.id AS zustand_id, zustand.bez AS zustand_bez " +
                  "FROM fahrt " +
                    "LEFT JOIN personal ON fahrt.personal_id = personal.id " +
                    "LEFT JOIN rolle ON personal.rolle_id = rolle.id " +
                    "LEFT JOIN fahrzeug ON fahrt.fahrzeug_id = fahrzeug.id " +
                    "LEFT JOIN marke ON fahrzeug.marke_id = marke.id " +
                    "LEFT JOIN modell ON fahrzeug.modell_id = modell.id " +
                    "LEFT JOIN typ ON modell.typ_id = typ.id " +
                    "LEFT JOIN zustand ON fahrzeug.zustand_id = zustand.id " +
                  "WHERE " +
                  "(@aktuell_gebucht = -1 OR " +
                    "(@aktuell_gebucht = 0 AND NOT (GETDATE() BETWEEN datum_start AND datum_ende OR datum_start < GETDATE() AND datum_ende IS NULL) " +
                    "OR @aktuell_gebucht = 1 AND (GETDATE() BETWEEN datum_start AND datum_ende OR datum_start < GETDATE() AND datum_ende IS NULL))) " +
                  "AND (@personal_id = -1 OR personal_id = @personal_id) " +
                  "AND (@fahrzeug_id = -1 OR fahrzeug_id = @fahrzeug_id)";

            command = new SqlCommand(qry);
            command.Parameters.AddWithValue("@aktuell_gebucht", aktuell_gebucht);
            command.Parameters.AddWithValue("@personal_id", personal_id);
            command.Parameters.AddWithValue("@fahrzeug_id", fahrzeug_id);

            dt = SQL.GetData(command);

            if (aktuell_gebucht > 1 || aktuell_gebucht < -1)
            {
                response = new Response("aktuell_gebucht muss -1 (nicht beachten), 0 (aktuell frei) oder 1 (aktuell gebucht) sein.");
            }
            else if (dt.Rows.Count == 0)
            {
                response = new Response("Keine Fahrten gefunden.");
            }
            else
            {
                List<Fahrt> fahrten = new List<Fahrt>();
                foreach (DataRow row in dt.Rows)
                {
                    long fahrt_id = row.Field<long>("fahrt_id");
                    int km_start = row.Field<int>("km_start");
                    int? km_ende = row.Field<int?>("km_ende");
                    DateTime datum_start = row.Field<DateTime>("datum_start");
                    DateTime? datum_ende = row.Field<DateTime?>("datum_ende");

                    personal_id = row.Field<long>("personal_id");
                    string vorname = row.Field<string>("vorname");
                    string nachname = row.Field<string>("nachname");
                    string plz = row.Field<string>("plz");
                    string strasse = row.Field<string>("strasse");
                    DateTime geburt = row.Field<DateTime>("geburt");

                    long rolle_id = row.Field<long>("rolle_id");
                    string rolle_bez = row.Field<string>("rolle_bez");

                    fahrzeug_id = row.Field<long>("fahrzeug_id");
                    int km_stand = row.Field<int>("km_stand");
                    string nummernschild = row.Field<string>("nummernschild");
                    DateTime tuev_bis = row.Field<DateTime>("tuev_bis");

                    long marke_id = row.Field<long>("marke_id");
                    string marke_bez = row.Field<string>("marke_bez");

                    long modell_id = row.Field<long>("modell_id");
                    string modell_bez = row.Field<string>("modell_bez");
                    int jahr = row.Field<int>("jahr");

                    long typ_id = row.Field<long>("typ_id");
                    string typ_bez = row.Field<string>("typ_bez");

                    long zustand_id = row.Field<long>("zustand_id");
                    string zustand_bez = row.Field<string>("zustand_bez");

                    Typ typ = new Typ(typ_id, typ_bez);
                    Modell modell = new Modell(modell_id, modell_bez, jahr, typ);
                    Marke marke = new Marke(marke_id, marke_bez);
                    Zustand zustand = new Zustand(zustand_id, zustand_bez);
                    Rolle rolle = new Rolle(rolle_id, rolle_bez);

                    Fahrzeug fahrzeug = new Fahrzeug(fahrzeug_id, marke, modell, zustand, km_stand, nummernschild, tuev_bis);
                    Personal personal = new Personal(personal_id, vorname, nachname, plz, strasse, rolle, geburt);

                    Fahrt fahrt = new Fahrt(fahrt_id, personal, fahrzeug, km_start, km_ende, datum_start, datum_ende);
                    fahrten.Add(fahrt);
                }
                response = new Response(fahrten);
            }
            return response;
        }

        [HttpGet("GetMarken")]
        public Response GetMarken()
        {
            Response response;
            SqlCommand command;
            DataTable dt;

            command = new SqlCommand("SELECT id, bez FROM marke");
            dt = SQL.GetData(command);

            if (dt.Rows.Count == 0)
            {
                response = new Response("Keine Marken gefunden.");
            }
            else
            {
                List<Marke> marken = new List<Marke>();
                foreach (DataRow row in dt.Rows) 
                {
                    long id = row.Field<long>("id");
                    string bez = row.Field<string>("bez");

                    Marke marke = new Marke(id, bez);
                    marken.Add(marke);
                }
                response = new Response(marken);
            }
            return response;
        }

        [HttpGet("GetModelle")]
        public Response GetModelle()
        {
            Response response;
            SqlCommand command;
            DataTable dt;

            command = new SqlCommand("SELECT modell.id AS modell_id, modell.bez AS marke_bez, jahr AS modell_jahr, typ.id AS typ_id, typ.bez AS typ_bez FROM marke LEFT JOIN typ ON marke.typ_id = typ.id");
            dt = SQL.GetData(command);

            if (dt.Rows.Count == 0)
            {
                response = new Response("Keine Modelle gefunden.");
            }
            else
            {
                List<Modell> modelle = new List<Modell>();
                foreach(DataRow row in dt.Rows)
                {
                    long modell_id = row.Field<long>("modell_id");
                    string modell_bez = row.Field<string>("modell_bez");
                    int modell_jahr = row.Field<int>("modell_jahr");

                    long typ_id = row.Field<long>("typ_id");
                    string typ_bez = row.Field<string>("typ_bez");

                    Typ typ = new Typ(typ_id, typ_bez);
                    Modell modell = new Modell(modell_id, modell_bez, modell_jahr, typ);
                    modelle.Add(modell);
                }
                response = new Response(modelle);
            }
            return response;
        }

        [HttpGet("GetTypen")]
        public Response GetTypen()
        {
            Response response;
            SqlCommand command;
            DataTable dt;

            command = new SqlCommand("SELECT id, bez FROM typ");
            dt = SQL.GetData(command);

            if (dt.Rows.Count == 0)
            {
                response = new Response("Keine Typen gefunden.");
            }
            else
            {
                List<Typ> typen = new List<Typ>();
                foreach (DataRow row in dt.Rows)
                {
                    long id = row.Field<long>("id");
                    string bez = row.Field<string>("bez");

                    Typ typ = new Typ(id, bez);
                    typen.Add(typ);
                }
                response = new Response(typen);
            }
            return response;
        }

        [HttpGet("GetZustände")]
        public Response GetZustände()
        {
            Response response;
            SqlCommand command;
            DataTable dt;

            command = new SqlCommand("SELECT id, bez FROM zustand");
            dt = SQL.GetData(command);

            if (dt.Rows.Count == 0)
            {
                response = new Response("Keine Zustände gefunden.");
            }
            else
            {
                List<Zustand> zustände = new List<Zustand>();
                foreach (DataRow row in dt.Rows)
                {
                    long id = row.Field<long>("id");
                    string bez = row.Field<string>("bez");

                    Zustand zustand = new Zustand(id, bez);
                    zustände.Add(zustand);
                }
                response = new Response(zustände);
            }
            return response;
        }

        [HttpPost("FahrzeugBuchen")]
        public Response FahrzeugBuchen(long personal_id, long fahrzeug_id, DateTime datum_start)
        {
            string qry;
            long inserted_id;
            Response response;
            SqlCommand command;
            DataTable dt;

            qry = "SELECT id FROM %TABLENAME% WHERE id = @id";

            command = new SqlCommand(qry.Replace("%TABLENAME%", "personal"));
            command.Parameters.AddWithValue("@id", personal_id);
            dt =SQL.GetData(command);

            if (dt.Rows.Count == 0)
            {
                response = new Response("PersonalID unbekannt.");
                return response;
            }

            command = new SqlCommand(qry.Replace("%TABLENAME%", "fahrzeug"));
            command.Parameters.AddWithValue("@id", fahrzeug_id);
            dt = SQL.GetData(command);

            if (dt.Rows.Count == 0)
            {
                response = new Response("FahrzeugID unbekannt.");
                return response;
            }

            qry = "INSERT INTO fahrt" +
                    "(personal_id, fahrzeug_id, km_start, datum_start) " +
                  "OUTPUT INSERTED.id " +
                  "VALUES" +
                    "(@personal_id, @fahrzeug_id, " +
                        "(SELECT km_stand FROM fahrzeug WHERE id = @fahrzeug_id), " +
                    "@datum_start)";

            command = new SqlCommand(qry);
            command.Parameters.AddWithValue("@personal_id", personal_id);
            command.Parameters.AddWithValue("@fahrzeug_id", fahrzeug_id);
            command.Parameters.AddWithValue("@datum_start", datum_start);

            inserted_id = (long) SQL.SetDataWithReturn(command);
            response = new Response(inserted_id);
            return response;
        }

        [HttpPost("AddPersonal")]
        public Response AddPersonal(Personal personal, string nutzername, string passwort)
        {
            string qry;
            long inserted_id;
            Response response;
            SqlCommand command;
            DataTable dt;

            qry = "SELECT id FROM personal WHERE nutzername = @nutzername";
            command = new SqlCommand(qry);
            command.Parameters.AddWithValue("@nutzername", nutzername);
            dt = SQL.GetData(command);

            if (dt.Rows.Count != 0)
            {
                response = new Response("Dieser Nutzername ist bereits vorhanden");
            }
            else
            {
                qry = "INSERT INTO personal " +
                        "(nutzername, passwort, rolle_id, vorname, nachname, plz, strasse, geburt) " +
                      "OUTPUT INSERTED.id " +
                      "VALUES " +
                        "(@nutzername, @passwort, @rolle_id, @vorname, @nachname, @plz, @strasse, @geburt)";

                command = new SqlCommand(qry);
                command.Parameters.AddWithValue("@nutzername", nutzername);
                command.Parameters.AddWithValue("@passwort", passwort);
                command.Parameters.AddWithValue("@rolle_id", personal.rolle.id);
                command.Parameters.AddWithValue("@vorname", personal.vorname);
                command.Parameters.AddWithValue("@nachname", personal.nachname);
                command.Parameters.AddWithValue("@plz", personal.plz);
                command.Parameters.AddWithValue("@strasse", personal.strasse);
                command.Parameters.AddWithValue("@geburt", personal.geburt);

                inserted_id = (long) SQL.SetDataWithReturn(command);
                Personal new_personal = new Personal(inserted_id, personal.vorname, personal.nachname, personal.plz, personal.strasse, personal.rolle, personal.geburt);

                response = new Response(new_personal);
            }
            return response;
        }

        [HttpPost("AddFahrzeug")]
        public Response AddFahrzeug(Fahrzeug fahrzeug)
        {
            string qry;
            long inserted_id;
            Response response;
            SqlCommand command;

            qry = "INSERT INTO fahrzeug " +
                    "(marke_id, modell_id, zustand_id, km_stand, nummernschild, tuev_bis) " +
                  "OUTPUT INSERTED.id " +
                  "VALUES " +
                    "(@marke_id, @modell_id, @zustand_id, @km_stand, @nummernschild, @tuev_bis)";

            command = new SqlCommand(qry);
            command.Parameters.AddWithValue("@marke_id", fahrzeug.marke.id);
            command.Parameters.AddWithValue("@modell_id", fahrzeug.modell.id);
            command.Parameters.AddWithValue("@zustand_id", fahrzeug.zustand.id);
            command.Parameters.AddWithValue("@km_stand", fahrzeug.km_stand);
            command.Parameters.AddWithValue("@nummernschild", fahrzeug.nummernschild);
            command.Parameters.AddWithValue("@tuev_bis", fahrzeug.tuev_bis);

            inserted_id = (long) SQL.SetDataWithReturn(command);
            Fahrzeug new_fahrzeug = new Fahrzeug(inserted_id, fahrzeug.marke, fahrzeug.modell, fahrzeug.zustand, fahrzeug.km_stand, fahrzeug.nummernschild, fahrzeug.tuev_bis);

            response = new Response(new_fahrzeug);
            return response;
        }

        [HttpPut("FahrzeugAusbuchen")]
        public Response FahrzeugAusbuchen(long fahrt_id, long fahrzeug_id, long zustand_id, int km_ende, DateTime datum_ende)
        {
            string qry;
            long? updated_id;
            Response response;
            SqlCommand command;
            DataTable dt;

            qry = "SELECT km_start, datum_start " +
                  "FROM fahrt " +
                  "WHERE id = @fahrt_id";

            command = new SqlCommand(qry);
            command.Parameters.AddWithValue("@fahrt_id", fahrt_id);
            dt = SQL.GetData(command);

            if (dt.Rows.Count == 0)
            {
                response = new Response("Keine Fahrt mit dieser ID gefunden.");
                return response;
            }
            else
            {
                int km_start = dt.Rows[0].Field<int>("km_start");
                DateTime datum_start = dt.Rows[0].Field<DateTime>("datum_start");

                if (km_ende < km_start)
                {
                    response = new Response("Die End-KM dürfen nicht geringer als die Start-KM sein.");
                    return response;
                }
                else if (datum_ende < datum_start)
                {
                    response = new Response("Das End-Datum darf nicht geringer als das Start-Datum sein.");
                    return response;
                }
            }

            qry = "UPDATE fahrzeug " +
                  "SET km_stand = @km_ende, zustand_id = @zustand_id " +
                  "WHERE id = @fahrzeug_id; " +
                  "" +
                  "UPDATE fahrt " +
                  "SET km_ende = @km_ende, datum_ende = @datum_ende " +
                  "OUTPUT INSERTED.id " +
                  "WHERE id = @fahrt_id";

            command = new SqlCommand(qry);
            command.Parameters.AddWithValue("@fahrzeug_id", fahrzeug_id);
            command.Parameters.AddWithValue("@fahrt_id", fahrt_id);
            command.Parameters.AddWithValue("@km_ende", km_ende);
            command.Parameters.AddWithValue("@zustand_id", zustand_id);
            command.Parameters.AddWithValue("@datum_ende", datum_ende);

            updated_id = (long?) SQL.SetDataWithReturn(command);
            response = new Response(updated_id);
            return response;
        }

        [HttpDelete("DeletePersonal")]
        public Response DeletePersonal(long personal_id)
        {
            string qry;
            int affected_rows_count;
            Response response;
            SqlCommand command;

            qry = "DELETE FROM personal WHERE id = @personal_id;" +
                  "SELECT @@ROWCOUNT";

            command = new SqlCommand(qry);
            command.Parameters.AddWithValue("@personal_id", personal_id);

            affected_rows_count = (int) SQL.SetDataWithReturn(command);
            
            if (affected_rows_count == 0)
            {
                response = new Response("Es wurde kein Personal mit dieser ID gefunden.");
            }
            else
            {
                response = new Response(affected_rows_count);
            }
            return response;
        }

        [HttpDelete("DeleteFahrzeug")]
        public Response DeleteFahrzeug(long fahrzeug_id)
        {
            string qry;
            int affected_rows_count;
            Response response;
            SqlCommand command;

            qry = "DELETE FROM fahrzeug WHERE id = @fahrzeug_id;" +
                  "SELECT @@ROWCOUNT";

            command = new SqlCommand(qry);
            command.Parameters.AddWithValue("@fahrzeug_id", fahrzeug_id);

            affected_rows_count = (int) SQL.SetDataWithReturn(command);

            if (affected_rows_count == 0)
            {
                response = new Response("Es wurde kein Fahrzeug mit dieser ID gefunden.");
            }
            else
            {
                response = new Response(affected_rows_count);
            }
            return response;
        }
    }
}