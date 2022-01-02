# Attribut de qualité
## Disponibilité
Nous n’avons aucun contrôle sur le serveur de la ville de Montréal et toutes extrapolations des données manquantes de la part de votre application ne peuvent qu’être erronées.
- AQD-1. Vous devez implémenter un mécanisme de monitorage pour tous les microservices.
- AQD-2. Chaque microservice doit supporter le mode écho (echo).
- AQD-3. Chaque microservice doit supporter le mode heartbeat.
- AQD-4. Vous devez être en mesure de récupérer les données d'un fournisseur externe (ex: google Map, waze…, autre équipe) différent si celui que vous utilisez tombe en panne.
- AQD-5. Vous devez implémenter une redondance passive sur l'un de vos microservice
- ADQ-6. Vous devez implémenter une redondance active sur l'un de vos microservice.  
- AQD-7. Vous devez être en mesure de faire terminer chacun de vos microservice à l'aide d'une ligne de commande ou par le microservice ChaosMonkey. Ce microservice mettera à rude épreuve la disponibilité de votre système. 
- AQD-8. Vous devez pouvoir détruire le microservice LOG430ServiceDiscovery sans que cela n'ait d'impact sur la performance ou la disponibilité.

## Modifiabilité: 
- AQM1.  Vous devez être en mesure de modifier les intersections utilisées (ajouter ou retrait) seulement en modifiant le fichier de configuration (RouteConfigurationData)».  L’application des changements à votre solution ne devrait pas prendre plus de 15 secondes une fois le fichier de configuration complété.
- AQM2. Vous devez être en mesure d’adapter les requêtes de votre solution pour récupérer les données d’intersection d’une nouvelle équipe en moins d'une heure.
- AQM3. Vous devriez être en mesure de généraliser la redondance active ou passive pour n'importe quel composant en moins d'une heure. 

## Testabilité: 
- AQT-1. Vous devez être en mesure de modifier dynamiquement la latence des microservices de votre architecture pour pouvoir tester comment celui-ci réagit.
- AQT-2. Comparez les données de votre service externe à au moins un service externe implémenté par une autre équipe durant une période d'au moins une heure à l’aide d’un graphique Excel.
- AQT-3. Utiliser le microservice LOG430ServiceDiscovery pour récupérer la liste de vos microservices en opération et modifier de façon spécifique ou aléatoire la latence de ceux-ci. Démonter quel est l'impact des changements de latence avec des chiffres à l'appui.
- AQT-4. Proposer un scénario de testabilité avec les tactiques nécessaires pour tester votre système avec les  données provenant du simulateur. 
- AQT-5. Vous devriez pouvoir configurer la source de données à utiliser pour vos test. Soit <s>le serveur MQTT de la ville de Montréal ou</s> votre simulateur ou le simulateur d'une autre équipe.
- AQT-6. Assurez vous que nous puissions suivre à la trace tous les messages entrant dans les microservices par l'association d'un numéro unique à chaque message entrant.  Ceci devrait permettre de faire la trace de tous les microservices actifs ayant été utilisés pour traiter un message.

## Usabilité: 
- AQU2-1. Comme votre application ne possède pas d'interface usager, vous devez pouvoir changer  l'état d'un élément de votre système avec une ligne de commande pour simuler un crash ou une variation de la latence.

## Performance
- AQP-1. Vous devez être en mesure de configurer votre système pour utiliser les microservices ayant les plus faible latences pour réaliser le calcul du temps de trajet. Sont inclus les microservices développés par toutes les équipes de votre sous-groupe.
- AQP-2. Proposer un scénario de performance pour que votre système soit en mesure de supporter 1000 clients simultanément.   
  -  Démontrez mathématiquement que la solution proposée est réalisable. 
  - Vous devez documenter cette solution sans en faire l'implémentation.  

## Interopérabilité:
- AQI-1. Vous devez pouvoir utiliser les services fournis par les autres équipes du laboratoire uniquement en modifiant dynamiquement la configuration de votre système.

## Sécurité
- AQS-1. Vous devez mettre en place un mécanisme pour que votre service ne soit accessible que pour les équipes autorisées.
