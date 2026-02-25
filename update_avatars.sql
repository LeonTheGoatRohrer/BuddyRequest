-- Avatar URLs für alle User setzen
UPDATE users SET AvatarUrl = 'https://api.dicebear.com/7.x/avataaars/svg?seed=leon&backgroundColor=random' WHERE Id = 1;
UPDATE users SET AvatarUrl = 'https://api.dicebear.com/7.x/avataaars/svg?seed=leon1&backgroundColor=random' WHERE Id = 2;
UPDATE users SET AvatarUrl = 'https://api.dicebear.com/7.x/avataaars/svg?seed=user&backgroundColor=random' WHERE Id = 3;
UPDATE users SET AvatarUrl = 'https://api.dicebear.com/7.x/avataaars/svg?seed=asd&backgroundColor=random' WHERE Id = 4;
UPDATE users SET AvatarUrl = 'https://api.dicebear.com/7.x/avataaars/svg?seed=LeonROhrer&backgroundColor=random' WHERE Id = 5;
UPDATE users SET AvatarUrl = 'https://api.dicebear.com/7.x/avataaars/svg?seed=FranzSepp&backgroundColor=random' WHERE Id = 6;

-- Überprüfung
SELECT Id, Username, AvatarUrl FROM users;
