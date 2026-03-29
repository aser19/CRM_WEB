IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Cegek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Adoszam] nvarchar(20) NULL,
    [Cim] nvarchar(500) NULL,
    [Email] nvarchar(200) NULL,
    [Telefon] nvarchar(50) NULL,
    [Weboldal] nvarchar(500) NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_Cegek] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Gyartok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Orszag] nvarchar(100) NULL,
    [Weboldal] nvarchar(500) NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_Gyartok] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Hatosagok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Rovidites] nvarchar(20) NULL,
    [Cim] nvarchar(500) NULL,
    [Weboldal] nvarchar(500) NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_Hatosagok] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Jogszabalyok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Szam] nvarchar(100) NOT NULL,
    [Cim] nvarchar(500) NOT NULL,
    [Leiras] nvarchar(2000) NULL,
    [Tipus] int NOT NULL,
    [HatalyosKezdet] datetime2 NULL,
    [HatalyosVege] datetime2 NULL,
    [Url] nvarchar(500) NULL,
    [Megjegyzes] nvarchar(1000) NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_Jogszabalyok] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Kepzesek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Datum] datetime2 NULL,
    [LejaratDatum] datetime2 NULL,
    [Resztvevo] nvarchar(500) NULL,
    [Megjegyzes] nvarchar(1000) NULL,
    CONSTRAINT [PK_Kepzesek] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [MeresTipusok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Leiras] nvarchar(1000) NULL,
    [ErvenyessegHonap] int NOT NULL,
    CONSTRAINT [PK_MeresTipusok] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoleClaims] (
    [Id] int NOT NULL IDENTITY,
    [RoleId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUsers] (
    [Id] nvarchar(450) NOT NULL,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [UtolsoBelepes] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Beosztas] nvarchar(100) NULL,
    [Telefon] nvarchar(50) NULL,
    [Aktiv] bit NOT NULL,
    [CegId] int NOT NULL,
    [UserName] nvarchar(256) NULL,
    [NormalizedUserName] nvarchar(256) NULL,
    [Email] nvarchar(256) NULL,
    [NormalizedEmail] nvarchar(256) NULL,
    [EmailConfirmed] bit NOT NULL,
    [PasswordHash] nvarchar(max) NULL,
    [SecurityStamp] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [PhoneNumberConfirmed] bit NOT NULL,
    [TwoFactorEnabled] bit NOT NULL,
    [LockoutEnd] datetimeoffset NULL,
    [LockoutEnabled] bit NOT NULL,
    [AccessFailedCount] int NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUsers_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Ugyfelek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Adoszam] nvarchar(20) NULL,
    [Cim] nvarchar(500) NULL,
    [Email] nvarchar(200) NULL,
    [Telefon] nvarchar(50) NULL,
    [UgyfelTipus] int NOT NULL,
    [Aktiv] bit NOT NULL,
    [CegId] int NOT NULL,
    CONSTRAINT [PK_Ugyfelek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Ugyfelek_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [AspNetUserClaims] (
    [Id] int NOT NULL IDENTITY,
    [UserId] nvarchar(450) NOT NULL,
    [ClaimType] nvarchar(max) NULL,
    [ClaimValue] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserLogins] (
    [LoginProvider] nvarchar(450) NOT NULL,
    [ProviderKey] nvarchar(450) NOT NULL,
    [ProviderDisplayName] nvarchar(max) NULL,
    [UserId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserRoles] (
    [UserId] nvarchar(450) NOT NULL,
    [RoleId] nvarchar(450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [AspNetUserTokens] (
    [UserId] nvarchar(450) NOT NULL,
    [LoginProvider] nvarchar(450) NOT NULL,
    [Name] nvarchar(450) NOT NULL,
    [Value] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Tanusitvanyok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Szam] nvarchar(100) NULL,
    [KiadoDatum] datetime2 NULL,
    [LejaratDatum] datetime2 NULL,
    [UgyfelId] int NOT NULL,
    [Megjegyzes] nvarchar(1000) NULL,
    CONSTRAINT [PK_Tanusitvanyok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Tanusitvanyok_Ugyfelek_UgyfelId] FOREIGN KEY ([UgyfelId]) REFERENCES [Ugyfelek] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Telephelyek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Cim] nvarchar(500) NULL,
    [UgyfelId] int NOT NULL,
    [Kapcsolattarto] nvarchar(200) NULL,
    [Telefon] nvarchar(50) NULL,
    [Email] nvarchar(200) NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_Telephelyek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Telephelyek_Ugyfelek_UgyfelId] FOREIGN KEY ([UgyfelId]) REFERENCES [Ugyfelek] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Ugyszamok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Szam] nvarchar(100) NOT NULL,
    [Targy] nvarchar(500) NULL,
    [UgyfelId] int NULL,
    [HatosagId] int NULL,
    [Beerkezett] datetime2 NULL,
    [Hatarido] datetime2 NULL,
    [Megjegyzes] nvarchar(1000) NULL,
    [Lezart] bit NOT NULL,
    CONSTRAINT [PK_Ugyszamok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Ugyszamok_Hatosagok_HatosagId] FOREIGN KEY ([HatosagId]) REFERENCES [Hatosagok] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_Ugyszamok_Ugyfelek_UgyfelId] FOREIGN KEY ([UgyfelId]) REFERENCES [Ugyfelek] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Eszkozok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [GyariSzam] nvarchar(100) NULL,
    [Tipus] nvarchar(100) NULL,
    [GyartoId] int NOT NULL,
    [UgyfelId] int NOT NULL,
    [TelephelyId] int NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_Eszkozok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Eszkozok_Gyartok_GyartoId] FOREIGN KEY ([GyartoId]) REFERENCES [Gyartok] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Eszkozok_Telephelyek_TelephelyId] FOREIGN KEY ([TelephelyId]) REFERENCES [Telephelyek] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Eszkozok_Ugyfelek_UgyfelId] FOREIGN KEY ([UgyfelId]) REFERENCES [Ugyfelek] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Terminalok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Azonosito] nvarchar(100) NULL,
    [IpCim] nvarchar(50) NULL,
    [TelephelyId] int NULL,
    [Megjegyzes] nvarchar(1000) NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_Terminalok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Terminalok_Telephelyek_TelephelyId] FOREIGN KEY ([TelephelyId]) REFERENCES [Telephelyek] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Hitelesitesek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [EszkozId] int NOT NULL,
    [HatosagId] int NULL,
    [Ugyszam] nvarchar(100) NULL,
    [Datum] datetime2 NOT NULL,
    [LejaratDatum] datetime2 NULL,
    [HitelesitesStatusz] int NOT NULL,
    [Megjegyzes] nvarchar(1000) NULL,
    CONSTRAINT [PK_Hitelesitesek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Hitelesitesek_Eszkozok_EszkozId] FOREIGN KEY ([EszkozId]) REFERENCES [Eszkozok] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Hitelesitesek_Hatosagok_HatosagId] FOREIGN KEY ([HatosagId]) REFERENCES [Hatosagok] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Kalibraciok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [EszkozId] int NOT NULL,
    [Datum] datetime2 NOT NULL,
    [KovetkezoDatum] datetime2 NULL,
    [Bizonyitvany] nvarchar(200) NULL,
    [Elvegzo] nvarchar(200) NULL,
    [Megjegyzes] nvarchar(1000) NULL,
    [Sikeres] bit NOT NULL,
    CONSTRAINT [PK_Kalibraciok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Kalibraciok_Eszkozok_EszkozId] FOREIGN KEY ([EszkozId]) REFERENCES [Eszkozok] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Karbantartasok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [EszkozId] int NOT NULL,
    [Datum] datetime2 NOT NULL,
    [KovetkezoDatum] datetime2 NULL,
    [Leiras] nvarchar(1000) NULL,
    [Elvegzo] nvarchar(200) NULL,
    CONSTRAINT [PK_Karbantartasok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Karbantartasok_Eszkozok_EszkozId] FOREIGN KEY ([EszkozId]) REFERENCES [Eszkozok] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Meresek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [EszkozId] int NOT NULL,
    [MeresTipusId] int NOT NULL,
    [Datum] datetime2 NOT NULL,
    [KovetkezoDatum] datetime2 NULL,
    [Eredmeny] nvarchar(500) NULL,
    [MeresStatusz] int NOT NULL,
    [Megjegyzes] nvarchar(1000) NULL,
    CONSTRAINT [PK_Meresek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Meresek_Eszkozok_EszkozId] FOREIGN KEY ([EszkozId]) REFERENCES [Eszkozok] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Meresek_MeresTipusok_MeresTipusId] FOREIGN KEY ([MeresTipusId]) REFERENCES [MeresTipusok] ([Id]) ON DELETE NO ACTION
);
GO

CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
GO

CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
GO

CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
GO

CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
GO

CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
GO

CREATE INDEX [IX_AspNetUsers_CegId] ON [AspNetUsers] ([CegId]);
GO

CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
GO

CREATE INDEX [IX_Eszkozok_GyartoId] ON [Eszkozok] ([GyartoId]);
GO

CREATE INDEX [IX_Eszkozok_TelephelyId] ON [Eszkozok] ([TelephelyId]);
GO

CREATE INDEX [IX_Eszkozok_UgyfelId] ON [Eszkozok] ([UgyfelId]);
GO

CREATE INDEX [IX_Hitelesitesek_EszkozId] ON [Hitelesitesek] ([EszkozId]);
GO

CREATE INDEX [IX_Hitelesitesek_HatosagId] ON [Hitelesitesek] ([HatosagId]);
GO

CREATE INDEX [IX_Kalibraciok_EszkozId] ON [Kalibraciok] ([EszkozId]);
GO

CREATE INDEX [IX_Karbantartasok_EszkozId] ON [Karbantartasok] ([EszkozId]);
GO

CREATE INDEX [IX_Meresek_EszkozId] ON [Meresek] ([EszkozId]);
GO

CREATE INDEX [IX_Meresek_MeresTipusId] ON [Meresek] ([MeresTipusId]);
GO

CREATE INDEX [IX_Tanusitvanyok_UgyfelId] ON [Tanusitvanyok] ([UgyfelId]);
GO

CREATE INDEX [IX_Telephelyek_UgyfelId] ON [Telephelyek] ([UgyfelId]);
GO

CREATE INDEX [IX_Terminalok_TelephelyId] ON [Terminalok] ([TelephelyId]);
GO

CREATE INDEX [IX_Ugyfelek_CegId] ON [Ugyfelek] ([CegId]);
GO

CREATE INDEX [IX_Ugyszamok_HatosagId] ON [Ugyszamok] ([HatosagId]);
GO

CREATE INDEX [IX_Ugyszamok_UgyfelId] ON [Ugyszamok] ([UgyfelId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260329183239_AddJogszabalyColumns', N'8.0.0');
GO

COMMIT;
GO

