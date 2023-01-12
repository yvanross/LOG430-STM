## [Modifiabilité](#da-modifiabilite)

### [OA01 Faciliter le recrutement des nouveaux chargés de laboratoire](#oa01) {#m-oa01}
- Décrivez comment vous pouvez satisfaire cette exigence avec cet attribut de qualité.

### [OA02 Promouvoir l'utilisation des données ouvertes](#oa02) {#m-oa02}
- Décrivez comment vous pouvez satisfaire cette exigence avec cet attribut de qualité.

### [CU01 Comparaison de temps de trajet](#cu01) {#m-cu01}
- Le système doit être facilement modifiable pour ajouter ou enlever un nouveau service externe en moins de 30 secondes.
- On veut connaitre votre estimation du temps nécessaire pour implémenter un nouveau service externe.

### [CU02 ChaosMonkey](#cu02) {#m-cu02}
- L'ajout d'un nouveau microservice dans les listes de service perturbable doit se faire en moins de 30 secondes.
- L'implémentation du mécanisme de perturbation pour un nouveau microservice doit se faire en moins de 3 heures.

### [CU03 Impact écologique](#cu03) {#m-cu03}
- Chaque composant supportant ce CU doit avoir une seule responsabilité (Single Responsibility Principle et Separation of Concerns Principle)


### [CU04 Service d'authentification](#cu04) {#m-cu04}
- L'ajout du mécanisme d'authentification doit pouvoir se faire en moins d'une heure pour chaque microservice.

### [CU05 Notification administrateur](#cu05) {#m-cu05}
- La modifiabilité est très importante étant donné la grande quantité d’informations qui peuvent être reçues par ce cas d’utilisation. Il faut donc prendre en considération qu’un grand nombre d’informations peut vite rendre un système extrêmement complexe à gérer et modifier selon les besoins du moment.

### [CU06 Service externe](#cu06) {#m-cu06}
- L'intégration d'un nouveau système externe devrait se réaliser en moins de 1 heures.

### [CU07 Partager comparaison trajet](#cu07) {#m-cu07}
- Le système doit pouvoir intégrer ce composant avec le moins de couplage possible.
  
### [CU08 Favoris](#cu08) {#m-cu08}
- s.o.

### [CU09 Temps de trajet de STM](#cu09) {#m-cu09}
- Il devrait être facile de remplacer le site de la STM par le site d'un autre organisme de transport 
  
### [CU10 Météo](#cu10) {#m-cu10}
- On doit être en mesure de remplacer le service Météo par un autre service en moins de 3h.

## Conception pilotée par les attributs

### [(SC) Réduire la taille des modules](#rdtq-réduire-la-taille-des-modules)
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [(SC) Augmenter la cohésion](#rdtq-augmenter-la-cohésion)
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [(SC) Réduire le couplage](#rdtq-réduire-le-couplage)
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [(SC) Defer binding](#rdtq-defer-binding)
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>
  
  
## Réalisation des tactiques de qualité

###  [RDTQ Réduire la taille des modules](#sc-réduire-la-taille-des-modules)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ Augmenter la cohésion](#sc-augmenter-la-cohésion)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ Réduire le couplage](#sc-réduire-le-couplage)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ Defer binding](#sc-defer-binding)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>




### Relation entre les éléments architecturale et les exigences
|Identifiant|Éléments|Description de la responsabilité|
|-----------|--------|-------------------------------|
|[M-OA01](#m-oa01) | |
|[M-OA02](#m-oa02) | |
|[M-CU01](#m-cu01) | |
|[M-CU02](#m-cu02) | |
|[M-CU03](#m-cu03) | |
|[M-CU04](#m-cu04) | |
|[M-CU05](#m-cu05) | |
|[M-CU06](#m-cu06) | |
|[M-CU07](#m-cu07) | |
|[M-CU08](#m-cu08) | |
|[M-CU09](#m-cu09) | |
|[M-CU10](#m-cu10) | |
