## [Disponibilité](#da-disponibilite)

### Pour tous les CU's
- En cas de détection d'un défaut ou d'une défaillance, il doit être enregistré dans les fichiers journaux (logs).

### [OA01 Faciliter le recrutement des nouveaux chargés de laboratoire](#oa01) {#d-oa01}
- Décrivez comment vous pouvez satisfaire cette exigence avec cet attribut de qualité.

### [OA02 Promouvoir l'utilisation des données ouvertes](#oa02) {#d-oa02}
- Décrivez comment vous pouvez satisfaire cette exigence avec cet attribut de qualité.

### [CU01 Comparaison de temps de trajet](#cu01) {#d-cu01}
- Si un service n'est pas disponible, il faut pouvoir quand même comparer le reste des services.
- Le service de la STM doit être disponible puisque c'est un élément central de ce projet. En effet, le premier cas d'utilisation a besoin du système de la STM pour pouvoir accomplir le but de l'utilisateur de comparer un temps de trajet de la STM avec un autre service externe.

### [CU02 ChaosMonkey](#cu02) {#d-cu02}
- Le chaos monkey est le seul service qui ne nécessite pas de redondance

### [CU03 Impact écologique](#cu03) {#d-cu03}
- Vous devez utiliser plus d'un service externe pour obtenir l'information sur l'impact écologique du moyen de transport.
- Je vous suggère qu'au moins deux équipes implémentent ces services.
- Vous devriez documenter la différence au niveau des résultats de ces deux microservices.

### [CU04 Service d'authentification](#cu04) {#d-cu04}
- Le service doit être disponible 99.95% du temps.
- vous devez accumuler les données et démonter la disponibilité de ce service externe.

### [CU05 Notification administrateur](#cu05) {#d-cu05}
- Vous devez utiliser plus d'un service de notification pour maximiser la chance de rejoindre l'administrateur lorsqu'une anomalie survient.
- Vous devez calculer en temps réel le taux de disponibilité de chaque microservice et afficher celui-ci dans la console d'administration
  
### [CU06 Service externe](#cu06) {#d-cu06}
- Si un service externe n'est pas disponible, le système peut décider de lui-même de remplacer celui-ci par un autre système externe.  L'interface usager et les logs doivent toutefois indiquer ce remplacement.

### [CU07 partager une comparaison de trajets](#cu07) {#d-cu07}
- Cette fonctionnalité est non essentielle. Il faut simplement désactiver cette fonctionnalité de l'interface usager et informer celui-ci lorsque celle-ci redevient disponible.

### [CU08 Favoris](#cu08) {#d-cu08}
- Le service doit être disponible 95% du temps.
- Le service doit avoir une capacité de stockage suffisante pour stocker les trajets favoris.

### [CU09 Temps de trajet de STM](#cu09) {#d-cu09}
- Votre système ne peut en aucun cas traiter la non-disponibilité du service de la STM.  Votre système devra alors fonctionner en mode dégradé pour quand même permettre la comparaison des temps de trajet des systèmes externes.
  
### [CU10 Météo0](#cu10) {#d-cu10}
- Le service doit être disponible 99.99% du temps.
- vous devez accumuler les données et démonter la disponibilité de ce service externe.

## Conception pilotée par les attributs

### [(SC) Détection de faute](#rdtq-détection-de-faute)
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [(SC) Préparation et réparation](#rdtq-préparation-et-réparation)
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [(SC) Réintroduction](#rdtq-réintroduction)
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [(SC) Prévention des fautes](#rdtq-prévention-des-fautes)  
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>


## Réalisation des tactiques de qualité

###  [RDTQ Détection de faute](#sc-détection-de-faute)
<span style="color:#FF0000">nom de la tactique</span>

<span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ Préparation et réparation](#sc-préparation-et-réparation)
  
  <span style="color:red">nom de la tactique</span>

  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ Réintroduction](#sc-réintroduction)

  <span style="color:red">nom de la tactique</span>
   
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>
  
### [RDTQ Prévention des fautes](#sc-prévention-des-fautes) 
  <span style="color:red">nom de la tactique</span>

  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

## Relation entre les éléments architecturale et les exigences de disponibilité
 |Identifiant|Éléments|Description de la responsabilité|
 |-----------|--------|-------------------------------|
 |[D-OA01](#d-oa01) | |
 |[D-OA02](#d-oa02) | |
 |[D-CU01](#d-cu01) | |
 |[D-CU02](#d-cu02) | |
 |[D-CU03](#d-cu03) | |
 |[D-CU04](#d-cu04) | |
 |[D-CU05](#d-cu05) | |
 |[D-CU06](#d-cu06) | |
 |[D-CU07](#d-cu07) | |
 |[D-CU08](#d-cu08) | |
 |[D-CU09](#d-cu09) | |
 |[D-CU10](#d-cu10) | |

