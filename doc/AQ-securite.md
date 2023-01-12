## [Sécurité](#da-securite)

### [OA01 Faciliter le recrutement des nouveaux chargés de laboratoire](#oa01){#s-oa01}
- Décrivez comment vous pouvez satisfaire cette exigence avec cet attribut de qualité.

### [OA02 Promouvoir l'utilisation des données ouvertes](#oa02){#s-oa02}
- Décrivez comment vous pouvez satisfaire cette exigence avec cet attribut de qualité.**


### Pour tous les Cu's
- Vos microservices doivent implémenter une méchanique d'authentification


### [CU01 Comparaison de temps de trajet](#cu01){#s-cu01}
- L'étudiant doit être authentifié

### [CU02 ChaosMonkey](#cu02){#s-cu02}
- Seul les administrateurs devraient avoir accès à ce service.  On ne veut pas que des gens malveillants aient accès à ce microservice. En effet, nous ne voulons pas des gens qui s'amusent à arrêter ou perturber le fonctionnement de nos services.

### [CU03 Impact écologique ](#cu03){#s-cu03}
- Ce CU ne peut qu'être exécuté par un utilisateur authentifié et autorisé.

### [CU04 Service d'authentification](#cu04){#s-cu04}
- Authentifier les acteurs: 
  - Le service ne peut être utilisé que par des utilisateurs authentifiés et autorisés.
  -  Le service doit implémenter l'approche Single Sign-On (SSO).
  - Le service d'authentification est séparé des autres modules de l'application.
- Encrypter les données:
  -  Le service doit implémenter une encryption des mots de passe.

### [CU05 Notification administrateur](#cu05){#s-cu05}
- L’attribut de qualité de sécurité devient essentiel pour s’assurer que seuls les utilisateurs ayant le rôle d’administrateur peuvent accéder à toutes ses informations des utilisateurs. En effet, ce cas est principalement utile pour des développeurs ou des personnes devant gérer le système. C’est pour cette raison qu’on doit s’assurer que seulement les personnes autorisées ont accès à ses informations pouvant être confidentielles.

### [CU06 Service externe](#cu06){#s-cu06}
- Vos services externes doivent implémenter une méchanique d'authentification

### [CU07 partager une comparaison de trajets](#cu07){#s-cu07}
- Ce CU ne peut qu'être exécuté par un utilisateur authentifié et autorisé.
- Un utilisateur non authentifié doit pouvoir accéder uniquement aux informations partagées.

### [CU08 Favoris](#cu08){#s-cu08}
- Le service ne peut être utilisé que par des utilisateurs authentifiés et autorisés.

### [CU09 Temps de trajet de STM](#cu09){#s-cu09}
- Votre microservice doit implémenter une mécanique d'authentification
  
### [CU10 Météo](#cu10){#s-cu10}
- L'utilisateur doit être authentifié pour consulter la météo.

## Conception pilotée par les attributs

### [(SC) Détecter les attaques](#rdtq-détecter-leattaques)
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [(SC) Résister aux attaques](#rdtq-résister-aux-attaques)
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>


### [(SC) Réagir aux attaques](#rdtq-réagir-aux-attaques)
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [(SC) Récupéré d'une attaque](#rdtq-récupérer-dune-attaque)
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

# Réalisation des tactiques de qualité

### RDTQ [Détecter les attaques](#détecter-leattaques)
   <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ Résister aux attaques](#sc-résister-aux-attaques)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ Réagir aux attaques](#sc-réagir-aux-attaques)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ Récupérer d'une attaque](#sc-récupérer-dune-attaque)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

 
### Relation entre les éléments architecturale et les exigences de sécurité

|Identifiant|Éléments|Description de la responsabilité|
|-----------|--------|-------------------------------|
|[S-OA01](#s-oa01) | |
|[S-OA02](#s-oa02) | |
|[S-CU01](#s-cu01) | |
|[S-CU02](#s-cu02) | |
|[S-CU03](#s-cu03) | |
|[S-CU04](#s-cu04) | |
|[S-CU05](#s-cu05) | |
|[S-CU06](#s-cu06) | |
|[S-CU07](#s-cu07) | |
|[S-CU08](#s-cu08) | |
|[S-CU09](#s-cu09) | |
|[S-CU10](#s-cu10) | | 
