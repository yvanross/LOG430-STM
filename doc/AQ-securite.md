## Sécurité

### [D-OA01] Faciliter le recrutement des nouveaux chargés de laboratoire.
**Décrive comment vous pouvez satisfaire cette exigence avec cet attribut de qualité.**

### [D-OA02] Promouvoir l'utilisation des données ouvertes
**Décrive comment vous pouvez satisfaire cette exigence avec cet attribut de qualité.**


### Pour tous les Cu's
- Vos microservices doivent implémenter une méchanique d'authentification


### [S-CU01](#cu01) Comparaison de temps de trajet
L'étudiant doit être authentifié

### [S-CU02](#cu02) ChaosMonkey
- Seul les administrateurs devraient avoir accès à ce service.  On ne veut pas que des gens malveillants aient accès à ce microservice. En effet, nous ne voulons pas des gens qui s'amusent à arrêter ou perturber le fonctionnement de nos services.

### [S-CU03](#cu03) Impact écologique 
- Ce CU ne peut qu'être exécuté par un utilisateur authentifié et autorisé.

### [S-CU04](#cu04) Service d'authentification
- 1. Authentifier les acteurs: 
      - Le service ne peut être utilisé que par des utilisateurs authentifiés et autorisés.
      -  Le service doit implémenter l'approche Single Sign-On (SSO).
      - Le service d'authentification est séparé des autres modules de l'application.
- 2. Encrypter les données:
      -  Le service doit implémenter une encryption des mots de passe.

### [S-CU05](#cu05) Notification administrateur
- L’attribut de qualité de sécurité devient essentiel pour s’assurer que seuls les utilisateurs ayant le rôle d’administrateur peuvent accéder à toutes ses informations des utilisateurs. En effet, ce cas est principalement utile pour des développeurs ou des personnes devant gérer le système. C’est pour cette raison qu’on doit s’assurer que seulement les personnes autorisées ont accès à ses informations pouvant être confidentielles.

### [S-CU06](#cu06) Service externe
- Vos services externes doivent implémenter une méchanique d'authentification

### [S-CU07](#cu07) partager une comparaison de trajets
- Ce CU ne peut qu'être exécuté par un utilisateur authentifié et autorisé.
- Un utilisateur non authentifié doit pouvoir accéder uniquement aux informations partagées.

### [S-CU08](#cu08) Favoris
- Le service ne peut être utilisé que par des utilisateurs authentifiés et autorisés.

### [S-CU09](#cu09) Temps de trajet de STM
- Votre microservice doit implémenter une mécanique d'authentification
  
### [S-CU10](#cu10) Météo
- L'utilisateur doit être authentifié pour consulter la météo.

## Conception pilotée par les attributs

### [Détecter les attaques](#rdtq-détecter-les-attaques)
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [Résister aux attaques](#rdtq-résister-aux-attaques)
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>


### [Réagir aux attaques](#rdtq-réagir-aux-attaques)
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [Récupéré d'une attaque](#rdtq-récupérer-dune-attaque)
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

# Réalisation des tactiques de qualité

### RDTQ-[Détecter les attaques](#détecter-les-attaques)
   <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ-Résister aux attaques](#résister-aux-attaques)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ-Réagir aux attaques](#réagir-aux-attaques)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ-Récupérer d'une attaque](#récupérer-dune-attaque)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

 
### Relation entre les éléments architecturale et les exigences de sécurité

|Identifiant|Éléments|Description de la responsabilité|
|-----------|--------|-------------------------------|
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
