# Jorge Cred - A banking Application
- Summary:
  - Architecture description;
  - Folder tree;
  - Architecture diagram;

## Architecture Description:
  - **Orchestrator**: Responsible for managing the HTTP interactions with the GraphQL server;
  - **Statistics**: Delivers insights about the users using the application;
  - **Transaction**: Manages the transactions in the system;
  - **Notifier**: Notifies everything that happens in the user (mainly transactions);
 
## Folder tree:
```
.
└── JorgeCredManager/
    ├── Orchestrator (Made with C#, Gustavo R. Pereira responsability)
    ├── StatisticsManager (Made with ??, Luan R. Boschini)
    ├── TransactionManager (Made with ??, Igor Zimmer)
    └── NotificationManager (Made with ??, Caio B. Pinho)
```
 
## Architecture Diagram:

![alt text](https://github.com/JorgeCred/JorgeCredBackend/blob/main/JorgeCredArchitecture.png)
