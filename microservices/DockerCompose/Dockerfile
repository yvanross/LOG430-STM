FROM rabbitmq:3.8-management

#RUN rabbitmq-plugins enable --offline rabbitmq_management

COPY .erlang.cookie /var/lib/rabbitmq/.erlang.cookie
RUN chown rabbitmq:rabbitmq /var/lib/rabbitmq/.erlang.cookie && \
    chmod 600 /var/lib/rabbitmq/.erlang.cookie

# Copy the RabbitMQ configuration file into the Docker image
COPY rabbitmq.conf /etc/rabbitmq/

# Copy the startup script into the Docker image
#COPY startup.sh /startup.sh
#RUN chmod +x /startup.sh
#RUN chmod +x /startup.sh

#CMD ["/bin/bash", "/startup.sh"]
