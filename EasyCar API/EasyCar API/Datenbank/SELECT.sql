----------PrüfeAnmeldung---------------------------------------------------------------------------------------------
SELECT 
	personal.id AS personal_id, 
	vorname, 
	nachname, 
	plz, 
	strasse, 
	geburt, 
	rolle.id AS rolle_id, 
	bez AS rolle_bez
FROM 
	personal 
	LEFT JOIN rolle ON rolle.id = personal.rolle_id
WHERE 
	nutzername = @nutzername 
	AND passwort = @passwort;


----------GetFahrzeuge-----------------------------------------------------------------------------------------------
SELECT 
    fahrzeug.id AS fahrzeug_id, 
    km_stand, 
    nummernschild, 
    tuev_bis, 
    marke.id AS marke_id, 
    marke.bez AS marke_bez, 
    modell.id AS modell_id, 
    modell.bez AS modell_bez, 
    modell.jahr AS modell_jahr, 
    typ.id AS typ_id, 
    typ.bez AS typ_bez, 
    zustand.id AS zustand_id, 
    zustand.bez AS zustand_bez 
FROM 
    fahrzeug 
    LEFT JOIN marke ON fahrzeug.marke_id = marke.id
    LEFT JOIN modell ON fahrzeug.modell_id = modell.id 
    LEFT JOIN typ ON modell.typ_id = typ.id
    LEFT JOIN zustand ON fahrzeug.zustand_id = zustand.id 
WHERE 
    (@aktuell_gebucht = -1 OR 
        (@aktuell_gebucht = 0 AND fahrzeug.id NOT IN (
                                                        SELECT 
                                                            DISTINCT fahrzeug_id 
                                                        FROM 
                                                            fahrt 
                                                        WHERE 
                                                            (GETDATE() BETWEEN datum_start AND datum_ende 
                                                                OR datum_start < GETDATE() AND datum_ende IS NULL)
                                                     ) 
        OR @aktuell_gebucht = 1 AND fahrzeug.id IN (
                                                    SELECT 
                                                        DISTINCT fahrzeug_id 
                                                    FROM 
                                                        fahrt 
                                                    WHERE 
                                                        (GETDATE() BETWEEN datum_start AND datum_ende 
                                                            OR datum_start < GETDATE() AND datum_ende IS NULL)
                                                   )
        )
    ) 
    AND (@personal_id = -1 OR fahrzeug.id IN (
                                                SELECT 
                                                    DISTINCT fahrzeug_id 
                                                FROM 
                                                    fahrt 
                                                WHERE 
                                                    personal_id = @personal_id
                                              )
        )
    AND (@marke_id = -1 OR marke.id = @marke_id)
    AND (@modell_id = -1 OR modell.id = @modell_id) 
    AND (@typ_id = -1 OR typ.id = @typ_id) 
    AND (@zustand_id = -1 OR zustand.id = @zustand_id);


----------GetFahrten-------------------------------------------------------------------------------------------------
SELECT 
    fahrt.id AS fahrt_id, 
    km_start, 
    km_ende, 
    datum_start, 
    datum_ende, 
    personal.id AS personal_id, 
    vorname, 
    nachname, 
    plz, 
    strasse, 
    geburt, 
    rolle.id AS rolle_id, 
    rolle.bez AS rolle_bez, 
    fahrzeug.id AS fahrzeug_id, 
    km_stand, 
    nummernschild, 
    tuev_bis, 
    marke.id AS marke_id, 
    marke.bez AS marke_bez, 
    modell.id AS modell_id, 
    modell.bez AS modell_bez, 
    jahr,
    typ.id AS typ_id, 
    typ.bez AS typ_bez, 
    zustand.id AS zustand_id, 
    zustand.bez AS zustand_bez 
FROM 
    fahrt 
    LEFT JOIN personal ON fahrt.personal_id = personal.id 
    LEFT JOIN rolle ON personal.rolle_id = rolle.id 
    LEFT JOIN fahrzeug ON fahrt.fahrzeug_id = fahrzeug.id 
    LEFT JOIN marke ON fahrzeug.marke_id = marke.id 
    LEFT JOIN modell ON fahrzeug.modell_id = modell.id 
    LEFT JOIN typ ON modell.typ_id = typ.id 
    LEFT JOIN zustand ON fahrzeug.zustand_id = zustand.id
