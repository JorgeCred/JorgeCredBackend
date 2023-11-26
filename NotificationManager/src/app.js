const webPush = require('web-push')
const amqp = require('amqplib')
const config = require('./config')

const queue = config.queue
const amqp_conn = config.amqp_conn
const publicVapidKey = config.publicVapidKey
const privateVapidKey = config.privateVapidKey
const jorgeCredIconUrl = config.jorgeCredIconUrl
const emailVapid = config.emailVapid

webPush.setVapidDetails(`mailto:${emailVapid}`, publicVapidKey, privateVapidKey)

/**
 * Recebe o conteudo e endereço da notificação e envia para o browser
 * @param message json que possui as chaves endpoint, keys, title e body
 */
function pushNotification(message) {
    const requestBody = JSON.parse(message.content).message

    console.log(requestBody)

    const payload = JSON.stringify({
        title: requestBody.title,
        body: requestBody.message,
        icon: jorgeCredIconUrl,
    })

    console.log(
        ` [*] Push: title: '${requestBody.title}', body: '${requestBody.body}'`
    )

    try {
        const targetCredentials = JSON.parse(
            requestBody.targetUserBrowserCredentials
        )
        sendNotification(targetCredentials, payload)
    } catch (e) {
        console.log(e)
    }

    try {
        const sourceCredentials = JSON.parse(
            requestBody.sourceUserBrowserCredentials
        )
        sendNotification(sourceCredentials, payload)
    } catch (e) {
        console.log(e)
    }
}

function sendNotification(credentials, payload) {
    const {
        Endpoint: endpoint,
        Keys: { Auth: auth, p256dh },
    } = credentials

    const subscription = {
        endpoint,
        keys: { auth, p256dh },
    }

    webPush
        .sendNotification(subscription, payload)
        .catch((err) => console.error(err))
}

/**
 * Leitura a fila e envia notificações ao receber
 */
;(async () => {
    try {
        const connection = await amqp.connect(amqp_conn)
        const channel = await connection.createChannel()

        process.once('SIGINT', async () => {
            await channel.close()
            await connection.close()
        })

        await channel.assertQueue(queue, { durable: true })
        await channel.consume(
            queue,
            (message) => {
                if (message) {
                    try {
                        pushNotification(message)
                    } catch (err) {
                        console.error(err)
                    }
                }
            },
            { noAck: true }
        )

        console.log(' [*] Waiting for messages. To exit press CTRL+C')
    } catch (err) {
        console.warn(err)
    }
})()
