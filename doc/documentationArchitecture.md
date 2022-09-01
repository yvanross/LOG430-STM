# Documentation de l'architecture du laboratoire de LOG430
- [Documentation de l'architecture du laboratoire de LOG430](#documentation-de-larchitecture-du-laboratoire-de-log430)
- [Page titre](#page-titre)
- [Introduction](#introduction)
- [Scénario d'objectif d'affaire](#scénario-dobjectif-daffaire)
  - [OA-1. Faciliter le recrutement des nouveaux chargés de laboratoire.](#oa-1-faciliter-le-recrutement-des-nouveaux-chargés-de-laboratoire)
    - [Démontrez la relation entre les éléments de votre architecture et votre scénario d'objectif d'affaire.](#démontrez-la-relation-entre-les-éléments-de-votre-architecture-et-votre-scénario-dobjectif-daffaire)
  - [OA-2. Validez si le transport par autobus est toujours plus rapide, peu importe l'heure de la journée](#oa-2-validez-si-le-transport-par-autobus-est-toujours-plus-rapide-peu-importe-lheure-de-la-journée)
    - [Démontrez la relation entre les éléments de votre architecture et votre scénario d'objectif d'affaire.](#démontrez-la-relation-entre-les-éléments-de-votre-architecture-et-votre-scénario-dobjectif-daffaire-1)
- [Cas d'utilisations](#cas-dutilisations)
  - [CU01. Veux comparer les temps de trajet](#cu01-veux-comparer-les-temps-de-trajet)
    - [Démontrez la relation entre les éléments de votre architecture et votre cas d'utilisation.](#démontrez-la-relation-entre-les-éléments-de-votre-architecture-et-votre-cas-dutilisation)
  - [CU02. Veut pouvoir mettre le chaos dans les services en mode manuel](#cu02-veut-pouvoir-mettre-le-chaos-dans-les-services-en-mode-manuel)
    - [Démontrez la relation entre les éléments de votre architecture et votre cas d'utilisation.](#démontrez-la-relation-entre-les-éléments-de-votre-architecture-et-votre-cas-dutilisation-1)
- [Vue architecturale de contexte](#vue-architecturale-de-contexte)
      - [Présentation primaire](#présentation-primaire)
      - [Catalogue d'éléments](#catalogue-déléments)
      - [<s>Diagramme de contexte</s>](#sdiagramme-de-contextes)
      - [Guide de variabilité](#guide-de-variabilité)
      - [Raisonnement](#raisonnement)
      - [<s>Vues associées</s>](#svues-associéess)
- [Scénarios de qualité](#scénarios-de-qualité)
  - [Disponibilité](#disponibilité)
    - [Rédaction du scénario de qualité](#rédaction-du-scénario-de-qualité)
    - [ADD](#add)
      - [(SC) détection de faute](#sc-détection-de-faute)
      - [(SC) Préparation et réparation](#sc-préparation-et-réparation)
      - [(SC) Réintroduction](#sc-réintroduction)
      - [(SC) Prévention des fautes](#sc-prévention-des-fautes)
    - [Vues architecturales](#vues-architecturales)
      - [Présentation primaire](#présentation-primaire-1)
      - [Catalogue d'éléments](#catalogue-déléments-1)
      - [Diagramme de contexte](#diagramme-de-contexte)
      - [Guide de variabilité](#guide-de-variabilité-1)
      - [Raisonnement](#raisonnement-1)
      - [Vues associées](#vues-associées)
  - [Performance](#performance)
    - [Rédaction du scénario de qualité](#rédaction-du-scénario-de-qualité-1)
    - [ADD](#add-1)
      - [(SC) Contrôler la demande en ressources](#sc-contrôler-la-demande-en-ressources)
      - [(SC) Gérer les ressources](#sc-gérer-les-ressources)
    - [Vues architecturales](#vues-architecturales-1)
      - [Présentation primaire](#présentation-primaire-2)
      - [Catalogue d'éléments](#catalogue-déléments-2)
      - [Diagramme de contexte](#diagramme-de-contexte-1)
      - [Guide de variabilité](#guide-de-variabilité-2)
      - [Raisonnement](#raisonnement-2)
      - [Vues associées](#vues-associées-1)
  - [Testabilité](#testabilité)
    - [Rédaction du scénario de qualité](#rédaction-du-scénario-de-qualité-2)
    - [ADD](#add-2)
      - [(SC) Controle and observe l'état du système](#sc-controle-and-observe-létat-du-système)
      - [(SC) limiter la complexité](#sc-limiter-la-complexité)
    - [Vues architecturales](#vues-architecturales-2)
      - [Présentation primaire](#présentation-primaire-3)
      - [Catalogue d'éléments](#catalogue-déléments-3)
      - [Diagramme de contexte](#diagramme-de-contexte-2)
      - [Guide de variabilité](#guide-de-variabilité-3)
      - [Raisonnement](#raisonnement-3)
      - [Vues associées](#vues-associées-2)
  - [Modifiabilité](#modifiabilité)
    - [Rédaction du scénario de qualité](#rédaction-du-scénario-de-qualité-3)
    - [ADD](#add-3)
      - [(SC) Réduire la taille des modules](#sc-réduire-la-taille-des-modules)
      - [(SC) Augmenter la cohésion](#sc-augmenter-la-cohésion)
      - [(SC) Réduire le couplage](#sc-réduire-le-couplage)
      - [(SC) Defer binding](#sc-defer-binding)
    - [Vues architecturales](#vues-architecturales-3)
      - [Présentation primaire](#présentation-primaire-4)
      - [Catalogue d'éléments](#catalogue-déléments-4)
      - [Diagramme de contexte](#diagramme-de-contexte-3)
      - [Guide de variabilité](#guide-de-variabilité-4)
      - [Raisonnement](#raisonnement-4)
      - [Vues associées](#vues-associées-3)
  - [Sécurité](#sécurité)
    - [Rédaction du scénario de qualité](#rédaction-du-scénario-de-qualité-4)
    - [ADD](#add-4)
      - [(SC) Détecter les attaques](#sc-détecter-les-attaques)
      - [(SC) Résister aux attaques](#sc-résister-aux-attaques)
      - [(SC) Réagir aux attaques](#sc-réagir-aux-attaques)
      - [(SC) Récupérer d'une attaque](#sc-récupérer-dune-attaque)
    - [Vues architecturales](#vues-architecturales-4)
      - [Présentation primaire](#présentation-primaire-5)
      - [Catalogue d'éléments](#catalogue-déléments-5)
      - [Diagramme de contexte](#diagramme-de-contexte-4)
      - [Guide de variabilité](#guide-de-variabilité-5)
      - [Raisonnement](#raisonnement-5)
      - [Vues associées](#vues-associées-4)
  - [Convivialité](#convivialité)
    - [Rédaction du scénario de qualité](#rédaction-du-scénario-de-qualité-5)
    - [ADD](#add-5)
      - [(SC) Supporter l'initiative de l'usager](#sc-supporter-linitiative-de-lusager)
      - [(SC) Supporter l'initiative du système](#sc-supporter-linitiative-du-système)
    - [Vues architecturales](#vues-architecturales-5)
      - [Présentation primaire](#présentation-primaire-6)
      - [Catalogue d'éléments](#catalogue-déléments-6)
      - [Diagramme de contexte](#diagramme-de-contexte-5)
      - [Guide de variabilité](#guide-de-variabilité-6)
      - [Raisonnement](#raisonnement-6)
      - [Vues associées](#vues-associées-5)
  - [Interopérabilité](#interopérabilité)
    - [Rédaction du scénario de qualité](#rédaction-du-scénario-de-qualité-6)
    - [ADD](#add-6)
      - [(SC) Localiser](#sc-localiser)
      - [(SC) Gérer les interfaces](#sc-gérer-les-interfaces)
    - [Vues architecturales](#vues-architecturales-6)
      - [Présentation primaire](#présentation-primaire-7)
      - [Catalogue d'éléments](#catalogue-déléments-7)
      - [Diagramme de contexte](#diagramme-de-contexte-6)
      - [Guide de variabilité](#guide-de-variabilité-7)
      - [Raisonnement](#raisonnement-7)
      - [Vues associées](#vues-associées-6)
- [Compilation des vues architecturales](#compilation-des-vues-architecturales)
  - [Vues architectrurale de type Module](#vues-architectrurale-de-type-module)
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
>**Sujet de l'objectif:**
>**Objet de l'objectif**
>**Objectif:** 
>**Environnement:**  
>**Mesure de l'objectif:** 
>**Valeur:** 
### Démontrez la relation entre les éléments de votre architecture et votre scénario d'objectif d'affaire. 


## OA-2. Validez si le transport par autobus est toujours plus rapide, peu importe l'heure de la journée
>**Sujet de l'objectif:**
>**Objet de l'objectif**
>**Objectif:** 
>**Environnement:**  
>**Mesure de l'objectif:** 
>**Valeur:** 
### Démontrez la relation entre les éléments de votre architecture et votre scénario d'objectif d'affaire. 

# Cas d'utilisations
## CU01. Veux comparer les temps de trajet
>### Parties prenantes et intérêt
>- sks
>- sasf

>### Préconditions
>- a
>- b

>### Acteur principal: Nom
>1. sc
>2. sc
>3. sc..

>### Extension (scénarios alternatifs)
>4a. lslsl

### Démontrez la relation entre les éléments de votre architecture et votre cas d'utilisation.

## CU02. Veut pouvoir mettre le chaos dans les services en mode manuel
>### Parties prenantes et intérêt
>- sks
>- sasf

>### Préconditions
>- a
>- b

>### Acteur principal: Nom
>1. sc
>2. sc
>3. sc..

>### Extension (scénarios alternatifs)
>4a. lslsl
### Démontrez la relation entre les éléments de votre architecture et votre cas d'utilisation.

# Vue architecturale de contexte
Utiliser le gabarit suivant: https://wiki.sei.cmu.edu/confluence/display/SAD/Template%3AArchitectureViewTemplate
#### Présentation primaire
#### Catalogue d'éléments
#### <s>Diagramme de contexte</s>
  Pas nécessaire puisque c'est déja un vue de contexte
#### Guide de variabilité
#### Raisonnement
  
#### <s>Vues associées</s>
pas nécessaire puisque c'est la première vue que vous réalisé pour votre système.


# Scénarios de qualité

## Disponibilité
### Rédaction du scénario de qualité
>**Source:** 
>**Stimuli:**
>**Environnement:**
>**Artefact:** 
>**Réponse:**
>**Mesure de la réponse:** 

### ADD
#### (SC) détection de faute
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez vous choisi et pourquoi?

#### (SC) Préparation et réparation
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez vous choisi et pourquoi?

#### (SC) Réintroduction
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez vous choisi et pourquoi?

#### (SC) Prévention des fautes  
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez vous choisi et pourquoi?

### Vues architecturales
Utiliser le gabarit suivant: https://wiki.sei.cmu.edu/confluence/display/SAD/Template%3AArchitectureViewTemplate
#### Présentation primaire
#### Catalogue d'éléments
Doit inclure les liens vers les [documents d'interfaces](#documentation-des-interfaces).
#### Diagramme de contexte
#### Guide de variabilité
#### Raisonnement
  - Doit inclure les diagrammes de séquence qui ont permis de concevoir les interfaces des différents éléments
#### Vues associées


## Performance
### Rédaction du scénario de qualité
>**Source:** 
>**Stimuli:**
>**Environnement:**
>**Artefact:** 
>**Réponse:**
>**Mesure de la réponse:** 

### ADD
#### (SC) Contrôler la demande en ressources
###(SC) Gérer les ressources
<div class="concept performance">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

#### (SC) Gérer les ressources
<div class="concept performance">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

### Vues architecturales
Utiliser le gabarit suivant: https://wiki.sei.cmu.edu/confluence/display/SAD/Template%3AArchitectureViewTemplate
#### Présentation primaire
#### Catalogue d'éléments
#### Diagramme de contexte
#### Guide de variabilité
#### Raisonnement
  - Doit inclure les diagrammes de séquence qui ont permis de concevoir les interfaces des différents éléments
#### Vues associées

## Testabilité
### Rédaction du scénario de qualité
>**Source:** 
>**Stimuli:**
>**Environnement:**
>**Artefact:** 
>**Réponse:**
>**Mesure de la réponse:** 

### ADD

#### (SC) Controle and observe l'état du système
<div class="concept testabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

#### (SC) limiter la complexité
<div class="concept testabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

### Vues architecturales
Utiliser le gabarit suivant: https://wiki.sei.cmu.edu/confluence/display/SAD/Template%3AArchitectureViewTemplate
#### Présentation primaire
#### Catalogue d'éléments
#### Diagramme de contexte
#### Guide de variabilité
#### Raisonnement
  - Doit inclure les diagrammes de séquence qui ont permis de concevoir les interfaces des différents éléments
#### Vues associées


## Modifiabilité
### Rédaction du scénario de qualité
>**Source:** 
>**Stimuli:**
>**Environnement:**
>**Artefact:** 
>**Réponse:**
>**Mesure de la réponse:** 

### ADD
#### (SC) Réduire la taille des modules
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

#### (SC) Augmenter la cohésion
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

#### (SC) Réduire le couplage
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

#### (SC) Defer binding
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

### Vues architecturales
Utiliser le gabarit suivant: https://wiki.sei.cmu.edu/confluence/display/SAD/Template%3AArchitectureViewTemplate
#### Présentation primaire
#### Catalogue d'éléments
#### Diagramme de contexte
#### Guide de variabilité
#### Raisonnement
  - Doit inclure les diagrammes de séquence qui ont permis de concevoir les interfaces des différents éléments
#### Vues associées

## Sécurité
### Rédaction du scénario de qualité
>**Source:** 
>**Stimuli:**
>**Environnement:**
>**Artefact:** 
>**Réponse:**
>**Mesure de la réponse:** 

### ADD
#### (SC) Détecter les attaques
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

#### (SC) Résister aux attaques
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

#### (SC) Réagir aux attaques
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

#### (SC) Récupérer d'une attaque
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

### Vues architecturales
Utiliser le gabarit suivant: https://wiki.sei.cmu.edu/confluence/display/SAD/Template%3AArchitectureViewTemplate
#### Présentation primaire
#### Catalogue d'éléments
#### Diagramme de contexte
#### Guide de variabilité
#### Raisonnement
  - Doit inclure les diagrammes de séquence qui ont permis de concevoir les interfaces des différents éléments
#### Vues associées

## Convivialité
### Rédaction du scénario de qualité
>**Source:** 
>**Stimuli:**
>**Environnement:**
>**Artefact:** 
>**Réponse:**
>**Mesure de la réponse:** 

### ADD
#### (SC) Supporter l'initiative de l'usager
<div class="concept usabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

#### (SC) Supporter l'initiative du système
<div class="concept usabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

### Vues architecturales
Utiliser le gabarit suivant: https://wiki.sei.cmu.edu/confluence/display/SAD/Template%3AArchitectureViewTemplate
#### Présentation primaire
#### Catalogue d'éléments
#### Diagramme de contexte
#### Guide de variabilité
#### Raisonnement
  - Doit inclure les diagrammes de séquence qui ont permis de concevoir les interfaces des différents éléments
#### Vues associées

## Interopérabilité
### Rédaction du scénario de qualité
>**Source:** 
>**Stimuli:**
>**Environnement:**
>**Artefact:** 
>**Réponse:**
>**Mesure de la réponse:** 

### ADD
#### (SC) Localiser
<div class="concept interoperabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

#### (SC) Gérer les interfaces
<div class="concept interoperabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
Quelle tactique avez-vous choisie et pourquoi?

### Vues architecturales
Utiliser le gabarit suivant: https://wiki.sei.cmu.edu/confluence/display/SAD/Template%3AArchitectureViewTemplate
#### Présentation primaire
#### Catalogue d'éléments
#### Diagramme de contexte
#### Guide de variabilité
#### Raisonnement
  - Doit inclure les diagrammes de séquence qui ont permis de concevoir les interfaces des différents éléments
#### Vues associées

# Compilation des vues architecturales 
Vous devez utiliser les vues architecturales de qualité et combiner l'information pour réaliser les vues architecturales correspondantes aux 3 familles suivantes.  Vues de type module, composant et connecteur ainsi qu'allocation.
## Vues architecturales de type Module
### Vue #1
>#### Présentation primaire
>#### Catalogue d'éléments
>#### Guide de variabilité
>#### Raisonnement
>#### Vues associées
### Vue #2...

## Vues architecturales de type composant et connecteur
### Vue #1
>#### Présentation primaire
>#### Catalogue d'éléments
>#### Guide de variabilité
>#### Raisonnement
>#### Vues associées
### Vue #2...

## Vues architecturales de type allocation
### Vue #1
>#### Présentation primaire
>#### Catalogue d'éléments
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