const webPush = require('web-push')
const amqp = require('amqplib')
const config = require('./config')

const queue = config.queue
const amqp_conn = config.amqp_conn
const publicVapidKey = config.publicVapidKey
const privateVapidKey = config.privateVapidKey
const jorgeCredIconUrl = config.jorgeCredIconUrl

webPush.setVapidDetails('mailto:test@test.com', publicVapidKey, privateVapidKey)

function pushNotification(message) {
    const requestBody = JSON.parse(message.content)

    const subscription = {
        endpoint: requestBody.endpoint,
        keys: requestBody.keys,
    }

    const payload = JSON.stringify({
        title: requestBody.title,
        body: requestBody.body,
        icon: jorgeCredIconUrl,
    })

    console.log(
        ` [*] Push: title: '${requestBody.title}', body: '${requestBody.body}'`
    )

    webPush
        .sendNotification(subscription, payload)
        .catch((err) => console.error(err))
}

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
