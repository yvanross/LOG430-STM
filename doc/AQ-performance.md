## [Performance](#da-performance)

### [OA01 Faciliter le recrutement des nouveaux chargés de laboratoire](#oa01) {#p-oa01}
- Décrivez comment vous pouvez satisfaire cette exigence avec cet attribut de qualité.

### [OA02 Promouvoir l'utilisation des données ouvertes](#oa02) {#p-oa02}
- Décrive comment vous pouvez satisfaire cette exigence avec cet attribut de qualité.

### Pour tous les Cu's
- Vous devez démontrer l'impact de la perturbation par le chaos monkey pour chacune des services externes.


### [CU01 Comparaison de temps de trajet](#cu01) {#p-cu01}
- Le système ne doit pas prendre beaucoup de temps pour afficher le résultat de la comparaison.
- Vous devez documenter la latence normale de chaque service et documenter la latence du service de comparaison.

### [CU02 ChaosMonkey](#cu02) {#p-cu02}
- Aucune perturbation de performance des microservices ne devrait être perceptible par l'usager.

### [CU03 Impact écologique](#cu03) {#p-cu03}
- Le service doit avoir un temps de réponse de moins de 1 seconde.

### [CU04 Service d'authentification](#cu04) {#p-cu04}
- le service doit avoir un temps de réponse de moins de 1 seconde.

### [CU05 Notification administrateur](#cu05) {#p-cu05}
- La performance est importante dans ce cas d’utilisation puisqu’il va y avoir énormément d’informations qui vont être reçues et affichées pour les utilisateurs ayant le rôle d’administrateur.
- Vous devez calculer en temps réel la latence de chaque microservice et afficher celui-ci dans la console d'administration.
- Vous devriez être en mesure de faire fonctionner votre architecture avec et sans mécanisme de performance pour démontrer l'efficacité de ceux-ci.

### [CU06 Service externe](#cu06) {#p-cu06}
- Vous devez démontrer la performance de chaque service externe.

### [CU07 partager une comparaison de trajets](#cu07)  {#p-cu07}
- Le service doit avoir un temps de réponse de moins de 3 secondes
  
### [CU08 Favoris](#cu08) {#p-cu08}
- Le service doit supporter plusieurs requêtes simultanées.

### [CU09 Temps de trajet de STM](#cu09) {#p-cu09}
- Le service doit avoir un temps de réponse de moins de 10 secondes
  
### [CU10 Météo](#cu10) {#p-cu10}
- Le service de météo doit avoir un temps de réponse de moins de 1 seconde.

## Conception pilotée par les attributs
  
### [(SC) Contrôler la demande en ressources](#rdtq-contrôler-la-demande-en-ressources)
<div class="concept performance">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [(SC) Gérer les ressources](#rdtq-gérer-les-ressources)
<div class="concept performance">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

# Réalisation des tactiques de qualité

### [RDTQ Contrôler la demande en ressources](#sc-contrôler-la-demande-en-ressources)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ Gérer les ressources](#sc-gérer-les-ressources)
  <span style="color:red">nom de la tactique</span>

  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>





### Relation entre les éléments architecturale et les exigences

|Identifiant|Éléments|Description de la responsabilité|
|-----------|--------|-------------------------------|
|[P-CU01](#p-cu01) | |
|[P-CU02](#p-cu02) | |
|[P-CU03](#p-cu03) | |
|[P-CU04](#p-cu04) | |
|[P-CU05](#p-cu05) | |
|[P-CU06](#p-cu06) | |
|[P-CU07](#p-cu07) | |
|[P-CU08](#p-cu08) | |
|[P-CU09](#p-cu09) | |
|[P-CU10](#p-cu10) | |
