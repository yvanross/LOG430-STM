## Performance

### [D-OA01] Faciliter le recrutement des nouveaux chargés de laboratoire.
**Décrive comment vous pouvez satisfaire cette exigence avec cet attribut de qualité.**

### [D-OA02] Promouvoir l'utilisation des données ouvertes
**Décrive comment vous pouvez satisfaire cette exigence avec cet attribut de qualité.**

### Pour tous les Cu's
- Vous devez démontrer l'impact de la perturbation par le chaos monkey pour chacune des services externes.


### [P-CU01](#cu01) Comparaison de temps de trajet
Le système ne doit pas prendre beaucoup de temps pour afficher le résultat de la comparaison.
- Vous devez documenter la latence normale de chaque service et documenter la latence du service de comparaison.

### [P-CU02](#cu02) ChaosMonkey
Aucune perturbation de performance des microservices ne devrait être perceptible par l'usager.

### [P-CU03](#cu03) Impact écologique 
- Le service doit avoir un temps de réponse de moins de 1 seconde.

### [P-CU04](#cu04) Service d'authentification
- le service doit avoir un temps de réponse de moins de 1 seconde.

### [P-CU05](#cu05) Notification administrateur
La performance est importante dans ce cas d’utilisation puisqu’il va y avoir énormément d’informations qui vont être reçues et affichées pour les utilisateurs ayant le rôle d’administrateur.
- Vous devez calculer en temps réel la latence de chaque microservice et afficher celui-ci dans la console d'administration.
- Vous devriez être en mesure de faire fonctionner votre architecture avec et sans mécanisme de performance pour démontrer l'efficacité de ceux-ci.

### [P-CU06](#cu06) Service externe
- Vous devez démontrer la performance de chaque service externe.

### [P-CU07](#cu07) partager une comparaison de trajets
- Le service doit avoir un temps de réponse de moins de 3 secondes
  
### [P-CU08](#cu08) Favoris
- Le service doit supporter plusieurs requêtes simultanées.

### [P-CU09](#cu09) Temps de trajet de STM
- Le service doit avoir un temps de réponse de moins de 10 secondes
  
### [P-CU10](#cu10) Météo
- Le service de météo doit avoir un temps de réponse de moins de 1 seconde.

## Conception pilotée par les attributs
  
### [Contrôler la demande en ressources](#rdtq-contrôler-la-demande-en-ressources)
<div class="concept performance">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

### [Gérer les ressources](#rdtq-gérer-les-ressources)
<div class="concept performance">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez-vous choisie et pourquoi?</span>

# Réalisation des tactiques de qualité

### [RDTQ-Contrôler la demande en ressources](#contrôler-la-demande-en-ressources)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

### [RDTQ-Gérer les ressources](#gérer-les-ressources)
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
