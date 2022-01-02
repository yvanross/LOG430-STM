![log](../architecture/logo-logti.png)
# LOG430 - Architecture logicielle¶

## Introduction
Dans le cadre de ce projet, on cherche à créer une application de calcul de temps de trajet se servant de différents microservices. La partie de l'application qui reçoit les requêtes de temps de trajet est aussi un microservice. Les microservices n'ont pas besoin d'être déployés sur un service d'hébergement externe.  Ce sera le sujet d'une prochaine itération ou d'un prochain cours.

Pour débuter votre projet nous vous fournissons une architecture de référence. Vous pouvez apporter toutes les modifications nécessaires à cet architecture pour permettre de satisfaire les exigences clients.

## Gabarit de réalisation
Vous pouvez utiliser les gabarits suivants pour vous aider à réaliser chacune des étapes de ce projet. 
1. [Software Architecture Documentation Template](https://wiki.sei.cmu.edu/confluence/display/SAD/Software+Architecture+Documentation+Template)
2. (Architecture View Template)(https://wiki.sei.cmu.edu/confluence/display/SAD/Template%3AArchitectureViewTemplate)
3. (Interface Template)(https://wiki.sei.cmu.edu/confluence/display/SAD/Template%3AInterfaceTemplate)



### Api de la ville de Montréal
Analyser les interfaces (API) de la plateforme de la ville de Montréal pour l’accès aux données en temps réel au niveau des capteurs sur le réseau routier à partir du site Websocket Client (from Paho) http://mqtt.cgmu.io/.¶

### Liste des capteurs sur une carte
https://www.google.com/maps/d/u/0/edit?mid=1Xm1J7Zuwp0Ra5GGbws8C4N7WcuNMT3M-&ll=45.5176601142996%2C-73.51752064002218&z=14

## Acteurs


## Acteurs
### ChargéDeLaboratoire
- CU2-1. Veut récupérer la liste des microservices disponibles
- CU2-2. Veut pouvoir obtenir la latence moyenne de chaque microservice
- CU2-3. Veut pouvoir ajouter une latence à un ou plusieurs microservices 
- CU2-4. Veut pouvoir ajouter une latence à tous les microservices d'une équipe
- CU2-5. Veut pouvoir détruire<sup>1</sup> un microservice de façon aléatoire 
- CU2-6. Veut pouvoir détruire<sup>1</sup> tous les microservices d'une équipe 
- CU2-7. Veut pouvoir écrire un script qui modifiera la latence ou détruira des microservices.
- CU2-8. Veut obtenir les temps de trajet suite aux modification de latence ou de destruction de service.
- CU2-9. Veut pouvoir obtenir les temps de trajet en utilisant les microservices d'une seule équipe

note 1: Envoie un signal à un microservice pour qu'il se termine automatiquement. 

### Équipes
- CU2-10. Intégrer un simulateur permettant de continuer le développement lorsque les services de la ville de Montréal ne sont plus disponibles.
- CU2-11. Permettre d'utiliser les données <s>en temps réel ou les données</s> de simulation.
- CU2-12. Intégration de ChaosMonkey

## Cas d'utilisation
- CU1. Votre système permettra d’estimer le temps nécessaire pour parcourir la distance à partir des données en temps réel de la ville de Montréal. De l'est vers l'ouest et de l'ouest vers l'est.
- CU2-1 Intégration pour la comparaison des temps de trajet de tous les microservices externes sur une durée de 3h avec le graphique Excel.
- CU2-2 Intégration pour la comparaison des temps de trajet de tous les microservices internes et externes sur une période de 24h avec le graphique Excel.
- <span style="color:red"><s>CU3-1. Récupérer les données GPS d'un mobile</s>  </span>
- <span style="color:red"><s>CU3-2. Transmettre les données GPS d'un mobile à votre système et sauvegarde des données de trajets calculés.</s></span>
- <span style="color:red"><s>CU3-3. Simuler la production de données GPS d'un mobile</s></span>  



# Rapport
- Décrivez les vues architecturales de votre système selon l'approche "Views and Beyond" du SEI en utilisant la Notation UML.
- Assurez vous que votre documentation démontre adéquatement comment sont supportés les cas d'utilisation et les exigences de qualité à chaque itération
- Documenter chacune des tactiques utilisées dans cette solution
- N'oubliez pas le code de couleur.


## Répondez aux questions suivantes
1. Expliquer quelle est la différence entre la redondance active et passive avec des chiffres (obtenus de vos expérimentations) pour appuyer votre explication.
1. Avez vous utilisé des tactiques autre que celle minimalement exigées dans ce document. Si oui, expliquer.

2. Identifier les points de risque
3. Identifier les points de non risque
4. Identifier les points de compromis 
5. Identifier les points de sensibilité </s>
6. Liste des décisions architecturales prises durant cette itération

## Commentaires
- Quelle est l'appréciation globale de votre équipe par rapport à ce projet?
- Comment pouvons nous rendre ce laboratoire plus pertinents, plus intéressant?
- Avez vous d'autre commentaires?

## Charge de laboratoire
- Vous êtes intéressé par une charge de laboratoire de LOG430. Contactez nous le plus rapidement possible.

## Itération #3
Le but de cette itération est d'utiliser des données de positionnement GPS en temps réel pour valider la précision des algorithmes de calcul du temps de trajet. 

Vous devez configuer votre système pour pouvoir expérimenter les calculs de temps de trajet sur le terrain en réel. Idéalement vous devriez être en mesure de comparer simultanément les algorithmes de toutes les équipes de votre sous-groupe.
