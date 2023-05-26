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

# Laboratoire #2

Groupe: 0x

Equipe: 0x

# Évaluation de la participation
|nom de l'étudiant| Facteur multiplicatif|
|:---------------:|:--------------------:|
|Jean Travaillant  |          1           |
|Joe Paresseux  |          0.75        |
|Jules Procrastinateu|        0.5         |
|Jeanne Parasite |        0.25         |
|Jay Oublié|      0         |

# Introduction
>TODO: insérer votre introduction

# Cas d'utilisations
- [CU02 - Mettre le chaos dans les microservices en termes de disponibilité.](#cu02){#da-cu02}

- [CU01 - Veux comparer les temps de trajet.](#cu01){#da-cu01}
- [CU05 - Veux informer l'administrateur sur les changements de statut des microservices.](#cu05){#da-cu05}
- [CU06 - Veux pouvoir récupérer le temps de trajets d'un service externe.](#cu06){#da-cu06}
- [CU09 - Veux pouvoir récupérer le temps de trajet de STM](#cu09){#da-cu09}

# Attributs de qualité

- Vous devez vous assurer que les attributs de qualité (A) associés à chacun de vos cas d'utilisation soient documentés et réalisés. Pour chacun des attributs de qualité (A), vous devrez concevoir et réaliser une architecture qui utilisera au minimum une tactique architecturale pour chacune des sous-catégories (SC) suivantes.

- Si un attribut de qualité ou une sous-catégorie (SC) n'est pas représenté dans votre architecture, vous devez ajouter de nouveau cas d'utilisation tant que tous n'auront pas été couverts.

- [Vues architecturales de disponibilité](#disponibilité){#da-disponibilite}
- Vous devez fournir les diagrammes de séquence démontrant le fonctionnenemnt de l'architecturte actuelle.
- Vous devez fournir les diagrammes de séquence démontrant le fonctionnement de l'architecture optimale, incluant:
  - L'état au démarrage,
  - La mécanique de détection d'un problème
  - La mécanique de rétablissement du service, de récupération ou de reconfiguration.

# Vues architecturales 
- [Vues architecturales de type Module](#vues-module){#da-vues-module}
- [Vues architecturales de type composant et connecteur](#vues-cetc){#da-vues-cetc}
- [Vues architecturales de type allocation](#vues-allocation){#da-vues-allocation}

# Conclusion
>TODO: insérer votre conclusion


- N'oubliez pas d'effacer les TODO
- Générer une version PDF de ce document pour votre remise finale.
- Assurez vous du bon format de votre rapport PDF.
- Créer un tag git avec la commande "git tag iterationX"


\newpage
# Annexes

\pagebreak

