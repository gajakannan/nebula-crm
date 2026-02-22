#!/bin/bash
# Creates additional databases required by Nebula services.
# Mounted into /docker-entrypoint-initdb.d/ and runs once on first container start.
# The default "nebula" database is already created via POSTGRES_DB.

set -euo pipefail

psql -v ON_ERROR_STOP=1 --username "$POSTGRES_USER" --dbname "$POSTGRES_DB" <<-EOSQL
    SELECT 'CREATE DATABASE keycloak'
    WHERE NOT EXISTS (SELECT FROM pg_database WHERE datname = 'keycloak')\gexec
EOSQL
