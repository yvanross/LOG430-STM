FROM rabbitmq:3.8-management

COPY .erlang.cookie /var/lib/rabbitmq/.erlang.cookie
RUN chown rabbitmq:rabbitmq /var/lib/rabbitmq/.erlang.cookie && \
    chmod 600 /var/lib/rabbitmq/.erlang.cookie

# Copy the RabbitMQ configuration file into the Docker image
COPY rabbitmq.conf /etc/rabbitmq/