#!/bin/bash

read -p "Enter the name of your local database copy: " DATABASE_NAME
read -p "Enter your database user, used to access the local database copy: " DATABASE_USER
read -p "Enter your database password, used to access the local database copy: " DATABASE_PASSWORD
read -p "Enter your proff api key. If you don't have access to one, the update function does not work: " PROFF_API_KEY
read -p "Enter your desired default login email for the local copy of PgAdmin: " PGADMIN_DEFAULT_LOGIN
read -p "Enter your desired password associated with the default login for your local copy of PgAdmin: " PGADMIN_DEFAULT_PW
read -p "Enter the apps tenant-id in Azure: " TENANT_ID
read -p "Enter the apps client-id in Azure: " CLIENT_ID

echo "All of these values can be changed later in the .env file."

echo "DATABASE_HOST=postgres" > .env
echo "DATABASE_USER=$DATABASE_USER" >> .env
echo "DATABASE_NAME=$DATABASE_NAME" >> .env
echo "DATABASE_PASSWORD=$DATABASE_PASSWORD" >> .env
echo "PROFF_API_KEY=$PROFF_API_KEY" >> .env
echo "PGADMIN_DEFAULT_LOGIN=$PGADMIN_DEFAULT_LOGIN" >> .env
echo "PGADMIN_DEFAULT_PW=$PGADMIN_DEFAULT_PW" >> .env
echo "TENANT_ID=$TENANT_ID" >> .env
echo "CLIENT_ID=$CLIENT_ID" >> .env
echo "LOCAL_ID=00000003-0000-0000-c000-000000000000" >> .env

echo ".env file created successfully."