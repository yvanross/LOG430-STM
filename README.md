![log](./doc/assets/logo-logti.png)

# LOG430 Architecture logicielle

# Objectif des laboratoires

L'objectif principal des 3 laboratoires du cours de LOG430 est de vous permettre de maitriser les concepts d'analyse et de conception d'une architecture logicielle d'un système complexe. Cette architecture sera développée sur la base d'une architecture de microservices. Le système fourni offre un service permettant de faire la comparaison de temps de trajet à partir de données sur les autobus de la STM et d'un service externe nommé TomTom fournissant des données sur les automobiles. Afin d'utiliser le service, vous fournirez les coordonnées de deux points à Montréal. Afin de trouver le temps du trajet en autobus, le système trouvera un autobus passant près de ces deux points et suivra en temps réel le temps d'un autobus de la STM passant entre ces coordonnées.

# Le contexte de l'organisation

Vous êtes nouvellement embauché par l'organisation LOG430STM pour améliorer l'architecture du comparateur de trajet.  La réussite de ce projet n'est pas optionnelle. La carrière des étudiants peut grandement être impactée s'ils échouent à ce cours. C'est pour cette raison que l'organisation a décidé de séparer les responsabilités selon les différentes parties prenantes.

# Parties prenantes du projet

## Chargé de laboratoire (client)

- Effectuera l'évaluation de l'architecture de chaque équipe (Documentation et Implémentation)
- Responsable de répondre aux questions des étudiants (durant les périodes de laboratoire seulement)
- Responsable d'aider les étudiants à maîtriser les concepts d'architecture
- Veux un rapport détaillé de l'architecture et des interfaces

## Contraintes de réalisation

### Services obligatoires

- TripComparator
- RouteTimeProvider
- STM

### Service Externes

- API de la STM
- API de TomTom

### Language

Nous n’imposons aucune contrainte au niveau du langage de développement utilisé à l'exception que celui-ci doit être de type **Orienté objet**.

## Directive de déploiement

Vous devez déployer vos solutions de chaque itération sur docker desktop sur votre propre ordinateur. (Exclusivement)

## Directive de Démonstration

- Vous aurez droit à 4 démonstrations maximum pour l'intégration et/ou l'implémentation.
- Chaque équipe disposera d'un maximum de 10 minutes par démonstration/exigence. (itération 1)
- Donc soyez bien préparé
  - Assurez-vous d'avoir testé votre architecture
  - Assurez-vous de ne pas faire des modifications de dernières minutes qui pourraient impacter votre démonstration
- L'objectif du système étant de comparer le temps d'un trajet en voiture et en autobus, il est essentiel de continuer d'offrir ce service. Le projet initial étant fonctionnel, on s'attend à ce que le projet fonctionne lors des démonstrations. L'incapacité à comparer le temps du trajet fera perdre des points.
- À chaque démonstration, le chargé de laboratoire peut vous demander de créer des tâches que vous devrez avoir satisfaites lors de la démonstration subséquente. Le non-respect de cette directive pourrait entrainer des pertes de points.

## Directive de remise

Vous devez mettre vos sources à jour dans la branche main, et ensuite vous générez un tag correspondant à l'itération ou vous faites votre remise.

Les rapports devront être également remis sur Moodle en version PDF.

| Semaine   |Tag                 | Rapport |
| --------: |:-------------------|------|
|         4 | git tag laboratoire-1 | Remise sur Moodle|
|       9 | git tag laboratoire-2 | Remise sur Moodle |
|        12 | git tag laboratoire-3 | Remise sur Moodle |

La date de chacune des remises peut être trouvée sur Moodle.

- Il pourra y avoir des directives supplémentaires pour la remise de la documentation de chaque itération. Veuiller suivre les indications de votre chargé de laboratoire.

# Évaluation par les pairs

À la fin de chaque laboratoire, les membres de l'équipe doivent réaliser leur propre évaluation par les pairs pour chacun des membres de l'équipe. Vous devez indiquer clairement dans votre rapport de laboratoire le résultat de cette évaluation. Les membres de l'équipe ont le droit de retirer l'un des membres de l'équipe, qui devra alors se trouver une autre équipe. Avant d'en arriver là, parlez-en avec votre chargé de laboratoire pour essayer de trouver une solution au problème!
