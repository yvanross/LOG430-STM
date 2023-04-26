# Guide de correction de l'itération 1

Voici ce que les étudiants devrait trouver lors de l'évaluation de l'architecture du système.

## Télémétrie
  - disponibilité
    - Temps de démarrage 
    - Temps de récupération
    - Temps de disponibilté
  - performance
    - Latence
    - Est-ce que votre configuration permet de support multiple copy of computation de chacun des composants (non Statefull)
  - Interopérabilité
    - Connection entre les services dynamique ou statique 
    - Comment le routing entre les composant fonctionnent (service discovery)
    - Est-ce que les adresse sont hardcod
  - Modifiabilité
    - Injection de dépendance
  - Autre
    - Tous les microservices sont de type Statefull comparativement a un type stateless, donc ils ne peuvent pas être répliqué
  
## Documenter la responsabilité détaillé de chaque composant du système
  - 4 composants
  - 1 connecteur - RabbitMQ
  - 3 composant de télémétrie

## Proposer des solution pour améliorer l'architecture du système
  - disponibilité
    - STM - statefull - réplication impossible
      - Stateless - Séparer en 3 microservices: Trouver le meilleur autobus, tracking des autobus, Mettre l'info dans la BD et utiliser un service de cache pour les update rapide
      - Update de la BD 
      - réplication de la BD
      - Temps de démarrage de la STM
  - performance   
    - Trip comparator (call au 5ms vers la STM)
    - Service de cache interne de la STM (multiple copy of data) -- sortir la cache pour la rentre stateless
  - interopérabilité
    - Utilisation des variables d'environnement pour les adresse des services
    - Utilisation de docker-compose pour le routing entre les services
    - Utilisation de docker-compose pour le service discovery

  

