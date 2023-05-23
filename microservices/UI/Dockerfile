# Stage 1: Build the Angular app
FROM node:14 AS build
WORKDIR /app
COPY package.json ./
COPY package-lock.json ./
RUN npm ci
COPY . .
RUN npm run build

# Stage 2: Serve the Angular app with Nginx
FROM nginx:1.21
COPY --from=build /app/dist/ui /usr/share/nginx/html
COPY nginx.conf /etc/nginx/conf.d/default.conf
