Foreløbig TODO

[] Adde excel reading capabilities. - Har lagt til noen prototype classes, men de er alt for spesifik for en type excel fil. Her er det kanskje bedre å omstrukturere til at readeren kan tilpasses flere classes. - Må legge inn et generisk endpoint + finne ut hvordan temp paths fungerer for å fange opp opplastede excel filer.

[] Adde query endpoint. - Må laste inn postgres + bygge query endpoint på nytt, kan bruke prefilled endpoints fra weatherforecast template. - Se på muligheten om å lage mer generel søk. kanskje kreve at query params tilsvarer søkefelt i database.

[] Containerisere via docker compose. - Finne en måte å containerisere postgres + server, gjør det lettere å hoste på Azure etter building.



[] SIK -> DELTASIK kode i database. 


