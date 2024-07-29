# Bedrift-tracker Arbeidsplan:

## Index:

- [Intro](#intro)
  - [Bruk](#bruk)
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
  - [Views](#views-DATABASE)

## Intro

Dette er Prosnapper. Et verktøy for innsamling, og utregning av nøkkeltall for VIS innovasjon.<br/>
Det er skrevet i C# med en React + VITE frontend. <br/>
Det er en CRUD applikasjon, med excel som primærverktøy for datamanipulasjon i backend. <br/>
<br/>

### Bruk

Hvordan bruke produktet:<br/>

- Key figures:<br/>
  Dette er lett tilgjengelige nøkkeltall fra datasettet.

- Bruke graf:<br/>
  1. Filter: <br/>
     Datasettet er gruppert i forskjellige grupper som kan filtreres i via filter knappen. Foreløbig kan du filtrere data basert på bedriftleder's alder, <br/>
     Hvilken fase bedriften har vært i, <br/>
     og hvilke bransje bedriftene er i. <br/>
  2. Koder: <br/>
     Grafen viser kunn et sett med data om gangen, basert på hvilken øko-kode som er valgt. Det er tre øko-koder lett tilgjengelig: <br/>
     Driftsresultat, <br/>
     Omsetning, <br/>
     og Sum Innskutt Egenkapital.<br/>
     Andre økokoder kan finnes i dropdown meny markert med øk. koder.<br/>
  3. Kan kan bruke Velg år slideren for å velge et start og slutt år på datasettet.
  4. Man kan midlertidig velge vekk valgte filtre ved å trykke på de i bunn av grafen. <br/>
     Dette vil filtrere vekk dataen fra grafen, og vise en strek over navnet i bunn av grafen.<br/>
     For å få data tilbake er det bare å trykke på navnet igjen. <br/>
  5. Velge datatype: <br/>
     Man kan velge å få presentert tre forskjellige verdier i datasettet. <br/> - Gjennomsnittsverdi - Akkumulert - Gjennomsnitts endring over tid
     Disse kan man velge mellom via radio knapper under grafen. <br/>
- Bruke Tabell:<br/>
  Tabellen viser bedrifter ranksjert etter høyest akkumulert verdi i gjeldene økokode.<br/>
  Man kan søke opp og filtrere etter verdiene man selv ønsker i grafen ved å trykke på tre dotter i kollonen det gjelder. <br/>
  Dataen i grafen er alltid for to år tilbake i tid, for å garantere at all dataen som mulig er hentet inn.<br/>
  Man kan velge mellom å vise 5 eller 10 bedrifter om gangen. <br/>
- Yearly Rapport:<br/>
  Her kan man generere en årsrapport excel fil ved å laste opp en excelfil med organisasjonsnr man ønsker data om.<br/>
  Er man usikker på oppsettet av excel arket, kan man bruke "Get Template" for å få en eksempelfil.
- Company Flow:<br/>
  Hovedvalg for manipulasjon av data:<br/>
  1. Add Company Data
     Desverre blir ikke databasen oppdatert automatisk når nye bedrifter blir tatt opp i VIS.<br/>
     Nye bedrifter kan legges til i Add Company Data.<br/>
     Er man usikker på hva data som skal legges til fra VIS, kan man bruke "Get Template" for å se en eksempelfil. <br/>
  2. Delete Company data
     Hvis man ønsker å slette en bedrift fra systemet kan det gjøres her.<br/>
     Man laster opp en excelfil med organisasjonsnr man ønsker å slette.<br/>
     Er man usikker på oppsettet av excelfilen, kan man bruke "Get Template".<br/>
- Get Full View:<br/>
  Her kan man laste ned hele datasettet i excelformat. <br/>

### Produkt

Fullstack app, som skal kombinere data fra PROFF.NO, og samkjøre dette med data fra VIS. <br/>
mth. Oppfølging av bedrifter som har vært gjennom VIS inkubasjon og/eller andre programmer. <br/>
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
| Graf           | `<LineChart/> `   | Graf for å visualisere dataset.                            | General Graph Components        | Ja, hentet fra mui: mui-x-bar-chart    |
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
Alle endepunktene til api ligger under fellesendepunktet ./API/ .<br/>

Liste over endepunkter i API: <br/>

1. yearlyreport:

   - Henter data fra yearlyreport materialized view, og leverer excelfil basert på det. <br/>

2. orgnummertemplate:

   - genererer excel template for orgnummer baserte operasjoner<br/>

3. dbupdatetemplate:

   - genererer exceltemplate for insetting av ny data til database.<br/>

4. graphdata:

   - genererer graphdata for frontend. Leverer i JSON format<br/>

5. workyear:

   - genererer tall for Årsverk Key Figure. <br/>

6. workercount:

   - genererer tall for Arbeidsplasser Key Figure. <br/>

7. totalturnover:

   - genererer tall for Omsetning Key Figure. <br/>

8. companycount:

   - generer tall for Antall Bedrifter Key Figure. <br/>

9. excelfullview:

   - genererer excel fil med fullstendig datasett fra database.<br/>

10. updatewithnewdata:

    - håndterer oppdatering av database med ny data fra excelark.<br/>

11. deletedata:

    - håndterer sletting av data basert på excelfil.<br/>

12. tabledata:

    - genererer data for frontend table. Leverer JSON format.<br/>

13. updateonschedule:

    - WIP endepunkt for å håndtere schedulert oppdatering av data via en Azure Function.<br/>

<br/>

### Micro Services

En Azure Funksjon vil kalle updateonschedule for å automatisere innhenting av ny data fra bl.a. PROFF.

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

Ferdig oppsett av database ser slik ut:<br>
![Bilde som viser relasjonsgraf for databasen](https://imgur.com/a/z6nvU25)
<br/>

<h3 id="funksjoner-DATABASE">Funksjoner</h3>
Databasen har to hjelpefunksjoner som blir trigget av EF core.<br/>

1. Update delta:
   Denne funksjonen oppdaterer data generert av databasen ved innsetting av ny data.<br/>
2. Update Views:
   Denne funksjonen oppdaterer materialized views ved innsetting av ny data.<br/>

<h3 id="views-DATABASE">Views</h3>
Databasen har flere materialized views for å hjelpe med rask levering av databasegenerert data.

1. Average Values:
   Dette er et materialized view som viser gjennomsnittsverdier for hele databasen, uten filter.
2. Data Sorted By Company Branch
   Dette er et materialized view som viser gjennomsnittsverdier for hele databasen, men er gruppert basert på bedriftenes bransje.
3. Data Sorted By Leader Age
   Dette er et materialized view som viser gjennomsnittsverdier for hele databasen, men er gruppert i aldergrupper basert på bedriftlederens alder.
4. Data Sorted By Phase
   Dette er et materialized view som viser gjennomsnittsverdier for hele databasen, men er gruppert basert på hvilken intern fase bedriften er i.
5. Full View
   Dette er et materialized view som kombinerer dataen til et dataset som passer Excel Format.
6. Årsrapport
   Dette er et materialized view som kombinerer dataen til et årsrapport format som passer Excel Format.
