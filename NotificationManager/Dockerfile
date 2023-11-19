FROM node:alpine
LABEL authors="caiopinho"

WORKDIR /app

COPY package*.json ./
COPY src src

ARG PRIVATE_VAPID_KEY
ARG PUBLIC_VAPID_KEY
ARG RABBITMQ_CONN
ARG QUEUE

RUN npm install

EXPOSE 3000

# Start the app using serve command
CMD [ "npm", "start" ]