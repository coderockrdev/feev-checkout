FROM node:24 AS build

WORKDIR /app

COPY . .

EXPOSE 4200

ENTRYPOINT ["npm", "run", "ng", "serve", "--", "--host", "0.0.0.0"]
