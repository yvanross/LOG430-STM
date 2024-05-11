![log](../doc/assets/logo-logti.png)

# LOG430 Architecture logicielle Itération #2

## Objectifs
- (60%) Documenter l'architecture optimale en créant une architecture utilisant des tactiques de disponibilités et de performance.
- (40%) Implémenter l'architecture optimale en termes de disponibilité et de performance.
  - Redondance active ou redondance passive ou autre tactiques de votre choix

## Perturbation de l'infrastructure
- Utiliser le chaos monkey pour perturber l'infrastructure et évaluer l'impact sur la disponibilité du système.  Utiliser la télémétrie pour documenter ces impacts.
  - ChaosMonkey détruit aléatoirement les conteneurs de calcul
  - Chaosmonkey attaque à la limitation du nombre d'instruction par seconde
  - ChaosMonkey s'attaque à la mémoire disponible pour les conteneurs
