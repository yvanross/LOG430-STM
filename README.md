![log](doc/assets/logo-logti.png)
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
  - [Sous-Groupe](#sous-groupe)
  - [Équipe de coordination](#équipe-de-coordination)
  - [Équipe (Étudiants)](#équipe-étudiants)
- [Exigences](#exigences)
  - [Objectifs d'affaires](#objectifs-daffaires)
    - [**OA01** - Faciliter le recrutement des nouveaux chargés de laboratoire.](#oa01---faciliter-le-recrutement-des-nouveaux-chargés-de-laboratoire)
    - [**OA02** - Vérifiez si le transport par autobus est toujours plus rapide, peu importe l'heure de la journée.](#oa02---vérifiez-si-le-transport-par-autobus-est-toujours-plus-rapide-peu-importe-lheure-de-la-journée)
  - [Exigences fonctionnelles](#exigences-fonctionnelles)
    - [**CU01** - Veux comparer les temps de trajet.](#cu01---veux-comparer-les-temps-de-trajet)
    - [**CU02** - Veux pouvoir mettre le chaos dans les services en mode.](#cu02---veux-pouvoir-mettre-le-chaos-dans-les-services-en-mode)
    - [**CU03** - <span style="color:red">Vous devez proposer un nouveau cas d'utilisation</span>](#cu03---vous-devez-proposer-un-nouveau-cas-dutilisation)
    - [**CU04** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>](#cu04---vous-devez-proposer-un-nouveau-cas-dutilisation)
    - [**CU05** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>](#cu05---vous-devez-proposer-un-nouveau-cas-dutilisation)
    - [**CU06** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>](#cu06---vous-devez-proposer-un-nouveau-cas-dutilisation)
    - [**CU07** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>](#cu07---vous-devez-proposer-un-nouveau-cas-dutilisation)
    - [**CU08** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>](#cu08---vous-devez-proposer-un-nouveau-cas-dutilisation)
    - [**CU09** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>](#cu09---vous-devez-proposer-un-nouveau-cas-dutilisation)
    - [**CU10** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation.</span>](#cu10---vous-devez-proposer-un-nouveau-cas-dutilisation)
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
  - [Directive pour la planification par les coordonnateurs](#directive-pour-la-planification-par-les-coordonnateurs)
  - [Directive de documentation de l'architecture](#directive-de-documentation-de-larchitecture)
    - [Suggestion d'outils pour la documentation](#suggestion-doutils-pour-la-documentation)
      - [notion.so](#notionso)
      - [confluence](#confluence)
      - [Google doc](#google-doc)
      - [markdown](#markdown)
      - [structurizer](#structurizer)
      - [Éclipse Papyrus](#éclipse-papyrus)
  - [Directive de gestion du flux de travail](#directive-de-gestion-du-flux-de-travail)
  - [Directive d'implémentation](#directive-dimplémentation)
  - [Directive d'intégration](#directive-dintégration)
  - [Directive de déploiement](#directive-de-déploiement)
  - [Directive de Démonstration](#directive-de-démonstration)
  - [Directives de vérification de la documentation](#directives-de-vérification-de-la-documentation)
  - [Directives pour la remise](#directives-pour-la-remise)
- [Parasites et mollasson](#parasites-et-mollasson)
- [Bonus projet laboratoire (5% point bonus)](#bonus-projet-laboratoire-5-point-bonus)
# Objectif principal
L'objectif principal de ce projet de cours est de proposer un système de comparaison de temps de trajet basée sur une architecture de microservices. Ce système permettra de comparer les temps de trajets entre les autobus et les automobiles.  Il sera réalisé au travers d'un seul laboratoire de 36 heures au total. Les spécifications des exigences portent sur l'analyse, la conception et l'implantation de ce système. Ce projet sera réalisé selon un seul livrable, qui devra être validé régulièrement auprès du chargé de laboratoire. Ce livrable comprendra les objectifs d'affaires, les cas d'utilisation, les scénarios de qualité priorisés, des vues architecturales permettant de démonter chacune des tactiques, une ou plusieurs vues de modules, une ou plusieurs vu composants et connecteurs, une ou plusieurs vues d'allocation.  

# Le contexte de l'organisation
Vous êtes nouvellement embauché par l'organisation LOG430STM pour développer le système de comparateur de trajet.  La réussite de ce projet n'est pas optionnelle. La carrière des étudiants peut grandement être impactée s'ils échouent à ce cours. C'est pour cette raison que l'organisation a décidé de séparer les responsabilités selon les différentes [parties prenantes.](doc/exigences/partiePrenantes.md)


# Le comparateur de trajet
Vous devez développer l'architecture d'un système de comparaison de temps de trajet en utilisant les données temps réel de la STM disponible sur le site suivant: https://www.stm.info/fr/a-propos/developpeurs et les donnés fournie par des [services externes](#services-externes)

# Services externes
Utiliser l'API ou la page web des fournisseurs externe pour l'estimation en temps réel du temps nécessaire pour parcourir la distance entre deux points correspondant aux points de départ et point d'arrivée d'un trajet d'autobus. L'estimation doit se faire en empruntant exactement le même trajet que l'autobus.
## Exemples
- https://www.google.ca/maps/dir/Marché+Jean-Talon/45.51433,+-73.55014/45.58927,+-73.50912/@45.5503686,-73.5965441,13z/data=!3m2!4b1!5s0x4cc919136bb582b5:0xf0b087b71589c626!4m20!4m19!1m5!1m1!1s0x4cc919136130849d:0x5c1098d838d87981!2m2!1d-73.6148902!2d45.5361095!1m5!1m1!1s0x0:0xf12664c8f968830d!2m2!1d-73.5502413!2d45.514437!1m5!1m1!1s0x0:0xfeb4b67d79e8fa7c!2m2!1d-73.5091582!2d45.589417!3e0

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
- Effectuera l'évaluation en continu de l'architecture de chaque équipe (Documentation, Intégration et Implémentation)
- Responsable de répondre aux questions des étudiants (durant les périodes de laboratoire seulement)
- Responsable d'aider les étudiants à maîtriser les concepts d'architecture
- Veux un rapport détaillé de l'architecture et des interfaces

## Classe
- Chaque classe est séparée en deux groupes

## Sous-Groupe
- Chaque sous-groupe est séparé en équipe de 5 étudiants.
- Chaque sous-groupe est en compétition avec les groupes de toutes les classes.
- Les équipes de chaque sous-groupe peuvent partager leurs microservices avec toutes les équipes du sous-groupe pour permettre aux autres équipes de faire de l'intégration au lieu de faire de l'implémentation.  Utiliser l'intégration pour satisfaire une exigence est plus payant en termes de point.  Voir le ficher de la grille de correction.
- Le chargé de cours crée les sous-groupes.

## Équipe de coordination
- L'équipe de coordination est  responsable de répartir équitablement les tâches de réalisation de la conception et l'implémentation des différents microservices nécessaire à ce projet.
- L'équipe de coordination gère les droits d'accès/d'utilisation des équipes aux intégrations réalisées par les autres équipes.
  - Dois conserver une trace écrite pour savoir quelle équipe implémente quel microservice.
- L'équipe a la responsabilité de valider et de diffuser la documentation des interfaces touchant aux composants implémentés/utilisés par plusieurs équipes. 
  - Une version d'interface publiée ne peut pas être changée. Vous devez obligatoirement publier une nouvelle version.
  - L'équipe de coordination peut exiger qu'il y ait simultanément plusieurs versions d'un même microservice en marche
- L'équipe de coordination peut démettre de ses fonctions un étudiant qui ne répond pas à ses attentes. 
  - L'équipe affectée devra nommer un nouveau représentant
- Les équipes de coordination ne doivent pas travailler ensemble ils sont des compétiteurs

## Équipe (Étudiants)
- L'équipe doit concevoir et réaliser une architecture qui satisfait toutes les [exigences](#exigences) documentées dans ce document de spécification.
- Je recommande à l'équipe d'utiliser les microservices implémentés par les autres équipes puisque c'est 2 fois plus payant en termes de points accumulés. Voir la [grille de pointage](#grille-de-pointage) pour plus d'information.
- Un étudiant par équipe est nommé pour faire partie de l'équipe de coordination 
  - Une équipe peut révoquer son représentant de l'équipe de coordination s'il ne répond pas à leurs attentes 
- L'équipe doit conserver une traçabilité de quel étudiant est responsable de quelles tâches. Ceci correspond à une vue d'allocation à insérer dans votre rapport.
- L'équipe doit connaître en tout temps l'état d'une tâche assignée à un étudiant
- L'équipe a la responsabilité de concevoir/documenter et diffuser (à l'équipe de coordination) la documentation des interfaces des microservices qui leur ont été assignés.
- Le chargé de cours crée les équipes de laboratoires.


# Exigences
## Objectifs d'affaires

### **OA01** - Faciliter le recrutement des nouveaux chargés de laboratoire.
### **OA02** - Vérifiez si le transport par autobus est toujours plus rapide, peu importe l'heure de la journée.

## Exigences fonctionnelles

### **CU01** - Veux comparer les temps de trajet.
### **CU02** - Veux pouvoir mettre le chaos dans les services en mode.
### **CU03** - <span style="color:red">Vous devez proposer un nouveau cas d'utilisation</span>
### **CU04** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>
### **CU05** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>
### **CU06** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>
### **CU07** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>
### **CU08** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>
### **CU09** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>
### **CU10** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation.</span>


## Exigences de qualité

- Vous devez vous assurer que les attributs de qualité (A) associés à chacun de vos cas d'utilisation soient documentés et réalisés. Pour chacun des attributs de qualité (A), vous devrez concevoir et réaliser une architecture qui utilisera au minimum une tactique architecturale pour chacune des sous-catégories (SC) suivantes.

- Si un attribut de qualité (A) ou une sous-catégorie (SC) n'est pas représenté dans votre architecture, vous devez ajouter de nouveau cas d'utilisation tant que tous n'auront pas été couverts.
  
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
    - (SC) Contrôle et observe l'état du système
    - (SC) limiter la complexité
  ### (A) Convivialité
    - (SC) Supporter l'initiative de l'usager
    - (SC) Supporter l'initiative du système
  ### (A) Interopérabilité
    - (SC) Localiser
    - (SC) Gérer les interfaces

## Contraintes de réalisation
### Language¶
Nous n’imposons aucune contrainte au niveau du langage de développement utilisé.

### Réalisation
Vous devez réaliser votre projet avec des microservices

### Contrainte d'équipe
L'équipe de coordonnateur peut imposer aux équipes les contraintes quelle juge nécessaires pour le bon déroulement du projet. 

# Grille de pointage
Voir la [grille de pointage](doc/grille-pointage.xlsx) pour connaître le nombre de points associé à chacun des artéfacts que vous réaliserez durant ce projet.

Le nombre normal d'étudiants dans une équipe est de 5 personnes. 

La note finale de chaque équipe sera pondérée en fonction du nombre d'étudiants dans l'équipe.

La note finale de chaque équipe sera pondérée en fonction des notes de toutes les équipes de toutes les classes.

## Directive pour la planification par les coordonnateurs

L'équipe de coordination répartit la charge de travail au niveau des équipes et indique clairement (avec document à l'appui) ses attentes par rapport à chaque équipe.

voir la [Directive de gestion du flux de travail](#directive-de-gestion-du-flux-de-travail)

## Directive de documentation de l'architecture 

Toutes les équipes d'un sous-groupe doivent utiliser les mêmes outils de documentation. L'équipe de coordination est responsable de gérer l'utilisation des outils, de définir et gérer le processus de réalisation de la documentation.  

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
http://www.confluence.com

#### Google doc
http://www.googledoc.com

#### markdown
avec le plugin [plantuml](https://plantuml.com/fr/) ou [c4-plantuml](https://github.com/plantuml-stdlib/C4-PlantUML)

#### structurizer
https://structurizr.com<sup>1</sup>

#### Éclipse Papyrus
https://www.eclipse.org/papyrus/<sup>1</sup>


## Directive de gestion du flux de travail
Vous devez utiliser le Kanban de Github pour planifier et réaliser votre projet.
Votre Kanban doit avoir au minimum les colonnes suivantes:
  - **backlog**: idée générique de tâches à considérer, priorisée par l'équipe.
  - **todo**: ce qu'on fait à la prochaine itération, à faire, assignée explicitement ou non.
  - **in progress**: tâches sur lesquels vous travaillez présentement. Généralement, une seule tâche par étudiant
  - **review**: les tâches qui doivent être révisées par un autre étudiant de l'équipe.  Idéalement sous forme de pull request.
  - **Done**: le pull request a été accepté et la tâche est terminée.

## Directive d'implémentation

Chaque équipe doit implémenter sa propre solution tout en réalisant l'intégration avec des microservices développés par d'autres [équipes](#équipe-étudiants). 

La coordination et l'échange d'information entre les équipes deviennent cruciaux pour le bon succès de votre projet. 

Vous devrez clairement démontrer comment vous faites l'intégration des composants développés par les autres équipes.  L'utilisation de diagramme de séquence et de diagramme de composant est particulièrement adaptée à ce besoin.

## Directive d'intégration
L'équipe qui développe un microservice qui sera intégré par d'autres équipes doit s'assurer de la robustesse de son microservice. Le microservice doit satisfaire les attributs de qualité associée à celui-ci. 

Ces microservices doivent être déployés par l'équipe propriétaire ou l'équipe de coordonnateur.

## Directive de déploiement
Vous pouvez déployer votre solution sur n'importe quel serveur. Dans le cadre du laboratoire nous vous fournirons l'accès à un serveur virtuel.  Ce serveur vous permettra de déployer des microservices réalisés à l'aide de docker et docker-compose.


## Directive de Démonstration
- Vous n'aurez droit qu'à une seule démonstration pour l'intégration et/ou l'implémentation de chaque exigence. 
- Chaque équipe disposera d'un maximum de 10 minutes par démonstration/exigence.
- Vous pouvez demander à faire une démonstration à n'importe quelle séance.
- Plus vous faites votre démonstration tôt durant la session plus cela sera payant (bonus) pour votre équipe. Voir la colonne pourcentage dans la [grille de pointage.](#grille-de-pointage)
- Donc soyez bien préparé
  - Assurez-vous d'avoir testé vos microservices individuellement et dans le système
  - Assurez-vous de ne pas faire des modifications de dernières minutes qui pourraient impacter votre démonstration
- À chaque démonstration, le chargé de laboratoire peut vous demander de créer des issues que vous devrez avoir satisfaites lors de la démonstration subséquente.  Le non respect de cette directive pourrait entrainer des pertes de points.
 
## Directives de vérification de la documentation
Une attention particulière sera portée sur les éléments suivants au niveau de votre documentation d'architecture:
1. Le stéréotype de chaque élément est bien identifié
2. Les interfaces sont explicites dans les diagrammes et documentées en fonction du type d’interface.  
3. Chaque interface doit être adéquatement détaillée dans un fichier séparé.
4. Les choix technologiques sont visibles dans les vues architecturales. 
5. Chaque diagramme possède un texte explicite pour le décrire.  Ne décrivez pas chaque élément du diagramme, vous le ferez dans le tableau des éléments. Nous voulons savoir à quoi sert ce diagramme, quelle est son utilité, qu'est-ce qu'il nous permet de comprendre ou démontrer.
6. Chaque diagramme possède une légende
7. La relation entre les vues est facilement compréhensible.
8. Les relations entre les cas d'utilisation et les éléments de votre architecture sont bien documentées. 
9. La relation entre les attributs de qualité et les tactiques de votre architecture sont bien documentées.
10. La relation entre les tactiques et les éléments de votre architecture sont bien documentés. 
11. Les tactiques sont clairement visibles et bien documentées dans les vues architecturales. 
   1. Les propriétés associées aux tactiques sont bien documentées.
12. Vous utilisez des liens pour toute référence à de l'information se trouvant dans le document
13. Votre rapport contient au moins une vue de module. 
14. Votre rapport contient au moins une vue de C&C. 
15. Votre rapport contient au moins une vue d'allocation.
   2. Assignation des tâches
   3. Déploiement
16. Votre rapport contient des diagrammes de séquence/activité pour démontrer le comportement des composants dans la réalisation des différentes tactiques.
17. Vous vous êtes assuré de la correspondance entre la documentation d’architecture et votre implémentation.

## Directives pour la remise
Toutes les remises se font directement sur le répertoire Github de votre équipe, sur la branche principale [«main»]. Assurez-vous que votre rapport est situé dans le répertoire DOC, qu'il est au format PDF et se nomme documentationArchitecture.PDF .

Vous devez mettre votre documentation et vos sources à jour dans la branche main, et ensuite vous générer un tag correspondant à la semaine ou vous faites votre remise. 

| Semaine   |Tag           |
| --------: |:-----------------|
|         2 | git tag semaine2 |
|         3 | git tag semaine3 |
|         n | git tag semainen |

# Parasites et mollasson
Les membres de l'équipe devront réaliser une évaluation par les pairs pour chacun des membres de l'équipe. La note finale du laboratoire de ce membre sera pondérée par rapport à cette évaluation. Référez-vous à l'article Parasites et mollasson pour vous aider à faire l'évaluation des autres étudiants. Voir les [fichiers Excel d'évaluation par les pairs](docs/../doc/EvaluationParLesPairs-etudiant1.xlsx) dans le répertoire docs. 

# Bonus projet laboratoire (5% point bonus)
Impressionnez-nous en intégrant de nouvelles fonctionnalités / Apis offrant de nouveaux services ou interagissant avec de nouveaux services externes.


