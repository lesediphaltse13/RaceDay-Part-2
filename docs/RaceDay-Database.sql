--CREATE DATABASE RaceDayDB;

--USE RaceDayDB;


--CREATE TABLE UserRole
--(
--    RoleId INT IDENTITY(1,1) PRIMARY KEY,
--    RoleName VARCHAR(30) NOT NULL UNIQUE
--);


--CREATE TABLE Organizers
--(
--    OrganiserId INT IDENTITY(1,1) PRIMARY KEY,
--    RoleId INT NOT NULL,
--    FirstName VARCHAR(50) NOT NULL,
--    LastName VARCHAR(50) NOT NULL,
--    Email VARCHAR(100) NOT NULL UNIQUE,
--    PasswordHash VARCHAR(255) NOT NULL,
--    PhoneNumber VARCHAR(20),
--    CreatedAt DATETIME2 DEFAULT GETDATE(),

--    FOREIGN KEY (RoleId) REFERENCES UserRole(RoleId)
--);


--CREATE TABLE Participants
--(
--    ParticipantId INT IDENTITY(1,1) PRIMARY KEY,
--    RoleId INT NOT NULL,
--    FirstName VARCHAR(50) NOT NULL,
--    LastName VARCHAR(50) NOT NULL,
--    Email VARCHAR(100) NOT NULL UNIQUE,
--    PasswordHash VARCHAR(255) NOT NULL,
--    PhoneNumber VARCHAR(20),
--    DateOfBirth DATE NOT NULL,
--    CreatedAt DATETIME2 DEFAULT GETDATE(),

--    FOREIGN KEY (RoleId) REFERENCES UserRole(RoleId)
--);


--CREATE TABLE Events
--(
--    EventId INT IDENTITY(1,1) PRIMARY KEY,
--    OrganiserId INT NOT NULL,
--    EventName VARCHAR(120) NOT NULL,
--    Description VARCHAR(500),
--    EventType VARCHAR(30) NOT NULL,
--    EventDate DATETIME2 NOT NULL,
--    RegistrationClosingDate DATETIME2 NOT NULL,
--    Venue VARCHAR(150) NOT NULL,
--    City VARCHAR(80) NOT NULL,
--    Province VARCHAR(50) NOT NULL,
--    RouteInformation VARCHAR(500),
--    CreatedAt DATETIME2 DEFAULT GETDATE(),

--    FOREIGN KEY (OrganiserId) REFERENCES Organizers(OrganiserId)
--);


--CREATE TABLE EventCategories
--(
--    CategoryId INT IDENTITY(1,1) PRIMARY KEY,
--    EventId INT NOT NULL,
--    CategoryName VARCHAR(100) NOT NULL,
--    DistanceKm DECIMAL(6,2) NOT NULL,
--    EntryFee DECIMAL(10,2) NOT NULL,
--    MaximumParticipants INT,
--    CreatedAt DATETIME2 DEFAULT GETDATE(),

--    FOREIGN KEY (EventId) REFERENCES Events(EventId)
--);


--CREATE TABLE Enrolments
--(
--    EnrolmentId INT IDENTITY(1,1) PRIMARY KEY,
--    ParticipantId INT NOT NULL,
--    CategoryId INT NOT NULL,
--    EnrolmentDate DATETIME2 DEFAULT GETDATE(),
--    Status VARCHAR(30) NOT NULL,
--    RaceNumber VARCHAR(30) UNIQUE,

--    FOREIGN KEY (ParticipantId) REFERENCES Participants(ParticipantId),
--    FOREIGN KEY (CategoryId) REFERENCES EventCategories(CategoryId),

--    UNIQUE (ParticipantId, CategoryId)
--);


--CREATE TABLE Results
--(
--    ResultId INT IDENTITY(1,1) PRIMARY KEY,
--    EnrolmentId INT NOT NULL UNIQUE,
--    FinishTime TIME,
--    Position INT,
--    Status VARCHAR(30) NOT NULL,
--    RecordedAt DATETIME2 DEFAULT GETDATE(),

--    FOREIGN KEY (EnrolmentId) REFERENCES Enrolments(EnrolmentId)
--);


---- Insert user roles

--INSERT INTO UserRole (RoleName)
--VALUES
--('Organiser'),
--('Participant');


---- Insert organisers

--INSERT INTO Organizers
--(
--    RoleId,
--    FirstName,
--    LastName,
--    Email,
--    PasswordHash,
--    PhoneNumber
--)
--VALUES
--(
--    1,
--    'Thabo',
--    'Mokoena',
--    'thabo@raceday.co.za',
--    'password123',
--    '0821111111'
--),
--(
--    1,
--    'Naledi',
--    'Khumalo',
--    'naledi@raceday.co.za',
--    'password456',
--    '0822222222'
--);


