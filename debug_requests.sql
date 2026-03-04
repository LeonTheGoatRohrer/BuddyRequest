-- Debug: Zeige alle Requests mit User-Namen
SELECT 
    r.Id,
    r.SenderId,
    sender.Username AS SenderName,
    r.ReceiverId,
    receiver.Username AS ReceiverName,
    r.RequestType,
    r.Message,
    r.Status,
    r.CreatedAt
FROM Requests r
LEFT JOIN Users sender ON r.SenderId = sender.Id
LEFT JOIN Users receiver ON r.ReceiverId = receiver.Id
ORDER BY r.CreatedAt DESC;

-- Zeige alle User mit IDs
SELECT Id, Username, Email FROM Users;
