# Exigences fonctionnelles

Vous devez rédiger les cas d'utilisation supplémentaire permettant de réaliser les objectifs d'affaires que vous avez rédigés.

## ChargéDeLaboratoire
### CU01. Veux comparer les temps de trajet (1pts)
  1. le CL sélectionne une intersection de départ et une intersection d'arrivée, ainsi que le taux de rafraichissement de la prise de mesure.
  1. Le CL sélectionne les sources de données qu'il veut utiliser. [Service externe](service-externe.md) et donné temps réel de la STM.
  2. Le système affiche un graphique du temps de déplacement et met celui-ci à jour selon le taux de rafraichissement.
  3. Le CL peut récupérer le fichier de données et générer ses propres graphiques à l'aide d’Excel</span>.
> #### Cas alternatifs (1pts)
>  - 2.a [Service externe](service-externe.md): Utiliser plusieurs [services externes](service-externe.md) disponibles pour faire le comparatif.

### CU02. Veux pouvoir mettre le chaos dans les services en mode.
  - **Option 1**: Manuel <b>(1pts)</b>
    1.1. Le CL consulte la liste des microservices avec leur latence moyenne.
    1.2. Le CL change la latence d'un ou plusieurs microservices</span>.
  
  - **Option 2**:  Automatique <b>(1pts)</b>
    2.1. Le CL sélectionne le mode automatique tout en spécifiant la fréquence de la perturbation en seconde.
    2.2. Le système détruit<sup>1</sup> un microservice de façon aléatoire a tous les x secondes</span>.
  
  - Le système conserve un log des différents changements apportés que nous pourrons utiliser pour vérifier les données accumulées.

> #### Cas alternatifs <b>(1pts)</b>
>  - 1.2.a Le CL détruit un ou plusieurs microservices</span>.
>  - 1.2.b Le CL détruit tous les microservices d'une équipe.
</span>
