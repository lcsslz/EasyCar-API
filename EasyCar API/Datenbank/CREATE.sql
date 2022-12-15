CREATE TABLE typ (
	id bigint IDENTITY,
	bez varchar(max) NOT NULL,
	CONSTRAINT pk_typ PRIMARY KEY (id)
);

CREATE TABLE marke (
	id bigint IDENTITY,
	bez varchar(max) NOT NULL,
	CONSTRAINT pk_marke PRIMARY KEY (id)
);

CREATE TABLE zustand (
	id bigint IDENTITY,
	bez varchar(max) NOT NULL,
	CONSTRAINT pk_zustand PRIMARY KEY (id)
);

CREATE TABLE rolle (
	id bigint IDENTITY,
	bez varchar(max) NOT NULL,
	CONSTRAINT pk_rolle PRIMARY KEY (id)
);

CREATE TABLE modell (
	id bigint IDENTITY,
	typ_id bigint,
	bez varchar(max) NOT NULL,
	jahr int NOT NULL,
	CONSTRAINT pk_modell PRIMARY KEY (id),
	CONSTRAINT fk_modell_typ FOREIGN KEY (typ_id) REFERENCES typ(id)
);

CREATE TABLE personal (
	id bigint IDENTITY,
	nutzername varchar(255) NOT NULL UNIQUE,
	passwort varchar(max) NOT NULL,
	rolle_id bigint,
	vorname varchar(max) NOT NULL, 
	nachname varchar(max) NOT NULL,
	plz varchar(5) NOT NULL,
	strasse varchar(max) NOT NULL,
	geburt date NOT NULL,
	CONSTRAINT pk_personal PRIMARY KEY (id),
	CONSTRAINT fk_personal_rolle FOREIGN KEY (rolle_id) REFERENCES rolle(id)
);

CREATE TABLE fahrzeug (
	id bigint IDENTITY,
	marke_id bigint,
	modell_id bigint,
	zustand_id bigint,
	km_stand int NOT NULL CHECK(km_stand > 0),
	nummernschild varchar(8) NOT NULL,
	tuev_bis date NOT NULL,
	CONSTRAINT pk_fahrzeug PRIMARY KEY (id),
	CONSTRAINT fk_fahrzeug_marke FOREIGN KEY (marke_id) REFERENCES marke(id),
	CONSTRAINT fk_fahrzeug_modell FOREIGN KEY (modell_id) REFERENCES modell(id),
	CONSTRAINT fk_fahrzeug_zustand FOREIGN KEY (zustand_id) REFERENCES zustand(id)
);

CREATE TABLE fahrt (
	id bigint IDENTITY,
	personal_id bigint,
	fahrzeug_id bigint, 
	km_start int NOT NULL,
	km_ende int,
	datum_start date NOT NULL DEFAULT GETDATE(),
	datum_ende date,
	CONSTRAINT pk_fahrt PRIMARY KEY (id),
	CONSTRAINT fk_fahrt_personal FOREIGN KEY (personal_id) REFERENCES personal(id),
	CONSTRAINT fk_fahrt_fahrzeug FOREIGN KEY (fahrzeug_id) REFERENCES fahrzeug(id),
);