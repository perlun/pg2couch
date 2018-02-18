POSTGRES_CONNECTION_STRING="Host=localhost;Database=cached_data;Username=pg2couch;Password=pg2couch" \
COUCHDB_URL="http://localhost:5984/" \
COUCHDB_DB_NAME="pg2couch" \
  dotnet run --verbose
