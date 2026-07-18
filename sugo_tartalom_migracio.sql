BEGIN TRANSACTION;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718144404_AddSugoTartalom'
)
BEGIN
    CREATE TABLE [SugoKategoriak] (
        [Id] int NOT NULL IDENTITY,
        [Nev] nvarchar(100) NOT NULL,
        [Icon] nvarchar(50) NOT NULL,
        [Sorrend] int NOT NULL,
        [Letrehozva] datetime2 NOT NULL,
        [Modositva] datetime2 NULL,
        CONSTRAINT [PK_SugoKategoriak] PRIMARY KEY ([Id])
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718144404_AddSugoTartalom'
)
BEGIN
    CREATE TABLE [SugoTemak] (
        [Id] int NOT NULL IDENTITY,
        [SugoKategoriaId] int NOT NULL,
        [Cim] nvarchar(200) NOT NULL,
        [Leiras] nvarchar(max) NOT NULL,
        [VideoUrl] nvarchar(500) NULL,
        [Sorrend] int NOT NULL,
        [Letrehozva] datetime2 NOT NULL,
        [Modositva] datetime2 NULL,
        CONSTRAINT [PK_SugoTemak] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SugoTemak_SugoKategoriak_SugoKategoriaId] FOREIGN KEY ([SugoKategoriaId]) REFERENCES [SugoKategoriak] ([Id]) ON DELETE CASCADE
    );
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718144404_AddSugoTartalom'
)
BEGIN
    CREATE INDEX [IX_SugoTemak_SugoKategoriaId] ON [SugoTemak] ([SugoKategoriaId]);
END;
GO

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20260718144404_AddSugoTartalom'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20260718144404_AddSugoTartalom', N'8.0.0');
END;
GO

COMMIT;
GO

