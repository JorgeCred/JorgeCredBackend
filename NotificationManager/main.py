from os import environ

import pika
import sys

from dotenv import load_dotenv

load_dotenv()

RABBITMQ_USERNAME = environ['RABBITMQ_USERNAME']
RABBITMQ_PASSWORD = environ['RABBITMQ_PASSWORD']
RABBITMQ_HOST = environ['RABBITMQ_HOST']
RABBITMQ_PORT = environ['RABBITMQ_PORT']

credentials = pika.PlainCredentials(RABBITMQ_USERNAME, RABBITMQ_PASSWORD)
parameters = pika.ConnectionParameters(
    RABBITMQ_HOST,
    RABBITMQ_PORT,
    RABBITMQ_USERNAME,
    credentials,
)

connection = pika.BlockingConnection(parameters)

channel = connection.channel()

channel.exchange_declare(exchange='Bitu', exchange_type='direct', durable=True)

message = ' '.join(sys.argv[1:]) or "info: Hello World!"
channel.basic_publish(exchange='Bitu', routing_key='', body=message)
print(f" [x] Sent {message}")
connection.close()
