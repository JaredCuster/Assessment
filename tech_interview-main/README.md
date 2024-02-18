# tech_interview

Provides scripts to build and run the backend code challenge for Entrata.

## Files to send to Candidate
We'd like the candidate to demonstrate writing an API. To help them get to a
working solution we've provided a sql file and a script to simulate traffic.

Send the candidate all files inside the `candidate` folder.

Before the candidate sets up the environment, they'll need to have Postgres and
curl installed on their machine.

Here are the steps for the Candidate to get up and running.

1. Run `setup_database.sh` if postgres is installed or `docker-compose up` if
   docker is installed. This will create a database named `entrata` and seed it
   with units and leases.
2. Run `post_leases.sh`. This will begin sending data to the `/leases` endpoint
   via POST.

## Helper Files
The `lease_generator.py` file is a script that can generate the lease data in
jsonl format.
