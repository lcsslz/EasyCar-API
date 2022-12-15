---TYP----------------------------
INSERT INTO 
	typ (bez) 
VALUES 	
	('Supersport'), 
	('Muscle Car');
	
---MODELL-------------------------
INSERT INTO
	modell (typ_id, bez, jahr)
VALUES
	(1, 'Aventador SVJ', 2019),
	(1, 'Chrion', 2016),
	(1, 'Panigale V4R', 2020),
	(2, 'Mustang GT500', 2015),
	(1, 'F40', 1988);
	
---MARKE--------------------------
INSERT INTO
	marke (bez)
VALUES
	('Bugatti'),
	('Lamborghini'),
	('Ducati'),
	('Ford'),
	('Ferrari');
	
---ZUSTAND------------------------
INSERT INTO
	zustand (bez)
VALUES
	('fahrtüchig'),
	('beschädigt'),
	('fahrunfähig');
	
---ROLLE--------------------------
INSERT INTO
	rolle (bez)
VALUES
	('Admin'),
	('Mitarbeiter');
	
---FAHRZEUG-----------------------
INSERT INTO
	fahrzeug (marke_id, modell_id, zustand_id, km_stand, nummernschild, tuev_bis)
VALUES
	(1, 2, 1, 5679, 'EC-11', '2023-07-31'),
	(2, 1, 1, 12000, 'EC-12', '2024-09-30'),
	(3, 3, 2, 3402, 'EC-13', '2024-06-30'),
	(4, 4, 1, 7454, 'EC-14', '2024-04-30'),
	(3, 3, 1, 6432, 'EC-15', '2024-03-31');
	
---PERSONAL-----------------------
INSERT INTO
	personal (nutzername, passwort, rolle_id, vorname, nachname, plz, strasse, geburt)
VALUES
	('lschilz', 'Start123' , 1, 'Lucas', 'Schilz', '66740', 'Stollenbergweg 10', '2000-07-30'),
	('bmars', 'Start123', 1, 'Brahim', 'Mars', '66333', 'Mainzer Strasse 33', '1990-08-28'),
	('fstroethoff', 'Start123', 2, 'Florian', 'Stroethoff', '66780', 'Zur Unk 5', '2000-01-23');
	
---FAHRT--------------------------
INSERT INTO
	fahrt (personal_id, fahrzeug_id, km_start, km_ende, datum_start, datum_ende)
VALUES
	(1, 1, 5670, 5679, '2022-12-10', '2022-12-11'),
	(2, 3, 3402, Null, '2022-12-12', Null);