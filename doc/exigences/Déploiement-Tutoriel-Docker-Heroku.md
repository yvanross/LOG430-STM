
## Déploiement Docker sur Heroku

### Docker

[Docker](https://www.docker.com/) est une technologie de virtualisation qui permet de développer et de déployer des logiciels dans des environnements isolés. C'est une alternative rapide et performante aux machines virtuelles.

### Heroku

[Heroku](https://www.heroku.com/) est une plateforme infonuagique qui offre de nombreux services, dont le déploiement et l'hébergement de logiciels.

## Étapes

### 1. Installer et configurer Docker

**Pour les utilisateurs de Windows :**

Ouvrez l'utilitaire _Activer ou désactiver des fonctionnalités Windows_.

- [ ] activez Hyper-V et Sous-système Windows pour Linux.

- [ ] redémarrez votre ordinateur.

**Pour tous les utilisateurs :**

- [ ] assurez-vous que votre ordinateur satisfait les prérequis nécessaires pour installer Docker sur [Windows](https://docs.docker.com/docker-for-windows/install/) ou [macOS](https://docs.docker.com/docker-for-mac/install/).

- [ ] installez [Docker Desktop](https://www.docker.com/get-started).

- [ ] exécutez Docker. Une icône apparaîtra dans la zone de notifications.

- [ ] effectuez un clic droit sur l'icône et sélectionnez _Switch to Linux containers_.

- [ ] ouvrez la ligne commande (PowerShell sur Windows, Terminal sur macOS).

- [ ] exécutez <code>docker version</code> pour confirmer l'installation de Docker. Vous devriez obtenir un résultat similaire à ceci :

    ```
    Client: Docker Engine - Community
      Cloud integration: 1.0.4
      Version:           20.10.2
      API version:       1.41
      ...
    ```

**Pour les utilisateurs de Windows, si vous obtenez une erreur :**

- [ ] exécutez la commande suivante : <code>& 'C:\Program Files\Docker\Docker\DockerCli.exe' -SwitchDaemon</code>, puis ouvrez à nouveau la ligne de commande.

### 2. Télécharger une image Docker

Une image Docker est un ensemble de scripts et de fichiers de configuration. De nombreuses images sont disponibles publiquement sur [Docker Hub](https://hub.docker.com/) pour vous aider à démarrer votre projet.

- [ ] cherchez l'image **php:7.1-apache** sur [Docker Hub](https://hub.docker.com/).

**Note :** Le développement du projet de session ne sera pas effectué en PHP, mais cette image est plus complète que celle d'Apache

- [ ] la télécharger grâce à <code>docker pull php:7.1-apache</code>.

- [ ] <code>docker images</code> affichera l'ensemble des images Docker sur votre ordinateur. Vous devriez obtenir un résultat similaire à ceci :

    ```
    REPOSITORY   TAG          IMAGE ID       CREATED          SIZE
    php          7.1-apache   b9858ffdd4d2   13 months ago    401MB
    ```

### 3. Créer et exécuter un conteneur

Dans Docker, un conteneur est une image exécutée dans un environnement. Il est possible d'avoir plusieurs conteneurs créés à partir de la même image.

- [ ] créez et exécutez un nouveau conteneur grâce à <code>docker run php:7.1-apache</code>. Vous pouvez ajouter l'option <code>--name \<nom de votre conteneur></code> afin de lui donner un nom personnalisé. Sinon, un nom lui sera attribué automatiquement.

- [ ] <code>docker ps</code> affichera les conteneurs qui s'exécutent sur votre ordinateur. L'option <code>- a</code> permet d'afficher l'ensemble des conteneurs.

    ```
    CONTAINER ID   IMAGE            COMMAND                  CREATED          STATUS          PORTS     NAMES
    ddee231c2232   php:7.1-apache   "docker-php-entrypoi…"   22 seconds ago   Up 17 seconds   80/tcp    distracted_stonebraker
    ```

- [ ] arrêtez l'exécution du conteneur grâce à <code>docker stop \<id ou nom du conteneur></code>.

:warning: Par défaut, les conteneurs n'ont aucun mécanisme de persistance des données. Toute modification sera perdue à l'arrêt de l'exécution.

- [ ] démarrez à nouveau le conteneur grâce à <code>docker start \<id ou nom du conteneur></code>.

- [ ] arrêtez à nouveau le conteneur et supprimez le grâce à <code>docker rm \<id ou nom du conteneur></code>.

Pour supprimer une image, tous les conteneurs qui l'utilisent doivent d'abord être supprimés. Par la suite, exécuter <code>docker rmi \<nom de l'image></code>

Par défaut, lorsqu'un conteneur est créé, il est isolé, car ses ports sont fermés. Puisqu'Apache est un serveur Web, il communique grâce au port <code>80</code>. Notre application, quant à elle, communiquera grâce au port <code>8080</code>.

- [ ] créer un nouveau conteneur en liant le port <code>8080</code> de l'ordinateur au port <code>80</code> du conteneur : <code>docker run -p 8080:80 php:7.1-apache</code>.

Ouvrir votre navigateur à l'adresse <code>127.0.0.1:8080</code> devrait afficher le message suivant. C'est parce qu'il n'y a aucun fichier dans le serveur.

    Forbidden
    You don't have permission to access this resource.

    Apache/2.4.38 (Debian) Server at 127.0.0.1 Port 8080

### 4. Déboguer le conteneur

- [ ] consultez les logs du conteneur grâce à <code>docker logs \<id ou nom du conteneur></code>.

- [ ] il est aussi possible d'accéder à l'environnement du conteneur grâce à <code>docker exec -it \<id ou nom du conteneur> bash</code>. Quittez l'environnement grâce à <code>exit</code>.

:warning: Cette commande doit être utilisée à des fins de débogage seulement. Elle ne doit pas être utilisée pour modifier le code ou la configuration du conteneur.

### 5. Créer une image Docker personnalisée

Dans le cadre de ce laboratoire, votre code se résume à une configuration et un fichier HTML.

- [ ] dans le dépôt git de votre projet, créez un dossier <code>src/partie1</code> qui accueillera votre code et ajoutez y le fichier <code>index.html</code>, qui contiendra le code suivant : 

    ```html
    <!DOCTYPE html>
    <html>
        <body><h1>Bonjour!</h1></body>
    </html>
    ```

Un fichier Dockerfile permet d'automatiser l'installation et la configuration du conteneur.

- [ ] à la racine du projet, créez un fichier nommé <code>Dockerfile</code> et ajoutez-y le contenu suivant :

    ```docker
    FROM php:7.1-apache

    COPY ./src/partie1/ /var/www/html/
    ```

La commande <code>FROM</code> permet de bâtir votre Dockerfile sur une image existante.

La commande <code>COPY</code> permet de copier du contenu sur votre ordinateur à l'emplacement spécifié dans le conteneur.

La commande <code>RUN</code> n'est pas utilisée, mais elle permet d'exécuter une commande dans l'environnement du conteneur.

La commande <code>CMD</code> n'est pas utilisée, mais elle permet d'exécuter une commande pour démarrer l'application (ex.: <code>CMD ["python", "main.py"]</code>). Il ne peut y avoir qu'une commande de ce type.

### 6. Créer un conteneur grâce à Docker Compose

Au début du tutoriel, vous avez créé un conteneur grâce à la commande <code>docker run</code>. Docker Composer permet d'automatiser cette étape et de définir des paramètres pour conteneur.

- [ ] à la racine du projet, créez le fichier docker-compose.yaml et ajoutez-y le contenu suivant :

    :warning: Les fichiers .yaml sont sensibles à l'indentation!

    ```yaml
    version: '3'
    services:
        lab0:
            image: lab0:1.0
            ports:
                - 8080:80
    ```

<code>version</code> permet de spécifier la version de Docker Compose à utiliser.

<code>services</code> défini l'ensemble des conteneurs à créer pour le projet.

<code>lab0</code> est le nom de l'image à créer.

<code>image</code> est l'image de base utilisée pour ce conteneur.

<code>ports</code> permet d'ouvrir un ou plusieurs ports du conteneur et de les lier à ceux de votre ordinateur.

- [ ] à partir de la racine, créez votre conteneur grâce à <code>docker build -t lab0:1.0 .</code>

Dans Docker, <code>1.0</code> est appelé un _tag_.

Le <code>.</code> représente l'emplacement du Dockerfile (le dossier actuel)

:warning: Vous devrez recréer l'image à chaque changement dans le Dockerfile. Si vous ne changez pas le _tag_ de l'image Docker, vous devrez d'abord supprimer les conteneurs qui l'utilisent et supprimer l'image.

- [ ] <code>docker images</code> devrait afficher votre nouvelle image :

    ```
    REPOSITORY   TAG          IMAGE ID       CREATED          SIZE
    lab0         1.0          cd753b6a0e4c   8 seconds ago    401MB
    php          7.1-apache   b9858ffdd4d2   13 months ago    401MB
    ```

- [ ] démarrez le conteneur <code>lab0:1.0</code> grâce à <code>docker-compose -f docker-compose.yaml up</code>.

Ouvrir votre navigateur à l'adresse <code>127.0.0.1:8080</code> devrait afficher le message suivant :

    Bonjour!

- [ ] arrêtez le conteneur <code>lab0:1.0</code> grâce à <code>docker-compose -f docker-compose.yaml down</code>. Le conteneur sera aussi retiré.

### 7. Déployer le conteneur en ligne

- [ ] créez une nouvelle branche <code>heroku</code> qui sera utilisée pour le déploiement et poussez (commit) son contenu sur GitHub :

    ```powershell
    git checkout -b heroku
    git add .
    git commit -m "Setup autodeployement"
    git push --set-upstream origin heroku
    ```

- [ ] retournez sur la branche master avec <code>git checkout master</code>.

- [ ] créez un compte [Heroku](https://www.heroku.com/).

- [ ] téléchargez et installez [Heroku CLI](https://devcenter.heroku.com/articles/heroku-cli), puis redémarrez la ligne de commande.

- [ ] connectez-vous à Heroku CLI grâce à <code>heroku auth:login</code>.

- [ ] à partir du tableau de bord, créez un nouveau _pipeline_ et nommez-le <code>gti540-lab0</code>.

- [ ] connectez-le à votre projet GitHub.

- [ ] dans Staging, créer une nouvelle application nommée gti540-lab0.

Par défaut, la nouvelle application n'utilise pas Docker pour le déploiement.

- [ ] activez le déploiement par conteneur grâce à <code>heroku stack:set container --app gti540-lab0</code>.

Dans votre _pipeline_, cliquez sur l'application nouvellement créée et accédez à la section Deploy.

- [ ] dans Automatic deploys, choisissez la branche <code>heroku</code> et cliquez sur _Enable Automatic Deploys_.

- à la racine de votre projet, créez le fichier <code>heroku.yml</code> et ajoutez-y le contenu suivant :

    ```
    build:
        docker:
            web: Dockerfile
    ```

Heroku assigne un port dynamique à votre application. Toutefois, Apache a été configuré pour écouter sur le projet <code>80</code>. Il faut modifier sa configuration.

- [ ] à la racine du projet, créez le fichier <code>ports.conf</code> et ajoutez-y le contenu suivant :

    ```
    Listen ${PORT}
    ```

- [ ] à la racine du projet, créez le fichier <code>apache.conf</code> et ajoutez-y le contenu suivant :

    ```apache
    <VirtualHost *:${PORT}>
        ServerAdmin webmaster@localhost
        DocumentRoot /var/www/html

        ErrorLog ${APACHE_LOG_DIR}/error.log
        Customlog ${APACHE_LOG_DIR}/access.log combined
    </VirtualHost>
    ```

- [ ] exécutez la commande suivante : <code>heroku labs:enable --app=gti540-lab0 runtime-new-layer-extract</code>.

- [ ] dans le Dockerfile, ajoutez les lignes suivantes :

    ```docker
    COPY ./ports.conf /etc/apache2/ports.conf
    COPY ./apache.conf /etc/apache2/sites-available/000-default.conf
    ```

- [ ] faites un _commit_ de votre code sur la branche _master_ et sur la branche _heroku_. L'application sera déployée automatiquement.

## 8. Célébrer votre succès

Bravo! Vous êtes maintenant familier avec des outils de livraison en continu.

<span style="font-size:20px"><strong>N'oubliez pas : déployez rapidement, déployez souvent!</strong></span>

## Références

[Introduce the foundation pillars of DevOps: Culture and Lean Product](https://docs.microsoft.com/en-us/learn/modules/introduce-foundation-pillars-devops)

[DevOps](https://software.af.mil/training/devops/)

[Docker Tutorial for Beginners](https://www.youtube.com/watch?v=3c-iBn73dDE)

[Deploying Docker on Heroku](https://www.youtube.com/watch?v=4axmcEZTE7M)
