BEGIN TRANSACTION;
GO

CREATE TABLE [UzemeltetoSablonFelhasznalok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [UzemeltetoSablonId] int NOT NULL,
    [FelhasznaloId] nvarchar(450) NOT NULL,
    [HozzarendeloFelhasznaloId] nvarchar(450) NOT NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_UzemeltetoSablonFelhasznalok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UzemeltetoSablonFelhasznalok_AspNetUsers_FelhasznaloId] FOREIGN KEY ([FelhasznaloId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_UzemeltetoSablonFelhasznalok_AspNetUsers_HozzarendeloFelhasznaloId] FOREIGN KEY ([HozzarendeloFelhasznaloId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_UzemeltetoSablonFelhasznalok_UzemeltetoSablonok_UzemeltetoSablonId] FOREIGN KEY ([UzemeltetoSablonId]) REFERENCES [UzemeltetoSablonok] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_UzemeltetoSablonFelhasznalok_Aktiv] ON [UzemeltetoSablonFelhasznalok] ([Aktiv]);
GO

CREATE INDEX [IX_UzemeltetoSablonFelhasznalok_FelhasznaloId] ON [UzemeltetoSablonFelhasznalok] ([FelhasznaloId]);
GO

CREATE INDEX [IX_UzemeltetoSablonFelhasznalok_HozzarendeloFelhasznaloId] ON [UzemeltetoSablonFelhasznalok] ([HozzarendeloFelhasznaloId]);
GO

CREATE UNIQUE INDEX [IX_UzemeltetoSablonFelhasznalok_UzemeltetoSablonId_FelhasznaloId] ON [UzemeltetoSablonFelhasznalok] ([UzemeltetoSablonId], [FelhasznaloId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260707210153_AddUzemeltetoSablonFelhasznalo', N'8.0.0');
GO

COMMIT;
GO

