import pika

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

result = channel.queue_declare(queue='Bilu', exclusive=False, durable=True)
queue_name = result.method.queue

channel.queue_bind(exchange='Bitu', queue=queue_name)

print(' [*] Waiting for logs. To exit press CTRL+C')

def callback(ch, method, properties, body):
    print(f" [x] {body}")

channel.basic_consume(
    queue=queue_name, on_message_callback=callback, auto_ack=True)

channel.start_consuming()