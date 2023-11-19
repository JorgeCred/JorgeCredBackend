const express = require('express')
const bodyParser = require('body-parser')
const path = require('path')
const amqp = require('amqplib/callback_api')
const config = require('../src/config')

const amqp_conn = config.amqp_conn

const app = express()

app.use(express.static(path.join(__dirname, 'static')))

app.use(bodyParser.json())

app.post('/send', (req, res) => {
    console.log('Sending Push to queue...')
    const body = req.body
    amqp.connect(amqp_conn, function (error0, connection) {
        if (error0) {
            throw error0
        }
        connection.createChannel(function (error1, channel) {
            if (error1) {
                throw error1
            }
            let exchange = 'Bitu'
            channel.assertExchange(exchange, 'direct', {
                durable: true,
            })
            channel.publish(exchange, '', Buffer.from(JSON.stringify(body)))
            console.log(' [x] Sent %s', body)
        })

        setTimeout(function () {
            connection.close()
        }, 500)
    })
    res.status(201).json({})
    console.log('Push in queue.')
})

const port = 5000

app.listen(port, () => console.log(`Server started on port ${port}`))
