<style>
    .concept {
        width: 1000%;
        text-align: center;
    }
    .concept th {
        background: grey;
        word-wrap: break-word;
        text-align: center;
    }
    .disponibilite tr:nth-child(1) { background: orange; }
    .performance tr:nth-child(2) { background: orange; }
    .securite tr:nth-child(3) { background: orange; }
    .usabilite tr:nth-child(1) { background: orange; }
    .interoperabilite tr:nth-child(2) { background: orange; }
    .modifiabilite tr:nth-child(3) { background: orange; }
    .testabilite tr:nth-child(1) { background: orange; }    
</style>
# Documentation de l'architecture du laboratoire de LOG430
- [Documentation de l'architecture du laboratoire de LOG430](#documentation-de-larchitecture-du-laboratoire-de-log430)
- [Page titre](#page-titre)
- [Introduction](#introduction)
- [Scénario d'objectif d'affaire](#scénario-dobjectif-daffaire)
  - [OA-1. Faciliter le recrutement des nouveaux chargés de laboratoire.](#oa-1-faciliter-le-recrutement-des-nouveaux-chargés-de-laboratoire)
  - [OA-2. Validez si le transport par autobus est toujours plus rapide, peu importe l'heure de la journée](#oa-2-validez-si-le-transport-par-autobus-est-toujours-plus-rapide-peu-importe-lheure-de-la-journée)
- [Cas d'utilisations](#cas-dutilisations)
    - [**CU01** - Veux comparer les temps de trajet.](#cu01---veux-comparer-les-temps-de-trajet)
    - [**CU02** - Veux pouvoir mettre le chaos dans les microservices.](#cu02---veux-pouvoir-mettre-le-chaos-dans-les-microservices)
    - [**CU03** - <span style="color:red">Vous devez proposer un nouveau cas d'utilisation</span>](#cu03---vous-devez-proposer-un-nouveau-cas-dutilisation)
    - [**CU04** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>](#cu04---vous-devez-proposer-un-nouveau-cas-dutilisation)
    - [**CU05** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>](#cu05---vous-devez-proposer-un-nouveau-cas-dutilisation)
    - [**CU06** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>](#cu06---vous-devez-proposer-un-nouveau-cas-dutilisation)
    - [**CU07** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>](#cu07---vous-devez-proposer-un-nouveau-cas-dutilisation)
    - [**CU08** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>](#cu08---vous-devez-proposer-un-nouveau-cas-dutilisation)
    - [**CU09** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>](#cu09---vous-devez-proposer-un-nouveau-cas-dutilisation)
    - [**CU10** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation.</span>](#cu10---vous-devez-proposer-un-nouveau-cas-dutilisation)
- [Vue architecturale de contexte](#vue-architecturale-de-contexte)
  - [Présentation primaire](#présentation-primaire)
  - [Catalogue d'éléments](#catalogue-déléments)
  - [<s>Diagramme de contexte</s> Pas nécessaire puisque c'est déja un vue de contexte](#sdiagramme-de-contextes-pas-nécessaire-puisque-cest-déja-un-vue-de-contexte)
  - [Guide de variabilité](#guide-de-variabilité)
  - [Raisonnement](#raisonnement)
  - [<s>Vues associées</s> pas nécessaire puisque c'est la première vue que vous réalisé pour votre système.](#svues-associéess-pas-nécessaire-puisque-cest-la-première-vue-que-vous-réalisé-pour-votre-système)
- [Conception axée sur les attributs de qualité](#conception-axée-sur-les-attributs-de-qualité)
  - [ADD-Disponibilité](#add-disponibilité)
    - [ADD-détection de faute](#add-détection-de-faute)
    - [ADD-Préparation et réparation](#add-préparation-et-réparation)
    - [ADD-Réintroduction](#add-réintroduction)
    - [ADD-Prévention des fautes](#add-prévention-des-fautes)
  - [ADD-Modifiabilité](#add-modifiabilité)
    - [ADD-Réduire la taille des modules](#add-réduire-la-taille-des-modules)
    - [ADD-Augmenter la cohésion](#add-augmenter-la-cohésion)
    - [ADD-Réduire le couplage](#add-réduire-le-couplage)
    - [ADD-Defer binding](#add-defer-binding)
  - [ADD-Performance](#add-performance)
    - [ADD-Contrôler la demande en ressources](#add-contrôler-la-demande-en-ressources)
  - [ADD-Sécurité](#add-sécurité)
    - [ADD-Détecter les attaques](#add-détecter-les-attaques)
    - [ADD-Résister aux attaques](#add-résister-aux-attaques)
    - [ADD-Réagir aux attaques](#add-réagir-aux-attaques)
    - [ADD-Récupérer d'une attaque](#add-récupérer-dune-attaque)
  - [ADD-Testabilité](#add-testabilité)
    - [ADD-Controle and observe l'état du système](#add-controle-and-observe-létat-du-système)
    - [ADD-Limiter la complexité](#add-limiter-la-complexité)
  - [ADD-Usabilité](#add-usabilité)
    - [ADD-Supporter l'initiative de l'usager](#add-supporter-linitiative-de-lusager)
    - [ADD-Supporter l'initiative du système](#add-supporter-linitiative-du-système)
  - [ADD-Interopérabilité](#add-interopérabilité)
    - [ADD-Localiser](#add-localiser)
    - [ADD-Gérer les interfaces](#add-gérer-les-interfaces)
- [Réalisation des cas d'utilisation](#réalisation-des-cas-dutilisation)
    - [**RDCU-CU01** - Veux comparer les temps de trajet.](#rdcu-cu01---veux-comparer-les-temps-de-trajet)
    - [**RDCU-CU02** - Veux pouvoir mettre le chaos dans les services en mode.](#rdcu-cu02---veux-pouvoir-mettre-le-chaos-dans-les-services-en-mode)
    - [**RDCU-CU03**](#rdcu-cu03)
    - [**RDCU-CU04** -](#rdcu-cu04--)
    - [**RDCU-CU05** -](#rdcu-cu05--)
    - [**RDCU-CU06** -](#rdcu-cu06--)
    - [**RDCU-CU07** -](#rdcu-cu07--)
    - [**RDCU-CU08** -](#rdcu-cu08--)
    - [**RDCU-CU09** -](#rdcu-cu09--)
    - [**RDCU-CU10** -](#rdcu-cu10--)
- [Réalisation des attributs de qualité](#réalisation-des-attributs-de-qualité)
  - [RDAQ-Disponibilité](#rdaq-disponibilité)
    - [RDTQ-Détection de faute](#rdtq-détection-de-faute)
    - [RDTQ-Préparation et réparation](#rdtq-préparation-et-réparation)
    - [RDTQ-Réintroduction](#rdtq-réintroduction)
    - [RDTQ-Prévention des fautes](#rdtq-prévention-des-fautes)
    - [Relation entre les éléments architectuale et les exigences de disponibilité](#relation-entre-les-éléments-architectuale-et-les-exigences-de-disponibilité)
  - [RDAQ-Modifiabilité](#rdaq-modifiabilité)
    - [RDTQ-Réduire la taille des modules](#rdtq-réduire-la-taille-des-modules)
    - [RDTQ-Augmenter la cohésion](#rdtq-augmenter-la-cohésion)
    - [RDTQ-Réduire le couplage](#rdtq-réduire-le-couplage)
    - [RDTQ-Defer binding](#rdtq-defer-binding)
    - [Relation entre les éléments architectuale et les exigences de disponibilité](#relation-entre-les-éléments-architectuale-et-les-exigences-de-disponibilité-1)
  - [RDAQ-Performance](#rdaq-performance)
    - [RDTQ-Contrôler la demande en ressources](#rdtq-contrôler-la-demande-en-ressources)
    - [RDTQ-Gérer les ressources](#rdtq-gérer-les-ressources)
  - [RDAQ-Sécurité](#rdaq-sécurité)
    - [RDTQ-Détecter les attaques](#rdtq-détecter-les-attaques)
    - [RDTQ-Résister aux attaques](#rdtq-résister-aux-attaques)
    - [RDTQ-Réagir aux attaques](#rdtq-réagir-aux-attaques)
    - [RDTQ-Récupérer d'une attaque](#rdtq-récupérer-dune-attaque)
    - [Relation entre les éléments architectuale et les exigences de sécurité](#relation-entre-les-éléments-architectuale-et-les-exigences-de-sécurité)
  - [RDAQ-Testabilité](#rdaq-testabilité)
    - [RDTQ-Contrôle et observe l'état du système[](https://file%2B.vscode-resource.vscode-cdn.net/Users/yvanross/sources/log430/LOG430-STM/doc/documentationArchitecture.md#add-usabilit%C3%A9)](#rdtq-contrôle-et-observe-létat-du-système)
    - [RDTQ-limiter la complexité](#rdtq-limiter-la-complexité)
    - [Relation entre les éléments architectuale et les exigences de testabilité](#relation-entre-les-éléments-architectuale-et-les-exigences-de-testabilité)
  - [RDAQ-Usabilité](#rdaq-usabilité)
    - [RDTQ-Supporter l'initiative de l'usager](#rdtq-supporter-linitiative-de-lusager)
    - [RDTQ-Supporter l'initiative du système](#rdtq-supporter-linitiative-du-système)
    - [Relation entre les éléments architectuale et les exigences d'usabilité](#relation-entre-les-éléments-architectuale-et-les-exigences-dusabilité)
  - [RDAQ-Interopérabilité](#rdaq-interopérabilité)
    - [RDTQ-Localiser](#rdtq-localiser)
    - [RDTQ-Gérer les interfaces](#rdtq-gérer-les-interfaces)
    - [Relation entre les éléments architectuale et les exigences d'interopérabilité](#relation-entre-les-éléments-architectuale-et-les-exigences-dinteropérabilité)
- [Vues architecturales](#vues-architecturales)
  - [Vues architecturales de type Module](#vues-architecturales-de-type-module)
    - [Vue #1](#vue-1)
    - [Vue #2...](#vue-2)
  - [Vues architecturales de type composant et connecteur](#vues-architecturales-de-type-composant-et-connecteur)
    - [Vue #1](#vue-1-1)
    - [Vue #2...](#vue-2-1)
  - [Vues architecturales de type allocation](#vues-architecturales-de-type-allocation)
    - [Vue #1](#vue-1-2)
    - [Vue #2 ...](#vue-2-)
- [Conclusion](#conclusion)
- [Documentation des interfaces](#documentation-des-interfaces)
# Page titre

# Introduction
>TODO: insérer votre introduction

# Scénario d'objectif d'affaire
## OA-1. Faciliter le recrutement des nouveaux chargés de laboratoire.
<span style="color:red">Expliquer et démontrez comment votre architecture permet la réalisation de votre scénario d'objectif d'affaire. </span>


## OA-2. Validez si le transport par autobus est toujours plus rapide, peu importe l'heure de la journée
<span style="color:red">Expliquer et démontrez comment votre architecture permet la réalisation de votre scénario d'objectif d'affaire. </span>

# Cas d'utilisations
### **CU01** - Veux comparer les temps de trajet.

**Acteurs externe:** 
- **Chargé de laboratoire:** Veut pouvoir faire la correction de chaque cas d'utilisation.

**Précondition:** 
- Tous les microservices sont opérationnels

**Évènement déclencheur:** 
- La documentation pour ce cas d'utilisation est terminé et l'équipe demande au chargé de laboratoire de corriger celle-ci. 
- L'intégration est complété et l'équipe demande au chargé de laboratoire de corriger celle-ci
- L'implémentation est complété est l'équipe demande au chargé de laboratoire de corriger celle-ci.

**Scénario**
    
1. Le (chargé de laboratoire) CL sélectionne une intersection de départ et une intersection d'arrivée, ainsi que le taux de rafraichissement de la prise de mesure.
2. Le CL sélectionne le [service externe](service-externe.md) qu'il veut utiliser pour faire la comparaison des temps de trajet avec les donnés temps réel de la STM.
3. Le système affiche un graphique du temps de déplacement et met celui-ci à jour selon le taux de rafraichissement.

**Évènement résultant:**
- Le système affiche un graphique des comparatifs de temps de déplacement qui se met à jours selon le taux de rafraichissement.

**Postcondition:**  
- Le système est en attente d'une nouvelle commande de l'utilisateur

**Cas alternatifs:**

1. a  **Service externe:** Utiliser plusieurs [services externes](service-externe.md) disponibles pour faire le comparatif.

**Attributs de qualité**
Documenter l'ensemble des attributs de qualité qui s'appliquent à ce scénario en terme d'objectif et de mesure.    
##### CU01-D1 [**Disponibilité**](#add-disponibilité)
<span style="color:red"> Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU01-M1 [**Modifiabilité**](#add-modifiabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU01-P1 [**Performance**](#add-performance)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU01-S1 [**Sécurité**](#add-sécurité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU01-T1 [**Testabilité**](#add-testabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU01-U1 [**Usabilité**](#add-testabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU01-I1 [**Interopérabilité**](#add-interopérabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>

**Commentaires:**

- <span style="color:red">Quel sont vos remarques/commentaires par rapport à ce scénario</span>

### **CU02** - Veux pouvoir mettre le chaos dans les microservices.

**Acteurs externe:** 
- Chargé de laboratoire: Veut pouvoir faire la correction de chaque cas d'utilisation.

**Précondition:** 
- Tous les microservices sont opérationnels

**Évènement déclencheur:** 
- La documentation pour cet attribut est terminé et l'équipe demande au chargé de laboratoire de corriger celle-ci. 
- L'intégration est complété et l'équipe demande au chargé de laboratoire de corriger celle-ci
- L'implémentation est complété est l'équipe demande au chargé de laboratoire de corriger celle-ci.

**Scénario**
  1. Un mécanisme automatique et aléatoire de perturbation vient modifier l'architecture de votre système et vous devez vous assurer de quand même respecter les exigences client en terme d'attribut de qualité et de fonctionnalité.
    
**Évènement résultant:**
- L'architecture de votre système est perturbé par le mécanisme.
- Le système conserve un log des perturbations
- Le système conserve un log de comment le système a réagit pour résoudre le problème.

**Postcondition:**  
- Les mécanismes de traitement des attributs de qualité détectent le problème et modifie automatiquement l'architecture de votre système pour qu'il continue à respecter les exigences client.

**Cas alternatifs:**
- 1.a La perturbation consiste à détruire un microservice
- 1.b La perturbation consiste à augmenter la latence d'un microservice

**Attributs de qualité**

Documenter l'ensemble des attributs de qualité qui s'appliquent à ce scénario en terme d'objectif et de mesure.    

#### CU02-D1 [**Disponibilité**](#add-disponibilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU02-M1 [**Modifiabilité**](#add-modifiabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU02-P1 [**Performance**](#add-performance) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU02-S1 [**Sécurité**](#add-sécurité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU02-T1 [**Testabilité**](#add-testabilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU02-U1 [**Usabilité**](#add-usabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU02-I1 [**Interopérabilité**](#add-interopérabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>

**Commentaires:**

- <span style="color:red">Quel sont vos remarques/commentaires par rapport à ce scénario</span>

### **CU03** - <span style="color:red">Vous devez proposer un nouveau cas d'utilisation</span>

**Acteurs externe:** 

**Précondition:** 

**Évènement déclencheur:** 

**Scénario**

**Évènement résultant:**

**Postcondition:** 

**Cas alternatifs:**

**Attributs de qualité**

#### CU03-D1 [**Disponibilité**](#add-disponibilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU03-M1 [**Modifiabilité**](#add-modifiabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU03-P1 [**Performance**](#add-performance) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU03-S1 [**Sécurité**](#add-sécurité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU03-T1 [**Testabilité**](#add-testabilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU03-U1 [**Usabilité**](#add-usabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU03-I1 [**Interopérabilité**](#add-interopérabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>

**Commentaires:**

### **CU04** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>

**Acteurs externe:** 

**Précondition:** 

**Évènement déclencheur:** 

**Scénario**

**Évènement résultant:**

**Postcondition:** 

**Cas alternatifs:**

**Attributs de qualité**

#### CU04-D1 [**Disponibilité**](#add-disponibilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU04-M1 [**Modifiabilité**](#add-modifiabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU04-P1 [**Performance**](#add-performance) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU04-S1 [**Sécurité**](#add-sécurité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU04-T1 [**Testabilité**](#add-testabilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU04-U1 [**Usabilité**](#add-usabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU04-I1 [**Interopérabilité**](#add-interopérabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>

**Commentaires:**

### **CU05** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>

**Acteurs externe:** 

**Précondition:** 

**Évènement déclencheur:** 

**Scénario**

**Évènement résultant:**

**Postcondition:** 

**Cas alternatifs:**

**Attributs de qualité**

#### CU05-D1 [**Disponibilité**](#add-disponibilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU05-M1 [**Modifiabilité**](#add-modifiabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU05-P1 [**Performance**](#add-performance) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU05-S1 [**Sécurité**](#add-sécurité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU05-T1 [**Testabilité**](#add-testabilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU05-U1 [**Usabilité**](#add-usabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU05-I1 [**Interopérabilité**](#add-interopérabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>

**Commentaires:**

### **CU06** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>

**Acteurs externe:** 

**Précondition:** 

**Évènement déclencheur:** 

**Scénario**

**Évènement résultant:**

**Postcondition:** 

**Cas alternatifs:**

**Attributs de qualité**

#### CU06-D1 [**Disponibilité**](#add-disponibilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU06-M1 [**Modifiabilité**](#add-modifiabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU06-P1 [**Performance**](#add-performance) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU06-S1 [**Sécurité**](#add-sécurité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU06-T1 [**Testabilité**](#add-testabilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU06-U1 [**Usabilité**](#add-usabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU06-I1 [**Interopérabilité**](#add-interopérabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>

**Commentaires:**

### **CU07** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>

**Acteurs externe:** 

**Précondition:** 

**Évènement déclencheur:** 

**Scénario**

**Évènement résultant:**

**Postcondition:** 

**Cas alternatifs:**

**Attributs de qualité**

#### CU07-D1 [**Disponibilité**](#add-disponibilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU07-M1 [**Modifiabilité**](#add-modifiabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU07-P1 [**Performance**](#add-performance) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU07-S1 [**Sécurité**](#add-sécurité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU07-T1 [**Testabilité**](#add-testabilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU07-U1 [**Usabilité**](#add-usabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU07-I1 [**Interopérabilité**](#add-interopérabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>

**Commentaires:**

### **CU08** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>

**Acteurs externe:** 

**Précondition:** 

**Évènement déclencheur:** 

**Scénario**

**Évènement résultant:**

**Postcondition:** 

**Cas alternatifs:**

**Attributs de qualité**

#### CU08-D1 [**Disponibilité**](#add-disponibilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU08-M1 [**Modifiabilité**](#add-modifiabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU08-P1 [**Performance**](#add-performance) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU08-S1 [**Sécurité**](#add-sécurité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU08-T1 [**Testabilité**](#add-testabilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU08-U1 [**Usabilité**](#add-usabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU08-I1 [**Interopérabilité**](#add-interopérabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>

**Commentaires:**

### **CU09** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation</span>

**Acteurs externe:** 

**Précondition:** 

**Évènement déclencheur:** 

**Scénario**

**Évènement résultant:**

**Postcondition:** 

**Cas alternatifs:**

**Attributs de qualité**

#### CU09-D1 [**Disponibilité**](#add-disponibilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU09-M1 [**Modifiabilité**](#add-modifiabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU09-P1 [**Performance**](#add-performance) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU09-S1 [**Sécurité**](#add-sécurité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU09-T1 [**Testabilité**](#add-testabilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU09-U1 [**Usabilité**](#add-usabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU09-I1 [**Interopérabilité**](#add-interopérabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>

**Commentaires:**

### **CU10** - <span style="color:red">vous devez proposer un nouveau cas d'utilisation.</span>

**Acteurs externe:** 

**Précondition:** 

**Évènement déclencheur:** 

**Scénario**

**Évènement résultant:**

**Postcondition:** 

**Cas alternatifs:**

**Attributs de qualité**

#### CU10-D1 [**Disponibilité**](#add-disponibilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU10-M1 [**Modifiabilité**](#add-modifiabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU10-P1 [**Performance**](#add-performance) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU10-S1 [**Sécurité**](#add-sécurité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU10-T1 [**Testabilité**](#add-testabilité) 
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU10-U1 [**Usabilité**](#add-usabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>
#### CU10-I1 [**Interopérabilité**](#add-interopérabilité)
<span style="color:red">Définir l'exigence que qualité associé à ce scénario ou N/a</span>

**Commentaires:**

# Vue architecturale de contexte

<span style="color:red">Utiliser le gabarit suivant: https://wiki.sei.cmu.edu/confluence/display/SAD/Template%3AArchitectureViewTemplate</span>
## Présentation primaire
## Catalogue d'éléments
## <s>Diagramme de contexte</s> Pas nécessaire puisque c'est déja un vue de contexte
## Guide de variabilité
## Raisonnement
## <s>Vues associées</s> pas nécessaire puisque c'est la première vue que vous réalisé pour votre système.


# Conception axée sur les attributs de qualité
A partir des qualités associées à tous vos cas d'utilisation, réaliser un mini ADD pour comparer les différents tactiques et identifier clairement la raison de votre choix.

## ADD-[Disponibilité](#rdaq-disponibilité)

  |Identifiant|Description|
  |-----------|------------|
  |[CU01-D1](#cu01-d1-disponibilité)| |
  |[CU02-D1](#cu02-d1-disponibilité)| |
  |[CU03-D1](#cu03-d1-disponibilité) |
  |[CU04-D1](#cu04-d1-disponibilité) |
  |[CU05-D1](#cu05-d1-disponibilité) |
  |[CU06-D1](#cu06-d1-disponibilité) |
  |[CU07-D1](#cu07-d1-disponibilité) |
  |[CU08-D1](#cu08-d1-disponibilité) |
  |[CU09-D1](#cu09-d1-disponibilité) |
  |[CU10-D1](#cu10-d1-disponibilité) |

### ADD-[détection de faute](#rdtq-détection-de-faute)
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

### ADD-[Préparation et réparation](#rdtq-préparation-et-réparation)
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

### ADD-[Réintroduction](#rdtq-réintroduction)
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

### ADD-[Prévention des fautes](#rdtq-prévention-des-fautes)  
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

## ADD-[Modifiabilité](#rdaq-modifiabilité)
  |Identifiant|Description|
  |-----------|------------|
  |[CU01-M1](#cu01-m1-modifiabilité) | |
  |[CU02-M1](#cu02-m1-modifiabilité) | |
  |[CU03-M1](#cu03-m1-modifiabilité) | |
  |[CU04-M1](#cu04-m1-modifiabilité) | |
  |[CU05-M1](#cu05-m1-modifiabilité) | |
  |[CU06-M1](#cu06-m1-modifiabilité) | |
  |[CU07-M1](#cu07-m1-modifiabilité) | |
  |[CU08-M1](#cu08-m1-modifiabilité) | |
  |[CU09-M1](#cu09-m1-modifiabilité) | |
  |[CU10-M1](#cu10-m1-modifiabilité) | |

### ADD-[Réduire la taille des modules](#rdtq-réduire-la-taille-des-modules)
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

### ADD-[Augmenter la cohésion](#rdtq-augmenter-la-cohésion)
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

### ADD-[Réduire le couplage](#rdtq-réduire-le-couplage)
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

### ADD-[Defer binding](#rdtq-defer-binding)
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>


## ADD-[Performance](#rdaq-performance)
   |Identifiant|Description|
  |-----------|------------|
  |[CU01-P1](#cu01-p1-performance) | 
  |[CU02-P1](#cu02-p1-performance) | 
  |[CU03-P1](#cu03-p1-performance) | 
  |[CU04-P1](#cu04-p1-performance) | 
  |[CU05-P1](#cu05-p1-performance) | 
  |[CU06-P1](#cu06-p1-performance) | 
  |[CU07-P1](#cu07-p1-performance) | 
  |[CU08-P1](#cu08-p1-performance) | 
  |[CU09-P1](#cu09-p1-performance) | 
  |[CU10-P1](#cu10-p1-performance) | 
  
### ADD-[Contrôler la demande en ressources](#rdtq-contrôler-la-demande-en-ressources)
<div class="concept performance">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

ADD-Gérer les ressources
<div class="concept performance">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

## ADD-[Sécurité](#rdaq-sécurité)

### ADD-[Détecter les attaques](#rdtq-détecter-les-attaques)
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

### ADD-[Résister aux attaques](#rdtq-résister-aux-attaques)
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>


### ADD-[Réagir aux attaques](#rdtq-réagir-aux-attaques)
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

### ADD-[Récupérer d'une attaque](#rdtq-récupérer-dune-attaque)
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>


## ADD-[Testabilité](#rdaq-testabilité)
  |Identifiant|Description|
  |-----------|------------|
  |[CU01-T1](#cu01-t1-testabilité)|  | 
  |[CU02-T1](#cu02-t1-testabilité)|  | 
  |[CU03-T1](#cu03-t1-testabilité)|  | 
  |[CU04-T1](#cu04-t1-testabilité)|  | 
  |[CU05-T1](#cu05-t1-testabilité)|  | 
  |[CU06-T1](#cu06-t1-testabilité)|  | 
  |[CU07-T1](#cu07-t1-testabilité)|  | 
  |[CU08-T1](#cu08-t1-testabilité)|  | 
  |[CU09-T1](#cu09-t1-testabilité)|  | 
  |[CU10-T1](#cu10-t1-testabilité)|  | 

### ADD-[Controle and observe l'état du système](#rdtq-contrôle-et-observe-létat-du-système)
<div class="concept testabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

### ADD-[Limiter la complexité](#rdtq-limiter-la-complexité)

<div class="concept testabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>


## ADD-[Usabilité](#rdaq-usabilité)
  |Identifiant|Description|
  |-----------|------------|
  |[CU01-U1](#cu01-u1-usabilité) |
  |[CU02-U1](#cu02-u1-usabilité) |
  |[CU03-U1](#cu03-u1-usabilité) |
  |[CU04-U1](#cu04-u1-usabilité) |
  |[CU05-U1](#cu05-u1-usabilité) |
  |[CU06-U1](#cu06-u1-usabilité) |
  |[CU07-U1](#cu07-u1-usabilité) |
  |[CU08-U1](#cu08-u1-usabilité) |
  |[CU09-U1](#cu09-u1-usabilité) |
  |[CU10-U1](#cu10-u1-usabilité) |

### ADD-[Supporter l'initiative de l'usager](#rdtq-supporter-linitiative-de-lusager)
<div class="concept usabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

### ADD-[Supporter l'initiative du système](#rdtq-supporter-linitiative-du-système)
<div class="concept usabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

## ADD-[Interopérabilité](#rdaq-interopérabilité)
  |Identifiant|Description|
  |-----------|------------|
  |[CU01-I1](#cu01-i1-interopérabilité)| | 
  |[CU02-I1](#cu01-i1-interopérabilité)| |
  |[CU03-I1](#cu01-i1-interopérabilité)| |
  |[CU04-I1](#cu01-i1-interopérabilité)| |
  |[CU05-I1](#cu01-i1-interopérabilité)| |
  |[CU06-I1](#cu01-i1-interopérabilité)| |
  |[CU07-I1](#cu01-i1-interopérabilité)| |
  |[CU08-I1](#cu01-i1-interopérabilité)| |
  |[CU09-I1](#cu01-i1-interopérabilité)| |
  |[CU10-I1](#cu01-i1-interopérabilité)| |
### ADD-[Localiser](#rdtq-localiser)
<div class="concept interoperabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

### ADD-[Gérer les interfaces](#rdtq-gérer-les-ressources)
<div class="concept interoperabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
<span style="color:red">Quelle tactique avez vous choisi et pourquoi?</span>

# Réalisation des cas d'utilisation
### [**RDCU-CU01**](#cu01---veux-comparer-les-temps-de-trajet) - Veux comparer les temps de trajet.
<span style="color:red">Diagramme(s) de séquence démontrant la réalisation de ce cas d'utilisation</span>
### [**RDCU-CU02**](#cu02---veux-pouvoir-mettre-le-chaos-dans-les-services-en-mode) - Veux pouvoir mettre le chaos dans les services en mode.
<span style="color:red">Diagramme(s) de séquence démontrant la réalisation de ce cas d'utilisation</span>
### [**RDCU-CU03**](#cu03)  
<span style="color:red">Diagramme(s) de séquence démontrant la réalisation de ce cas d'utilisation</span>
### **RDCU-CU04** -
<span style="color:red">Diagramme(s) de séquence démontrant la réalisation de ce cas d'utilisation</span>
### **RDCU-CU05** - 
<span style="color:red">Diagramme(s) de séquence démontrant la réalisation de ce cas d'utilisation</span>
### **RDCU-CU06** - 
<span style="color:red">Diagramme(s) de séquence démontrant la réalisation de ce cas d'utilisation</span>
### **RDCU-CU07** - 
<span style="color:red">Diagramme(s) de séquence démontrant la réalisation de ce cas d'utilisation</span>
### **RDCU-CU08** - 
<span style="color:red">Diagramme(s) de séquence démontrant la réalisation de ce cas d'utilisation</span>
### **RDCU-CU09** - 
<span style="color:red">Diagramme(s) de séquence démontrant la réalisation de ce cas d'utilisation</span>
### **RDCU-CU10** - 
<span style="color:red">Diagramme(s) de séquence démontrant la réalisation de ce cas d'utilisation</span>

# Réalisation des attributs de qualité

## RDAQ-[Disponibilité](#add-disponibilité) 

  ###  [RDTQ-Détection de faute](#add-détection-de-faute)
  <span style="color:red">nom de la tactique</span>

  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

  ### [RDTQ-Préparation et réparation](#add-préparation-et-réparation)
  
  <span style="color:red">nom de la tactique</span>

  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

  ### [RDTQ-Réintroduction](#add-réintroduction)

  <span style="color:red">nom de la tactique</span>
   
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>
  
  ### [RDTQ-Prévention des fautes](#add-prévention-des-fautes) 
  <span style="color:red">nom de la tactique</span>

  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

  ### Relation entre les éléments architectuale et les exigences de disponibilité
 |Identifiant|Éléments|Description de la responabilité|
 |-----------|--------|-------------------------------|
 |[CU01-D1](#cu01-d1-disponibilité) | |
 |[CU02-D1](#cu02-d1-disponibilité) | |
 |[CU03-D1](#cu03-d1-disponibilité) | |
 |[CU04-D1](#cu04-d1-disponibilité) | |
 |[CU05-D1](#cu05-d1-disponibilité) | |
 |[CU06-D1](#cu06-d1-disponibilité) | |
 |[CU07-D1](#cu07-d1-disponibilité) | |
 |[CU08-D1](#cu08-d1-disponibilité) | |
 |[CU09-D1](#cu09-d1-disponibilité) | |
 |[CU10-D1](#cu10-d1-disponibilité) | |
  
## RDAQ-[Modifiabilité](#add-modifiabilité)

  ###  [RDTQ-Réduire la taille des modules](#add-réduire-la-taille-des-modules)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

  ### [RDTQ-Augmenter la cohésion](#add-augmenter-la-cohésion)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

  ### [RDTQ-Réduire le couplage](#add-réduire-le-couplage)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

  ### [RDTQ-Defer binding](#add-defer-binding)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

  ### Relation entre les éléments architectuale et les exigences de disponibilité
|Identifiant|Éléments|Description de la responabilité|
|-----------|--------|-------------------------------|
 |[CU01-M1](#cu01-m1-modifiabilité) | | 
 |[CU02-M1](#cu02-m1-modifiabilité) | |  
 |[CU03-M1](#cu03-m1-modifiabilité) | |  
 |[CU04-M1](#cu04-m1-modifiabilité) | |  
 |[CU05-M1](#cu05-m1-modifiabilité) | |  
 |[CU06-M1](#cu06-m1-modifiabilité) | |  
 |[CU07-M1](#cu07-m1-modifiabilité) | |  
 |[CU08-M1](#cu08-m1-modifiabilité) | |  
 |[CU09-M1](#cu09-m1-modifiabilité) | |  
 |[CU10-M1](#cu10-m1-modifiabilité) | |  
  
## RDAQ-[Performance](#add-performance)          

   ### [RDTQ-Contrôler la demande en ressources](#add-contrôler-la-demande-en-ressources)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

  ### [RDTQ-Gérer les ressources](#add-gérer-les-ressources)
  <span style="color:red">nom de la tactique</span>

  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

## RDAQ-[Sécurité](#add-sécurité)

  ### RDTQ-[Détecter les attaques](#add-détecter-les-attaques)
   <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

  ### [RDTQ-Résister aux attaques](#add-résister-aux-attaques)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

  ### [RDTQ-Réagir aux attaques](#add-réagir-aux-attaques)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

  ### [RDTQ-Récupérer d'une attaque](#add-récupérer-dune-attaque)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

  ### Relation entre les éléments architectuale et les exigences de sécurité
|Identifiant|Éléments|Description de la responabilité|
|-----------|--------|-------------------------------|
  |[CU01-P1](#cu01-p1-performance) | |
  |[CU02-P1](#cu02-p1-performance) | |
  |[CU03-P1](#cu03-p1-performance) | |
  |[CU04-P1](#cu04-p1-performance) | |
  |[CU05-P1](#cu05-p1-performance) | |
  |[CU06-P1](#cu06-p1-performance) | |
  |[CU07-P1](#cu07-p1-performance) | |
  |[CU08-P1](#cu08-p1-performance) | |
  |[CU09-P1](#cu09-p1-performance) | |
  |[CU10-P1](#cu10-p1-performance) | |
  
## RDAQ-[Testabilité](#add-testabilité)

  ### [RDTQ-Contrôle et observe l'état du système](#add-controle-and-observe-létat-du-système)[](https://file%2B.vscode-resource.vscode-cdn.net/Users/yvanross/sources/log430/LOG430-STM/doc/documentationArchitecture.md#add-usabilit%C3%A9)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>
 
  ### [RDTQ-limiter la complexité](#add-limiter-la-complexité)
  <span style="color:red">nom de la tactique</span>
 
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>
  
  ### Relation entre les éléments architectuale et les exigences de testabilité
  |Identifiant|Éléments|Description de la responabilité|
  |-----------|--------|-------------------------------|
  |[CU01-T1](#cu01-t1-testabilité) | |
  |[CU02-T1](#cu02-t1-testabilité) | |
  |[CU03-T1](#cu03-t1-testabilité) | |
  |[CU04-T1](#cu04-t1-testabilité) | |
  |[CU05-T1](#cu05-t1-testabilité) | |
  |[CU06-T1](#cu06-t1-testabilité) | |
  |[CU07-T1](#cu07-t1-testabilité) | |
  |[CU08-T1](#cu08-t1-testabilité) | |
  |[CU09-T1](#cu09-t1-testabilité) | |
  |[CU10-T1](#cu10-t1-testabilité) | |
  
## RDAQ-[Usabilité](#add-usabilité)

  ### [RDTQ-Supporter l'initiative de l'usager](#add-supporter-linitiative-de-lusager)
  <span style="color:red">nom de la tactique</span>
 
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>
  
  ### [RDTQ-Supporter l'initiative du système](#add-supporter-linitiative-du-système)
  <span style="color:red">nom de la tactique</span>

  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>

  ### Relation entre les éléments architectuale et les exigences d'usabilité
|Identifiant|Éléments|Description de la responabilité|
|-----------|--------|-------------------------------|
  |[CU01-U1](#cu01-u1-usabilité) | |
  |[CU01-U2](#cu01-u1-usabilité) | |
  |[CU02-U1](#cu02-u1-usabilité) | |
  |[CU03-U1](#cu03-u1-usabilité) | |
  |[CU04-U1](#cu04-u1-usabilité) | |
  |[CU05-U1](#cu05-u1-usabilité) | |
  |[CU06-U1](#cu06-u1-usabilité) | |
  |[CU07-U1](#cu07-u1-usabilité) | |
  |[CU08-U1](#cu08-u1-usabilité) | |
  |[CU09-U1](#cu09-u1-usabilité) | |
  |[CU10-U1](#cu10-u1-usabilité) | |
  
 ## RDAQ-[Interopérabilité](#add-interopérabilité)

  ### [RDTQ-Localiser](#add-localiser)
  <span style="color:red">nom de la tactique</span>

  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>
  
  ### [RDTQ-Gérer les interfaces](#add-gérer-les-interfaces)
  <span style="color:red">nom de la tactique</span>
  
  <span style="color:red">Diagramme(s) de séquence ou autre information pertinente démontrant la réalisation de(s) tactique(s)</span>
  
  ### Relation entre les éléments architectuale et les exigences d'interopérabilité
|Identifiant|Éléments|Description de la responabilité|
|-----------|--------|-------------------------------|
  |[CU01-I1](#cu01-i1-interopérabilité) | |
  |[CU01-I2](#cu01-i1-interopérabilité) | |
  |[CU02-I1](#cu02-i1-interopérabilité) | |
  |[CU03-I1](#cu03-i1-interopérabilité) | |
  |[CU04-I1](#cu04-i1-interopérabilité) | |
  |[CU05-I1](#cu05-i1-interopérabilité) | |
  |[CU06-I1](#cu06-i1-interopérabilité) | |
  |[CU07-I1](#cu07-i1-interopérabilité) | |
  |[CU08-I1](#cu08-i1-interopérabilité) | |
  |[CU09-I1](#cu09-i1-interopérabilité) | |
  |[CU10-I1](#cu10-i1-interopérabilité) | |


# Vues architecturales 
## Vues architecturales de type Module
### Vue #1
>#### Présentation primaire
>#### Catalogue d'éléments
|Élement|Description|lien vers document d'interfaces|
|-------|-----------|-------------------------------|
|el1|responsabilité incluant les liens vers les diagrammes de séquence démontrant le fonctionnement de celui-ci|http://www.etsmtl.ca|
>#### Diagramme de contexte
>#### Guide de variabilité
>#### Raisonnement
>#### Vues associées
### Vue #2...

## Vues architecturales de type composant et connecteur
### Vue #1
>#### Présentation primaire
>#### Catalogue d'éléments
|Élement|Description|lien vers document d'interfaces|
|-------|-----------|-------------------------------|
|el1|responsabilité incluant les liens vers les diagrammes de séquence démontrant le fonctionnement de celui-ci|http://www.etsmtl.ca|
>#### Diagramme de contexte
>#### Guide de variabilité
>#### Raisonnement
>#### Vues associées
### Vue #2...

## Vues architecturales de type allocation
### Vue #1
>#### Présentation primaire
>#### Catalogue d'éléments
|Élement|Description|lien vers document d'interfaces|
|-------|-----------|-------------------------------|
|el1|responsabilité incluant les liens vers les diagrammes de séquence démontrant le fonctionnement de celui-ci|http://www.etsmtl.ca|
>#### Diagramme de contexte
>#### Guide de variabilité
>#### Raisonnement
>#### Vues associées
### Vue #2 ...


# Conclusion
>TODO: insérer votre conclusion


N'oubliez pas d'effacer les TODO et ce texte et de générer une version PDF de ce document pour votre remise finale.
Créer un tag git avec la commande "git tag rapport1"


# Documentation des interfaces
Les catalogues d'élément devraient être des tableaux qui contiennent la description des éléments en plus d'un lien vers la documentation de l'interface de ceux-ci.
Je vous suggère d'utiliser un document par interface pour vous faciliter la tâche. Il sera ainsi plus facile de distribuer la documentation d'une interface aux équipes en ayant besoin.
La documentation des interfaces de vos éléments doit se faire en utilisant le [gabarit suivant](template-interface.md).

Voici quelques exemples de documentation d'interface utilisant ce gabarit:
- https://wiki.sei.cmu.edu/confluence/display/SAD/OpcPurchaseOrderService+Interface+Documentation
- https://wiki.sei.cmu.edu/confluence/display/SAD/OpcOrderTrackingService+Interface+Documentation
- https://wiki.sei.cmu.edu/confluence/display/SAD/WebServiceBroker+Interface+Documentation
