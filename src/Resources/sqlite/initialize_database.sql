
-- Create Owners Table
CREATE TABLE IF NOT EXISTS Owners (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT,
    Address TEXT,
    PhoneNumber TEXT
);

-- Create Pets Table
CREATE TABLE IF NOT EXISTS Pets (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT,
    Age INTEGER,
    Breed TEXT,
    OwnerId INTEGER,
    FoodPreferences TEXT,
    Type TEXT,
    Size TEXT,
    WalkingPreference INTEGER,
    FOREIGN KEY (OwnerId) REFERENCES Owners(Id)
);

-- Create Kennels Table
CREATE TABLE IF NOT EXISTS Kennels (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT,
    Size TEXT,
    SuitableFor TEXT,
    IsOutOfService INTEGER,
    OutOfServiceComment TEXT
);

-- Create Bookings Table
CREATE TABLE IF NOT EXISTS Bookings (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    GroupId TEXT,
    PetId INTEGER,
    KennelId INTEGER,
    Date INTEGER,
    FOREIGN KEY (PetId) REFERENCES Pets(Id),
    FOREIGN KEY (KennelId) REFERENCES Kennels(Id)
);


-- Preloadding Data --

-- Owners
INSERT OR IGNORE INTO Owners (Id, Name, Address, PhoneNumber)
                      VALUES (1, 'Sally Smith', '123 Main Street', '098 123-7382');
INSERT OR IGNORE INTO Owners (Id, Name, Address, PhoneNumber)
                      VALUES (2, 'Joe Jones', '59 Second Avenue', '084 826-9173');

-- Pets
INSERT OR IGNORE INTO Pets (Id, Name, Age, Breed, OwnerId, FoodPreferences, Type, Size, WalkingPreference)
          VALUES (1, 'Mittens', 3, 'Common Houst Cat', 1, 'prefers wet food', 'CAT', 'SMALL', NULL);
INSERT OR IGNORE INTO Pets (Id, Name, Age, Breed, OwnerId, FoodPreferences, Type, Size, WalkingPreference)
          VALUES (2, 'Butch', 7, 'Bulldog', 2, 'will eat anything', 'DOG', 'MEDIUM', 'SOLO');

-- Kennels
INSERT OR IGNORE INTO Kennels (Id, Name, Size, SuitableFor, IsOutOfService, OutOfServiceComment)
                       VALUES (1, 'Unit 001', 'SMALL', 'CAT', 0, NULL);
INSERT OR IGNORE INTO Kennels (Id, Name, Size, SuitableFor, IsOutOfService, OutOfServiceComment)
                       VALUES (2, 'Unit 002', 'SMALL', 'CAT', 1, 'the door is broken');
INSERT OR IGNORE INTO Kennels (Id, Name, Size, SuitableFor, IsOutOfService, OutOfServiceComment)
                       VALUES (3, 'Unit 003', 'SMALL', 'CAT', 0, NULL);
INSERT OR IGNORE INTO Kennels (Id, Name, Size, SuitableFor, IsOutOfService, OutOfServiceComment)
                       VALUES (4, 'Unit 011', 'MEDIUM', 'DOG', 0, NULL);
INSERT OR IGNORE INTO Kennels (Id, Name, Size, SuitableFor, IsOutOfService, OutOfServiceComment)
                       VALUES (5, 'Unit 012', 'MEDIUM', 'DOG', 0, NULL);
