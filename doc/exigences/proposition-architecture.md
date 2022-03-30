# Proposition d'architecture
Dans le but d'aider les équipes de coordonnateur à démarrer le projet le plus rapidement possible, je vous fournis une ébauche de solution que vous devez compléter. Celle-ci devrait vous permettre de planifier la distribution des tâches initiale du projet.  Vous devez apporter toutes les modifications nécessaires à cette proposition et l'utiliser comme point de départ.  

![Architecture](./architecture/proposition.svg)

### Légende
- Composant en vert:  Nouveau microservice pour cette itération.  cu05
- Composant en blues: Composant modifié dans le cadre de cette itération
- Composant en rouge: Composant détruit durant cette itération

## Composants
| Composant | Description |
| ---|--|
|ChargéDeLaboratoire|Doit pouvoir vérifier chacune des exigences client le plus rapidement possible|
|GpsAppSimulator|Simulation de l'application pour la transmission des coordonnées GPS|
|RouteComparatorApp|Microservice permettant d'afficher les courbes de comparaison de données|
| ChaosMonkey| Générateur de chaos qui peut modifier la latence ou détruire un microservice selon différents critères.  Doit aussi pouvoir détruire/faire arrêter/planter/terminer le processus de ServiceDiscovery pour que celui soit temporairement non disponible. Référence.: https://principlesofchaos.org |
|RouteComparatorService| Comparateur des calculs du temps de trajet entre deux coordonnées GPS. Le calcul se fait sur le tronçon de la rue Notre-Dame seulement. |
|ServerSwitch | Commutateur permettant de fournir les données de simulation ou les données en temps réel. Devrait aussi permettre de sélectionner un serveur de simulation particulier|
|PerturbateurDeTraffic|Élément perturbant le trafic pour voir l'impact sur le temps total de trajet. Ex.: Fermer une voie, modifier la vitesse, etc.
|ExternalService|Service externe permettant de calculer le temps de trajet entre deux points GPS|
|ServiceDiscovery| Microservice permettant d'enregistrer et de découvrir la liste des microservices actifs pour pouvoir obtenir leurs services | 
|ServerSimulation | Serveur simulant le serveur de la ville de Montréal et utilisant les données de simulation|
|TimeTravelCalculator</span> | Serveur permettant de faire le calcul des données de chacune des intersections pour permettre d'obtenir le temps de trajet entre deux points sur la rue Notre-Date</span>|
|TrafficMonitoring </span>| Microservice permettra d'afficher et d'archiver en temps réel les données de temps de trajet sous forme de graphique avec lignes multiples </span>|
