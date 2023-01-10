## Disponibilité

### Pour tous les CU's
- En cas de détection d'un défaut ou d'une défaillance, il doit être enregistré dans les fichiers journaux (logs).

### [D-CU01](#cu01) Comparaison de temps de trajet
- Si un service n'est pas disponible, il faut pouvoir quand même comparer le reste des services.
- Le service de la STM doit être disponible puisque c'est un élément central de ce projet. En effet, le premier cas d'utilisation a besoin du système de la STM pour pouvoir accomplir le but de l'utilisateur de comparer un temps de trajet de la STM avec un autre service externe.

### [D-CU02](#cu02) ChaosMonkey
- Le chaos monkey est le seul service qui ne nécessite pas de redondance

### [D-CU03](#cu03) Impact écologique 
- Vous devez utiliser plus d'un service externe pour obtenir l'information sur l'impact écologique du moyen de transport.
- Je vous suggère qu'au moins deux équipes implémentent ces services.
- Vous devriez documenter la différence au niveau des résultats de ces deux microservices.

### [D-CU04](#cu04) Service d'authentification
- Le service doit être disponible 99.95% du temps.
- vous devez accumuler les données et démonter la disponibilité de ce service externe.

### [D-CU05](#cu05) Notification administrateur
- Vous devez utiliser plus d'un service de notification pour maximiser la chance de rejoindre l'administrateur lorsqu'une anomalie survient.
- Vous devez calculer en temps réel le taux de disponibilité de chaque microservice et afficher celui-ci dans la console d'administration
  
### [D-CU06](#cu06) Service externe
- Si un service externe n'est pas disponible, le système peut décider de lui-même de remplacer celui-ci par un autre système externe.  L'interface usager et les logs doivent toutefois indiquer ce remplacement.

### [D-CU07](#cu07) partager une comparaison de trajets
- Cette fonctionnalité est non essentielle. Il faut simplement désactiver cette fonctionnalité de l'interface usager et informer celui-ci lorsque celle-ci redevient disponible.

### [D-CU08](#cu08)Favoris
- Le service doit être disponible 95% du temps.
- Le service doit avoir une capacité de stockage suffisante pour stocker les trajets favoris.

### [D-CU09](#cu09) Temps de trajet de STM
Votre système ne peut en aucun cas traiter la non-disponibilité du service de la STM.  Votre système devra alors fonctionner en mode dégradé pour quand même permettre la comparaison des temps de trajet des systèmes externes.
  
### [D-CU10](#cu10) Météo
- Le service doit être disponible 99.99% du temps.
- vous devez accumuler les données et démonter la disponibilité de ce service externe.

## Conception pilotée par les attributs

### [Détection de faute](#rdtq-détection-de-faute)
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [Préparation et réparation](#rdtq-préparation-et-réparation)
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [Réintroduction](#rdtq-réintroduction)
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [Prévention des fautes](#rdtq-prévention-des-fautes)  
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>


## Réalisation des tactiques de qualité

###  [RDTQ-Détection de faute](#détection-de-faute)
<span style="color:#FF0000">nom de la tactique</span>

<span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ-Préparation et réparation](#préparation-et-réparation)
  
  <span style="color:red">nom de la tactique</span>

  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ-Réintroduction](#réintroduction)

  <span style="color:red">nom de la tactique</span>
   
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>
  
### [RDTQ-Prévention des fautes](#prévention-des-fautes) 
  <span style="color:red">nom de la tactique</span>

  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

## Relation entre les éléments architecturale et les exigences de disponibilité
 |Identifiant|Éléments|Description de la responsabilité|
 |-----------|--------|-------------------------------|
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

