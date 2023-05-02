![log](../doc/assets/logo-logti.png)

# LOG430 Architecture logicielle Itération #3

## Objectifs
- (60%) Évaluer et documenter l'impact de trois archictectures différentes sur la performance du système.
  - Maintain multiple copie of computation
  - Maintain multiple copie of data
  - Limit event response
- (40%) Implémenter la meilleur des trois architectures en terme de performance. 
  

## Configuration de votre environnement
- Le changement de configuration entre vos différents architecture doit se faire en quelques secondes lors du déploiement.

## Perturbation de l'infrastructure
- Utiliser le chaos monkey pour perturber l'infrastructure et évaluer l'impact sur la performance du système.  Utiliser la télémétrie pour documenter ces impacts.

  - Réalisation des tactiques de performance
  - ChaosMonkey détruit aléatoirement les conteneurs de calcul
  - ChaosMonkey s'attaque aux connecteurs (message queue, bus, etc.)
  - Chaosmonkey attaque à la limitation du nombre d'instruction par secondes (cpu)
  - ChaosMonkey s'attaque à la mémoire disponible pour les containers

# Génération du rapport
Utiliser la commande suivante pour générer le PDF de la documentation avec l'outil [Pandoc](https://pandoc.org)
```bash
pandoc --verbose documentationArchitecture.md ../doc/vues-module.md ../doc/footer.md ../doc/vues-cetc.md ../doc/footer.md ../doc/vues-allocation.md ../doc/footer.md ../doc/telemetrie.md ../doc/footer.md  ../doc/cu01.md ../doc/footer.md  ../doc/cu05.md ../doc/footer.md ../doc/cu06.md ../doc/footer.md ../doc/cu09.md ../doc/footer.md ../doc/cu11.md ../doc/footer.md ../doc/AQ-performance.md  ../doc/footer.md  -o documentationArchitecture.pdf && open documentationArchitecture.pdf
```
