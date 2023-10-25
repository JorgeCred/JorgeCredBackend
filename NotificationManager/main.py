import pika
import sys

credentials = pika.PlainCredentials('fzrrkjya', '4hKfmc3shjyOXU4Jr_ptSem0hh5LfP-0')
parameters = pika.ConnectionParameters(
    'chimpanzee.rmq.cloudamqp.com',
    5672,
    'fzrrkjya',
    credentials,
)

connection = pika.BlockingConnection(parameters)

channel = connection.channel()

channel.exchange_declare(exchange='Bitu', exchange_type='direct', durable=True)

message = ' '.join(sys.argv[1:]) or "info: Hello World!"
channel.basic_publish(exchange='Bitu', routing_key='', body=message)
print(f" [x] Sent {message}")
connection.close()


