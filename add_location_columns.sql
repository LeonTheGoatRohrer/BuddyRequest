-- Add Location columns to Users table
ALTER TABLE `Users` ADD COLUMN `Latitude` DOUBLE NULL;
ALTER TABLE `Users` ADD COLUMN `Longitude` DOUBLE NULL;
ALTER TABLE `Users` ADD COLUMN `LastLocationUpdate` DATETIME(6) NULL;

-- Verify the columns were added
SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'Users' AND TABLE_SCHEMA = 'Messanger';
