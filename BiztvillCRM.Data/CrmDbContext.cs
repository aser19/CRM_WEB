using BiztvillCRM.Shared.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BiztvillCRM.Data;

public class CrmDbContext : IdentityDbContext<Felhasznalo>
{
    public CrmDbContext(DbContextOptions<CrmDbContext> options) : base(options) { }

    // --- Cégek (tenants) ---
    public DbSet<Ceg> Cegek { get; set; }

    // --- Törzsadatok ---
    public DbSet<Ugyfel> Ugyfelek { get; set; }
    public DbSet<Telephely> Telephelyek { get; set; }
    public DbSet<Gyarto> Gyartok { get; set; }
    public DbSet<Eszkoz> Eszkozok { get; set; }
    public DbSet<Terminal> Terminalok { get; set; }
    public DbSet<EszkozTipus> EszkozTipusok { get; set; }
    public DbSet<KarbantartasTipus> KarbantartasTipusok { get; set; } // <-- Új DbSet a KarbantartasTipus-hoz

    // --- Mérések ---
    public DbSet<MeresTipus> MeresTipusok { get; set; }
    public DbSet<Meres> Meresek { get; set; }
    public DbSet<Kalibracio> Kalibraciok { get; set; }

    // --- Hitelesítések ---
    public DbSet<Hatosag> Hatosagok { get; set; }
    public DbSet<Hitelesites> Hitelesitesek { get; set; }

    // --- Tanúsítványok / Képzések ---
    public DbSet<Tanusitvany> Tanusitvanyok { get; set; }
    public DbSet<Kepzes> Kepzesek { get; set; }

    // --- Karbantartás ---
    public DbSet<Karbantartas> Karbantartasok { get; set; }  // <-- JAVÍTVA: Karbantartasok volt Karbantartasok

