USE [bd];
GO

SET NOCOUNT ON;

DECLARE @Client1Id int;
DECLARE @Client2Id int;

IF NOT EXISTS (
    SELECT 1
    FROM Clients
    WHERE Email = 'amine.benali@example.com'
)
BEGIN
    INSERT INTO Clients (Nom, Prenom, Adresse, Email)
    VALUES ('Benali', 'Amine', 'Tunis', 'amine.benali@example.com');
END;

IF NOT EXISTS (
    SELECT 1
    FROM Clients
    WHERE Email = 'sarra.trabelsi@example.com'
)
BEGIN
    INSERT INTO Clients (Nom, Prenom, Adresse, Email)
    VALUES ('Trabelsi', 'Sarra', 'Sousse', 'sarra.trabelsi@example.com');
END;

SELECT @Client1Id = ClientId
FROM Clients
WHERE Email = 'amine.benali@example.com';

SELECT @Client2Id = ClientId
FROM Clients
WHERE Email = 'sarra.trabelsi@example.com';

IF NOT EXISTS (
    SELECT 1
    FROM Comptes
    WHERE NumeroCompte = 1001
)
BEGIN
    INSERT INTO Comptes (NumeroCompte, Solde, DateCreation, ClientId)
    VALUES (1001, 0, GETDATE(), @Client1Id);
END;

IF NOT EXISTS (
    SELECT 1
    FROM Comptes
    WHERE NumeroCompte = 1002
)
BEGIN
    INSERT INTO Comptes (NumeroCompte, Solde, DateCreation, ClientId)
    VALUES (1002, 0, GETDATE(), @Client2Id);
END;

SELECT ClientId, Nom, Prenom, Email
FROM Clients
WHERE Email IN ('amine.benali@example.com', 'sarra.trabelsi@example.com');

SELECT CompteId, NumeroCompte, Solde, ClientId
FROM Comptes
WHERE NumeroCompte IN (1001, 1002);
GO
