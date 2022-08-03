![log](doc/exigences/assets/logo-logti.png)
# LOG430 Architecture logicielle
- [LOG430 Architecture logicielle](#log430-architecture-logicielle)
- [Objectif principal](#objectif-principal)
- [Le contexte de l'organisation](#le-contexte-de-lorganisation)
- [Le comparateur de trajet](#le-comparateur-de-trajet)
- [Services externes](#services-externes)
  - [Exemples](#exemples)
  - [Fournisseurs potentiels](#fournisseurs-potentiels)
- [Parties prenantes du projet](#parties-prenantes-du-projet)
  - [Chargé de laboratoire (client)](#chargé-de-laboratoire-client)
  - [Classe](#classe)
  - [Groupe](#groupe)
  - [Équipe de coordination](#équipe-de-coordination)
  - [Équipe (Étudiants)](#équipe-étudiants)
- [Exigences](#exigences)
  - [Objectifs d'affaires](#objectifs-daffaires)
    - [OA-1. Rendre le laboratoire intéressant pour les étudiants.](#oa-1-rendre-le-laboratoire-intéressant-pour-les-étudiants)
    - [OA-2. Permettre aux étudiants de constater l'importance de l'architecture logicielle.](#oa-2-permettre-aux-étudiants-de-constater-limportance-de-larchitecture-logicielle)
    - [OA-3. Faciliter le recrutement des nouveaux chargés de laboratoire.](#oa-3-faciliter-le-recrutement-des-nouveaux-chargés-de-laboratoire)
    - [OA-4. Faciliter le processus de correction des laboratoires.](#oa-4-faciliter-le-processus-de-correction-des-laboratoires)
    - [OA-5. Arrimer le laboratoire (pratique) et le cours (théorique)](#oa-5-arrimer-le-laboratoire-pratique-et-le-cours-théorique)
    - [OA-6. Faire expérimenter le travail d'architecte logiciel aux étudiants.](#oa-6-faire-expérimenter-le-travail-darchitecte-logiciel-aux-étudiants)
    - [OA-7. Validez si le transport par autobus est toujours plus rapide, peu importe l'heure de la journée.](#oa-7-validez-si-le-transport-par-autobus-est-toujours-plus-rapide-peu-importe-lheure-de-la-journée)
    - [OA-8. Démontrer aux gestionnaires de la STM que votre solution est robuste.](#oa-8-démontrer-aux-gestionnaires-de-la-stm-que-votre-solution-est-robuste)
    - [OA-9. Démontrer le niveau de performance de votre solution.](#oa-9-démontrer-le-niveau-de-performance-de-votre-solution)
    - [OA-10. Propostion objectif affaire #1.](#oa-10-propostion-objectif-affaire-1)
    - [OA-11. Propostion objectif affaire #2.](#oa-11-propostion-objectif-affaire-2)
  - [Exigences fonctionnelles](#exigences-fonctionnelles)
    - [CU01. Veux comparer les temps de trajet](#cu01-veux-comparer-les-temps-de-trajet)
    - [CU02. Veux pouvoir mettre le chaos dans les services en mode.](#cu02-veux-pouvoir-mettre-le-chaos-dans-les-services-en-mode)
    - [CU03 - Nouveau cas d'utlisation](#cu03---nouveau-cas-dutlisation)
    - [CU04 - Nouveau cas d'utilisation](#cu04---nouveau-cas-dutilisation)
  - [Exigences de qualité](#exigences-de-qualité)
    - [(A) Disponibilité](#a-disponibilité)
    - [(A) Modifiabilité](#a-modifiabilité)
    - [(A) Performance](#a-performance)
    - [(A) Sécurité](#a-sécurité)
    - [(A) Testabilité](#a-testabilité)
    - [(A) Convivialité](#a-convivialité)
    - [(A) Interopérabilité](#a-interopérabilité)
  - [Contraintes de réalisation](#contraintes-de-réalisation)
    - [Language¶](#language)
    - [Réalisation](#réalisation)
    - [Contrainte d'équipe](#contrainte-déquipe)
- [Grille de pointage](#grille-de-pointage)
- [Directives pour chaque itération](#directives-pour-chaque-itération)
  - [Directive de planification des itérations](#directive-de-planification-des-itérations)
  - [Directire de documentation de l'architecture](#directire-de-documentation-de-larchitecture)
    - [Suggestion d'outils pour la documentation](#suggestion-doutils-pour-la-documentation)
      - [notion.so](#notionso)
      - [confluence](#confluence)
      - [Google doc](#google-doc)
      - [markdown](#markdown)
      - [structurizer](#structurizer)
      - [Eclipse Papyrus](#eclipse-papyrus)
  - [Directive d'implémentation](#directive-dimplémentation)
  - [Directive de déploiement](#directive-de-déploiement)
  - [Directive de Démonstration](#directive-de-démonstration)
  - [Directives de vérification avant la Remise](#directives-de-vérification-avant-la-remise)
  - [Directives pour la remise](#directives-pour-la-remise)
- [Parasites et molassons](#parasites-et-molassons)
- [Bonus projet laboratoire (5% point bonus)](#bonus-projet-laboratoire-5-point-bonus)

# Objectif principal
L'objectif principal de ce projet de cours est de proposer un système de comparaison de temps de trajet basée une architecture de microservices. Ce système permettra de comparer les temps de trajets entre les autobus et les automobiles.  Il sera réalisé au travers des trois (3) laboratoires du cours (36 heures au total). Les spécifications des exigences portent sur l'analyse, la conception et l'implantation de ce systême. Ce projet sera réalisé selon trois livrables, un par laboratoire. Le premier livrable permettra de générer les scénarios de qualité, de les prioriser et de proposer l'architecture initiale du projet. Le deuxième livrable consistera à réaliser le logiciel selon l'architecture réaliser par un groupe de coordonnateur. Le troisième livrable permettra de compléter l'implémentation et la documentation de l'architecture et de réaliser une (révision par les pairs) ATAM sur celle-ci.



# Le contexte de l'organisation
Vous êtes nouvellement embauché par l'organisation LOG430STM pour développer le système de comparateur de trajet.  La réussite de ce projet n'est pas optionnelle. La carrière des étudiants peut grandement être impactée s'ils échouent ce cours. C'est pour cette raison que l'organisation a décidé de séparer les responsabilités selon les différentes [parties prenantes](doc/exigences/partiePrenantes.md)


# Le comparateur de trajet
Vous devez développer l'architecture d'un système de compaison de temps de trajet en utilisant les données temps réel de la STM disponible sur le site suivant: https://www.stm.info/fr/a-propos/developpeurs et les donnés fournie par des [service externes](#services-externes)

# Services externes
Utiliser l'API ou la page web des fournisseurs externe pour l'estimation en temps réel du temps nécessaire pour parcourir la distance entre deux points sur le boulevard Notre-Dame
## Exemples
- https://www.google.ca/maps/dir/45.58927,-73.50912/45.51577,+-73.55196/@45.5541942,-73.5686356,13z/am=t/data=!3m1!4b1!4m7!4m6!1m0!1m3!2m2!1d-73.55196!2d45.51577!3e0
- https://www.google.ca/maps/dir/45.51433,+-73.55014/45.58927,+-73.50912/@45.5519338,-73.5641068,13z/data=!3m1!4b1!4m10!4m9!1m3!2m2!1d-73.55014!2d45.51433!1m3!2m2!1d-73.50912!2d45.58927!3e0

## Fournisseurs potentiels
- google map
- https://www.viamichelin.com/web/Routes
- https://en.mappy.com/itineraire
- https://ca.bonnesroutes.com
- Waze
- BingMaps 
- en.mappy.com
- Ajouter vos suggestions…

# Parties prenantes du projet
## Chargé de laboratoire (client)
- Effectuera l'évaluation de l'architecture de chaque équipe
- Responsable de répondre aux questions des étudiants
- Responsable d'aider les étudiants à maîtriser les concepts d'architecture
- Veux un rapport détaillé de l'architecture et des interfaces

## Classe
- Chaque classe est séparée en deux groupes

## Groupe
- Chaque groupe est séparée en équipe de 5 étudiants.
- Le chargé de cours crée les groupes.

## Équipe de coordination
- L'équipe à la responsabilité de définir les exigences qui seront utilisées par les équipes d'étudiant pour réaliser le système.
- L'équipe a la responsabilité de valider et de diffuser la documentation des interfaces touchant aux composants implémentés/utilisés par plusieurs équipes. 
  - Une version d'interface publiée ne peut pas être changée. Vous devez obligatoirement publier une nouvelle version.
- L'équipe de coordination peut démettre de ses fonctions un étudiant qui ne répond pas à ses attentes. 
  - L'équipe affectée devra nommer un nouveau représentant
- Les équipes de coordination ne doivent pas travailler ensemble ils sont des compétiteurs
- L'équipe de coordination est aussi responsable de répartir équitablement les tâches de réalisation de la conception et l'implémentation des différents microservices nécessaire à ce projet.
  - Dois conserver une trace écrite pour savoir quelle équipe implémente quel microservice.

## Équipe (Étudiants)
- **L'équipe doit concevoir et réaliser une architecture qui satisfait toutes les [exigences](#exigences) documentées dans ce document de spécification.**
- **L'équipe doit utiliser les microservices implémentés par les autres équipes mais n'obtiendra que la moitié des points associé aux exigences réalisé par ceux-ci**. Voir la [grille de pointage](#grille-de-pointage) pour plus d'information.
- **L'équipe ne peut implémenté plus de 75% des microservices de son architecture. Elle doit nécessairement utiliser au moins 25% des microservices provenant de d'autres équipes.**
- Le chargé de cours crée les équipes de laboratoires.
- Un étudiant par équipe est nommé pour faire partie de l'équipe de coordination 
  - Une équipe peut révoquer son représentant de l'équipe de coordination s'il ne répond pas à leurs attentes 
- Doit conserver une traçabilité de quel étudiant est responsable de quelles tâches
- Dois connaître en tout temps l'état d'une tâche assignée à un étudiant
- Ont la responsabilité de concevoir/documenter et diffuser (à l'équipe de coordination) la documentation des interfaces des microservices qui leur ont été assignés.
- Ont la responsabilité d'implémenter les microservices
- Ont la responsabilité de tester leur implémentation
- Ont la responsabilité d'intégrer leurs microservices avec les microservices des autres équipes pour obtenir une application permettant de satisfaire aux exigences clients.
- Chaque étudiant est conjointement et solidairement responsable des livrables.¶
- Ils ont la responsabilité de faire l'intégration de tous les microservices pour réaliser l'application qui satisfait aux exigences client.


# Exigences
## Objectifs d'affaires

### OA-1. Rendre le laboratoire intéressant pour les étudiants.
### OA-2. Permettre aux étudiants de constater l'importance de l'architecture logicielle.
### OA-3. Faciliter le recrutement des nouveaux chargés de laboratoire.
### OA-4. Faciliter le processus de correction des laboratoires.
### OA-5. Arrimer le laboratoire (pratique) et le cours (théorique)
### OA-6. Faire expérimenter le travail d'architecte logiciel aux étudiants.
### OA-7. Validez si le transport par autobus est toujours plus rapide, peu importe l'heure de la journée.
### OA-8. Démontrer aux gestionnaires de la STM que votre solution est robuste.
### OA-9. Démontrer le niveau de performance de votre solution.
### OA-10. Propostion objectif affaire #1.
### OA-11. Propostion objectif affaire #2.

Chaque équipe doit réaliser deux nouveaux scénarios d'objectif d'affaires

A l'itération #3 vous devrez clarement démonter comment votre architecture satisfait chacun de ces objectifs d'affaire.


## Exigences fonctionnelles

### CU01. Veux comparer les temps de trajet
  1. Le (chargé de laboratoire) CL sélectionne une intersection de départ et une intersection d'arrivée, ainsi que le taux de rafraichissement de la prise de mesure.
  2. Le CL sélectionne les [services externes](service-externe.md) qu'il veut utiliser pour faire la comparaison des temps de traget avec les donnés temps réel de la STM.
  3. Le système affiche un graphique du temps de déplacement et met celui-ci à jour selon le taux de rafraichissement.
> #### Cas alternatifs
>  - 2.a [Service externe](service-externe.md): Utiliser plusieurs [services externes](service-externe.md) disponibles pour faire le comparatif.

### CU02. Veux pouvoir mettre le chaos dans les services en mode.
  - **Option 1**: Manuel 
    1.1. Le CL consulte la liste des microservices avec leur latence moyenne.
    1.2. Le CL change la latence d'un ou plusieurs microservices</span>.
  
  - **Option 2**:  Automatique 
    2.1. Le CL sélectionne le mode automatique tout en spécifiant la fréquence de la perturbation en seconde.
    2.2. Le système détruit<sup>1</sup> un microservice de façon aléatoire a tous les x secondes</span>.
  
  - Le système conserve un log des différents changements apportés que nous pourrons utiliser pour vérifier les données accumulées.

> #### Cas alternatifs
>  - 1.2.a Le CL détruit un ou plusieurs microservices</span>.
>  - 1.2.b Le CL détruit tous les microservices d'une équipe.
</span>


Chaque équipe doit créer deux cas d'utilisation supplémentaires qu'ils devront implémenter.
### CU03 - Nouveau cas d'utlisation
### CU04 - Nouveau cas d'utilisation

## Exigences de qualité

- Vous devez réaliser un scénario de qualité pour chacun des attributs (A) de qualité et vous devrez concevoir une architecture qui utilisera au minimum une tactique architecturale pour chacune des sous-catégories (SC) suivantes:
  
  ### (A) Disponibilité
    - (SC) détection de faute
    - (SC) Préparation et réparation
    - (SC) Réintroduction
    - (SC) Prévention des fautes
  ### (A) Modifiabilité
    - (SC) Réduire la taille des modules
    - (SC) Augmenter la cohésion
    - (SC) Réduire le couplage
    - (SC) Defer binding
  ### (A) Performance
    - (SC) Contrôler la demande en ressources
    - (SC) Gérer les ressources
  ### (A) Sécurité
    - (SC) Détecter les attaques
    - (SC) Résister aux attaques
    - (SC) Réagir aux attaques
    - (SC) Récupérer d'une attaque
  ### (A) Testabilité
    - (SC) Controle and observe l'état du système
    - (SC) limiter la complexité
  ### (A) Convivialité
    - (SC) Supporter l'initiative de l'usager
    - (SC) Supporter l'initiative du système
  ### (A) Interopérabilité
    - (SC) Localiser
    - (SC) Gérer les interfaces

A l'itération #2 , l'équipe de coordonnateur devra récupérer les scénarios de l'itération #1 de toutes les équipes, créer un arbre d'utilité, prioriser les scénarios avec tous les étudiants du sous-groupe.  Ensuite vous devrez sélectionner les scénarios de qualité les plus importants <b>pour chaque attribut de qualité</b> et concevoir l'architecture de votre système en fonctions de ces scénarios de qualité. N'oubliez pas que vous devez satisfaire les sous-catégories des tactiques architecturale pour chacun de ces scénarios.

## Contraintes de réalisation
### Language¶
Nous n’imposons aucune contrainte au niveau du langage de développement utilisé.

### Réalisation
Vous devez réaliser votre projet avec des microservices

### Contrainte d'équipe
L'équipe de coordonnateur peut imposer les contraintes quelle juge nécessaires pour le bon déroulement du projet. 


# Grille de pointage
Voir la [grille de pointage](doc/grille-pointage.xlsx) pour connaître le nombre de points associé à chacun des artéfacts que vous réalisez durant ce projet.
Le nombre normal d'étudiants dans une équipe est de 5 personnes. Nous ajusterons le nombre de points à implémenter proportionnellement au nombre d'étudiants dans chaque équipe.

# Directives pour chaque itération
Le projet doit être divisé en 3 itérations distinctes.  

Notez que le calendrier des séances est différent pour chaque groupe-cours, mais les dates de remises suivent cette planification. Le rapport doit être prêt pour la démonstation afin de montrer la correspondance entre la conception et la solution.

| Itération |Date remise | Démo/Rapport   | Démonstration |
| --------: |------------|:--------------| --------------|
|         1 |Fin séance 2| [Rapport1.md](rapports/rapport1.md) | N/A |
|         2 |Début séance 3| [Rapport1-coordonateurs.md](rapports/rapport1-coordonateurs.md) | N/A |
|         2 |Fin séance 6|  [Rapport2.md](rapports/rapport2.md) | N/A|
|         3 |Fin séance 7| [Rapport2-coordonnateurs](rapports/rapport2-coordonateurs.md) | N/A |
|         3 |Début séance 12| N/A | Démonstration |
|         3 |Fin journée séance 12| [Rapport3.md](rapports/rapport3.md) <br> ou <br>[Rapport3-V2.md](rapports/rapport3-v2.md) | N/A |


  
## Directive de planification des itérations

L'équipe de coordination répartit la charge de travail au niveau des équipes et indique clairement (avec document à l'appui) ses attentes par rapport à chaque équipe.

Je vous recommande d'utiliser un outil de suivi des tâches. 
  - Trello
  - Github issue
  - todoist
  - etc.
  - Notion task list

La répartition du travail entre les membres de l'équipe doit être clairement documentée.  Vous devrez inclure cette documentation dans une vue d'allocation.

## Directire de documentation de l'architecture 

Toutes les équipes d'une classe doivent utiliser les mêmes outils de documentation. L'équipe de coordination est responsable de gérer l'utilisation des outils, de définir et gérer le processus de réalisation de la documentation.  


### Suggestion d'outils pour la documentation
#### notion.so
https://notion.so/<sup>1</sup>
ÉTAPES POUR ÊTRE ENREGISTRÉ DANS LA DOCUMENTATION NOTION
1. Créer un compte notion avec votre adresse Google de l'ÉTS @etsmtl.net
2. Associer votre compte notion à un compte équipe
2.0. pour complété l'étape 2 entrée sur l'interface de notion authentifier avec votre compte google
2.1.  sélectionner l'onglet Settings & Members dans notion
2.2. Cliquer l'onglet Plans
2.4. Scroll passer la table de prix
2.3. Cliquer le bouton Activate Student plan
3. Mettre votre courriel etsmtl.ca ici pour qu'on vous ajoute à l'espace de documentation 

#### confluence
confluence.com

#### Google doc
googledoc.com

#### markdown
avec le plugin [plantuml](https://plantuml.com/fr/) ou [c4-plantuml](https://github.com/plantuml-stdlib/C4-PlantUML)

#### structurizer
https://structurizr.com<sup>1</sup>

#### Eclipse Papyrus
https://www.eclipse.org/papyrus/<sup>1</sup>


## Directive d'implémentation

Chaque équipe doit implémenter sa propre solution tout en réalisant l'intégration de microservices développés par d'autre [équipes](#équipe-étudiants). 

La seule contrainte est que ces composants doivent être déployés par l'équipe propriétaire ou l'équipe de coordonnateur.

La coordination et l'échange d'information entre les équipes deviennent cruciaux pour le bon succès de votre projet. 

Assurez-vous d'avoir accès à la documentation des composants que vous utilisez puisque vous devrez démontrer que votre architecture satisfait toutes les exigences client.

## Directive de déploiement
Vous devez déployer votre solution sur le serveur virtuel du laboratoire.  Ce serveur vous permettra de déployer des microservices réalisés à l'aide de docker et docker-compose

## Directive de Démonstration
- Chaque équipe disposera d'un maximum de 15 minutes pour démontrer la réalisation des cas d'utilisation et des attributs de qualité.
- Une fonctionnalité non démontrée dans une itération pourra être démontrée à l'itération suivante tout en respectant le maximum de 15 minutes.
- Donc soyez bien préparé
  - Assurez-vous d'avoir testé vos microservices individuellement et dans le système
  - Assurez-vous de ne pas faire des modifications de dernières minutes qui pourraient impacter votre démonstration
- À chaque démonstration, le chargé de laboratoire peut vous demander de créer des issues que vous devrez avoir satisfaites lors de la démonstration subséquente.
 
## Directives de vérification avant la Remise
Une attention particulière sera portée sur les éléments suivants au niveau de votre documentation d'architecture:
1. Le stéréotype de chaque élément est bien identifié
2. Les interfaces sont explicites dans les diagrammes et documentées en fonction du type d’interface.  
3. Chaque interface doit être adéquatement détaillée dans un fichier séparé.
4. Les choix technologiques sont visibles dans les vues architecturales. 
5. Chaque diagramme possède un texte explicite pour le décrire.  Ne décrivez pas chaque élément du diagramme, vous le ferez dans le tableau des éléments. Nous voulons savoir à quoi sert ce diagramme, quelle est son utilité, qu'est ce qu'il nous permet de comprendre ou démontrer.
6. Chaque diagramme possède une légende
7. La relation entre les vues est facilement compréhensible.
8. Les relations entre les éléments et les cas d'utilisation sont bien documentés. 
9. La relation entre les tactiques et les scénarios d'attributs de qualité est bien documentée.
10. La relation entre les éléments et les tactiques est  bien documentée. 
11. Les tactiques sont clairement visibles et bien documentées dans les vues architecturales. 
   1. Les propriétés associées aux tactiques sont bien documentées.
12. Vous utilisez des liens pour toute référence à de l'information se trouvant dans le document
13. Votre rapport contient au moins une vue de module. (Itération 2 et 3)
14. Votre rapport contient au moins une vue de C&C. (Itération 2 et 3)
15. Votre rapport contient au moins une vue d'allocation. Itération 1,2 et 3
   2. Assignation des tâches à toutes les itérations
   3. Déploiement à l'itération #2 et #3.
16. Votre rapport contient des diagrammes de séquence/activité pour démontrer les principales opérations.
17. Vous vous êtes assuré de la correspondance entre la documentation d’architecture et votre implémentation.

## Directives pour la remise
Toutes les remises se font directement sur le répertoire Github de votre équipe, sur la branche principale [«main»]. Assurez-vous que votre rapport est situé dans le répertoire doc/rapports et qu'il est au format PDF.

Vous devez mettre vos sources à jour dans la branche main, et ensuite vous générer un tag correspondant à votre remise. Voir le tableau suivant pour savoir quel tag générer selon votre remise.

Prenez note que tous les rapports en format markdown doivent aussi être remis sous le format PDF.

| Itération |Rapport           |
| --------: |:-----------------|
|         1 | git tag rapport1 |     
|         2 | git tag rapport2 |
|         3 | git tag rapport3 |
# Parasites et molassons
Les membres de l'équipe ayant réalisé un travail peuvent décider de ne pas mettre sur le rapport le nom d’un ou de plusieurs autres membres qui n'ont pas fait une **contribution significative** (analyse et conception) à l’itération. **Avant la remise** du travail, un courriel doit être envoyé en copie conforme à tous les membres de l’équipe, à l'étudiant visé, aux chargés de laboratoire ainsi qu’à l’enseignant pour indiquer les raisons du retrait du nom. Un membre de l'équipe dont son nom n'est pas sur un travail de laboratoire reçoit une note de "0" pour le travail. Référez vous à l'article Parasites et molassons pour justifier votre décision

# Bonus projet laboratoire (5% point bonus)
Impressionnez-nous en intégrant de nouvelles fonctionnalités / Apis offrant de nouveaux services ou interagissant avec de nouveaux services externes.


