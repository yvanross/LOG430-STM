########################################
############ Itération 2 ###############
########################################

############ Programmation #############

- Lecture des données pré-enregsitrés (dans le google drive de l'itération 1)
- hébergement de toutes les microservices sur ngrock --- url (sécurité)
- récupérer la liste des microservices disponibles des autres équipes (organisation des coordonnateurs)
- ChaosMonkey: latence (+variation) + Kill avec une ligne de commande, sur une ou plusieurs micro
- messages de healthcheck
- redondance
- vos micro et les micro des autres

###### Testabilité 1 #######

1- Spécifier un interval de temp et date (ex. votre client voudrais voir les donnés entre 22.Oct.2021 @ 13H00 et 22.oct.2021 @ 14h00) ... date future seulement
2- Configurer l'agrégateur de la moyenne des vitesses
3- Agréger les donnés temps réel sur une durée (fréquence) spécifique (ex. chaque 10 sec ou chaque 10 min) dans cet interval spécifique, sur un topic spécifique de votre choix
4- récupérer les données (temps) de mqtt (temps réel)
5- Afficher les donnés temps réel sur votre terminal (mqtt) puis stocker le output de l'agrégation et du fournisseur externe (google/waze....) dans une BD/fichier 

###### Testabilité 2 #######

1- Spécifier un interval de temp et date (ex. votre client voudrais voir les donnés entre 20.Juin.2021 @ 13H00 et 22.Juin.2021 @ 13h00) ... date passé seulement
2- Configurer l'agrégateur de la moyenne des vitesses
3- Agréger les donnés temps passé sur une durée (fréquence) spécifique (ex. chaque 10 sec ou chaque 10 min) dans cet interval spécifique, sur le topic spécifique des vitesses
4- récupérer les données pré-enregistrés (temps) de mqtt
Soit par:
*Un Lecteur MQTT: Un microservice d'un lecteur/parser/loader qui fait une lecture direct d'un fichier de données pré-enregistrés
OU
Un Simulateur MQTT:
*Un microservice qui fait l'émission en temps réel de ces données pré-enregistrés (service en-ligne)
5- Afficher les donnés temps réel sur votre terminal (mqtt) puis stocker le output de l'agrégation et du fournisseur (google/waze....) dans une BD/fichier 

###### Disponibilité - Latence #######
###### sur les données temps réel - si mqtt est down, sur le données pré-enregistrés

1- Vos 2 fournisseurs internes (mqtt) et externes (google/waze....) continuent à fournir des données 
(deux messages qui le prouvent: ping-echo + heartbeat)
2- Simuler une latence (ex. TimeOut) sur une microservice
3- Le système détecte automatiquement que il y une latence
4- Afficher un message de 'Latence' sur votre consol
5- Le système déclenche/active automatiquement la solution
(redondance active + redondance passive + re-démarrage)
6- Afficher un message de 'l'activation de la solution' sur votre console (un message dès que ça arrive)
7- Afficher les messages de ping-echo + heartbeat sur votre terminal
8- Afficher les donnés temps réel sur votre terminal (mqtt) puis stocker le output de l'agrégation et du fournisseur (google/waze....) dans une BD/fichier
9- répéter 2 en modifiant la latence, puis continuer jusqu'à 8

###### Disponibilité - Crash #######
###### sur les données temps réel - si mqtt est down, sur le données pré-enregistrés

1- Vos 2 fournisseurs internes (mqtt) et externe (google/waze....) continuent à fournir des données 
(deux messages qui le prouve: ping-echo + heartbeat)
2- Simuler un crash (ex. kill) sur une microservice
3- Le système détect automatiquement qu' il y a un crash
4- Afficher un message de 'crash' sur votre consol
5- Le système déclenche/active automatiquement la solution 
(redondance active + redondance passive + re-démarrage: vos services seulement)
6- Afficher un message de l'activation de la solution' sur votre console (un message dès que ça arrive)
7- Afficher les messages de ping-echo + heartbeat sur votre terminal
8- Afficher les donnés temps réel sur votre terminal (mqtt) puis stocker le output de l'agrégation et du fournisseur (google/waze....) dans une BD/fichier
9- répéter 2 en faisant un kill sur 2 autres microservice ensemble, puis continuer jusqu'à 8
10- répéter 2 en faisant un kill sur les micro service du fournisseur externe, puis continuer jusqu'à 8 et récupérer les données d'un fournisseur externe (ex: google Map, waze…) d'autre équipe

###### Interopérabilité #######

après un crash sur une de vos microservice, votre système switch au micro service d'une autre équipe

###### Sécurité #######
votre service ne soit accessible que pour les équipes autorisée

########################################
######################################## 
########################################
############ Documentation #############

Documentation de l’architecture des microservices implémenté par l’équipe
Respect du template de documentation d’une vue architecture 
https://wiki.sei.cmu.edu/confluence/display/SAD/Template%3AArchitectureViewTemplate
Respect du template pour la documentation des interfaces
https://wiki.sei.cmu.edu/confluence/display/SAD/Template%3AInterfaceTemplate

############## Rapport des équipes (SAD)
- Documenter la vue architecturale de chaque microservice
- Décrivez les vues architecturales de votre système selon l'approche "Views and Beyond" du SEI en utilisant la Notation UML.
- Assurez-vous que votre documentation démontre adéquatement comment sont supportés les cas d'utilisation et les exigences de qualité.
- N'oubliez pas le code de couleur.
- Assurez-vous que votre documentation démontre adéquatement comment sont supportés les cas d'utilisation et les exigences de qualité spécifiques à cette itération.
- Documenter chacune des tactiques utilisées dans cette solution
- Générer les rapports spécifiques pour chacune des parties prenantes.
- Expliquer quelle est la différence entre la redondance active et passive avec des chiffres (obtenus de vos expérimentations) pour appuyer votre explication.
- Avez-vous utilisé des tactiques autres que celles minimalement exigées dans ce document. Si oui, expliquer.
- Identifier les points de risque
- Identifier les points de non risque
- Identifier les points de compromis 
- Identifier les points de sensibilité 
- Liste des décisions architecturales prises durant cette itération 

############## Rapport des coordonnateurs (SAD)

- Respect du template SAD pour la documentation d’une vue architecture globale
- Respect du template SAD pour la documentation de toutes les interfaces
- Respect du template SAD pour la documentation de toutes les microservice
- Ajout   du template SAD pour la traçabilité des responsabilités de chaque équipe

########################################
############ Itération 2 ###############
########################################