    // --- Jogszabályok ---
    public DbSet<Jogszabaly> Jogszabalyok { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // --- Ceg ---
        modelBuilder.Entity<Ceg>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nev).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Adoszam).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Telefon).HasMaxLength(50);
            entity.Property(e => e.Cim).HasMaxLength(500);
            entity.Property(e => e.Weboldal).HasMaxLength(500);
        });

        // --- Felhasznalo ---
        modelBuilder.Entity<Felhasznalo>(entity =>
        {
            entity.Property(e => e.Nev).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Beosztas).HasMaxLength(100);
            entity.Property(e => e.Telefon).HasMaxLength(50);
            entity.HasOne(e => e.Ceg).WithMany(c => c.Felhasznalok).HasForeignKey(e => e.CegId).OnDelete(DeleteBehavior.Restrict);
        });

        // --- Ugyfel ---
        modelBuilder.Entity<Ugyfel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nev).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Adoszam).HasMaxLength(20);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Telefon).HasMaxLength(50);
            entity.Property(e => e.Cim).HasMaxLength(500);
            entity.HasOne(e => e.Ceg).WithMany(c => c.Ugyfelek).HasForeignKey(e => e.CegId).OnDelete(DeleteBehavior.Restrict);
        });

        // --- Telephely ---
        modelBuilder.Entity<Telephely>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nev).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Cim).HasMaxLength(500);
            entity.Property(e => e.Email).HasMaxLength(200);
            entity.Property(e => e.Telefon).HasMaxLength(50);
            entity.Property(e => e.Kapcsolattarto).HasMaxLength(200);
            entity.HasOne(e => e.Ugyfel).WithMany(u => u.Telephelyek).HasForeignKey(e => e.UgyfelId).OnDelete(DeleteBehavior.Restrict);
        });

        // --- Gyarto ---
        modelBuilder.Entity<Gyarto>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nev).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Orszag).HasMaxLength(100);
            entity.Property(e => e.Weboldal).HasMaxLength(500);
        });

        // --- Eszkoz ---
        modelBuilder.Entity<Eszkoz>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nev).IsRequired().HasMaxLength(200);
            entity.Property(e => e.GyariSzam).HasMaxLength(100);
            entity.Property(e => e.Tipus).HasMaxLength(100);
            entity.HasOne(e => e.Gyarto).WithMany().HasForeignKey(e => e.GyartoId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Ugyfel).WithMany().HasForeignKey(e => e.UgyfelId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Telephely).WithMany().HasForeignKey(e => e.TelephelyId).OnDelete(DeleteBehavior.Restrict);
        });

        // --- Terminal ---
        modelBuilder.Entity<Terminal>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nev).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Azonosito).HasMaxLength(100);
            entity.Property(e => e.IpCim).HasMaxLength(50);
            entity.Property(e => e.Megjegyzes).HasMaxLength(1000);
            entity.HasOne(e => e.Telephely).WithMany().HasForeignKey(e => e.TelephelyId).OnDelete(DeleteBehavior.SetNull);
        });

        // --- MeresTipus ---
        modelBuilder.Entity<MeresTipus>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nev).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Leiras).HasMaxLength(1000);
        });

        // --- Meres ---
        modelBuilder.Entity<Meres>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Eredmeny).HasMaxLength(500);
            entity.Property(e => e.Megjegyzes).HasMaxLength(1000);
            entity.HasOne(e => e.Ugyfel).WithMany().HasForeignKey(e => e.UgyfelId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Telephely).WithMany().HasForeignKey(e => e.TelephelyId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.MeresTipus).WithMany().HasForeignKey(e => e.MeresTipusId).OnDelete(DeleteBehavior.Restrict);
        });

        // --- Kalibracio ---
        modelBuilder.Entity<Kalibracio>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Bizonyitvany).HasMaxLength(200);
            entity.Property(e => e.Elvegzo).HasMaxLength(200);
            entity.Property(e => e.Megjegyzes).HasMaxLength(1000);
            entity.HasOne(e => e.Eszkoz).WithMany().HasForeignKey(e => e.EszkozId).OnDelete(DeleteBehavior.Restrict);
        });

        // --- Hatosag ---
        modelBuilder.Entity<Hatosag>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nev).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Rovidites).HasMaxLength(100);  // <-- 20-ról 100-ra növelve
            entity.Property(e => e.Cim).HasMaxLength(500);
            entity.Property(e => e.Weboldal).HasMaxLength(500);
        });

        // --- EszkozTipus ---
        modelBuilder.Entity<EszkozTipus>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nev).IsRequired().HasMaxLength(200);
        });

        // --- Hitelesites ---
        modelBuilder.Entity<Hitelesites>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Megjegyzes).HasMaxLength(1000);
            entity.HasOne(e => e.Ugyfel).WithMany().HasForeignKey(e => e.UgyfelId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.Telephely).WithMany().HasForeignKey(e => e.TelephelyId).OnDelete(DeleteBehavior.SetNull);
            entity.HasOne(e => e.EszkozTipus).WithMany().HasForeignKey(e => e.EszkozTipusId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Hatosag).WithMany().HasForeignKey(e => e.HatosagId).OnDelete(DeleteBehavior.SetNull);
        });

        // --- Tanusitvany ---
        modelBuilder.Entity<Tanusitvany>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nev).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Szam).HasMaxLength(100);
            entity.Property(e => e.Megjegyzes).HasMaxLength(1000);
            entity.HasOne(e => e.Ugyfel).WithMany().HasForeignKey(e => e.UgyfelId).OnDelete(DeleteBehavior.Restrict);
        });

        // --- Kepzes ---
        modelBuilder.Entity<Kepzes>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nev).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Resztvevo).HasMaxLength(500);
            entity.Property(e => e.Megjegyzes).HasMaxLength(1000);
        });

        // --- KarbantartasTipus ---
        modelBuilder.Entity<KarbantartasTipus>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nev).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Leiras).HasMaxLength(1000);
        });

        // --- Karbantartas (frissített) ---
        modelBuilder.Entity<Karbantartas>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Leiras).HasMaxLength(1000);
            entity.Property(e => e.Elvegzo).HasMaxLength(200);
            entity.HasOne(e => e.Ceg).WithMany().HasForeignKey(e => e.CegId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Ugyfel).WithMany().HasForeignKey(e => e.UgyfelId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.Telephely).WithMany().HasForeignKey(e => e.TelephelyId).OnDelete(DeleteBehavior.Restrict);
            entity.HasOne(e => e.KarbantartasTipus).WithMany().HasForeignKey(e => e.KarbantartasTipusId).OnDelete(DeleteBehavior.Restrict);
        });

        // --- Jogszabaly ---
        modelBuilder.Entity<Jogszabaly>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Szam).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Cim).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Leiras).HasMaxLength(2000);
            entity.Property(e => e.Url).HasMaxLength(500);
            entity.Property(e => e.Megjegyzes).HasMaxLength(1000);
            // A Terulet automatikusan int-ként tárolódik (Flags enum)
        });

        // Seed adatok az EszkozTipus-hoz:
        modelBuilder.Entity<EszkozTipus>().HasData(
            new EszkozTipus { Id = 1, Nev = "Kútoszlop", Aktiv = true, Letrehozva = new DateTime(2024, 1, 1) },
            new EszkozTipus { Id = 2, Nev = "Szintmérő", Aktiv = true, Letrehozva = new DateTime(2024, 1, 1) },
            new EszkozTipus { Id = 3, Nev = "Átfolyásmérő", Aktiv = true, Letrehozva = new DateTime(2024, 1, 1) },
            new EszkozTipus { Id = 4, Nev = "Tartály", Aktiv = true, Letrehozva = new DateTime(2024, 1, 1) }
        );

        // Seed adatok a KarbantartasTipus-hoz:
        modelBuilder.Entity<KarbantartasTipus>().HasData(
            new KarbantartasTipus { Id = 1, Nev = "Eseti karbantartás", IsmetlodesHonap = 0, Aktiv = true, Letrehozva = new DateTime(2024, 1, 1) },
            new KarbantartasTipus { Id = 2, Nev = "Negyedéves karbantartás", IsmetlodesHonap = 3, Aktiv = true, Letrehozva = new DateTime(2024, 1, 1) },
            new KarbantartasTipus { Id = 3, Nev = "Féléves karbantartás", IsmetlodesHonap = 6, Aktiv = true, Letrehozva = new DateTime(2024, 1, 1) },
            new KarbantartasTipus { Id = 4, Nev = "Éves karbantartás", IsmetlodesHonap = 12, Aktiv = true, Letrehozva = new DateTime(2024, 1, 1) }
        );
    }
}
