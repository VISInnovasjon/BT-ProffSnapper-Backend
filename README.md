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

7. Vi følger bergen-stacken og har som mål å skrive en front-end i NextJS, med en backend i C#, og bruker postgreSQL som database.
   <br/>

### Del Mål

1. Lage et wireframe oppsett på hvordan siden skal se ut, og hvordan brukeropplevelsen skal være. <a href="https://excalidraw.com/#json=qp0am50gVeJC4uG-84dPQ,30uxisDeckPDPbJXrDlwWg"> Foreløbig ide</a>
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
Et verktøy som kan gjøre det lettere å hente, manipulere og bruke data fra bl.a. PROFF for bl.a. generer årsrapporter.

<br/>

## Bruk av GitHub

Vi jobber primært på VIS INNOVASJON sin github, i BT repo.
<br/>

### Brancher

Vi utvikler og pusher endringer til DEV, og holder av push til main primært når vi har nådd milepæler.
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
Målet er å skape en stil-ren, lettleselig og enkel frontend for å gjennomføre de arbeidsoppgaver som sluttbruker krever.

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

Her er en liste over komponenter som trengs til prosjektet.

| Komponent navn | Komponent form | Komponent Beskrivelse |
| :------------- | :------------- | :-------------------- |
| Komponent      | `<Komponent/>` | Er en komponent       |

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

| bedrift_id (SERIAL PRIMARY KEY) | Orgnummer (INTEGER UNIQUE NOT NULL) | MålBedrift (VARCHAR(255) NOT NULL) | Bransje (VARCHAR(255) DEFAULT NULL) | Beskrivelse (VARCHAR(255) DEFAULT NULL) |
| :------------------------------ | :---------------------------------- | :--------------------------------- | :---------------------------------- | --------------------------------------- |
| 1                               | 43234324                            | Bedriften                          | IT                                  | Beste Bedrift                           |

Tabell 2.

Oversikt*bedrift_fase_lokasjon_pr*år

| bedrift_id (INTEGER REFERENCES bedrift_info(bedrift_id)) | rapportår (INTEGER NOT NULL) | fase (VARCHAR(255)[]) | PRIMARY KEY(bedrift_id, rapportår) | Bedrift_adrr (VARCHAR(255))    |
| :------------------------------------------------------- | :--------------------------- | :-------------------- | :--------------------------------- | :----------------------------- |
| 1                                                        | 2023                         | '{Alumni}'            | (1,2023)                           | Trollskogen 99, 1234 Helgoland |

Tabell 3.

Årlig_økonomisk_data

| bedrift_id (INTEGER REFERENCES bedrift_info(bedrift_id)) | rapportår (INTEGER) | øko_kode(VARCHAR(255) REFERENCES øko_kode_lookup(øko_kode)) | øko_verdi (INTEGER DEFAULT NULL) | PRIMARY KEY(bedrift_id, rapportår, øko_kode) |
| :------------------------------------------------------- | :------------------ | :---------------------------------------------------------- | :------------------------------- | -------------------------------------------- |
| 1                                                        | 2023                | EK                                                          | 230                              | (1,2023,EK)                                  |

Tabell 4.

Øko_kode_lookup

| øko_kode(VARCHAR(255)) | kode_beskrivelse(text) |
| :--------------------- | :--------------------- |
| EK                     | 'Egen Kapital'         |

<br/>

<h3 id="funksjoner-DATABASE">Funksjoner</h3>
```
