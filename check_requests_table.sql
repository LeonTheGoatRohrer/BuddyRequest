-- Prüfe ob Requests-Tabelle existiert
SELECT * FROM information_schema.tables 
WHERE table_schema = 'Messanger' 
AND table_name = 'Requests';

-- Wenn die Tabelle existiert, zeige ihre Struktur
DESCRIBE Requests;

-- Zeige alle Requests
SELECT * FROM Requests;
