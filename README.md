# Bedrift-tracker Arbeidsplan:

## Index:

- [Intro](#intro)
  - [Produkt](#produkt)
  - [Del mål](#del-mål)
  - [Endemål](#endemål)
- [Bruk av Github](#git-hub)
  - [Brancher](#brancher)
  - [Code Review](#code-review)
- [Kode Standarer](#kode-standarer)
  - [Error Håndtering](#error-håndtering)
- [Front End](#front-end)
  - [Formål](#formål-FRONTEND)
  - [Framework](#framework-FRONTEND)
  - [Delmål](#delmål-FRONTEND)
  - [Komponent Liste](#Komponent-liste)
  - [Håndtering av State](#håndtering-av-state)
  - [Routing](#routing-FRONTEND)
  - [Authorizering](#Authorizering)
- [API](#api)
  - [Formål](#formål-API)
  - [Framework](#framework-API)
  - [Routing](#Routing-API)
  - [Micro Services](#micro-services)
  - [Middleware](#middleware)
- [Database](#database)
  - [Formål](#Formål-DATABASE)
  - [Schema](#schema)
  - [Funksjoner](#funksjoner-DATABASE)

## Intro

<br/>

### Produkt

Fullstack app, som skal kombinere data fra PROFF.NO, og samkjøre dette med data fra VIS. <br/>
mth. Oppfølging av bedrifter som har vært gjennom VIS inkubasjon og/eller andre programmer.
Hva skal produktet gjøre:

1. Kobles opp mot Intern Microsoft Authentication Layer for å kunne ta i bruk eksisterende brukere.

2. Ved suksessfull login, skal brukeren presenteres med en graf som viser Gjennomsnittet av omsetting for alle bedrifter i databasen.

3. Header skal presentere logo + navn og to knappe for funksjonalitet.
   - Generer årsrapport.
     - Denne skal hjelpe med å automatisk fylle ut en økonomisk årsrapport for hver bedrift bruker ønsker.
     - Skal ta inn enten en string av organisasjonsnummer, evt et excel ark fult av organisjasjonsnr. Så levere ut et excel ark bruker kan laste ned.
   - Legg til eller oppdater bedrift i tracker.
     - Her skal bruker kunne legge til nye bedrifter til trackersystemet. Det skal kunne ta i mot et excel ark med bedrifter. Excel arket bør inneholde følgende data:
     ```json
     {
       "målbedrift": "bedriftnavn",
       "orgnummer": "bedriftens organisasjonsnavn",
       "fase": "hvilken fase bedriften er i VIS systemet",
       "år": "Hvilket år dette gjelder"
     }
     ```
     - Dette skal og kunne ta i mot oppdatering av eksisterende bedrifter. Dette burde kunne legges med i samme fil, og så filtrerer backenden om dette er en ny, eller en ooppdatering av eksisterende bedrift.
4. En logg-ut knapp under header.

5. På hoved siden, under graf, bør bruker ha mulighet å filtrere data, og evt exportere datasettet til excel.

6. Siden skal også presentere en oversiktlig tabell, som viser bedrift, gjeldene år, samt hva data som blir vist. <br> Bruker skal kunne trykke på en bedrift i tabellene, og grafen skal oppdateres til å vise gjeldene data for den bedriften.

7. Vi bruker følgende "stack":

- Front-end: React + Vite
- Back-End: .NET 8.0
- Database: Postgresql
  <br/>

### Del Mål

1. Lage et wireframe oppsett på hvordan siden skal se ut, og hvordan brukeropplevelsen skal være. <a href="https://excalidraw.com/#json=t7vlG0xVLfCtKv7Kl61zd,2p75ewZ4-Q4NYjYgLlf8kA"> Foreløbig ide</a>
2. Designe oppsett og komponenter i FIGMA, Tenke på bruk og formål. Dette er mer et enterprise verktøy enn en butikk. (Legg til figma link her.)
3. Få tilbakemelding fra sluttbruker når det kommer til brukervennlighet og design.
4. Bestemme bruk av Frontend framework, hva som egner seg best, og hva som er lettest å self-hoste på azure. Next? Ren react? JS + HTML? <br> Vi har som mål å følge bergen stacken tett, og velger NextJS som frontend og C# som backend.
5. Bestemme oss for database oppsett. Vi har som mål å bruke POSTGRESQL for å lagre brukerdata samt data fra proff og vis.
6. Når det kommer til brukertyper, mener vi mht formål at alle brukere har tilgang til samme funksjonalitet.
7. Nå et punkt hvor C# backend kan snakke med databasen på samme måte som nodejs prototype backend.
8. Bestemme for endepunkter til backend. Pluss hvordan det er best å sette opp CRON jobber.

   <br/>

### Endemål

Vi ønsker å lage et enkelt verktøy for å samle, presentere, og hjelpe med organisering av data for bedrifter som har vært med i VIS sine systemer. <br>
Et verktøy som kan gjøre det lettere å hente, manipulere og bruke data fra bl.a. PROFF for bl.a. generer årsrapporter. <br>

<br/>

## Bruk av GitHub

Vi jobber primært på VIS INNOVASJON sin github, i BT repo.
<br/>

### Brancher

Vi utvikler og pusher endringer til DEV, og holder av push til MAIN primært når vi har nådd milepæler.
Hvis det blir laget nye feature-set eller test brancher kan disse navngis etter formål i.e. EXCEL_IMPORT_TESTING.
<br/>

### Code Review

Vi bruker primært pull-requests for å oppdatere koden i dev. Det gjør at vi har to øyne på koden som går opp til en hver tid.
<br/>

## Kode Standarer

Vi forholder oss til <a href="https://nextjs.org/docs">NextJS docs</a> for best practise. <br>
Vi bruker primært en jsdoc blokk kommentar for å kommentere funksjoner: <br>

```javascript
/**
 * Funksjon som consol.logger "hello world!"
 */
const helloWorld = () => {
  console.log("Hello World!");
};
```

<br>

Vi bruker korte blokk kommentarer for å forklare enkelt funksjonen til en komponent, hvis det trengs:

```javascript
/**
 * underoverskrift til prosjekt.
 *
 */
const MyComponent = () => {
  return <h2>Hello!</h2>;
};
```

<br>

### Error Håndtering

Errorhåndtering bør håndteres så tidlig som mulig.

Eksempel:

```javascript
/**
 * wrapper function over nodefetch
 * @params{string, requestInit}
 */
const fetchData = async (url: string, options: requestInit) => {
  try {
    const response = await fetch(url, options);
    const result = await response.json();
    return { error: null, result: result };
  } catch (error) {
    saveToLog(error);
    return { error: error, result: null };
  }
};
```

Da har vi en oversiktlig måte å passe states gjort av funksjoner som kan throwe errors videre. slik at både server kan evt. korrektere, eller bruker kan håndtere error.

Brukerinputs bør håndteres med en enkel boolean, slik at vi kan bruke conditional rendering for å vise front-end error til brukeren:

```javascript
/**
 *@params{a: number, b:number}
 *@returns boolean
 */
const userInputWithinRange = (a: number, b: number) => {
  return a < b && b >= 100 && a >= 0;
};
```

Det gjør at vi ender opp med enkle boolean checks som kan hjelpe med conditional rendering:

```javascript
const MyComponent = () => {
  return (
    <>
      {userInputWithinRange(10, 20) ? (
        <h1>You're good to go!</h1>
      ) : (
        <h1>Your numbers are outside permited range</h1>
      )}
    </>
  );
};
```

Dette gjør også at vi kan fange potensielle input feil i frontend før det havner i backend.

<br/>

## Front End

Vi har som mål å følge Bergenstacken så tett som mulig. Den mest populære frameworken i bergenstacken er NextJS.

<br/>

<h3 id="formål-FRONTEND">Formål</h3>
Målet er å skape en stil-ren, lettleselig og enkel frontend for å gjennomføre de arbeidsoppgaver som sluttbruker krever.<br>
Målet er å ha en interaktiv graf hvor bruker lett kan sammenligne flere datapunkter i en linjegraf.<br>
DVS. Sammenligne bedrifter basert på hvisse kriterier.<br>
Kriterier som:
  - Alder på daglig leder.
  - Kjønn daglig leder.
  - Faser i interne inkubatorprogrammer.
  - Bransje.
<br>
Datapunktene bruker skal kunne velge mellom er f.eks:
  - Innskutt egenkapital.
  - Driftsresultat.
<br>

<br/>

<h3 id="delmål-FRONTENT">Delmål</h3>

Delmål for å oppnå frontend:

1. Ta i bruk testData json fil, og lag en funksjon som kan vise en og en bedrift i en grafekomponent.

2. Lage en tabell som kan presentere top fem bedrifter i testData.json for gjeldende økonomisk data.

3. Lage en felles Header for alle endepunkt. Headeren skal inneholde en logo, pluss to knapper: Oppdater database, Generer Årsrapport.

4. Lage en filtermeny, som dropdown. som kan filtrere gjennom økonomisk data set i testdata.json.

5. Gjøre tabellen interaktiv, trykke på en bedrift i tabell vises i graf.

6. Koble opp mot API, passe på at graf og tabell fremdeles er interaktiv og kan jobbe med fetched data fra api.

7. Årsrapport Endpoint.
   - Drag and Drop endpoint som kan ta imot excel filer.
   - Encrypte og sende excelfiler til backend i en POST til /Årsrapport.
   - Initialisere en nedlasting av returnert EXCEL fil.
8. Oppdatere database endpoint.
   - Drag and drop endpoint for å ta imot excel filer.
   - Endcrypte og sende filer til backen i en POST til /UpdateDb
   - Presentere bruker med svar om lagring er ok.
9. Integrere med MAL.
   - Integrere med MAL for Azure.
   - endre endepunkter til å inkludere /:uid/ før endepunkt.

<br/>

<h3 id="framework-FRONTEND">Framework</h3>

Vi har som mål å basere frontend på NextJS.

<br/>

### Komponent Liste

Front-end skal ha følgende funksjonalitet:
| Endpoint | Funksjonalitet |
| :---------- | :--------------------------------------------------------------------------------------------------------------------------------------------------- |
| felles alle | <ul>Header:<li>Logo</li><li>Navn</li><li>LINK: Oppdater database</li><li>LINK: Generer årsrapport</li></ul> |
| /login | <ul>Log in with Microsoft Account: <li>Email(or autocomplete with MSAL components)</li><li>Password(not needed when using MSAL components)</li></ul> |
| /Query | <ul>graf:<li>x-akse i år</li><li>y-akse bestemt av dataset.</li></ul><ul>funksjonalitetsknapper: <li>Filter, for å filtre i gjeldende dataset.</li><li>Søk, for å gjøre nye søk</li></ul><ul>tabell: <li>Gjeldende Bedrift</li><li>Gjeldene år</li><li>Data</li><li>Exporter dataset</li></ul> |
| /UpdateDb | <ul>Import Excel: <li>drag and drop file-upload</li><li>Upload button</li><li>user feedback/error</li></ul> |
| /Årsrapport | <ul>Import Excel: <li>drag and drop file-upload</li><li>Upload button</li><li>user feedback/error</li></ul><ul>Handling av generert excel fil: <li>Forhåndsvisning og/eller automatisk nedlastning</li></ul> |

For å gjennomføre dette trenger vi følgende komponenter:

| Komponent navn | Komponent form    | Komponent Beskrivelse                                      | Children:                       | Finnes i bibliotek                     |
| :------------- | :---------------- | :--------------------------------------------------------- | :------------------------------ | :------------------------------------- |
| Header         | `<Header/>`       | Header hoved component                                     | `<Logo/>` `<Link/>`             | Nei                                    |
| Link           | `<Link/>`         | Håndterer routing internt i siden                          | none                            | Ja, react base component               |
| Input: radial  | `<Radial/>`       | Radial knapp for veksling mellom funksjonalitet            | none                            | Kanskje                                |
| Input: Button  | `<Button/>`       | Knapp for generell funksjonalitet.                         | none                            | Kanskje                                |
| Graf           | `<BarChart/> `    | Graf for å visualisere dataset.                            | General Graph Components        | Ja, hentet fra mui: mui-x-bar-chart    |
| Tabell         | `<Table/>`        | Tabell for å vise deler av datasett på en strukturert måte | Bygget opp av `<tr>` components | Ja, usikker hvilken som skal bli brukt |
| AutoComplete   | `<AutoComplete/>` | Komponent som slår sammen dropdown med et søkbart felt.    | Liste eller array av options.   | Ja, hentet fra mui                     |

|

<br/>

### Håndtering av state

<br/>

<h3 id="routing-FRONTEND">Routing</h3>

<br/>

### Authorizering

Authorizering skal samkjøres via MAL og MALJS, Dette må gjøres når siden skal hostes på Azure.

<br/>

## API

<br/>

<h3 id="formål-API">Formål</h3>

Skape en rask og lett api med to formål:

1. være et mellomledd mellom frontend og database.

2. Styre oppdateringsprosess av databasen både via bruker inputs og via CRON jobs.
   <br/>

<h3 id="framework-API">Framework</h3>
Vi har som mål å skrive backenden i C#. <br>
Dette for å ha en stack som følger tett opp mot bergen stacken, dette for å gjøre det lett for andre som potensielt skal overta prosjektet når vi er ferdig.

<br/>

<h3 id="routing-API">Routing</h3>
Vi trenger endepunkt for følgende systemer:

1. Authorisering layer.
   Dette laget må bruker passe gjennom for å få komme til de neste endepunktene. Her må vi passe på å ungå at en potensiell bruker kan snike seg rundt layeret å få tilgang til de andre lagene uten authorisering.
   dette vil være /, men vil passe deg ned til /:uid, som så vil passe deg ned til hvorenn du skulle ønske. Siden dette er et internt system kan potensielt dette samkjøres med frontend, hvis frontend og backend deler samme routing.

2. Query
   Her gjør bruker queries til databasen. Her må vi finne beste måten å gjennomføre dette på. Vi har som mål å ta i bruk URL req query systemet til dette. Dette vil være et /GET med queryparams.

3. Update
   Dette vil være en /POST som skal extracte ut data fra et excel ark, og poste data fra excel arket med rett struktur til databasen.

4. ÅrsRapport
   Dette endepunktet vil levere ut en årsrapport fil for ønskede organisasjonsnr.

<br/>

### Micro Services

En Cronjob skal være kjørende på serveren som en gang i året gjør calls til /Changes endepunktet til PROFF for å finne ny data for alle organisasjonsnr i databasen. Den vil så hente data for hvert ORGNR med oppdatert data.

<br/>

### Middleware

Vi bruker MAL auth som authorizering middleware, da dette skal hostest og brukes internt på Azure.

<br/>

## Database

<br/>

<h3 id="formål-DATABASE">Formål</h3>
Databasen skal være lett, ta lite plass. Og håndtere så mye logikk den kan selv. DVS API skal gjøre en query for hvert "spørsmål" og ikke trenge å loope.

<br/>

### Schema

Foreløbig ide til database setup.

Tabell 1.

Bedrift_Info

| bedrift_id (SERIAL PRIMARY KEY) | Orgnummer (INTEGER UNIQUE NOT NULL) | MålBedrift (VARCHAR(255) NOT NULL) | Bransje (VARCHAR(255) DEFAULT NULL) | Beskrivelse (VARCHAR(255) DEFAULT NULL) | NavneListe (VARCHAR(255)[]) DEFAULT NULL |
| :------------------------------ | :---------------------------------- | :--------------------------------- | :---------------------------------- | --------------------------------------- | :--------------------------------------- |
| 1                               | 43234324                            | Bedriften                          | IT                                  | Beste Bedrift                           | [startuppen, helt-okay-bedrift]          |

Tabell 2.

oversikt_bedrift_fase_status

| bedrift_id (INTEGER REFERENCES bedrift_info(bedrift_id)) | rapportår (INTEGER NOT NULL) | fase (VARCHAR(255)[]) | PRIMARY KEY(bedrift_id, rapportår, fase) |
| :------------------------------------------------------- | :--------------------------- | :-------------------- | :--------------------------------------- |
| 1                                                        | 2023                         | '{Alumni}'            | (1,2023, '{Alumni}')                     |

Tabell 3.

Årlig_økonomisk_data

| bedrift_id (INTEGER REFERENCES bedrift_info(bedrift_id)) | rapportår (INTEGER) | øko_kode(VARCHAR(255) REFERENCES øko_kode_lookup(øko_kode)) | øko_verdi (NUMERIC(10,4) DEFAULT NULL) | PRIMARY KEY(bedrift_id, rapportår, øko_kode) |
| :------------------------------------------------------- | :------------------ | :---------------------------------------------------------- | :------------------------------------- | -------------------------------------------- |
| 1                                                        | 2023                | EK                                                          | 230.4                                  | (1,2023,EK)                                  |

Denne tabellen kan potensielt partisjoneres etter BERDIFT_ID hvis den blir stor.

Tabell 4.

Øko_kode_lookup

| øko_kode(VARCHAR(255)) | kode_beskrivelse(text) |
| :--------------------- | :--------------------- |
| EK                     | 'Egen Kapital'         |

Tabell 5.

bedrift_kunngjøringer

| bedrift_id (INTEGER REFERENCES bedrift_info(bedrift_id)) | kunngjørings_id (VARCHAR(255)) | dato (VARCHAR(255)) | kunngjøringstekst (text)                | kunngjøringstype (VARCHAR(255)) | PRIMARY KEY (bedrift_id, kunngjørings_id) |
| :------------------------------------------------------- | :----------------------------- | :------------------ | :-------------------------------------- | :------------------------------ | :---------------------------------------- |
| 1                                                        | 2309400923498234               | "20.12.12"          | "Noe ganske stort og kult er kunngjort" | "Generell"                      | (1, 2309400923498234)                     |

Tabell 6.

bedrift_leder_oversikt<br>
Her beholder vi kun info om de som er markert med Kode DAGL eller LEDE

| bedrift_id (INTEGER REFERENCES bedrift_info(bedrift_id)) | tittel (VARCHAR (255)) | navn (VARCHAR(255)) | fødselsdag (VARCHAR(255)) | tittelkode (VARCHAR255) | rapportår (INTEGER) |
| :------------------------------------------------------- | :--------------------- | :------------------ | :------------------------ | :---------------------- | :------------------ |
| 1                                                        | Styrets Leder          | John Johnson        | "20.12.12"                | LEDE                    | 2024                |

Tabell 7.

bedrift_shareholder_info

| bedrift_id (INTEGER REFERENCES bedrift_info(bedrift_id)) | rapportår (INTEGER) | antall_shares (INTEGER) | shareholder_bedrift_id (VARCHAR (255)DEFAULT NULL) | navn (VARCHAR(255)) | sharetype (VARCHAR(255)) | shareholder_fornavn (VARCHAR (255) DEFAULT NULL) | shareholder_etternavn (VARCHAR(255) DEFAULT NULL) |
| :------------------------------------------------------- | :------------------ | :---------------------- | :------------------------------------------------- | :------------------ | :----------------------- | :----------------------------------------------- | :------------------------------------------------ |
| 1                                                        | 2024                | 300000                  | null                                               | Fattern             | 50                       | null                                             | null                                              |

Tabell 8.

generell_årlig_bedrift_info

| bedrift_id(INTEGER REFERENCES bedrift_info(bedrift_id)) | rapportår INTEGER | antall_ansatte INTEGER | landsdel (VARCHAR(255)) | fylke (VARCHAR(255)) | kommune (VARCHAR(255)) | post_kode (VARCHAR(255)) | post_adresse (VARCHAR(255)) |
| :------------------------------------------------------ | :---------------- | :--------------------- | :---------------------- | :------------------- | :--------------------- | :----------------------- | :-------------------------- |
| 1                                                       | 2024              | 1                      | Vestlandet              | Vestland             | Bergen                 | 5050                     | Sandefjordsvika 98          |

<br/>

<h3 id="funksjoner-DATABASE">Funksjoner</h3>

Når C# backend fetcher ny data fra proff dekonstruerer den JSON data til classer som passer til følgende databasefunksjoner:

1. Insert*bedrift_leder_info(orgnr INTEGER, input_navn VARCHAR(255), input_tittel VARCHAR(255), input_tittelkode VARCHAR(255), input*år INTEGER, input_fødselsår VARCHAR(255))<br>

```sql

DECLARE
	id INTEGER;
BEGIN
	SELECT bedrift_id
	FROM bedrift_info
	INTO id
	WHERE orgnummer = orgnr;
	INSERT INTO bedrift_leder_oversikt(bedrift_id, tittel, navn, fødselsdag, tittelkode, rapportår)
	VALUES (id, input_tittel, input_navn, input_fødselsår, input_tittelkode, input_år)
	ON CONFLICT do nothing;
END;
```

2. insert*generell*årlig_bedrift_info(orgnr INTEGER, år INTEGER ,input_landsdel VARCHAR, input_fylke VARCHAR, input_kommune VARCHAR, input_post_kode VARCHAR, input_post_adresse VARCHAR, input_antall_ansatte INTEGER DEFAULT NULL)<br>

```sql

DECLARE
	id INTEGER;
BEGIN
	SELECT bedrift_id
	FROM bedrift_info
	INTO id
	WHERE orgnummer = orgnr;
	INSERT INTO generell_årlig_bedrift_info(bedrift_id, rapportår, landsdel, fylke, kommune, post_kode, post_addresse, antall_ansatte)
	VALUES (id, år, input_landsdel, input_fylke, input_kommune, input_post_kode, input_post_addresse, input_antall_ansatte)
	ON CONFLICT do nothing;
END;
```

3. insert_kunngjøringer(orgnr INTEGER, input_id BIGINT, input_dato VARCHAR, input_desc TEXT, input_type VARCHAR)<br>

```sql

DECLARE
	id INTEGER;
BEGIN
	SELECT bedrift_id
	FROM bedrift_info
	INTO id
	WHERE orgnummer = orgnr;
	INSERT INTO bedrift_kunngjøringer(bedrift_id, kunngjøring_id, dato, kunngjøringstekst, kunngjøringstype)
	VALUES (id, input_id, input_dato, input_desc, input_type)
	ON CONFLICT do nothing;
END;
```

4. insert_shareholder_info(orgnr INTEGER, år INTEGER, antall INTEGER, input_navn VARCHAR, input_type VARCHAR, bedrift_navn VARCHAR DEFAULT NULL, fornavn VARCHAR DEFAULT NULL, etternavn VARCHAR DEFAULT NULL)<br>

```sql

DECLARE
	id INTEGER;
BEGIN
	SELECT bedrift_id
	FROM bedrift_info
	INTO id
	WHERE orgnummer = orgnr;
	INSERT INTO bedrift_shareholder_info(bedrift_id, rapportår, antal_shares, shareholder_bedrift_id, shareholder_fornavn, shareholder_etternavn, navn, sharetype)
	VALUES (id, år, antall, bedrift_navn, fornavn, etternavn, input_navn, input_type)
		ON CONFLICT do nothing;
END;
```

5. insert_øko_data(orgnr INTEGER, år INTEGER, kodenavn VARCHAR[], kodeverdier NUMERIC[])<br>

```sql

DECLARE
	id INTEGER;
	curr_kode VARCHAR(255);
	curr_val NUMERIC;
BEGIN
	SELECT bedrift_id
	FROM bedrift_info
	INTO id
	WHERE orgnummer = orgnr;
	FOR i in 1..array_length(kodenavn, 1)
	LOOP
		curr_kode := kodenavn[i];
		curr_val := kodeverdier[i];
		INSERT INTO årlig_økonomisk_data(bedrift_id, rapportår, øko_kode, øko_verdi)
		VALUES (id, år, curr_kode, curr_val)
		ON CONFLICT do nothing;
	END LOOP;
END;
```

6. update_bedrift_info_with_name(orgnr INTEGER, navn VARCHAR, tidligere_navn VARCHAR[])<br>

```sql
BEGIN
	UPDATE bedrift_info
	SET målbedrift = navn,
		navneliste = tidligere_navn
	WHERE orgnummer = orgnr;
END
```

Når bruker vil legge til flere bedrifter via excel-ark gjennom dashboard kjøres følgende funksjon:

1. insert_bedrift_data_vis(orgnr INTEGER, bransjenavn VARCHAR, rapportår INTEGER[], faser VARCHAR[])<br>

```sql

DECLARE
	id INTEGER;
	år INTEGER;
	fasetext VARCHAR(255);
BEGIN
	INSERT INTO bedrift_info(orgnummer, bransje)
	VALUES (orgnr, bransjenavn)
	ON CONFLICT(orgnummer)
	DO UPDATE SET bransje = EXCLUDED.bransje
	RETURNING bedrift_id INTO id;

	FOR i in 1..array_length(rapportÅrarray, 1)
	LOOP
		år := rapportÅrarray[i];
		fasetext := Faser[i];
		INSERT INTO oversikt_bedrift_fase_status(bedrift_id, rapportår, fase)
		VALUES (id, år, fasetext)
		ON CONFLICT do nothing;
	END LOOP;
END;
```

Når ny data fra proff er lagt inn i databasen, trigrer C# følgende funksjon for å generere DELTA verdier:

1. update_delta()

```sql

BEGIN
	UPDATE årlig_økonomisk_data t1
	SET delta = t1.øko_verdi - t2.øko_verdi
	FROM årlig_økonomisk_data t2
	WHERE t1.bedrift_id = t2.bedrift_id
	AND t2.øko_kode = t2.øko_kode
	AND t1.rapportår = t2.rapportår+1;
	UPDATE årlig_økonomisk_data t1
	SET delta = 0
	WHERE NOT EXISTS(
		SELECT 1
		FROM årlig_økonomisk_data t2
		WHERE t1.bedrift_id = t2.bedrift_id
		AND t2.øko_kode = t2.øko_kode
		AND t1.rapportår = t2.rapportår+1
	);
END;
```

Grunnen for at dette gjøres på C# er fordi postgreSQL ikke har en dedikert batch insert trigger. Dette er en mer nøyaktig måte å gjøre dette på,<br> og garanterer at delta blir laget korrekt uavhengig av hvor mange år som legges inn om gangen.
