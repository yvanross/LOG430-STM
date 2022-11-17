#### Identité de l'interface
Ceci est le microservice web REST nommé **Authentification**. Le but principal de ce service est d'authentifier et d'autoriser les clients de l'application pour effectuer des opérations.
#### Ressources
*Token signup(User user)* \
Enregistre un utilisateur dans le système, dans la base de données géré par le microservice d'authentification. Le microservice vérifie la conformité de l'adresse email donnée ainsi que du mot de passe, puis enregistre l'utilisateur si il n'existe pas déjà. \
**Pré-conditions**
- Le paramètre utilisateur ne peut être null : une adresse mail valide et un mot de passe valide.
- L'utilisateur ne doit pas déjà exister dans le système.
  **Post-conditions**
- Le système retourne un Token d'autorisation pour l'utilisateur qui vient de s'enregistrer, valide 1 heure.
  **Erreurs**
- L'utilisateur existe déjà dans le système.

*Token login(User user)* \
Authentifie un utilisateur dans le système, en le récupérant dans la base de données à partir de son adresse email et de son mot de passe. L'adresse email et le mot de passe doivent êtres valides et correspondre à un utilisateur enregsitré. \
**Pré-conditions**
- Le paramètre utilisateur ne peut être null : une adresse mail valide et un mot de passe valide.
- L'utilisateur doit exister dans le système.
- L'adresse email et le mot de passe doivent matcher avec un utilisateur existant.
  **Post-conditions**
- Le système retourne un Token d'autorisation pour l'utilisateur qui vient de s'enregistrer, valide 1 heure.
  **Erreurs**
- L'utilisateur n'existe pas dans le système.
- Trop de tentatives ont été essayées avec un utilisateur invalide.


*void verify(Token token)* \
Autorise un utilisateur à l'aide de son token, donné dans le header "Authorization" de la requête HTTP, sous la forme 'Bearer <token>'. \
**Pré-conditions**
- Le token ne peut pas être null.
- Le token doit être valide : Il doit avoir été généré par le microservice auparavant.
- Le token ne peut pas être expiré.
  **Post-conditions**
- Le système retourne un message d'autorisation pour l'utilisateur.
  **Erreurs**
- Le token n'est pas valide.
- Trop de tentatives ont été essayées avec un token invalide.


#### Types de données et constantes
**Type User** \
Ce type représente un utilisateur du système, enregistré en base de données.
- String email
- String password
  **Type Token** \
  Ce type représente un token d'autorisation de type JWT.
- String token
#### La gestion des erreurs
- 404 Utilisateur n'existe pas
- 401 Token invalide
- 409 Utilisateur existe déjà en BDD
- 500 Erreur générique du serveur
- 400 Mot de passe ou email invalide
- 429 Trop de requêtes eronnées

#### Variabilité
La variabilité est configurée dans un fichier .env dans le dossier racine de l'application
- APP_NAME le nom de l'application
- DEV_KEY Clé secrète d'accès à la base de données.
- MDB_URI URL pointant vers la base de données MongoDB.
- NODE_ENV Environnement d'exécution de l'application Node. Dev ou Prod.

#### Caractéristiques des attributs de qualité
Les attributs de qualités suivants sont ici disponibles :
- Disponibilité
- Performance
- Interopérabilité
- Modifiabilité
- Testabilité
- Sécurité
#### Problèmes de justification et de conception
- MongoDB a été utilisé ici pour une meilleure intégration avec notre microservice codé en Javascript
- Un Access Token JWT est ici retourné, ce qui est plus simple pour notre application tout en satisfaisant les contraintes de sécurité de nos objectifs d'affaires.
- Ce service d'authentification a été externalisé dans un microservice indépendant afin de respecter la doctrine du "Separation of Concerns"
#### Guide d'utilisation
POST https://portainer.log430-20223-02.logti.etsmtl.ca.nip.io/signup \
body : {"username": "John.Smith54@gmail.com", "password": "HelloWorld1234!"}

POST https://portainer.log430-20223-02.logti.etsmtl.ca.nip.io/login \
body : {"username": "John.Smith54@gmail.com", "password": "HelloWorld1234!"}

GET https://portainer.log430-20223-02.logti.etsmtl.ca.nip.io/verify \
Authorization-header: Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJFUzI1NiIsImtpZCI6IjNkMmI0NzNlMDYxN2I5OGY0MDAxNGZkYjFmMjVkNDYwIn0.e30.V_Y4tiO-fNchQjszRnIgHbjbvlILK_H2G33OsV4sBXHGnJKvBFOQsCWSjrHRuQQue4G1ByaGIreLdzsrqa1aog
