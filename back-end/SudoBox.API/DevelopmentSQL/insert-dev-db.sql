CREATE OR REPLACE FUNCTION truncate_tables() RETURNS void AS $$
DECLARE
    statements CURSOR FOR
        SELECT tablename, schemaname FROM pg_tables
        WHERE tableowner = 'postgres' AND (schemaname = 'users' OR schemaname = 'trainings') AND tablename != '__EFMigrationsHistory';
BEGIN
    FOR stmt IN statements LOOP
            EXECUTE 'TRUNCATE TABLE ' || stmt.schemaname || '.' || quote_ident(stmt.tablename) || ' CASCADE;';
        END LOOP;
END;
$$ LANGUAGE plpgsql;
SELECT truncate_tables();