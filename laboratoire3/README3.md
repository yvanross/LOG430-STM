![log](../doc/assets/logo-logti.png)

# LOG430 Architecture logicielle Itération #3

## Objectifs
- (60%) Documenter l'architecture optimale en créant une architecture utilisant des tactiques de disponibilités et de performance.
- (40%) Implémenter l'architecture optimale en terme de disponibilité et de performance.

## Perturbation de l'infrastructure
- Utiliser le chaos monkey pour perturber l'infrastructure et évaluer l'impact sur la performance du système.  Utiliser la télémétrie pour documenter ces impacts.
  - Réalisation des tactiques de performance
  - ChaosMonkey détruit aléatoirement les conteneurs de calcul
  - ChaosMonkey s'attaque aux connecteurs (message queue, bus, etc.)
  - Chaosmonkey attaque à la limitation du nombre d'instruction par secondes (cpu)
  - ChaosMonkey s'attaque à la mémoire disponible pour les containers
  - ChaosMonkey s'attaque au Data store (BD, cache, etc.)

# Génération du rapport
Utiliser la commande suivante pour générer le PDF de la documentation avec l'outil [Pandoc](https://pandoc.org)
```bash
pandoc --verbose documentationArchitecture.md ../doc/vues-module.md ../doc/footer.md ../doc/vues-cetc.md ../doc/footer.md ../doc/vues-allocation.md ../doc/footer.md ../doc/telemetrie.md ../doc/footer.md  ../doc/cu01.md ../doc/footer.md ../doc/cu02.md ../doc/footer.md ../doc/cu05.md ../doc/footer.md ../doc/cu06.md ../doc/footer.md ../doc/cu09.md ../doc/footer.md ../doc/cu11.md ../doc/footer.md ../doc/AQ-disponibilite.md ../doc/footer.md ../doc/AQ-performance.md ../doc/footer.md -o documentationArchitecture.pdf && open documentationArchitecture.pdf
```
