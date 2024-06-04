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
4. Bestemme bruk av Frontend framework, hva som egner seg best, og hva som er lettest å self-hoste på azure. Next? Ren react? JS + HTML? <br> Vi velger React + Vite som frontend og C# som backend.
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

Ferdig oppsett av database ser slik ut:<br>
![Bilde som viser relasjonsgraf for databasen](https://i.imgur.com/21q4rG5.jpeg)
<br/>

<h3 id="funksjoner-DATABASE">Funksjoner</h3>
Mesteparten av operasjoner mot databasen er gjort gjennom Entity Framework Core.<br>
<br>
For å simplifisere queries er det laget følgende views på databasen:<br>

1. Årsrapport.
   Denne viewen er laget for å hjelpe med generering av årsrapporter.<br>
   ```sql
   SELECT b.orgnummer,
   g.antall_ansatte,
   "øk_data".driftsresultat,
   "øk_data".sum_drifts_intekter,
   "øk_data".sum_innskutt_egenkapital,
   "øk_data".delta_innskutt_egenkapital,
   "øk_data"."ordinært_resultat",
   g.post_addresse,
   g.post_kode,
   shareholder_data.antal_shares AS antall_shares_vis,
   shareholder_data.sharetype AS shares_prosent
   FROM bedrift_info b
   JOIN "generell_årlig_bedrift_info" g ON b.bedrift_id = g.bedrift_id AND g."rapportår" = (EXTRACT(year FROM CURRENT_DATE)::integer - 1)
   LEFT JOIN LATERAL ( SELECT max(
             CASE
                 WHEN "ø"."øko_kode"::text = 'DR'::text THEN "ø"."øko_verdi"
                 ELSE NULL::numeric
             END) AS driftsresultat,
         max(
             CASE
                 WHEN "ø"."øko_kode"::text = 'SI'::text THEN "ø"."øko_verdi"
                 ELSE NULL::numeric
             END) AS sum_drifts_intekter,
         max(
             CASE
                 WHEN "ø"."øko_kode"::text = 'SIK'::text THEN "ø"."øko_verdi"
                 ELSE NULL::numeric
             END) AS sum_innskutt_egenkapital,
         max(
             CASE
                 WHEN "ø"."øko_kode"::text = 'SIK'::text THEN "ø".delta
                 ELSE NULL::numeric
             END) AS delta_innskutt_egenkapital,
         max(
             CASE
                 WHEN "ø"."øko_kode"::text = 'OR'::text THEN "ø"."øko_verdi"
                 ELSE NULL::numeric
             END) AS "ordinært_resultat"
        FROM "årlig_økonomisk_data" "ø"
       WHERE "ø".bedrift_id = b.bedrift_id AND "ø"."rapportår" = (EXTRACT(year FROM CURRENT_DATE)::integer - 1)) "øk_data" ON true
   LEFT JOIN LATERAL ( SELECT s.antal_shares,
         s.sharetype
        FROM bedrift_shareholder_info s
       WHERE s.bedrift_id = b.bedrift_id AND s."rapportår" = (EXTRACT(year FROM CURRENT_DATE)::integer - 1) AND s.shareholder_bedrift_id::text = '987753153'::text) shareholder_data ON true;
   `
   Denne joiner sammen alle verdier ønsket i en årsrapport, og kan queries etter bestemte organisasjoner. <br>
   ```
2. Gjennomsnittsverdier:
   ```sql
    SELECT "ø"."rapportår",
     "ø"."øko_kode",
     l.kode_beskrivelse,
     avg("ø"."øko_verdi") AS "avg_øko_verdi",
     avg("ø".delta) AS avg_delta
    FROM bedrift_info b
      JOIN "årlig_økonomisk_data" "ø" ON b.bedrift_id = "ø".bedrift_id AND "ø"."rapportår" >= 2014
      JOIN "øko_kode_lookup" l ON "ø"."øko_kode"::text = l."øko_kode"::text
   GROUP BY "ø"."rapportår", "ø"."øko_kode", l.kode_beskrivelse
   ORDER BY "ø"."rapportår", "ø"."øko_kode", l.kode_beskrivelse;
   ```

````
Genererer gjennomsnittsverdier for alle øko koder siden VIS var aktiv, og sorterer de etter år.<br>
3. Data_sortert_etter_fase:
	```sql
	 SELECT f.fase,
  "ø"."rapportår",
  "ø"."øko_kode",
  l.kode_beskrivelse,
  avg("ø"."øko_verdi") AS "avg_øko_verdi",
  avg("ø".delta) AS avg_delta
 FROM bedrift_info b
   JOIN oversikt_bedrift_fase_status f ON b.bedrift_id = f.bedrift_id
   JOIN "årlig_økonomisk_data" "ø" ON b.bedrift_id = "ø".bedrift_id AND "ø"."rapportår" >= 2014
   JOIN "øko_kode_lookup" l ON "ø"."øko_kode"::text = l."øko_kode"::text
GROUP BY f.fase, "ø"."rapportår", "ø"."øko_kode", l.kode_beskrivelse
ORDER BY f.fase, "ø"."rapportår", "ø"."øko_kode", l.kode_beskrivelse;
````

Leverer ut gjennomsnittsdata pr år, men sortert etter fase.<br> 4. Data\*sortert_etter_bransje:

```sql
SELECT b.bransje,
"ø"."rapportår",
"ø"."øko_kode",
l.kode_beskrivelse,
avg("ø"."øko_verdi") AS "avg*øko*verdi",
avg("ø".delta) AS avg_delta
FROM bedrift_info b
JOIN "årlig*økonomisk_data" "ø" ON b.bedrift_id = "ø".bedrift_id AND "ø"."rapportår" >= 2014
JOIN "øko_kode_lookup" l ON "ø"."øko_kode"::text = l."øko_kode"::text
GROUP BY b.bransje, "ø"."rapportår", "ø"."øko_kode", l.kode_beskrivelse
ORDER BY b.bransje, "ø"."rapportår", "ø"."øko_kode", l.kode_beskrivelse;

```

Leverer ut gjennomsnittsdata, sortert etter bransje. <br> 5. Data\*sortert_etter_aldersgruppe:

```sql
WITH lederalder AS (
SELECT b_1.bedrift_id,
date_part('year'::text, age(a."fødselsdag"::timestamp with time zone)) AS alder,
COALESCE(max(
CASE
WHEN a.tittelkode::text = 'DAGL'::text THEN a.tittelkode
ELSE NULL::character varying
END::text) OVER (PARTITION BY b_1.bedrift_id), max(
CASE
WHEN a.tittelkode::text = 'LEDE'::text THEN a.tittelkode
ELSE NULL::character varying
END::text) OVER (PARTITION BY b_1.bedrift_id)) AS tittelkode
FROM bedrift_info b_1
JOIN bedrift_leder_oversikt a ON b_1.bedrift_id = a.bedrift_id
), aldersgrupper AS (
SELECT lederalder.bedrift_id,
CASE
WHEN lederalder.alder >= 10::double precision AND lederalder.alder <= 19::double precision THEN '10-20'::text
WHEN lederalder.alder >= 20::double precision AND lederalder.alder <= 29::double precision THEN '20-29'::text
WHEN lederalder.alder >= 30::double precision AND lederalder.alder <= 39::double precision THEN '30-39'::text
WHEN lederalder.alder >= 40::double precision AND lederalder.alder <= 49::double precision THEN '40-49'::text
WHEN lederalder.alder >= 50::double precision AND lederalder.alder <= 59::double precision THEN '50-59'::text
WHEN lederalder.alder >= 60::double precision AND lederalder.alder <= 69::double precision THEN '60-69'::text
WHEN lederalder.alder >= 70::double precision AND lederalder.alder <= 79::double precision THEN '70-79'::text
WHEN lederalder.alder >= 80::double precision AND lederalder.alder <= 89::double precision THEN '80-89'::text
WHEN lederalder.alder >= 90::double precision AND lederalder.alder <= 99::double precision THEN '90-99'::text
ELSE '100+'::text
END AS alders_gruppe
FROM lederalder
)
SELECT "ø"."rapportår",
ag.alders_gruppe,
"ø"."øko_kode",
avg("ø"."øko_verdi") AS "avg*øko*verdi",
avg("ø".delta) AS avg_delta,
l.kode_beskrivelse
FROM bedrift_info b
JOIN aldersgrupper ag ON b.bedrift_id = ag.bedrift_id
JOIN "årlig*økonomisk_data" "ø" ON b.bedrift_id = "ø".bedrift_id AND "ø"."rapportår" >= 2014
JOIN "øko_kode_lookup" l ON "ø"."øko_kode"::text = l."øko_kode"::text
GROUP BY "ø"."rapportår", ag.alders_gruppe, "ø"."øko_kode", l.kode_beskrivelse;

```

Leverer gjennomsnitsverdier, men sorterer basert på alder av enten Daglig Leder eller Styreleder.<br>

```

```
