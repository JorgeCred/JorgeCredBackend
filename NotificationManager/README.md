# Notification Manager

Criado por Caio Broering Pinho

### Objetivo:

Esse modulo é responsável por fazer a leitura da fila do RabbitMQ e enviar as notificações solicitadas.

### Objeto da Notificação:

Para enviar uma notificação é necessário que seja enviado para a fila um JSON contendo as seguintes informações:

- endpoint: Enviado do frontend para localizar o dispositivo do usuário.
- keys: Enviado do frontend para localizar o dispositivo do usuário.
- title: Titulo da notificação.
- body: Conteúdo da notificação.

