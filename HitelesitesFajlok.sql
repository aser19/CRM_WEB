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

CREATE TABLE [AlapertelmezettEmailBeallitasok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [ErtesitesTipusok] int NOT NULL,
    [CimzettTipusok] int NOT NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_AlapertelmezettEmailBeallitasok] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AspNetRoles] (
    [Id] nvarchar(450) NOT NULL,
    [Name] nvarchar(256) NULL,
    [NormalizedName] nvarchar(256) NULL,
    [ConcurrencyStamp] nvarchar(max) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AutomatikaFutasNaplok] (
    [Id] int NOT NULL IDENTITY,
    [FutasiIdo] datetime2 NOT NULL,
    [Sikeres] bit NOT NULL,
    [FeldolgozottHitelesitesek] int NOT NULL,
    [FeldolgozottMeresek] int NOT NULL,
    [KuldottEmailek] int NOT NULL,
    [SikertelenEmailek] int NOT NULL,
    [Hiba] nvarchar(2000) NULL,
    CONSTRAINT [PK_AutomatikaFutasNaplok] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [AvkVedelemTipusok] (
    [Id] int NOT NULL IDENTITY,
    [Nev] nvarchar(max) NOT NULL,
    [TipusKod] nvarchar(max) NOT NULL,
    [In] decimal(8,2) NOT NULL,
    [IDn] decimal(8,2) NOT NULL,
    [Un] decimal(8,2) NOT NULL,
    [Polusszam] int NOT NULL,
    [Leiras] nvarchar(max) NULL,
    [Aktiv] bit NOT NULL,
    [Letrehozva] datetime2 NOT NULL,
    CONSTRAINT [PK_AvkVedelemTipusok] PRIMARY KEY ([Id])
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
    [MatricaElotag] nvarchar(3) NULL,
    [Tevekenyseg] int NOT NULL,
    [AktivModulok] int NOT NULL,
    CONSTRAINT [PK_Cegek] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EmailKuldesNaplok] (
    [Id] int NOT NULL IDENTITY,
    [Kuldve] datetime2 NOT NULL,
    [CegId] int NULL,
    [Tipus] int NOT NULL,
    [Cimzett] nvarchar(500) NOT NULL,
    [Targy] nvarchar(500) NOT NULL,
    [Sikeres] bit NOT NULL,
    [Hiba] nvarchar(2000) NULL,
    [HitelesitesId] int NULL,
    [MeresId] int NULL,
    CONSTRAINT [PK_EmailKuldesNaplok] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [ErintesvedelmiModOsztalyok] (
    [Id] int NOT NULL IDENTITY,
    [Nev] nvarchar(50) NOT NULL,
    [Leiras] nvarchar(500) NULL,
    [Aktiv] bit NOT NULL,
    [Sorrend] int NOT NULL,
    [Letrehozva] datetime2 NOT NULL,
    CONSTRAINT [PK_ErintesvedelmiModOsztalyok] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [EszkozTipusok] (
    [Id] int NOT NULL IDENTITY,
    [Nev] nvarchar(200) NOT NULL,
    [Aktiv] bit NOT NULL,
    [Letrehozva] datetime2 NOT NULL,
    [HitelesitesiIdotartamHonap] int NOT NULL,
    CONSTRAINT [PK_EszkozTipusok] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [FelhasznaloErtesitesBeallitasok] (
    [Id] int NOT NULL IDENTITY,
    [FelhasznaloId] int NOT NULL,
    [PopupEngedelyezve] bit NOT NULL,
    [EmailEngedelyezve] bit NOT NULL,
    [SzuneteltetesDatum] datetime2 NULL,
    [UtolsoPopupDatum] datetime2 NULL,
    CONSTRAINT [PK_FelhasznaloErtesitesBeallitasok] PRIMARY KEY ([Id])
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
    [Tevekenyseg] int NOT NULL,
    CONSTRAINT [PK_Gyartok] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Hatosagok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Rovidites] nvarchar(100) NULL,
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
    [Terulet] int NOT NULL,
    [HatalyosKezdet] datetime2 NULL,
    [HatalyosVege] datetime2 NULL,
    [Url] nvarchar(500) NULL,
    [Megjegyzes] nvarchar(1000) NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_Jogszabalyok] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [KarbantartasTipusok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Leiras] nvarchar(1000) NULL,
    [IsmetlodesHonap] int NOT NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_KarbantartasTipusok] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [KepzesTipusok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Nev] nvarchar(max) NOT NULL,
    [Label] nvarchar(max) NULL,
    [Lejar] bit NOT NULL,
    [LejaratEvek] int NULL,
    [TovabbkepzesKotelezo] bit NOT NULL,
    [TovabbkepzesEvek] int NULL,
    [TovabbkepzesCsakFelulvizsgalonak] bit NOT NULL,
    [Leiras] nvarchar(max) NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_KepzesTipusok] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [KotelezoHitelesitesek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Megnevezes] nvarchar(max) NOT NULL,
    [JogszabalyiHivatkozas] nvarchar(max) NULL,
    [HitelesitesiIdoszakHonap] int NOT NULL,
    [Megjegyzes] nvarchar(max) NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_KotelezoHitelesitesek] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [MeresTipusok] (
    [Id] int NOT NULL IDENTITY,
    [Nev] nvarchar(200) NOT NULL,
    [Leiras] nvarchar(1000) NULL,
    [ErvenyessegHonap] int NULL,
    [JegyzokonyvPrefix] nvarchar(20) NULL DEFAULT N'JKV',
    [SablonId] nvarchar(100) NULL,
    [OcrModelId] nvarchar(100) NULL,
    [MellekletTipusKod] nvarchar(20) NULL,
    [Aktiv] bit NOT NULL,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    CONSTRAINT [PK_MeresTipusok] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SmtpBeallitasok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [SzerverCim] nvarchar(200) NOT NULL,
    [Port] int NOT NULL,
    [TitkositasTipus] int NOT NULL,
    [SslHasznalata] bit NOT NULL,
    [FelhasznaloNev] nvarchar(200) NOT NULL,
    [Jelszo] nvarchar(500) NOT NULL,
    [KuldoNev] nvarchar(200) NOT NULL,
    [KuldoEmail] nvarchar(200) NOT NULL,
    [Aktiv] bit NOT NULL,
    [EmailFutasiOra] int NOT NULL,
    [EmailFutasiPerc] int NOT NULL,
    CONSTRAINT [PK_SmtpBeallitasok] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TularamvedelemTipusok] (
    [Id] int NOT NULL IDENTITY,
    [Nev] nvarchar(100) NOT NULL,
    [NevlegesAram] decimal(10,2) NOT NULL,
    [Leiras] nvarchar(500) NULL,
    [Aktiv] bit NOT NULL,
    [Letrehozva] datetime2 NOT NULL,
    [KarakterisztikaFeluliras] nvarchar(1) NULL,
    CONSTRAINT [PK_TularamvedelemTipusok] PRIMARY KEY ([Id])
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

CREATE TABLE [EmailBeallitasok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [CegId] int NOT NULL,
    [ErtesitesTipusok] int NOT NULL,
    [CimzettTipusok] int NOT NULL,
    [EgyediEmailCimek] nvarchar(2000) NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_EmailBeallitasok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmailBeallitasok_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [EmailSablonok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Tipus] int NOT NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Targy] nvarchar(500) NOT NULL,
    [Szoveg] nvarchar(max) NOT NULL,
    [Aktiv] bit NOT NULL,
    [CegId] int NULL,
    CONSTRAINT [PK_EmailSablonok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EmailSablonok_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [EszkozSablonok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [CegId] int NULL,
    [Megnevezes] nvarchar(200) NOT NULL,
    [Tipus] nvarchar(100) NULL,
    [Azonosito] nvarchar(100) NULL,
    [VedelmiOsztaly] nvarchar(10) NOT NULL,
    [Telj] nvarchar(20) NOT NULL,
    [Megtekint] nvarchar(10) NOT NULL,
    [Aktiv] bit NOT NULL,
    [Megjegyzes] nvarchar(max) NULL,
    CONSTRAINT [PK_EszkozSablonok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EszkozSablonok_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Felulvizsgalok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Jogosultsag] int NOT NULL,
    [Email] nvarchar(200) NULL,
    [Telefon] nvarchar(50) NULL,
    [Megjegyzes] nvarchar(1000) NULL,
    [Aktiv] bit NOT NULL,
    [CegId] int NOT NULL,
    CONSTRAINT [PK_Felulvizsgalok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Felulvizsgalok_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [MunkaszamSzamlalok] (
    [Id] int NOT NULL IDENTITY,
    [CegId] int NOT NULL,
    [Ev] int NOT NULL,
    [UtolsoSorszam] int NOT NULL,
    CONSTRAINT [PK_MunkaszamSzamlalok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MunkaszamSzamlalok_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]) ON DELETE CASCADE
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
    [Tevekenyseg] int NOT NULL,
    [CegId] int NOT NULL,
    CONSTRAINT [PK_Ugyfelek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Ugyfelek_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Kepzesek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Jogosultsag] int NOT NULL,
    [KepzesTipusId] int NULL,
    [BizonyitvanySzam] nvarchar(100) NULL,
    [BizonyitvanyKelte] datetime2 NULL,
    [BizonyitvanyLejarat] datetime2 NULL,
    [TovabbkepzesSzam] nvarchar(100) NULL,
    [UtolsoTovabbkepzes] datetime2 NULL,
    [FelujtoKepzesSzam] nvarchar(100) NULL,
    [KepzesLejarat] datetime2 NULL,
    [Megjegyzes] nvarchar(1000) NULL,
    [Aktiv] bit NOT NULL,
    [CegId] int NOT NULL,
    CONSTRAINT [PK_Kepzesek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Kepzesek_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Kepzesek_KepzesTipusok_KepzesTipusId] FOREIGN KEY ([KepzesTipusId]) REFERENCES [KepzesTipusok] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [KepzesSzabalyok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Tipus] int NOT NULL,
    [ForrasKepzesTipusId] int NOT NULL,
    [CelKepzesTipusId] int NOT NULL,
    [Megjegyzes] nvarchar(max) NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_KepzesSzabalyok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_KepzesSzabalyok_KepzesTipusok_CelKepzesTipusId] FOREIGN KEY ([CelKepzesTipusId]) REFERENCES [KepzesTipusok] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_KepzesSzabalyok_KepzesTipusok_ForrasKepzesTipusId] FOREIGN KEY ([ForrasKepzesTipusId]) REFERENCES [KepzesTipusok] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [JegyzokonyvSablonTetelek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [MeresTipusId] int NOT NULL,
    [OldalSzam] int NOT NULL,
    [Kategoria] nvarchar(200) NOT NULL,
    [Sorrend] int NOT NULL,
    [Felirat] nvarchar(500) NOT NULL,
    [LehetsegesErtekek] nvarchar(200) NOT NULL,
    [AlapertelmezettErtek] nvarchar(50) NOT NULL,
    [VanMegjegyzesMezo] bit NOT NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_JegyzokonyvSablonTetelek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_JegyzokonyvSablonTetelek_MeresTipusok_MeresTipusId] FOREIGN KEY ([MeresTipusId]) REFERENCES [MeresTipusok] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [MeresTipusJogszabalyok] (
    [Id] int NOT NULL IDENTITY,
    [MeresTipusId] int NOT NULL,
    [JogszabalyId] int NOT NULL,
    [Sorrend] int NOT NULL,
    CONSTRAINT [PK_MeresTipusJogszabalyok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MeresTipusJogszabalyok_Jogszabalyok_JogszabalyId] FOREIGN KEY ([JogszabalyId]) REFERENCES [Jogszabalyok] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MeresTipusJogszabalyok_MeresTipusok_MeresTipusId] FOREIGN KEY ([MeresTipusId]) REFERENCES [MeresTipusok] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [MeresTipusKepzesKovetelemenyei] (
    [Id] int NOT NULL IDENTITY,
    [MeresTipusId] int NOT NULL,
    [KepzesTipusId] int NOT NULL,
    [SablonLabel] nvarchar(50) NULL,
    [Kotelezo] bit NOT NULL,
    [AlternativaCsoport] int NOT NULL,
    [Prioritas] int NOT NULL,
    CONSTRAINT [PK_MeresTipusKepzesKovetelemenyei] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MeresTipusKepzesKovetelemenyei_KepzesTipusok_KepzesTipusId] FOREIGN KEY ([KepzesTipusId]) REFERENCES [KepzesTipusok] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_MeresTipusKepzesKovetelemenyei_MeresTipusok_MeresTipusId] FOREIGN KEY ([MeresTipusId]) REFERENCES [MeresTipusok] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [VizsgalatiSablonok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [MeresTipusId] int NOT NULL,
    [Nev] nvarchar(200) NOT NULL,
    [Leiras] nvarchar(500) NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_VizsgalatiSablonok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VizsgalatiSablonok_MeresTipusok_MeresTipusId] FOREIGN KEY ([MeresTipusId]) REFERENCES [MeresTipusok] ([Id]) ON DELETE CASCADE
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

CREATE TABLE [EszkozSablonAlkatreszek] (
    [Id] int NOT NULL IDENTITY,
    [EszkozSablonId] int NOT NULL,
    [Sorrend] int NOT NULL,
    [Megnevezes] nvarchar(200) NOT NULL,
    [Tipus] nvarchar(100) NULL,
    [Azonosito] nvarchar(100) NULL,
    [VedelmiOsztaly] nvarchar(max) NOT NULL,
    [Telj] nvarchar(max) NOT NULL,
    [Megtekint] nvarchar(max) NOT NULL,
    CONSTRAINT [PK_EszkozSablonAlkatreszek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_EszkozSablonAlkatreszek_EszkozSablonok_EszkozSablonId] FOREIGN KEY ([EszkozSablonId]) REFERENCES [EszkozSablonok] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [FelulvizsgaloKepzesek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [FelulvizsgaloId] int NOT NULL,
    [KepzesTipusId] int NULL,
    [BizonyitvanySzam] nvarchar(100) NULL,
    [BizonyitvanyKelte] datetime2 NULL,
    [BizonyitvanyLejarat] datetime2 NULL,
    [Megjegyzes] nvarchar(1000) NULL,
    [Aktiv] bit NOT NULL,
    CONSTRAINT [PK_FelulvizsgaloKepzesek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_FelulvizsgaloKepzesek_Felulvizsgalok_FelulvizsgaloId] FOREIGN KEY ([FelulvizsgaloId]) REFERENCES [Felulvizsgalok] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_FelulvizsgaloKepzesek_KepzesTipusok_KepzesTipusId] FOREIGN KEY ([KepzesTipusId]) REFERENCES [KepzesTipusok] ([Id]) ON DELETE SET NULL
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

CREATE TABLE [UgyfelLekerdezesiTokenek] (
    [Id] int NOT NULL IDENTITY,
    [UgyfelId] int NOT NULL,
    [Token] nvarchar(max) NOT NULL,
    [Aktiv] bit NOT NULL,
    [Lejarat] datetime2 NULL,
    [Letrehozva] datetime2 NOT NULL,
    [UtolsoHasznalat] datetime2 NULL,
    CONSTRAINT [PK_UgyfelLekerdezesiTokenek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_UgyfelLekerdezesiTokenek_Ugyfelek_UgyfelId] FOREIGN KEY ([UgyfelId]) REFERENCES [Ugyfelek] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [VizsgalatiSablonTetelErtekek] (
    [Id] int NOT NULL IDENTITY,
    [SablonId] int NOT NULL,
    [TetelId] int NOT NULL,
    [AlapertelmezettErtek] nvarchar(50) NOT NULL,
    CONSTRAINT [PK_VizsgalatiSablonTetelErtekek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_VizsgalatiSablonTetelErtekek_JegyzokonyvSablonTetelek_TetelId] FOREIGN KEY ([TetelId]) REFERENCES [JegyzokonyvSablonTetelek] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_VizsgalatiSablonTetelErtekek_VizsgalatiSablonok_SablonId] FOREIGN KEY ([SablonId]) REFERENCES [VizsgalatiSablonok] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [KepzesTovabbkepzesek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [FelulvizsgaloKepzesId] int NOT NULL,
    [Datum] datetime2 NOT NULL,
    [BizonyitvanySzam] nvarchar(100) NULL,
    [Hely] nvarchar(200) NULL,
    [Megjegyzes] nvarchar(1000) NULL,
    CONSTRAINT [PK_KepzesTovabbkepzesek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_KepzesTovabbkepzesek_FelulvizsgaloKepzesek_FelulvizsgaloKepzesId] FOREIGN KEY ([FelulvizsgaloKepzesId]) REFERENCES [FelulvizsgaloKepzesek] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [Eszkozok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [GyariSzam] nvarchar(100) NULL,
    [Tipus] nvarchar(100) NULL,
    [Kategoria] nvarchar(max) NULL,
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

CREATE TABLE [Helyisegek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL DEFAULT (CURRENT_TIMESTAMP),
    [Modositva] datetime2 NULL,
    [Nev] nvarchar(200) NOT NULL,
    [TelephelyId] int NULL,
    CONSTRAINT [PK_Helyisegek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Helyisegek_Telephelyek_TelephelyId] FOREIGN KEY ([TelephelyId]) REFERENCES [Telephelyek] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Hitelesitesek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [UgyfelId] int NULL,
    [TelephelyId] int NULL,
    [EszkozTipusId] int NOT NULL,
    [HatosagId] int NULL,
    [Darabszam] int NOT NULL,
    [Datum] datetime2 NOT NULL,
    [LejaratDatum] datetime2 NULL,
    [HitelesitesStatusz] int NOT NULL,
    [Megjegyzes] nvarchar(1000) NULL,
    [EgyediLejaratok] nvarchar(max) NULL,
    CONSTRAINT [PK_Hitelesitesek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Hitelesitesek_EszkozTipusok_EszkozTipusId] FOREIGN KEY ([EszkozTipusId]) REFERENCES [EszkozTipusok] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Hitelesitesek_Hatosagok_HatosagId] FOREIGN KEY ([HatosagId]) REFERENCES [Hatosagok] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_Hitelesitesek_Telephelyek_TelephelyId] FOREIGN KEY ([TelephelyId]) REFERENCES [Telephelyek] ([Id]) ON DELETE SET NULL,
    CONSTRAINT [FK_Hitelesitesek_Ugyfelek_UgyfelId] FOREIGN KEY ([UgyfelId]) REFERENCES [Ugyfelek] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [Karbantartasok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [CegId] int NOT NULL,
    [UgyfelId] int NOT NULL,
    [TelephelyId] int NOT NULL,
    [KarbantartasTipusId] int NOT NULL,
    [Datum] datetime2 NOT NULL,
    [KovetkezoDatum] datetime2 NULL,
    [Leiras] nvarchar(1000) NULL,
    [Elvegzo] nvarchar(200) NULL,
    [Elvegezve] bit NOT NULL,
    CONSTRAINT [PK_Karbantartasok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Karbantartasok_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Karbantartasok_KarbantartasTipusok_KarbantartasTipusId] FOREIGN KEY ([KarbantartasTipusId]) REFERENCES [KarbantartasTipusok] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Karbantartasok_Telephelyek_TelephelyId] FOREIGN KEY ([TelephelyId]) REFERENCES [Telephelyek] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Karbantartasok_Ugyfelek_UgyfelId] FOREIGN KEY ([UgyfelId]) REFERENCES [Ugyfelek] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [Kockazatertekelesek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Megnevezes] nvarchar(max) NOT NULL,
    [ErtekelesDatuma] datetime2 NOT NULL,
    [KovetkezoFelulvizsgalat] datetime2 NULL,
    [KockazatiSzint] int NOT NULL,
    [Leiras] nvarchar(max) NULL,
    [Intezkedesek] nvarchar(max) NULL,
    [FelelosNeve] nvarchar(max) NULL,
    [Statusz] int NOT NULL,
    [Aktiv] bit NOT NULL,
    [UgyfelId] int NOT NULL,
    [TelephelyId] int NULL,
    [CegId] int NOT NULL,
    CONSTRAINT [PK_Kockazatertekelesek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Kockazatertekelesek_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Kockazatertekelesek_Telephelyek_TelephelyId] FOREIGN KEY ([TelephelyId]) REFERENCES [Telephelyek] ([Id]),
    CONSTRAINT [FK_Kockazatertekelesek_Ugyfelek_UgyfelId] FOREIGN KEY ([UgyfelId]) REFERENCES [Ugyfelek] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [MunkavedelmiOktatasok] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Megnevezes] nvarchar(max) NOT NULL,
    [Leiras] nvarchar(max) NULL,
    [OktatasDatuma] datetime2 NOT NULL,
    [KovetkezoOktatas] datetime2 NULL,
    [IdoszakHonap] int NOT NULL,
    [OktatoNeve] nvarchar(max) NULL,
    [Megjegyzes] nvarchar(max) NULL,
    [Aktiv] bit NOT NULL,
    [UgyfelId] int NOT NULL,
    [TelephelyId] int NULL,
    [CegId] int NOT NULL,
    CONSTRAINT [PK_MunkavedelmiOktatasok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MunkavedelmiOktatasok_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MunkavedelmiOktatasok_Telephelyek_TelephelyId] FOREIGN KEY ([TelephelyId]) REFERENCES [Telephelyek] ([Id]),
    CONSTRAINT [FK_MunkavedelmiOktatasok_Ugyfelek_UgyfelId] FOREIGN KEY ([UgyfelId]) REFERENCES [Ugyfelek] ([Id]) ON DELETE CASCADE
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

CREATE TABLE [Zonaterkepek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [Megnevezes] nvarchar(max) NOT NULL,
    [ZonaTipus] int NOT NULL,
    [Leiras] nvarchar(max) NULL,
    [FajlNev] nvarchar(max) NULL,
    [FajlUtvonal] nvarchar(max) NULL,
    [ErvenyessegKezdete] datetime2 NULL,
    [ErvenyessegVege] datetime2 NULL,
    [Aktiv] bit NOT NULL,
    [UgyfelId] int NOT NULL,
    [TelephelyId] int NULL,
    [CegId] int NOT NULL,
    CONSTRAINT [PK_Zonaterkepek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Zonaterkepek_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_Zonaterkepek_Telephelyek_TelephelyId] FOREIGN KEY ([TelephelyId]) REFERENCES [Telephelyek] ([Id]),
    CONSTRAINT [FK_Zonaterkepek_Ugyfelek_UgyfelId] FOREIGN KEY ([UgyfelId]) REFERENCES [Ugyfelek] ([Id]) ON DELETE CASCADE
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

CREATE TABLE [Meresek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [UgyfelId] int NOT NULL,
    [TelephelyId] int NOT NULL,
    [HelyisegId] int NULL,
    [MeresTipusId] int NOT NULL,
    [Datum] datetime2 NOT NULL,
    [KovetkezoDatum] datetime2 NULL,
    [Eredmeny] nvarchar(500) NULL,
    [MeresStatusz] int NOT NULL,
    [Megjegyzes] nvarchar(1000) NULL,
    [JegyzokonyvAdatokJson] nvarchar(max) NULL,
    CONSTRAINT [PK_Meresek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Meresek_Helyisegek_HelyisegId] FOREIGN KEY ([HelyisegId]) REFERENCES [Helyisegek] ([Id]),
    CONSTRAINT [FK_Meresek_MeresTipusok_MeresTipusId] FOREIGN KEY ([MeresTipusId]) REFERENCES [MeresTipusok] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Meresek_Telephelyek_TelephelyId] FOREIGN KEY ([TelephelyId]) REFERENCES [Telephelyek] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_Meresek_Ugyfelek_UgyfelId] FOREIGN KEY ([UgyfelId]) REFERENCES [Ugyfelek] ([Id]) ON DELETE NO ACTION
);
GO

CREATE TABLE [MunkavedelmiOktatasResztvevok] (
    [Id] int NOT NULL IDENTITY,
    [MunkavedelmiOktatasId] int NOT NULL,
    [Nev] nvarchar(max) NOT NULL,
    [Beosztas] nvarchar(max) NULL,
    [Megjegyzes] nvarchar(max) NULL,
    CONSTRAINT [PK_MunkavedelmiOktatasResztvevok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MunkavedelmiOktatasResztvevok_MunkavedelmiOktatasok_MunkavedelmiOktatasId] FOREIGN KEY ([MunkavedelmiOktatasId]) REFERENCES [MunkavedelmiOktatasok] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [MellekletJegyzokonyvek] (
    [Id] int NOT NULL IDENTITY,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    [MeresId] int NOT NULL,
    [Tipus] nvarchar(20) NOT NULL,
    [Szam] nvarchar(100) NOT NULL,
    [Statusz] nvarchar(20) NOT NULL,
    [AdatokJson] nvarchar(max) NULL,
    [MellekletMeresId] int NULL,
    CONSTRAINT [PK_MellekletJegyzokonyvek] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MellekletJegyzokonyvek_Meresek_MellekletMeresId] FOREIGN KEY ([MellekletMeresId]) REFERENCES [Meresek] ([Id]),
    CONSTRAINT [FK_MellekletJegyzokonyvek_Meresek_MeresId] FOREIGN KEY ([MeresId]) REFERENCES [Meresek] ([Id]) ON DELETE CASCADE
);
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Aktiv', N'Leiras', N'Letrehozva', N'Nev', N'Sorrend') AND [object_id] = OBJECT_ID(N'[ErintesvedelmiModOsztalyok]'))
    SET IDENTITY_INSERT [ErintesvedelmiModOsztalyok] ON;
INSERT INTO [ErintesvedelmiModOsztalyok] ([Id], [Aktiv], [Leiras], [Letrehozva], [Nev], [Sorrend])
VALUES (1, CAST(1 AS bit), N'I. védelmi osztály, 0 ohm ellenállás', '2024-01-01T00:00:00.0000000', N'I/0Ω', 1),
(2, CAST(1 AS bit), N'II. védelmi osztály (kettős vagy megerősített szigetelés)', '2024-01-01T00:00:00.0000000', N'II/Ω', 2),
(3, CAST(1 AS bit), N'III. védelmi osztály (SELV/PELV - kisfeszültség)', '2024-01-01T00:00:00.0000000', N'III/Ω', 3),
(4, CAST(1 AS bit), N'I. védelmi osztály - alternatív', '2024-01-01T00:00:00.0000000', N'I/0Ω', 4);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Aktiv', N'Leiras', N'Letrehozva', N'Nev', N'Sorrend') AND [object_id] = OBJECT_ID(N'[ErintesvedelmiModOsztalyok]'))
    SET IDENTITY_INSERT [ErintesvedelmiModOsztalyok] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Aktiv', N'HitelesitesiIdotartamHonap', N'Letrehozva', N'Nev') AND [object_id] = OBJECT_ID(N'[EszkozTipusok]'))
    SET IDENTITY_INSERT [EszkozTipusok] ON;
INSERT INTO [EszkozTipusok] ([Id], [Aktiv], [HitelesitesiIdotartamHonap], [Letrehozva], [Nev])
VALUES (1, CAST(1 AS bit), 12, '2024-01-01T00:00:00.0000000', N'Kútoszlop'),
(2, CAST(1 AS bit), 12, '2024-01-01T00:00:00.0000000', N'Szintmérő'),
(3, CAST(1 AS bit), 12, '2024-01-01T00:00:00.0000000', N'Átfolyásmérő'),
(4, CAST(1 AS bit), 12, '2024-01-01T00:00:00.0000000', N'Tartály');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Aktiv', N'HitelesitesiIdotartamHonap', N'Letrehozva', N'Nev') AND [object_id] = OBJECT_ID(N'[EszkozTipusok]'))
    SET IDENTITY_INSERT [EszkozTipusok] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Aktiv', N'IsmetlodesHonap', N'Leiras', N'Letrehozva', N'Modositva', N'Nev') AND [object_id] = OBJECT_ID(N'[KarbantartasTipusok]'))
    SET IDENTITY_INSERT [KarbantartasTipusok] ON;
INSERT INTO [KarbantartasTipusok] ([Id], [Aktiv], [IsmetlodesHonap], [Leiras], [Letrehozva], [Modositva], [Nev])
VALUES (1, CAST(1 AS bit), 0, NULL, '2024-01-01T00:00:00.0000000', NULL, N'Eseti karbantartás'),
(2, CAST(1 AS bit), 3, NULL, '2024-01-01T00:00:00.0000000', NULL, N'Negyedéves karbantartás'),
(3, CAST(1 AS bit), 6, NULL, '2024-01-01T00:00:00.0000000', NULL, N'Féléves karbantartás'),
(4, CAST(1 AS bit), 12, NULL, '2024-01-01T00:00:00.0000000', NULL, N'Éves karbantartás');
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Aktiv', N'IsmetlodesHonap', N'Leiras', N'Letrehozva', N'Modositva', N'Nev') AND [object_id] = OBJECT_ID(N'[KarbantartasTipusok]'))
    SET IDENTITY_INSERT [KarbantartasTipusok] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Aktiv', N'Label', N'Leiras', N'Lejar', N'LejaratEvek', N'Letrehozva', N'Nev', N'TovabbkepzesCsakFelulvizsgalonak', N'TovabbkepzesEvek', N'TovabbkepzesKotelezo') AND [object_id] = OBJECT_ID(N'[KepzesTipusok]'))
    SET IDENTITY_INSERT [KepzesTipusok] ON;
INSERT INTO [KepzesTipusok] ([Id], [Aktiv], [Label], [Leiras], [Lejar], [LejaratEvek], [Letrehozva], [Nev], [TovabbkepzesCsakFelulvizsgalonak], [TovabbkepzesEvek], [TovabbkepzesKotelezo])
VALUES (1, CAST(1 AS bit), NULL, NULL, CAST(0 AS bit), NULL, '2024-01-01T00:00:00.0000000', N'Alapító okirat szerinti képzés', CAST(0 AS bit), NULL, CAST(0 AS bit)),
(2, CAST(1 AS bit), NULL, NULL, CAST(0 AS bit), NULL, '2024-01-01T00:00:00.0000000', N'ISO 9001:2015 átállási képzés', CAST(0 AS bit), NULL, CAST(0 AS bit)),
(3, CAST(1 AS bit), NULL, NULL, CAST(0 AS bit), NULL, '2024-01-01T00:00:00.0000000', N'Minőségügyi auditor képzés', CAST(0 AS bit), NULL, CAST(0 AS bit)),
(4, CAST(1 AS bit), NULL, NULL, CAST(0 AS bit), NULL, '2024-01-01T00:00:00.0000000', N'Vezetői képzés', CAST(0 AS bit), NULL, CAST(0 AS bit));
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Aktiv', N'Label', N'Leiras', N'Lejar', N'LejaratEvek', N'Letrehozva', N'Nev', N'TovabbkepzesCsakFelulvizsgalonak', N'TovabbkepzesEvek', N'TovabbkepzesKotelezo') AND [object_id] = OBJECT_ID(N'[KepzesTipusok]'))
    SET IDENTITY_INSERT [KepzesTipusok] OFF;
GO

IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Aktiv', N'KarakterisztikaFeluliras', N'Leiras', N'Letrehozva', N'Nev', N'NevlegesAram') AND [object_id] = OBJECT_ID(N'[TularamvedelemTipusok]'))
    SET IDENTITY_INSERT [TularamvedelemTipusok] ON;
INSERT INTO [TularamvedelemTipusok] ([Id], [Aktiv], [KarakterisztikaFeluliras], [Leiras], [Letrehozva], [Nev], [NevlegesAram])
VALUES (1, CAST(1 AS bit), NULL, N'Schneider Electric 16A típusú védőkapcsoló', '2024-01-01T00:00:00.0000000', N'A9Z422316', 16.0),
(2, CAST(1 AS bit), NULL, NULL, '2024-01-01T00:00:00.0000000', N'TDK-C16', 16.0),
(3, CAST(1 AS bit), NULL, NULL, '2024-01-01T00:00:00.0000000', N'TDK-C25', 25.0);
IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Aktiv', N'KarakterisztikaFeluliras', N'Leiras', N'Letrehozva', N'Nev', N'NevlegesAram') AND [object_id] = OBJECT_ID(N'[TularamvedelemTipusok]'))
    SET IDENTITY_INSERT [TularamvedelemTipusok] OFF;
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

CREATE INDEX [IX_AutomatikaFutasNaplok_FutasiIdo] ON [AutomatikaFutasNaplok] ([FutasiIdo] DESC);
GO

CREATE INDEX [IX_AutomatikaFutasNaplok_Sikeres_FutasiIdo] ON [AutomatikaFutasNaplok] ([Sikeres], [FutasiIdo]);
GO

CREATE UNIQUE INDEX [IX_EmailBeallitasok_CegId] ON [EmailBeallitasok] ([CegId]);
GO

CREATE INDEX [IX_EmailKuldesNaplok_Kuldve] ON [EmailKuldesNaplok] ([Kuldve]);
GO

CREATE INDEX [IX_EmailSablonok_CegId] ON [EmailSablonok] ([CegId]);
GO

CREATE INDEX [IX_Eszkozok_GyartoId] ON [Eszkozok] ([GyartoId]);
GO

CREATE INDEX [IX_Eszkozok_TelephelyId] ON [Eszkozok] ([TelephelyId]);
GO

CREATE INDEX [IX_Eszkozok_UgyfelId] ON [Eszkozok] ([UgyfelId]);
GO

CREATE INDEX [IX_EszkozSablonAlkatreszek_EszkozSablonId_Sorrend] ON [EszkozSablonAlkatreszek] ([EszkozSablonId], [Sorrend]);
GO

CREATE INDEX [IX_EszkozSablonok_CegId_Megnevezes] ON [EszkozSablonok] ([CegId], [Megnevezes]);
GO

CREATE UNIQUE INDEX [IX_FelhasznaloErtesitesBeallitasok_FelhasznaloId] ON [FelhasznaloErtesitesBeallitasok] ([FelhasznaloId]);
GO

CREATE INDEX [IX_Felulvizsgalok_CegId] ON [Felulvizsgalok] ([CegId]);
GO

CREATE UNIQUE INDEX [IX_FelulvizsgaloKepzesek_FelulvizsgaloId_KepzesTipusId] ON [FelulvizsgaloKepzesek] ([FelulvizsgaloId], [KepzesTipusId]) WHERE [KepzesTipusId] IS NOT NULL;
GO

CREATE INDEX [IX_FelulvizsgaloKepzesek_KepzesTipusId] ON [FelulvizsgaloKepzesek] ([KepzesTipusId]);
GO

CREATE INDEX [IX_Helyisegek_TelephelyId] ON [Helyisegek] ([TelephelyId]);
GO

CREATE INDEX [IX_Hitelesitesek_EszkozTipusId] ON [Hitelesitesek] ([EszkozTipusId]);
GO

CREATE INDEX [IX_Hitelesitesek_HatosagId] ON [Hitelesitesek] ([HatosagId]);
GO

CREATE INDEX [IX_Hitelesitesek_TelephelyId] ON [Hitelesitesek] ([TelephelyId]);
GO

CREATE INDEX [IX_Hitelesitesek_UgyfelId] ON [Hitelesitesek] ([UgyfelId]);
GO

CREATE INDEX [IX_JegyzokonyvSablonTetelek_MeresTipusId] ON [JegyzokonyvSablonTetelek] ([MeresTipusId]);
GO

CREATE INDEX [IX_Kalibraciok_EszkozId] ON [Kalibraciok] ([EszkozId]);
GO

CREATE INDEX [IX_Karbantartasok_CegId] ON [Karbantartasok] ([CegId]);
GO

CREATE INDEX [IX_Karbantartasok_KarbantartasTipusId] ON [Karbantartasok] ([KarbantartasTipusId]);
GO

CREATE INDEX [IX_Karbantartasok_TelephelyId] ON [Karbantartasok] ([TelephelyId]);
GO

CREATE INDEX [IX_Karbantartasok_UgyfelId] ON [Karbantartasok] ([UgyfelId]);
GO

CREATE INDEX [IX_Kepzesek_CegId] ON [Kepzesek] ([CegId]);
GO

CREATE INDEX [IX_Kepzesek_KepzesTipusId] ON [Kepzesek] ([KepzesTipusId]);
GO

CREATE INDEX [IX_KepzesSzabalyok_CelKepzesTipusId] ON [KepzesSzabalyok] ([CelKepzesTipusId]);
GO

CREATE INDEX [IX_KepzesSzabalyok_ForrasKepzesTipusId] ON [KepzesSzabalyok] ([ForrasKepzesTipusId]);
GO

CREATE UNIQUE INDEX [IX_KepzesSzabalyok_Tipus_ForrasKepzesTipusId_CelKepzesTipusId] ON [KepzesSzabalyok] ([Tipus], [ForrasKepzesTipusId], [CelKepzesTipusId]);
GO

CREATE INDEX [IX_KepzesTovabbkepzesek_FelulvizsgaloKepzesId] ON [KepzesTovabbkepzesek] ([FelulvizsgaloKepzesId]);
GO

CREATE INDEX [IX_Kockazatertekelesek_CegId] ON [Kockazatertekelesek] ([CegId]);
GO

CREATE INDEX [IX_Kockazatertekelesek_TelephelyId] ON [Kockazatertekelesek] ([TelephelyId]);
GO

CREATE INDEX [IX_Kockazatertekelesek_UgyfelId] ON [Kockazatertekelesek] ([UgyfelId]);
GO

CREATE INDEX [IX_MellekletJegyzokonyvek_MellekletMeresId] ON [MellekletJegyzokonyvek] ([MellekletMeresId]);
GO

CREATE INDEX [IX_MellekletJegyzokonyvek_MeresId] ON [MellekletJegyzokonyvek] ([MeresId]);
GO

CREATE INDEX [IX_Meresek_HelyisegId] ON [Meresek] ([HelyisegId]);
GO

CREATE INDEX [IX_Meresek_MeresTipusId] ON [Meresek] ([MeresTipusId]);
GO

CREATE INDEX [IX_Meresek_TelephelyId] ON [Meresek] ([TelephelyId]);
GO

CREATE INDEX [IX_Meresek_UgyfelId] ON [Meresek] ([UgyfelId]);
GO

CREATE INDEX [IX_MeresTipusJogszabalyok_JogszabalyId] ON [MeresTipusJogszabalyok] ([JogszabalyId]);
GO

CREATE INDEX [IX_MeresTipusJogszabalyok_MeresTipusId] ON [MeresTipusJogszabalyok] ([MeresTipusId]);
GO

CREATE INDEX [IX_MeresTipusKepzesKovetelemenyei_KepzesTipusId] ON [MeresTipusKepzesKovetelemenyei] ([KepzesTipusId]);
GO

CREATE UNIQUE INDEX [IX_MeresTipusKepzesKovetelemenyei_MeresTipusId_KepzesTipusId] ON [MeresTipusKepzesKovetelemenyei] ([MeresTipusId], [KepzesTipusId]);
GO

CREATE UNIQUE INDEX [IX_MunkaszamSzamlalok_CegId_Ev] ON [MunkaszamSzamlalok] ([CegId], [Ev]);
GO

CREATE INDEX [IX_MunkavedelmiOktatasok_CegId] ON [MunkavedelmiOktatasok] ([CegId]);
GO

CREATE INDEX [IX_MunkavedelmiOktatasok_TelephelyId] ON [MunkavedelmiOktatasok] ([TelephelyId]);
GO

CREATE INDEX [IX_MunkavedelmiOktatasok_UgyfelId] ON [MunkavedelmiOktatasok] ([UgyfelId]);
GO

CREATE INDEX [IX_MunkavedelmiOktatasResztvevok_MunkavedelmiOktatasId] ON [MunkavedelmiOktatasResztvevok] ([MunkavedelmiOktatasId]);
GO

CREATE INDEX [IX_Tanusitvanyok_UgyfelId] ON [Tanusitvanyok] ([UgyfelId]);
GO

CREATE INDEX [IX_Telephelyek_UgyfelId] ON [Telephelyek] ([UgyfelId]);
GO

CREATE INDEX [IX_Terminalok_TelephelyId] ON [Terminalok] ([TelephelyId]);
GO

CREATE UNIQUE INDEX [IX_TularamvedelemTipusok_Nev] ON [TularamvedelemTipusok] ([Nev]);
GO

CREATE INDEX [IX_Ugyfelek_CegId] ON [Ugyfelek] ([CegId]);
GO

CREATE INDEX [IX_UgyfelLekerdezesiTokenek_UgyfelId] ON [UgyfelLekerdezesiTokenek] ([UgyfelId]);
GO

CREATE INDEX [IX_VizsgalatiSablonok_MeresTipusId] ON [VizsgalatiSablonok] ([MeresTipusId]);
GO

CREATE INDEX [IX_VizsgalatiSablonTetelErtekek_SablonId] ON [VizsgalatiSablonTetelErtekek] ([SablonId]);
GO

CREATE INDEX [IX_VizsgalatiSablonTetelErtekek_TetelId] ON [VizsgalatiSablonTetelErtekek] ([TetelId]);
GO

CREATE INDEX [IX_Zonaterkepek_CegId] ON [Zonaterkepek] ([CegId]);
GO

CREATE INDEX [IX_Zonaterkepek_TelephelyId] ON [Zonaterkepek] ([TelephelyId]);
GO

CREATE INDEX [IX_Zonaterkepek_UgyfelId] ON [Zonaterkepek] ([UgyfelId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260509103833_UgyfelLekerdezesiToken', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

EXEC sp_rename N'[UgyfelLekerdezesiTokenek].[Lejarat]', N'LejarDatum', N'COLUMN';
GO

EXEC sp_rename N'[Karbantartasok].[Elvegezve]', N'Aktiv', N'COLUMN';
GO

ALTER TABLE [VizsgalatiSablonok] ADD [AdatokJson] nvarchar(max) NULL;
GO

ALTER TABLE [VizsgalatiSablonok] ADD [CegId] int NULL;
GO

ALTER TABLE [TularamvedelemTipusok] ADD [FelulvizsgalasraVar] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [MeresTipusok] ADD [FoMeres] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Meresek] ADD [Aktiv] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Karbantartasok] ADD [Statusz] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [JegyzokonyvSablonTetelek] ADD [CegId] int NULL;
GO

ALTER TABLE [Hitelesitesek] ADD [Aktiv] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Hitelesitesek] ADD [CsoportTagLejaratok] nvarchar(max) NULL;
GO

