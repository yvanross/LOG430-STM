## Modifiabilité

### [M-CU01](#cu01) Comparaison de temps de trajet
- Le système doit être facilement modifiable pour ajouter ou enlever un nouveau service externe en moins de 30 secondes.
- On veut connaitre votre estimation du temps nécessaire pour implémenter un nouveau service externe.
### [M-CU02](#cu02) ChaosMonkey
- L'ajout d'un nouveau microservice dans les listes de service perturbable doit se faire en moins de 30 secondes.
- L'implémentation du mécanisme de perturbation pour un nouveau microservice doit se faire en moins de 3 heures.

### [M-CU03](#cu03) Impact écologique 
- Chaque composant supportant ce CU doit avoir une seule responsabilité (Single Responsibility Principle et Separation of Concerns Principle)


### [M-CU04](#cu04) Service d'authentification
- L'ajout du mécanisme d'authentification doit pouvoir se faire en moins d'une heure pour chaque microservice.

### [M-CU05](#cu05) Notification administrateur
La modifiabilité est très importante étant donné la grande quantité d’informations qui peuvent être reçues par ce cas d’utilisation. Il faut donc prendre en considération qu’un grand nombre d’informations peut vite rendre un système extrêmement complexe à gérer et modifier selon les besoins du moment.

### [M-CU06](#cu06) Service externe
- L'intégration d'un nouveau système externe devrait se réaliser en moins de 1 heures.

### [M-CU07](#cu07) Partager comparaison trajet
- Le système doit pouvoir intégrer ce composant avec le moins de couplage possible.
  
### [M-CU08](#cu08) Favoris
s.o.

### [M-CU09](#cu09) Temps de trajet de STM
- Il devrait être facile de remplacer le site de la STM par le site d'un autre organisme de transport 
  
### [M-CU10](#cu10) Météo
- On doit être en mesure de remplacer le service Météo par un autre service en moins de 3h.

## Conception pilotée par les attributs

### [Réduire la taille des modules](#rdtq-réduire-la-taille-des-modules)
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [Augmenter la cohésion](#rdtq-augmenter-la-cohésion)
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [Réduire le couplage](#rdtq-réduire-le-couplage)
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [Defer binding](#rdtq-defer-binding)
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>
  
  
## Réalisation des tactiques de qualité

###  [RDTQ-Réduire la taille des modules](#réduire-la-taille-des-modules)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ-Augmenter la cohésion](#augmenter-la-cohésion)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ-Réduire le couplage](#réduire-le-couplage)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ-Defer binding](#defer-binding)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>




### Relation entre les éléments architecturale et les exigences
|Identifiant|Éléments|Description de la responsabilité|
|-----------|--------|-------------------------------|
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
