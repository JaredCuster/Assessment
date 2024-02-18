#!/bin/bash

# Default API endpoint configuration
HOST="localhost"
DEFAULT_PORT="8080"
PORT="$DEFAULT_PORT"  # Default to the standard port

# Process flags
while [[ "$#" -gt 0 ]]; do
    case $1 in
        --port) PORT="$2"; shift ;;  # If --port is provided, set the PORT variable
        *) echo "Unknown parameter passed: $1"; exit 1 ;;
    esac
    shift
done

API_ENDPOINT="http://$HOST:$PORT/leases"
JSONL_FILE="leases.jsonl"

# Check if the JSONL file exists
if [ ! -f "$JSONL_FILE" ]; then
    echo "The file $JSONL_FILE does not exist."
    exit 1
fi

# Read the JSONL file line by line
while IFS= read -r line
do
    # POST the lease data to the API
    response=$(curl --silent --write-out "HTTPSTATUS:%{http_code}" -X POST -H "Content-Type: application/json" -d "$line" "$API_ENDPOINT")

    # Extract the body and the status
    body=$(echo "$response" | sed -e 's/HTTPSTATUS\:.*//g')
    status=$(echo "$response" | tr -d '\n' | sed -e 's/.*HTTPSTATUS://')

    # Print the response
    if [ "$status" -eq 200 ] || [ "$status" -eq 201 ]; then
        echo "Lease created successfully: $body"
    else
        echo "Failed to create lease: $body"
    fi

    # Sleep for 100 ms
    sleep 0.1
done < "$JSONL_FILE"