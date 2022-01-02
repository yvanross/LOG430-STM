  ## Architecture du comparateur de trajet

**Proposition** de l'architecture de départ

![Architecture](../architecture/proposition.svg)


### Légende
- Composant en vert: Nouveau microservice pour cet itération.  
- Composant en blue: Composant modifié dans le cadre de cette itération
- Composant en rouge: Composant détruit durant cette itération
  

## Composants
| Composant | Description |
| ---|--|
|TesteurSystem|  |
|ChargéDeLaboratoire| |
| GpsMobileComparatorApp | Application mobile qui transmet en temps réel ses coordonnées GPS et qui récupére le temps de trajet restant |
|GpsMobileComparatorAppSimulator|Simulation de l'application mobile pour la transmission des coordonnées GPS|
|RouteComparatorApp|Microservice permettant d'afficher les courbes de comparaison de données|
| ChaosMonkey| Générateur de chaos qui peut modifier la latence ou détruire un microservice selon différents critères.  Doit aussi pouvoir détruire/faire arrêter/planter/terminer le processus ServiceDiscovery pour que celui soit temporairement non disponible. Référence.: https://principlesofchaos.org |
|RouteComparatorService| Comparateur des calcul du temps de trajet entre deux coordonnées GPS. Le calcul se fait sur le tronçon de la rue Notre-Dame seulement. |
|MqttServerSwitch | Commutateur permettant de fournir les données de simulation ou le données temps réel. Devrait aussi permettre de sélectionner un serveur de simulation particulier|
|PerturbateurDeTraffic|Élément perturbant le traffic pour voir l'impact sur le temps total de trajet. ex: Fermer une voie, modifier la vitesse, etc...
|ExternalService|Service externe permettant de calculer le temps de trajet entre deux points GPS|
|ServiceDiscovery| Microservice permettant de d'enregistrer et de découvrir la liste des microservices actifs pour pouvoir obtenir leurs services | 
|MqttServerSimulation | Serveur MQTT simulant le serveur de la ville de Montréal et utilisant les données de simulation|
|MqttServerVilleDeMontreal | Serveur permettant d'obtenir les données des capteurs de la ville de Montréal|

