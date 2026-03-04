-- Migration: Add Requests Table
-- Date: 01.03.2026

CREATE TABLE IF NOT EXISTS Requests (
    Id INT AUTO_INCREMENT PRIMARY KEY,
    SenderId INT NOT NULL,
    ReceiverId INT NOT NULL,
    RequestType VARCHAR(50) NOT NULL,
    Message VARCHAR(500) NOT NULL,
    Status VARCHAR(20) NOT NULL DEFAULT 'Pending',
    CreatedAt DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    RespondedAt DATETIME NULL,
    INDEX IX_Requests_SenderId (SenderId),
    INDEX IX_Requests_ReceiverId (ReceiverId)
);
