package main

import (
	"encoding/json"
	"fmt"
	"log"

	amqps "github.com/rabbitmq/amqp091-go"
)

func failOnError(err error, msg string) {
	if err != nil {
		log.Panicf("%s: %s", msg, err)
	}
}

type transactionMessage struct {
	// defining struct variables
	SenderId    int
	RecipientId int
	Amount      float64
	RequestDate string
	RequestCity string
}

func main() {
	conn, err := amqps.Dial("PLACEHOLDER KEY")
	failOnError(err, "Failed to connect to RabbitMQ")
	defer conn.Close()

	ch, err := conn.Channel()
	failOnError(err, "Failed to open a channel")
	defer ch.Close()

	err = ch.ExchangeDeclare(
		"Transaction", // name
		"direct",      // type
		true,          // durable
		false,         // auto-deleted
		false,         // internal
		false,         // no-wait
		nil,           // arguments
	)
	failOnError(err, "Failed to declare an exchange")

	q, err := ch.QueueDeclare(
		"TransactionQueue", // name
		true,               // durable
		false,              // delete when unused
		false,              // exclusive
		false,              // no-wait
		nil,                // arguments
	)
	failOnError(err, "Failed to declare a queue")

	err = ch.QueueBind(
		q.Name,        // queue name
		"",            // routing key
		"Transaction", // exchange
		false,
		nil,
	)
	failOnError(err, "Failed to bind a queue")

	msgs, err := ch.Consume(
		q.Name, // queue
		"",     // consumer
		true,   // auto-ack
		false,  // exclusive
		false,  // no-local
		false,  // no-wait
		nil,    // args
	)
	failOnError(err, "Failed to register a consumer")

	var forever chan struct{}

	go func() {
		for d := range msgs {
			var transaction transactionMessage
			err := json.Unmarshal(d.Body, &transaction)
			if err != nil {
				fmt.Println(err)
				fmt.Print(d.Body)
			}

			fmt.Printf("senderID: %v\n", transaction.SenderId)
			fmt.Printf("recipientID: %v\n", transaction.RecipientId)
			fmt.Printf("transactionAmount: %v\n", transaction.Amount)
			fmt.Printf("requestDate: %v\n", transaction.RequestDate)
			fmt.Printf("requestCity: %v\n", transaction.RequestCity)
		}
	}()

	log.Printf(" [*] Waiting for logs. To exit press CTRL+C")
	<-forever
}
