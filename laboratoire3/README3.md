![log](../doc/assets/logo-logti.png)

# LOG430 Architecture logicielle Itération #3

## Objectifs
- (60%) Documenter l'architecture optimale en créant une architecture utilisant des tactiques de disponibilité, de performance et de modifiabilité.
- (40%) Implémenter l'architecture optimale en termes de disponibilité et de performance.

## Perturbation de l'infrastructure
- Utiliser le chaos monkey pour perturber l'infrastructure et évaluer l'impact sur la performance du système.  Utiliser la télémétrie pour documenter ces impacts.
  - Réalisation des tactiques de performance
  - ChaosMonkey détruit aléatoirement les conteneurs de calcul
  - ChaosMonkey s'attaque aux connecteurs (message queue, bus, etc.)
  - Chaosmonkey attaque à la limitation du nombre d'instruction par seconde (cpu)
  - ChaosMonkey s'attaque à la mémoire disponible pour les conteneurs
  - ChaosMonkey s'attaque aux volumes de conteneurs de calcul
