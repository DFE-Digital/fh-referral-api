# fh-referral-api
Api for the management of a person/family referral to a Local Authority, Voluntary, Charitable or Faith organisation.
#Rabbit Mq Server (in Docker)
docker run -d --hostname my-rabbitmq-server --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
or
docker run -p 5672:5672 -p 15672:15672 rabbitmq:management   
