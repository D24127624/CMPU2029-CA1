
INSERT INTO Bookings (GroupId, PetId, KennelId, Date)
VALUES (@groupId, @petId, @kennelId, @date);
SELECT last_insert_rowid();
