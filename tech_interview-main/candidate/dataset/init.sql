DO $$
BEGIN
    -- Check if the 'leases' table exists
    IF EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'leases') THEN
        -- Truncate the 'leases' table and restart identity
        EXECUTE 'TRUNCATE TABLE public.leases RESTART IDENTITY CASCADE';
    END IF;

    -- Check if the 'units' table exists
    IF EXISTS (SELECT FROM pg_tables WHERE schemaname = 'public' AND tablename = 'units') THEN
        -- Truncate the 'units' table and restart identity
        EXECUTE 'TRUNCATE TABLE public.units RESTART IDENTITY CASCADE';
    END IF;
END$$;

CREATE TABLE IF NOT EXISTS public.units (
	id serial PRIMARY KEY,
	max_capacity int NOT NULL
);

CREATE TABLE IF NOT EXISTS public.leases (
	id serial PRIMARY KEY,
	move_in timestamptz NOT NULL,
	move_out timestamptz NULL,
	resident_name varchar(240) NOT NULL,
	unit_id int NOT NULL,
	FOREIGN KEY (unit_id) REFERENCES public.units(id) ON DELETE RESTRICT ON UPDATE RESTRICT
);

DO $$
DECLARE
    v_max_capacity int;
BEGIN
    FOR i IN 1..100 LOOP
        -- max_capacity cycles from 1 to 4 based on the id
        v_max_capacity := (i % 4) + 1;
        -- Only insert if a unit with the current ID does not exist
        IF NOT EXISTS (SELECT 1 FROM public.units WHERE id = i) THEN
            INSERT INTO public.units (id, max_capacity) VALUES (i, v_max_capacity);
        ELSE
            UPDATE public.units SET max_capacity = v_max_capacity WHERE id = i;
        END IF;
    END LOOP;
END$$;

DO $$
DECLARE
    unit_record RECORD;
    total_units_count INT;
    one_third_count INT;
    existing_leases_count INT;
    lease_counter INT;
    v_move_in_date DATE;
    v_move_out_date DATE;
BEGIN
    -- Calculate a third of the total number of units
    SELECT COUNT(*) INTO total_units_count FROM public.units;
    one_third_count := total_units_count / 3;

    -- Loop through all units
    FOR unit_record IN SELECT * FROM public.units ORDER BY id LOOP
        -- Skip a third of the units for no leases
        IF unit_record.id > 2 * one_third_count THEN
            CONTINUE;
        END IF;

        -- Determine how many existing leases have already ended for the unit
        SELECT COUNT(*) INTO existing_leases_count 
        FROM public.leases 
        WHERE unit_id = unit_record.id AND move_out < NOW();

        -- If unit is in the first third, it should be at max capacity
        IF unit_record.id <= one_third_count THEN
            lease_counter := unit_record.max_capacity - existing_leases_count;
        ELSIF unit_record.id <= 2 * one_third_count THEN
            -- If unit is in the second third, set a historical lease count, but not max capacity
            -- Ensuring that it is less than max_capacity
            lease_counter := (unit_record.max_capacity / 2) - existing_leases_count;
        END IF;

        -- Insert leases if the unit does not have enough historical leases
        WHILE lease_counter > 0 LOOP
            -- Generate initial move-in and move-out dates
						v_move_in_date := (DATE '2025-01-01' + (unit_record.id * INTERVAL '1 day'))::TIMESTAMP;
            v_move_out_date := (v_move_in_date + INTERVAL '6 months')::TIMESTAMP;

            INSERT INTO public.leases (move_in, move_out, resident_name, unit_id)
            VALUES (v_move_in_date, v_move_out_date, 'Resident ' || (unit_record.max_capacity - lease_counter + 1) || ' of Unit ' || unit_record.id, unit_record.id);

            lease_counter := lease_counter - 1;
        END LOOP;
    END LOOP;
END$$;

