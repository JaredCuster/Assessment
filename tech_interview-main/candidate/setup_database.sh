#!/bin/bash

# Assign default postgres user and desired database name
POSTGRES_USER=${POSTGRES_USER:-postgres}  # Default to 'postgres' if not set
POSTGRES_DB=${POSTGRES_DB:-entrata}  # Set your desired database name
POSTGRES_HOST=${POSTGRES_HOST:-localhost}  # Default to 'localhost' if not set
POSTGRES_PORT=${POSTGRES_PORT:-5432}  # Default to '5432' if not set

# SQL file path
SQLFILE="dataset/init.sql"

# Ask for the PostgreSQL password
echo "Please enter the password for PostgreSQL user $POSTGRES_USER:"
read -s PGPASSWORD

# Export the password so that subsequent commands don't prompt for it
export PGPASSWORD

# Create the database
createdb -U "$POSTGRES_USER" -h "$POSTGRES_HOST" -p "$POSTGRES_PORT" "$POSTGRES_DB" 2>/dev/null
CREATION_STATUS=$?

# Check if the database was created successfully
if [ $CREATION_STATUS -eq 0 ]; then
    echo "Database $POSTGRES_DB created successfully."
elif [ $CREATION_STATUS -eq 1 ]; then
    echo "Database $POSTGRES_DB already exists."
else
    echo "Failed to create database $POSTGRES_DB."
    exit $CREATION_STATUS
fi

# Run the SQL file
psql -U "$POSTGRES_USER" -h "$POSTGRES_HOST" -p "$POSTGRES_PORT" -d "$POSTGRES_DB" -f "$SQLFILE"
EXECUTION_STATUS=$?

# Check if the SQL file was executed successfully
if [ $EXECUTION_STATUS -eq 0 ]; then
    echo "SQL file executed successfully."
else
    echo "Failed to execute SQL file."
    exit $EXECUTION_STATUS
fi

# Unset the password for security reasons
unset PGPASSWORD

