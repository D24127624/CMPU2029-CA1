
INSERT INTO Kennels (Name, Size, SuitableFor, IsOutOfService, OutOfServiceComment)
VALUES (@name, @size, @suitableFor, @outOfService, @serviceComment);
SELECT last_insert_rowid();
