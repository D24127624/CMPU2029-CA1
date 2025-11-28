
INSERT INTO Owners (Name, Address, PhoneNumber)
VALUES (@name, @address, @phone);
SELECT last_insert_rowid();
