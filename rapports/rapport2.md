
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
    .disponibilite tr:nth-child(2) { background: orange; }
    .performance tr:nth-child(2) { background: orange; }
    .securite tr:nth-child(2) { background: orange; }
    .usabilite tr:nth-child(2) { background: orange; }
    .interoperabilite tr:nth-child(2) { background: orange; }
    .modifiabilite tr:nth-child(2) { background: orange; }
    .testabilite tr:nth-child(2) { background: orange; }    
</style>

# Rapport #2
- [Rapport #2](#rapport-2)
- [Introduction](#introduction)
  - [Conception dirigée par les attributs (ADD)](#conception-dirigée-par-les-attributs-add)
  - [Disponibilité](#disponibilité)
    - [(SC) détection de faute](#sc-détection-de-faute)
    - [(SC) Préparation et réparation](#sc-préparation-et-réparation)
    - [(SC) Réintroduction](#sc-réintroduction)
    - [(SC) Prévention des fautes](#sc-prévention-des-fautes)
    - [Présentation primaire](#présentation-primaire)
    - [Catalogue d'éléments](#catalogue-déléments)
    - [Interfaces](#interfaces)
  - [Performance](#performance)
    - [(SC) Contrôler la demande en ressources](#sc-contrôler-la-demande-en-ressources)
    - [(SC) Gérer les ressources](#sc-gérer-les-ressources)
    - [Présentation primaire](#présentation-primaire-1)
    - [Catalogue d'éléments](#catalogue-déléments-1)
    - [Interfaces](#interfaces-1)
  - [Testabilité](#testabilité)
    - [(SC) Controle and observe l'état du système](#sc-controle-and-observe-létat-du-système)
    - [(SC) limiter la complexité](#sc-limiter-la-complexité)
    - [Présentation primaire](#présentation-primaire-2)
    - [Catalogue d'éléments](#catalogue-déléments-2)
    - [Interfaces](#interfaces-2)
  - [Modifiabilité](#modifiabilité)
    - [(SC) Réduire la taille des modules](#sc-réduire-la-taille-des-modules)
    - [(SC) Augmenter la cohésion](#sc-augmenter-la-cohésion)
    - [(SC) Réduire le couplage](#sc-réduire-le-couplage)
    - [(SC) Defer binding](#sc-defer-binding)
    - [Présentation primaire](#présentation-primaire-3)
    - [Catalogue d'éléments](#catalogue-déléments-3)
    - [Interfaces](#interfaces-3)
  - [Sécurité](#sécurité)
    - [(SC) Détecter les attaques](#sc-détecter-les-attaques)
    - [(SC) Résister aux attaques](#sc-résister-aux-attaques)
    - [(SC) Réagir aux attaques](#sc-réagir-aux-attaques)
    - [Présentation primaire](#présentation-primaire-4)
    - [Catalogue d'éléments](#catalogue-déléments-4)
    - [Interfaces](#interfaces-4)
  - [Convivialité](#convivialité)
    - [(SC) Supporter l'initiative de l'usager](#sc-supporter-linitiative-de-lusager)
    - [(SC) Supporter l'initiative du système](#sc-supporter-linitiative-du-système)
    - [Présentation primaire](#présentation-primaire-5)
    - [Catalogue d'éléments](#catalogue-déléments-5)
    - [Interfaces](#interfaces-5)
  - [Interopérabilité](#interopérabilité)
    - [(SC) Localiser](#sc-localiser)
    - [(SC) Gérer les interfaces](#sc-gérer-les-interfaces)
    - [Présentation primaire](#présentation-primaire-6)
    - [Catalogue d'éléments](#catalogue-déléments-6)
    - [Interfaces](#interfaces-6)
- [Conclusion](#conclusion)
# Introduction
>TODO: Vous devez concevoir une architecture permettant de satisfaire les objectifs d'affaire, les cas d'utilisations et les scénarios de qualité sélectionnés par les coordonnateurs à l'itération #1.
>TODO: insérer votre introduction

Ce rapport contient les résultats de l'application du processus ADD
## Conception dirigée par les attributs (ADD)
- On fait un brainstorm et chaque membre de l'équipe propose une rangée
- On essaie de mettre une valeur et un cout à chaque rangée, (H)igh, (M)edium, (L)ow
- On identifie en orange les approches retenues dans la solution finale
- Normalement on cherche les concepts de design qui ont une plus grande valeur au plus faible cout.

## [Disponibilité](rapport1-coordonateurs.md#disponibilité)

### (SC) détection de faute
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
### (SC) Préparation et réparation
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
### (SC) Réintroduction
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
### (SC) Prévention des fautes  
<div class="concept disponibilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>

### Présentation primaire
TODO : Ajoutez ici le diagramme (ou la représentation non graphique) qui montre les éléments et les relations dans cette vue. Indiquez la langue ou la notation utilisée. S'il ne s'agit pas d'une notation standard telle qu’UML, ajoutez une clé de notation. Peut-être un diagramme de type module, composant & connecteur, allocation, séquence, activité, état.
### Catalogue d'éléments
TODO : Cette section peut être organisée comme un dictionnaire où chaque entrée est un élément de la présentation principale. Pour chaque élément, fournissez des informations et des propriétés supplémentaires dont les lecteurs auraient besoin et qui ne rentreraient pas dans la présentation principale. En option, vous pouvez ajouter des spécifications d'interface et des diagrammes de comportement (par exemple, des diagrammes de séquence UML, des diagrammes d'états).
### Interfaces
Documentation des interfaces de votre présentation primaire en utilisant le [gabarit suivant](template-interface.md).

## [Performance](rapport1-coordonateurs.md#performance)
### (SC) Contrôler la demande en ressources
###(SC) Gérer les ressources
<div class="concept performance">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
### (SC) Gérer les ressources
<div class="concept performance">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>

### Présentation primaire
  TODO : Ajoutez ici le diagramme (ou la représentation non graphique) qui montre les éléments et les relations dans cette vue. Indiquez la langue ou la notation utilisée. S'il ne s'agit pas d'une notation standard telle qu’UML, ajoutez une clé de notation. Peut-être un diagramme de type module, composant & connecteur, allocation, séquence, activité, état.
### Catalogue d'éléments
TODO : Cette section peut être organisée comme un dictionnaire où chaque entrée est un élément de la présentation principale. Pour chaque élément, fournissez des informations et des propriétés supplémentaires dont les lecteurs auraient besoin et qui ne rentreraient pas dans la présentation principale. En option, vous pouvez ajouter des spécifications d'interface et des diagrammes de comportement (par exemple, des diagrammes de séquence UML, des diagrammes d'états).
### Interfaces
Documentation des interfaces de votre présentation primaire en utilisant le [gabarit suivant](template-interface.md).

## [Testabilité](rapport1-coordonateurs.md#testabilité)
### (SC) Controle and observe l'état du système
<div class="concept testabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>

### (SC) limiter la complexité
<div class="concept testabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
### Présentation primaire
TODO : Ajoutez ici le diagramme (ou la représentation non graphique) qui montre les éléments et les relations dans cette vue. Indiquez la langue ou la notation utilisée. S'il ne s'agit pas d'une notation standard telle qu’UML, ajoutez une clé de notation. Peut-être un diagramme de type module, composant & connecteur, allocation, séquence, activité, état.
### Catalogue d'éléments
TODO : Cette section peut être organisée comme un dictionnaire où chaque entrée est un élément de la présentation principale. Pour chaque élément, fournissez des informations et des propriétés supplémentaires dont les lecteurs auraient besoin et qui ne rentreraient pas dans la présentation principale. En option, vous pouvez ajouter des spécifications d'interface et des diagrammes de comportement (par exemple, des diagrammes de séquence UML, des diagrammes d'états).
### Interfaces
Documentation des interfaces de votre présentation primaire en utilisant le [gabarit suivant](template-interface.md).

## [Modifiabilité](rapport1-coordonateurs.md#modifiabilité)
### (SC) Réduire la taille des modules
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
### (SC) Augmenter la cohésion
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
### (SC) Réduire le couplage
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
### (SC) Defer binding
<div class="concept modifiabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
### Présentation primaire
TODO : Ajoutez ici le diagramme (ou la représentation non graphique) qui montre les éléments et les relations dans cette vue. Indiquez la langue ou la notation utilisée. S'il ne s'agit pas d'une notation standard telle qu’UML, ajoutez une clé de notation. Peut-être un diagramme de type module, composant & connecteur, allocation, séquence, activité, état.
### Catalogue d'éléments
TODO : Cette section peut être organisée comme un dictionnaire où chaque entrée est un élément de la présentation principale. Pour chaque élément, fournissez des informations et des propriétés supplémentaires dont les lecteurs auraient besoin et qui ne rentreraient pas dans la présentation principale. En option, vous pouvez ajouter des spécifications d'interface et des diagrammes de comportement (par exemple, des diagrammes de séquence UML, des diagrammes d'états).
### Interfaces
Documentation des interfaces de votre présentation primaire en utilisant le [gabarit suivant](template-interface.md).

## [Sécurité](rapport1-coordonateurs.md#sécurité)
### (SC) Détecter les attaques
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
### (SC) Résister aux attaques
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
### (SC) Réagir aux attaques
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
###(SC) Récupérer d'une attaque
<div class="concept securite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
### Présentation primaire
TODO : Ajoutez ici le diagramme (ou la représentation non graphique) qui montre les éléments et les relations dans cette vue. Indiquez la langue ou la notation utilisée. S'il ne s'agit pas d'une notation standard telle qu’UML, ajoutez une clé de notation. Peut-être un diagramme de type module, composant & connecteur, allocation, séquence, activité, état.
### Catalogue d'éléments
TODO : Cette section peut être organisée comme un dictionnaire où chaque entrée est un élément de la présentation principale. Pour chaque élément, fournissez des informations et des propriétés supplémentaires dont les lecteurs auraient besoin et qui ne rentreraient pas dans la présentation principale. En option, vous pouvez ajouter des spécifications d'interface et des diagrammes de comportement (par exemple, des diagrammes de séquence UML, des diagrammes d'états).
### Interfaces
Documentation des interfaces de votre présentation primaire en utilisant le [gabarit suivant](template-interface.md).

## [Convivialité](rapport1-coordonateurs.md#usabilité)
### (SC) Supporter l'initiative de l'usager
<div class="concept usabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
### (SC) Supporter l'initiative du système
<div class="concept usabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
### Présentation primaire
TODO : Ajoutez ici le diagramme (ou la représentation non graphique) qui montre les éléments et les relations dans cette vue. Indiquez la langue ou la notation utilisée. S'il ne s'agit pas d'une notation standard telle qu’UML, ajoutez une clé de notation. Peut-être un diagramme de type module, composant & connecteur, allocation, séquence, activité, état.
### Catalogue d'éléments
TODO : Cette section peut être organisée comme un dictionnaire où chaque entrée est un élément de la présentation principale. Pour chaque élément, fournissez des informations et des propriétés supplémentaires dont les lecteurs auraient besoin et qui ne rentreraient pas dans la présentation principale. En option, vous pouvez ajouter des spécifications d'interface et des diagrammes de comportement (par exemple, des diagrammes de séquence UML, des diagrammes d'états).
### Interfaces
Documentation des interfaces de votre présentation primaire en utilisant le [gabarit suivant](template-interface.md).

## [Interopérabilité](rapport1-coordonateurs.md#usabilité)
### (SC) Localiser
<div class="concept interoperabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>
### (SC) Gérer les interfaces
<div class="concept interoperabilite">

|Concept de design| Pour | Contre| Valeur | Cout|
|-----------------|------|-------|--------|-----|
| <li>tactique 1</li>|avantages| désavantages|M|M|
| <li>tactique 2</li>|avantages| désavantages|M|M|
| <li>tactique 3</li>|avantages| désavantages|M|M|
</div>

### Présentation primaire
TODO : Ajoutez ici le diagramme (ou la représentation non graphique) qui montre les éléments et les relations dans cette vue. Indiquez la langue ou la notation utilisée. S'il ne s'agit pas d'une notation standard telle qu’UML, ajoutez une clé de notation. Peut-être un diagramme de type module, composant & connecteur, allocation, séquence, activité, état.
### Catalogue d'éléments
TODO : Cette section peut être organisée comme un dictionnaire où chaque entrée est un élément de la présentation principale. Pour chaque élément, fournissez des informations et des propriétés supplémentaires dont les lecteurs auraient besoin et qui ne rentreraient pas dans la présentation principale. En option, vous pouvez ajouter des spécifications d'interface et des diagrammes de comportement (par exemple, des diagrammes de séquence UML, des diagrammes d'états).
### Interfaces
Documentation des interfaces de votre présentation primaire en utilisant le [gabarit suivant](template-interface.md).


# Conclusion
>TODO: insérer votre introduction


N'oubliez pas d'effacer les TODO et ce texte et de générer une version PDF de ce document pour votre remise finale.
Créer un tag git avec la commande "git tag rapport2"