---- Insert participants

--INSERT INTO Participants
--(
--    RoleId,
--    FirstName,
--    LastName,
--    Email,
--    PasswordHash,
--    PhoneNumber,
--    DateOfBirth
--)
--VALUES
--(
--    2,
--    'Sipho',
--    'Dlamini',
--    'sipho@email.com',
--    'password123',
--    '0831111111',
--    '1998-05-14'
--),
--(
--    2,
--    'Lerato',
--    'Nkosi',
--    'lerato@email.com',
--    'password456',
--    '0832222222',
--    '2001-09-20'
--);


---- Insert events

--INSERT INTO Events
--(
--    OrganiserId,
--    EventName,
--    Description,
--    EventType,
--    EventDate,
--    RegistrationClosingDate,
--    Venue,
--    City,
--    Province,
--    RouteInformation
--)
--VALUES
--(
--    1,
--    'Pretoria Road Race',
--    'A road running event in Pretoria.',
--    'Running',
--    '2026-10-17 06:00:00',
--    '2026-10-10 23:59:00',
--    'Loftus Versfeld',
--    'Pretoria',
--    'Gauteng',
--    'Road route through Pretoria'
--),
--(
--    1,
--    'Centurion Charity Walk',
--    'A community charity walking event.',
--    'Walking',
--    '2026-11-07 07:00:00',
--    '2026-10-31 23:59:00',
--    'Centurion Park',
--    'Centurion',
--    'Gauteng',
--    'Walking route around Centurion'
--),
--(
--    2,
--    'Gauteng Cycle Challenge',
--    'A recreational cycling event.',
--    'Cycling',
--    '2026-12-05 06:30:00',
--    '2026-11-28 23:59:00',
--    'Waterkloof',
--    'Pretoria',
--    'Gauteng',
--    'Cycling route around Pretoria'
--);


---- Insert event categories

--INSERT INTO EventCategories
--(
--    EventId,
--    CategoryName,
--    DistanceKm,
--    EntryFee,
--    MaximumParticipants
--)
--VALUES
--(1, '5 km Fun Run', 5.00, 100.00, 500),
--(1, '10 km Road Race', 10.00, 180.00, 800),
--(1, '21.1 km Half Marathon', 21.10, 300.00, 600),

--(2, '5 km Charity Walk', 5.00, 80.00, 300),
--(2, '10 km Charity Walk', 10.00, 120.00, 250),

--(3, '40 km Cycle', 40.00, 250.00, 400),
--(3, '80 km Cycle', 80.00, 400.00, 300);


---- Insert enrolments

--INSERT INTO Enrolments
--(
--    ParticipantId,
--    CategoryId,
--    Status,
--    RaceNumber
--)
--VALUES
--(1, 2, 'Confirmed', 'PRR1001'),
--(2, 3, 'Confirmed', 'PRR2101'),
--(1, 4, 'Confirmed', 'CCW5001'),
--(2, 6, 'Confirmed', 'GCC4001');


---- Insert results

--INSERT INTO Results
--(
--    EnrolmentId,
--    FinishTime,
--    Position,
--    Status
--)
--VALUES
--(
--    1,
--    '00:52:14',
--    35,
--    'Finished'
--),
--(
--    2,
--    '01:58:40',
--    21,
--    'Finished'
--);


---- Display all tables

--SELECT * FROM UserRole;

--SELECT * FROM Organizers;

--SELECT * FROM Participants;

--SELECT * FROM Events;

--SELECT * FROM EventCategories;

--SELECT * FROM Enrolments;

--SELECT * FROM Results;


---- Display participant enrolments

--SELECT
--    Participants.FirstName,
--    Participants.LastName,
--    Events.EventName,
--    EventCategories.CategoryName,
--    Enrolments.RaceNumber,
--    Enrolments.Status
--FROM Enrolments
--INNER JOIN Participants
--    ON Enrolments.ParticipantId = Participants.ParticipantId
--INNER JOIN EventCategories
--    ON Enrolments.CategoryId = EventCategories.CategoryId
--INNER JOIN Events
--    ON EventCategories.EventId = Events.EventId;


---- Display participant results

--SELECT
--    Participants.FirstName,
--    Participants.LastName,
--    Events.EventName,
--    EventCategories.CategoryName,
--    Results.FinishTime,
--    Results.Position,
--    Results.Status
--FROM Results
--INNER JOIN Enrolments
--    ON Results.EnrolmentId = Enrolments.EnrolmentId
--INNER JOIN Participants
--    ON Enrolments.ParticipantId = Participants.ParticipantId
--INNER JOIN EventCategories
--    ON Enrolments.CategoryId = EventCategories.CategoryId
--INNER JOIN Events
--    ON EventCategories.EventId = Events.EventId;