WHERE 
    (@aktuell_gebucht = -1
        OR (@aktuell_gebucht = 0 
                AND NOT (GETDATE() BETWEEN datum_start AND datum_ende 
                         OR datum_start < GETDATE() AND datum_ende IS NULL
                        )
            OR @aktuell_gebucht = 1 
                AND (GETDATE() BETWEEN datum_start AND datum_ende 
                     OR datum_start < GETDATE() AND datum_ende IS NULL
                    )
            )
    )
    AND (@personal_id = -1 OR personal_id = @personal_id)
    AND (@fahrzeug_id = -1 OR fahrzeug_id = @fahrzeug_id);


----------GetMarken--------------------------------------------------------------------------------------------------
SELECT 
    id, 
    bez 
FROM 
    marke;


----------GetModelle-------------------------------------------------------------------------------------------------
SELECT 
    modell.id AS modell_id, 
    modell.bez AS marke_bez, 
    jahr AS modell_jahr, 
    typ.id AS typ_id, 
    typ.bez AS typ_bez 
FROM 
    marke 
    LEFT JOIN typ ON marke.typ_id = typ.id;


----------GetTypen---------------------------------------------------------------------------------------------------
SELECT 
    id, 
    bez 
FROM 
    typ;


----------GetZustände------------------------------------------------------------------------------------------------
SELECT 
    id, 
    bez 
FROM 
    zustand;


----------FahrzeugBuchen---------------------------------------------------------------------------------------------
SELECT 
    id 
FROM 
    personal 
WHERE 
    id = @id;
----------------------
SELECT 
    id 
FROM 
    fahrzeug 
WHERE 
    id = @id;
----------------------
INSERT INTO 
    fahrt (
            personal_id, 
            fahrzeug_id, 
            km_start, 
            datum_start
          )
OUTPUT 
    INSERTED.id 
VALUES (
            @personal_id, 
            @fahrzeug_id, 
            (
                SELECT 
                   km_stand 
                FROM 
                   fahrzeug 
                WHERE 
                   fahrzeug_id = @fahrzeug_id
             ), 
            @datum_start
       );


----------AddPersonal------------------------------------------------------------------------------------------------
SELECT 
    id 
FROM 
    personal 
WHERE 
    nutzername = @nutzername;
----------------------
INSERT INTO 
    personal (
                rolle_id, 
                vorname, 
                nachname, 
                plz, 
                strasse, 
                geburt
             ) 
OUTPUT 
    INSERTED.id 
VALUES (
            @rolle_id, 
            @vorname, 
            @nachname, 
            @plz, 
            @strasse, 
            @geburt
       );


----------AddFahrzeug------------------------------------------------------------------------------------------------
INSERT INTO 
    fahrzeug (
                marke_id, 
                modell_id, 
                zustand_id, 
                km_stand, 
                nummernschild, 
                tuev_bis
             )
OUTPUT 
    INSERTED.id 
VALUES (
            @marke_id, 
            @modell_id, 
            @zustand_id, 
            @km_stand, 
            @nummernschild, 
            @tuev_bis
       );


----------FahrzeugAusbuchen------------------------------------------------------------------------------------------
UPDATE 
    fahrzeug 
SET 
    km_stand = @km_stand,
	zustand_id = @zustand_id
WHERE 
    id = @fahrzeug_id; 
----------------------
UPDATE 
    fahrt 
SET 
    km_ende = @km_ende, 
    datum_ende = @datum_ende 
OUTPUT 
    INSERTED.id
WHERE 
    id = @fahrt_id;


----------DeletePersonal---------------------------------------------------------------------------------------------
DELETE FROM 
    personal 
WHERE 
    id = @personal_id;
----------------------
SELECT @@ROWCOUNT;


----------DeleteFahrzeug---------------------------------------------------------------------------------------------
DELETE FROM 
    fahrzeug 
WHERE 
    id = @fahrzeug_id;
----------------------
SELECT @@ROWCOUNT;