ALTER TABLE [Hitelesitesek] ADD [EszkozAzonosito] nvarchar(200) NULL;
GO

ALTER TABLE [ErintesvedelmiModOsztalyok] ADD [VanAvk] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [ErintesvedelmiModOsztalyok] ADD [VanPeFolyt] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [EmailKuldesNaplok] ADD [KarbantartasId] int NULL;
GO

ALTER TABLE [Cegek] ADD [NavLoginName] nvarchar(max) NULL;
GO

ALTER TABLE [Cegek] ADD [NavPassword] nvarchar(max) NULL;
GO

ALTER TABLE [Cegek] ADD [NavTaxNumber] nvarchar(max) NULL;
GO

ALTER TABLE [Cegek] ADD [NavTesztKornyezet] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [Cegek] ADD [NavXmlSignKey] nvarchar(max) NULL;
GO

ALTER TABLE [AvkVedelemTipusok] ADD [FelulvizsgalasraVar] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

CREATE TABLE [FelhasznaloCegek] (
    [FelhasznaloId] nvarchar(450) NOT NULL,
    [CegId] int NOT NULL,
    [Szerep] nvarchar(max) NULL,
    [Hozzaadva] datetime2 NOT NULL,
    CONSTRAINT [PK_FelhasznaloCegek] PRIMARY KEY ([FelhasznaloId], [CegId]),
    CONSTRAINT [FK_FelhasznaloCegek_AspNetUsers_FelhasznaloId] FOREIGN KEY ([FelhasznaloId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_FelhasznaloCegek_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [HitelesitesCsoportok] (
    [Id] int NOT NULL IDENTITY,
    [Nev] nvarchar(150) NOT NULL,
    [Leiras] nvarchar(500) NULL,
    [FoEszkozTipusId] int NULL,
    [Aktiv] bit NOT NULL,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    CONSTRAINT [PK_HitelesitesCsoportok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HitelesitesCsoportok_EszkozTipusok_FoEszkozTipusId] FOREIGN KEY ([FoEszkozTipusId]) REFERENCES [EszkozTipusok] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [JogszabalyTagek] (
    [Id] int NOT NULL IDENTITY,
    [Nev] nvarchar(100) NOT NULL,
    [Szin] nvarchar(20) NOT NULL,
    CONSTRAINT [PK_JogszabalyTagek] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [MeresCsoportok] (
    [Id] int NOT NULL IDENTITY,
    [Nev] nvarchar(150) NOT NULL,
    [Leiras] nvarchar(500) NULL,
    [FoMeresTipusId] int NULL,
    [Aktiv] bit NOT NULL,
    [Letrehozva] datetime2 NOT NULL,
    [Modositva] datetime2 NULL,
    CONSTRAINT [PK_MeresCsoportok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MeresCsoportok_MeresTipusok_FoMeresTipusId] FOREIGN KEY ([FoMeresTipusId]) REFERENCES [MeresTipusok] ([Id]) ON DELETE SET NULL
);
GO

CREATE TABLE [HitelesitesCsoportTagok] (
    [Id] int NOT NULL IDENTITY,
    [HitelesitesCsoportId] int NOT NULL,
    [EszkozTipusId] int NOT NULL,
    [Kotelezo] bit NOT NULL,
    [Sorrend] int NOT NULL,
    [Megjegyzes] nvarchar(500) NULL,
    CONSTRAINT [PK_HitelesitesCsoportTagok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_HitelesitesCsoportTagok_EszkozTipusok_EszkozTipusId] FOREIGN KEY ([EszkozTipusId]) REFERENCES [EszkozTipusok] ([Id]) ON DELETE NO ACTION,
    CONSTRAINT [FK_HitelesitesCsoportTagok_HitelesitesCsoportok_HitelesitesCsoportId] FOREIGN KEY ([HitelesitesCsoportId]) REFERENCES [HitelesitesCsoportok] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [JogszabalyTagKapcsolatok] (
    [JogszabalyId] int NOT NULL,
    [TagekId] int NOT NULL,
    CONSTRAINT [PK_JogszabalyTagKapcsolatok] PRIMARY KEY ([JogszabalyId], [TagekId]),
    CONSTRAINT [FK_JogszabalyTagKapcsolatok_JogszabalyTagek_TagekId] FOREIGN KEY ([TagekId]) REFERENCES [JogszabalyTagek] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_JogszabalyTagKapcsolatok_Jogszabalyok_JogszabalyId] FOREIGN KEY ([JogszabalyId]) REFERENCES [Jogszabalyok] ([Id]) ON DELETE CASCADE
);
GO

CREATE TABLE [MeresCsoportTagok] (
    [Id] int NOT NULL IDENTITY,
    [MeresCsoportId] int NOT NULL,
    [MeresTipusId] int NOT NULL,
    [Kotelezo] bit NOT NULL,
    [Sorrend] int NOT NULL,
    [Megjegyzes] nvarchar(500) NULL,
    CONSTRAINT [PK_MeresCsoportTagok] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_MeresCsoportTagok_MeresCsoportok_MeresCsoportId] FOREIGN KEY ([MeresCsoportId]) REFERENCES [MeresCsoportok] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_MeresCsoportTagok_MeresTipusok_MeresTipusId] FOREIGN KEY ([MeresTipusId]) REFERENCES [MeresTipusok] ([Id]) ON DELETE NO ACTION
);
GO

UPDATE [ErintesvedelmiModOsztalyok] SET [VanAvk] = CAST(1 AS bit), [VanPeFolyt] = CAST(1 AS bit)
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [ErintesvedelmiModOsztalyok] SET [VanAvk] = CAST(1 AS bit), [VanPeFolyt] = CAST(1 AS bit)
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [ErintesvedelmiModOsztalyok] SET [VanAvk] = CAST(1 AS bit), [VanPeFolyt] = CAST(1 AS bit)
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

UPDATE [ErintesvedelmiModOsztalyok] SET [VanAvk] = CAST(1 AS bit), [VanPeFolyt] = CAST(1 AS bit)
WHERE [Id] = 4;
SELECT @@ROWCOUNT;

GO

UPDATE [TularamvedelemTipusok] SET [FelulvizsgalasraVar] = CAST(0 AS bit)
WHERE [Id] = 1;
SELECT @@ROWCOUNT;

GO

UPDATE [TularamvedelemTipusok] SET [FelulvizsgalasraVar] = CAST(0 AS bit)
WHERE [Id] = 2;
SELECT @@ROWCOUNT;

GO

UPDATE [TularamvedelemTipusok] SET [FelulvizsgalasraVar] = CAST(0 AS bit)
WHERE [Id] = 3;
SELECT @@ROWCOUNT;

GO

CREATE INDEX [IX_VizsgalatiSablonok_CegId] ON [VizsgalatiSablonok] ([CegId]);
GO

CREATE INDEX [IX_JegyzokonyvSablonTetelek_CegId] ON [JegyzokonyvSablonTetelek] ([CegId]);
GO

CREATE INDEX [IX_FelhasznaloCegek_CegId] ON [FelhasznaloCegek] ([CegId]);
GO

CREATE INDEX [IX_HitelesitesCsoportok_FoEszkozTipusId] ON [HitelesitesCsoportok] ([FoEszkozTipusId]);
GO

CREATE INDEX [IX_HitelesitesCsoportTagok_EszkozTipusId] ON [HitelesitesCsoportTagok] ([EszkozTipusId]);
GO

CREATE UNIQUE INDEX [IX_HitelesitesCsoportTagok_HitelesitesCsoportId_EszkozTipusId] ON [HitelesitesCsoportTagok] ([HitelesitesCsoportId], [EszkozTipusId]);
GO

CREATE INDEX [IX_JogszabalyTagKapcsolatok_TagekId] ON [JogszabalyTagKapcsolatok] ([TagekId]);
GO

CREATE INDEX [IX_MeresCsoportok_FoMeresTipusId] ON [MeresCsoportok] ([FoMeresTipusId]);
GO

CREATE UNIQUE INDEX [IX_MeresCsoportTagok_MeresCsoportId_MeresTipusId] ON [MeresCsoportTagok] ([MeresCsoportId], [MeresTipusId]);
GO

CREATE INDEX [IX_MeresCsoportTagok_MeresTipusId] ON [MeresCsoportTagok] ([MeresTipusId]);
GO

ALTER TABLE [JegyzokonyvSablonTetelek] ADD CONSTRAINT [FK_JegyzokonyvSablonTetelek_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]);
GO

ALTER TABLE [VizsgalatiSablonok] ADD CONSTRAINT [FK_VizsgalatiSablonok_Cegek_CegId] FOREIGN KEY ([CegId]) REFERENCES [Cegek] ([Id]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260624203518_KarbantartasStatusz', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Hitelesitesek] ADD [BizonyitvanyPath] nvarchar(500) NULL;
GO

ALTER TABLE [Hitelesitesek] ADD [MunkalapPath] nvarchar(500) NULL;
GO

ALTER TABLE [FelulvizsgaloKepzesek] ADD [BizonyitvanyPath] nvarchar(max) NULL;
GO

ALTER TABLE [Felulvizsgalok] ADD [AlairasPath] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260629163936_HitelesitesFileUploads', N'8.0.0');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Hitelesitesek] ADD [MunkalapPath] nvarchar(500) NULL;
GO

ALTER TABLE [Hitelesitesek] ADD [BizonyitvanyPath] nvarchar(500) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20260629170906_AddHitelesitesFajlok', N'8.0.0');
GO

COMMIT;
GO

