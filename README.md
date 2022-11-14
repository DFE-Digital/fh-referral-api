# fh-referral-api
Api for the management of a person/family referral to a Local Authority, Voluntary, Charitable or Faith organisation.

## Requirements

* DotNet Core 6.0 and any supported IDE for DEV running.

## About

This API is for the management of a person/family referrals to a Local Authority, Voluntary, Charitable or Faith organisations.

This repos has been built using the "Clean Architecture Design" taken from Steve Smith (ardalis)

## Local running

In the appsetting.json file set the UseDbType with one of the following options:

* "UseInMemoryDatabase"
* "SqlServerDatabase"
* "Postgres"

The startup project is: FamilyHubs.ServiceDirectoryApi.Api
Starting the API will then show the swagger definition with the available operations.

## Migrations

To Add Migration
<br />
dotnet ef migrations add CreateIntialSchema -c ApplicationDbContext 
<br />
dotnet ef database update -c ApplicationDbContext


#Rabbit Mq Server (in Docker)

To use the Rabbit Mq Option in appsetting.json set "UseRabbitMQ": true

<br />
Docker Rabbit mq Server
<br />
docker run -d --hostname my-rabbitmq-server --name rabbitmq -p 5672:5672 -p 15672:15672 rabbitmq:3-management
or
docker run -p 5672:5672 -p 15672:15672 rabbitmq:management   
