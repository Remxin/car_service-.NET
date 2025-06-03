db = db.getSiblingDB('reportDb'); // przełącz na bazę reportDb

db.createCollection('reports')
db.createCollection('users')
