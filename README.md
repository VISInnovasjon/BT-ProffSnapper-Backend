# Proffsnapper, Bruk og utvikling.

## Index:

- [Intro](#intro)
  - [Bruk](#bruk)
  - [Produkt](#produkt)
  - [Lokal Kopi](#lokal-kopi)
- [Bruk av Github](#git-hub)
  - [Brancher](#brancher)
  - [Code Review](#code-review)
- [Kode Standarer](#kode-standarer)

  - [Error Håndtering](#error-håndtering)

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

Dette er Proffsnapper. Et verktøy for innsamling, og utregning av nøkkeltall for VIS innovasjon.<br/>
Det er skrevet i C# med en React + VITE frontend. <br/>
Det er en CRUD applikasjon, med excel som primærverktøy for datamanipulasjon i backend. <br/>
<br/>

### Bruk

Proffsnapper sin backend består av flere forskjellige endepunkter som kan nåes ved å calle /api/endepunktnavn.<br/>
Mesteparten av dataen og skriften på frontenden kan også fåes ut i JSON format ved å bruke disse endepunktene. <br/>
Det er også mulig å kjøre en lokal versjon av programmet, mer detaljer om dette finner du under "Lokal kopi".<br/>
<br/>
Appen følger "Bergenstacken" og bruker en C# Backend med EF Core, postgreSQL database og en React basert frontend. <br/>

### Lokal Kopi

For å sette opp en lokal kopi må bruker installere en utgave av Docker Daemon på pc. <br/>
Den enkleste måten å gjøre dette på er å installere Docker Desktop fra <a href="https://www.docker.com/products/docker-desktop/">Docker</a>.<br/>
Når dette er gjort kan man forke ned repoet. <br/>
Det første man bør gjøre er å kikke på filen som heter docker-compose.yaml<br/>
Her kan man se at den krever en del environmentvariabler som skal lastes inn når Docker Daemon starter de forskjellige bildene.<br/>
Mange av disse kreves også i serverkoden, som f.eks databasepassordene. <br/>
For å lage en env fil, kan man kopiere .env_example og rename den til .env.<br/>
For så å fylle ut variablene inni, til de ønskede verdier.<br/>

Hvis man er usikker, og vil ha mer forklaring kan man bruke setup_env_file.sh <br/>
Denne filen er en shell script som kjører i en BASH terminal, så den krever <a href="https://itsfoss.com/install-bash-on-windows/"> WSL og BASH</a>. <br/>
Hvis du har bash installert, og er i en bash terminal, kan man gjøre følgende:

- Skriv følgende kommando inn i terminal for å gjøre shell scriptet "executable": <br/>

```bash
chmod +X setup_env_file.sh
```

- Kjør følgende kommando inn i terminal for å kjøre scriptet:<br/>

```bash
./setup_env_file.sh
```

- Følg instruksjonene som kommer opp i terminal, disse er på engelsk. <br/>
  Verdiene du skriver inn er kun for lokalt bruk, og fungerer ikke andre steder enn i din lokale kopi av prosjektet, med unntak i PROFF_API nøkkel. <br/>
  Når man har enten har laget en manuel kopi selv, eller kjørt setup scriptet, skal man nå ha en .env fil i rootfolderen til prosjektet. <br/>

Det neste steget er å skjekke at docker daemon kjører.<br/>
I terminal skriv følgende kommando: <br/>

```bash
docker ps
```

<br/>
Her vil man enten få opp en error during connect, eller info om noen images kjører.<br/>
Får man en error, prøv å start docker daemon enten via service start, eller ved å starte Docker Desktop. <br/>
Hvis man ser at docker er aktiv og kan kobles til. Kan man prøve å starte den lokale kopien via følgende kommando: <br/>

```bash
docker-compose up --build
```

Da skal docker laste ned bildene de trenger fra docker hub, og initialisere og starte opp alle bildene.<br/>
I terminalen vil du se etter byggstadiet er gjort, at terminalen "Attacher" til postgres-1, csserver-1 og pg_admin-1.<br/>
terminallinjer fra serveren vil da vises her. <br/>

Legg merke til --build flagget. <br/>
Dette flagget må legges ved første gang man kjører prosjektet, samt hver gang man gjør endringer i C# koden,<br/> dette er for å si til docker at programmet er endret, og må bygges på nytt før man kjører. <br/>
Hvis man ikke har gjort noen endringer, og allerede har gjort --build steget en gang før, kan man også starte bildene med en enklere kommando: <br/>

```bash
docker-compose up
```

<br/>
Da bruker docker filene de allerede har generert, for å starte programmet.<br/>
Hvis man ikke ønsker å se terminal feeden til docker imagene, kan man også starte i detached modus.<br>

```bash
docker-compose up -d
```

<br/>
Da får man ikke opp terminalfeeden til docker imagene i terminalen i f.eks VS-code, men kan fremdeles finne terminalene til imagene i Docker Desktop. <br/>
Man kan også kombinere disse flaggene:<br/>

```bash
docker-compose up -d --build
```

<br/>
For å stenge bildene bruker man følgende kommando: <br/>

```bash
docker-compose down
```

<br/>
Hvis man kjører uten -d flagget kan man også stoppe via shortcut ctrl+c.<br/>
Når bildene er oppe finner man ProffSnapper sin homepage på localhost:5000, og PgAdmin sin login side på localhost:5050.<br/>
<br/>
Det første man bør gjøre når man ser docker bildene fungerer som det skal, er å åpne PgAdmin, og legge til både Azure databasen, og den lokale kopien. <br/>
Når man har logget inn med sin lokale bruker i PgAdmin, kan man legge til en ny database ved å registrere en ny server ved å høyreklikke på "Servers" gruppen som ligger i object exlorer:<br/>

![Bilde som viser Object Explorer -> Register -> Server...](https://i.imgur.com/p0VETEV.png)

<br/>
Da vil det dukke opp en Register Server popup, og man er på "tab" markert General.<br/>
Her kan man fylle inn Navnet serveren skal være registrert i på PgAdmin. Det lureste er å gi det samme navnet du gav i .evn filen. <br/>

![Bilde som viser General Tab hvor man fyller inn Name](https://i.imgur.com/kTM7PKz.png)

<br/>
Etter det må man inn på Connection Tabben, her fyller man inn detaljene man trenger for å koble seg til en ekstern database.<br/>
Her fyller man inn andre verdier fra .env filen, som DATABASE_USER og DATABASE_PASSWORD. Samt DATABASE_HOST.<br/>
Disse verdiene kan hentes ut fra Azure, for å koble seg til produksjonsdatabasen. Man finner de under Environment Variables på prosjektet. <br/>

![Bilde som viser General Tab hvor man fyller inn Connection tab](https://i.imgur.com/1CFvJQb.png)
<br/>

Tilkobling til Produksjonsdatabasen er kun tilgjengelig på Collaborate nettverket. <br/>

Når man ser at begge databasene dukker opp i PgAdmin, kan man bruke pgdump og pgrestore for å bygge din lokale database som en kopi av produksjonsdatabasen. Dette kan gjøres via SQL kommandoer i terminalen til pgadmin,<br/> eller ved å høyreklikke på en database, velge backup, så fylle inn hva man trenger. <br/>

Velg databasen som heter postgres under Azure serveren. Høyre klikk på denne og velg backup... <br/>
![Bilde som viser Azure server -> postgres -> backup](https://i.imgur.com/nqoJh7B.png)
<br/>
Da får du opp en backup meny. Siden vi er interessert i en full backup, trenger man kun å gi et filnavn. Her er det viktig at man avslutter med .backup <br/>

![Bilde som viser backup menyen](https://i.imgur.com/GDpycRN.png)
<br/>

Trykk på "Backup" knappen nede til høyre i menyen for å lagre en backup av databasen. <br/>

Gå så inn i din lokale postgres server, og finn databasen med navnet du gav den. Høyreklikk og velg restore...<br/>

![Bilde som viser lokal server -> mydatabasename -> restore](https://i.imgur.com/uNtGxS4.png)
<br/>
Da åpner det seg en ny meny, hvor man kan enten skrive inn filnavnet til backup filen direkte, eller velge en undermeny for å finne filen i filstrukturen til pgAdmin.<br/>

![Bilde som viser restore meny](https://i.imgur.com/UqGMgQb.png)

<br>
Undermeny for å velge fil direkte, ved å trykke på mappe ikonet: <br/>

![Bilde som viser undermeny for å velge fil](https://i.imgur.com/lR3zynx.png)

<br/>

Når dette er gjort vil din lokale kopi være en nøyaktig kopi av prod, og ha samme data tilgjengelig.<br/> Din lokale kopi av proffsnapper er satt opp til å kun snakke med din lokale postgres server, slik at endringer som blir gjort kan testes skikkelig før de bli lagt opp i Azure. <br/>

<br/>

## Bruk av GitHub

Vi jobber primært på VIS INNOVASJON sin github, i BT proff-snapper backend repo.<br/>
<br/>

### Brancher

Vi har to hovedrepoer. Main, som er en stabil build, identisk til Azure Prod. Og Dev som er hvor endringer blir testet og klargjort før merging med Main. <br/>
For å passe på stabilitet og ryddighet, lagres endringer i egne brancher, som merges inn i Dev og slettes etterhvert som endringer er utført.<br/>
Her er det lurt å ha en god detaljert mergekommentar så det er lett å lese og dømme hva endringer som blir gjort, og hvorfor. <br/>
<br/>

### Code Review

Vi bruker primært pull-requests for å oppdatere koden i dev. Det gjør at vi har to øyne på koden som går opp til en hver tid. <br/>Når flere jobber i repoet, er det lurt å kjøre en code-review på merge requesten for å skjekke over at filer plutselig ikke dukker opp som ikke skal dukke opp, eller kode ikke gjør det den skal.<br/>
<br/>

## Authorizering

Authorisering håndteres av MSAL-react på frontend, og ENTRA på Azure. 

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

   - genererer excel fil med fullstendig datasett fra database. Ligger bak entra auth.<br/>

10. updatewithnewdata:

    - håndterer oppdatering av database med ny data fra excelark. Ligger bak entra auth.<br/>

11. deletedata:

    - håndterer sletting av data basert på excelfil. Ligger bak entra auth.<br/>

12. tabledata:

    - genererer data for frontend table. Leverer JSON format.<br/>

13. updateonschedule:

    - WIP endepunkt for å håndtere schedulert oppdatering av data via en Azure Function. Ligger bak entra auth.<br/>

<br/>

### Micro Services

En Azure Funksjon vil kalle updateonschedule for å automatisere innhenting av ny data fra bl.a. PROFF. Er gjort via en webjob på Azure.

<br/>


<br/>

## Database

<br/>

<h3 id="formål-DATABASE">Formål</h3>
Databasen skal være lett, ta lite plass. Og håndtere så mye logikk den kan selv. DVS API skal gjøre en query for hvert "spørsmål" og ikke trenge å loope.

<br/>

### Schema

Ferdig oppsett av database ser slik ut:<br>
![Bilde som viser relasjonsgraf for databasen](https://i.imgur.com/UXei4yO.png)
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
