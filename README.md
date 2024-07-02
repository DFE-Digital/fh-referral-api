# fh-referral-api
Api for the management of a person/family referral to a Local Authority, Voluntary, Charitable or Faith organisation.

## Requirements

* DotNet Core 7.0 and any supported IDE for DEV running.

## About

This API is for the management of a person/family referrals to a Local Authority, Voluntary, Charitable or Faith organisations.

This repos has been built using the "Clean Architecture Design" taken from Steve Smith (ardalis)

## Local running

In the appsetting.json file you set the Database to be Sql Server or SQLite


The startup project is: FamilyHubs.Referral.Api
Starting the API will then show the swagger definition with the available operations.

## Migrations

To Add Migration

<br />
 dotnet ef migrations add CreateIntialSchema --project ..\FamilyHubs.Referral.Data\FamilyHubs.Referral.Data.csproj
<br />

To Apply Latest Schema Manually

<br />
 dotnet ef database update --project ..\FamilyHubs.Referral.Data\FamilyHubs.Referral.Data.csproj
<br />

## cypress tests
Run the API (debug or non debug both fine)
open powershell at ..\fh-referral-api\tests\cypress

<br />
 npx cypress open 
<br />
