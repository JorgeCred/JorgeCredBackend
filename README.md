# JorgeCred
JorgeCred é uma aplicação bancária fake.

## Stack utilizada:
- **NodeJS**
    - Typescript
    - HTML
    - SCSS
- **C#**
    - ASP.NET
- **RabbitMQ**
- **PostgreSQL**

## Link para o repositório
https://github.com/JorgeCred/JorgeCredBackend

## Participantes
- Caio Broering Pinho - (22215427)
- Gustavo da Rocha Pereira - (22200498)
- Igor Zimmer Gonçalves - (22202682)
- Luan Rodrigues Boschini - (22200503)
- Vinicius Guazzelli Dias - (22200520)

## Como rodar o código?

### Frontend
Em um novo terminal, digite:
> cd .\src\  
cd .\JorgeCredBackend\  
cd .\FRONTEND\  
cd .\JorgeCredFE\  

Na pasta JorgeCredFE, se for sua **primeira vez** rodando o projeto, digite:
> npm install  
npm start

Se **não** for sua primeira vez rodando o projeto, digite:
> npm start

Vá até o http://localhost:4200/ em seu navegador de preferência.

### Backend
Em um outro terminal, digite:
> cd .\src\
cd .\JorgeCredBackend\  
cd .\Orchestrator\  
cd .\JorgeCred\  
cd .\src\  
cd .\JorgeCred.API\  
dotnet run

Em outro terminal, digite:
> cd .\src\
cd .\JorgeCredBackend\  
cd .\NotificationManager\  
npm install  
npm start