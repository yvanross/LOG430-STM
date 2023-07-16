#!/bin/bash

# Start RabbitMQ server
rabbitmq-server -detached

# Wait for RabbitMQ startup
echo "Waiting for RabbitMQ to start..."
while ! rabbitmqctl status >/dev/null 2>&1; do
    sleep 1
done

rabbitmqctl start_app

# Create user and set permissions
rabbitmqctl add_user david secret
rabbitmqctl set_user_tags david administrator
rabbitmqctl set_permissions -p / david ".*" ".*" ".*"

# Stop RabbitMQ server
rabbitmqctl stop

# Start RabbitMQ server in foreground
rabbitmq-server
