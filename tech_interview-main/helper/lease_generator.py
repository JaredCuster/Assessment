import json
import random
from datetime import datetime, timedelta

# Parameters
number_of_units = 100
lease_duration_days = 90
start_date = datetime(2025, 1, 1)

# Function to generate a random date within the year
def random_date_generator(start):
    end = start + timedelta(days=365)
    random_date = start + (end - start) * random.random()
    return random_date

# Generate lease information
leases = []
unit_lease_count = {unit_id: 0 for unit_id in range(1, number_of_units + 1)}

for unit_id in range(1, number_of_units + 1):
    max_capacity = (unit_id % 4) + 1
    for slot in range(random.randint(1, max_capacity)):
        move_in_date = random_date_generator(start_date)
        move_out_date = move_in_date + timedelta(days=lease_duration_days)
        lease = {
            "move_in": move_in_date.strftime("%Y-%m-%dT%H:%M:%S"),
            "move_out": move_out_date.strftime("%Y-%m-%dT%H:%M:%S"),
            "resident_id": unit_id * 100 + slot,  # Example pattern for resident IDs
            "resident_name": f"Resident {unit_id * 100 + slot}",
            "unit_id": unit_id
        }
        leases.append(lease)
        unit_lease_count[unit_id] += 1

# Add leases to reach max capacity for each unit
for unit_id, count in unit_lease_count.items():
    while count < max_capacity:
        move_in_date = random_date_generator(start_date)
        move_out_date = move_in_date + timedelta(days=lease_duration_days)
        lease = {
            "move_in": move_in_date.strftime("%Y-%m-%dT%H:%M:%S"),
            "move_out": move_out_date.strftime("%Y-%m-%dT%H:%M:%S"),
            "resident_id": unit_id * 100 + count,  # Continue with the resident ID pattern
            "resident_name": f"Resident {unit_id * 100 + count}",
            "unit_id": unit_id
        }
        leases.append(lease)
        count += 1

random.shuffle(leases)

##
# Edge cases
#

move_in_date = random_date_generator(start_date)
move_out_date = move_in_date + timedelta(days=lease_duration_days)

full_unit = {
    "move_in": move_in_date.strftime("%Y-%m-%dT%H:%M:%S"),
    "move_out": move_out_date.strftime("%Y-%m-%dT%H:%M:%S"),
    "resident_id": 100,
    "resident_name": f"Resident 1",
    "unit_id": 1
}

leases.append(full_unit)

invalid_unit = {
    "move_in": move_in_date.strftime("%Y-%m-%dT%H:%M:%S"),
    "move_out": move_out_date.strftime("%Y-%m-%dT%H:%M:%S"),
    "resident_id": 10001,
    "resident_name": f"Resident 10001",
    "unit_id": 1000
}

leases.append(invalid_unit)

# Write to JSONL file
with open('leases.jsonl', 'w') as outfile:
    for lease in leases:
        json_record = json.dumps(lease)
        outfile.write(f"{json_record}\n")

print("Lease data generated in leases.jsonl.")

