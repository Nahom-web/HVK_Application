using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

// Code scaffolded by EF Core assumes nullable reference types (NRTs) are not used or disabled.
// If you have enabled NRTs for your project, then un-comment the following line:
// #nullable disable

namespace HVK_Queen.Models {
    public partial class HVK_QueenContext : DbContext {
        public HVK_QueenContext() {
        }

        public HVK_QueenContext(DbContextOptions<HVK_QueenContext> options)
            : base(options) {
        }

        public virtual DbSet<DailyRate> DailyRate { get; set; }

        public virtual DbSet<Discount> Discount { get; set; }

        public virtual DbSet<KennelLog> KennelLog { get; set; }

        public virtual DbSet<Medication> Medication { get; set; }

        public virtual DbSet<Owner> Owner { get; set; }

        public virtual DbSet<Pet> Pet { get; set; }

        public virtual DbSet<PetReservation> PetReservation { get; set; }

        public virtual DbSet<PetReservationDiscount> PetReservationDiscount { get; set; }

        public virtual DbSet<PetReservationService> PetReservationService { get; set; }

        public virtual DbSet<PetVaccination> PetVaccination { get; set; }

        public virtual DbSet<Reservation> Reservation { get; set; }

        public virtual DbSet<ReservationDiscount> ReservationDiscount { get; set; }

        public virtual DbSet<Run> Run { get; set; }

        public virtual DbSet<Service> Service { get; set; }

        public virtual DbSet<SpecialNotes> SpecialNotes { get; set; }

        public virtual DbSet<Vaccination> Vaccination { get; set; }

        public virtual DbSet<Veterinarian> Veterinarian { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) {
            if (!optionsBuilder.IsConfigured) {
                //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Server=cssql.cegep-heritage.qc.ca; Database=HVK_Queen;User id=HVKTEAM3; Password=HVKTEAM3;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<DailyRate>(entity => {
                entity.HasIndex(e => e.DailyRateId)
                    .HasName("IX_DAILY_RATE_PK")
                    .IsUnique();

                entity.Property(e => e.DogSize)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Rate).HasColumnType("numeric(5, 2)");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.DailyRate)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("DR_SERV_FK");
            });

            modelBuilder.Entity<Discount>(entity => {
                entity.HasIndex(e => e.DiscountId)
                    .HasName("IX_DISCOUNT_PK")
                    .IsUnique();

                entity.Property(e => e.Desciption)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Percentage).HasColumnType("numeric(3, 2)");

                entity.Property(e => e.Type)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasDefaultValueSql("('D')");
            });

            modelBuilder.Entity<KennelLog>(entity => {
                entity.HasKey(e => new { e.LogDate, e.SequenceNumber, e.PetReservationId })
                    .HasName("KENNEL_LOG_PK");

                entity.HasIndex(e => new { e.LogDate, e.SequenceNumber, e.PetReservationId })
                    .HasName("IX_KENNEL_LOG_PK")
                    .IsUnique();

                entity.Property(e => e.LogDate).HasColumnType("date");

                entity.Property(e => e.SequenceNumber).HasDefaultValueSql("((1))");

                entity.Property(e => e.Notes)
                    .IsRequired()
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.PetReservation)
                    .WithMany(p => p.KennelLog)
                    .HasForeignKey(d => d.PetReservationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("KL_PR_FK");
            });

            modelBuilder.Entity<Medication>(entity => {
                entity.Property(e => e.Dosage)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.SpecialInstruct)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.HasOne(d => d.PetReservation)
                    .WithMany(p => p.Medications)
                    .HasForeignKey(d => d.PetReservationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("MED_PR_FK");
            });

            modelBuilder.Entity<Owner>(entity => {
                entity.HasIndex(e => e.OwnerId)
                    .HasName("IX_OWNER_PK")
                    .IsUnique();

                entity.Property(e => e.CellPhone)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.City)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.EmergencyContactFirstName)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.EmergencyContactLastName)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.EmergencyContactPhone)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Password)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Province)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasDefaultValueSql("('QC')");

                entity.Property(e => e.Street)
                    .HasMaxLength(40)
                    .IsUnicode(false);

                entity.HasOne(d => d.Veterinarian)
                    .WithMany(p => p.Owner)
                    .HasForeignKey(d => d.VeterinarianId)
                    .HasConstraintName("OWNER_VET_FK");
            });

            modelBuilder.Entity<Pet>(entity => {
                entity.HasIndex(e => e.PetId)
                    .HasName("IX_PET_PK")
                    .IsUnique();

                entity.Property(e => e.Birthdate).HasColumnType("string");

                entity.Property(e => e.Breed)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.DogSize)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Gender)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.HasOne(d => d.Owner)
                    .WithMany(p => p.Pet)
                    .HasForeignKey(d => d.OwnerId)
                    .HasConstraintName("PET_OWN_FK");
            });

            modelBuilder.Entity<PetReservation>(entity => {
                entity.HasIndex(e => e.PetReservationId)
                    .HasName("IX_PET_RESERVATION_PK")
                    .IsUnique();

                entity.HasOne(d => d.Pet)
                    .WithMany(p => p.PetReservation)
                    .HasForeignKey(d => d.PetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PR_PET_FK");

                entity.HasOne(d => d.Reservation)
                    .WithMany(p => p.PetReservation)
                    .HasForeignKey(d => d.ReservationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PR_RES_FK");

                entity.HasOne(d => d.Run)
                    .WithMany(p => p.PetReservation)
                    .HasForeignKey(d => d.RunId)
                    .HasConstraintName("PR_RUN_FK");
            });

            modelBuilder.Entity<PetReservationDiscount>(entity => {
                entity.HasKey(e => new { e.DiscountId, e.PetReservationId })
                    .HasName("PET_RES_DISC_PK");

                entity.HasIndex(e => new { e.DiscountId, e.PetReservationId })
                    .HasName("IX_PET_RES_DISC_PK")
                    .IsUnique();

                entity.HasOne(d => d.Discount)
                    .WithMany(p => p.PetReservationDiscount)
                    .HasForeignKey(d => d.DiscountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PRD_DISC_FK");

                entity.HasOne(d => d.PetReservation)
                    .WithMany(p => p.PetReservationDiscount)
                    .HasForeignKey(d => d.PetReservationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PRD_PR_FK");
            });

            modelBuilder.Entity<PetReservationService>(entity => {
                entity.HasKey(e => new { e.PetReservationId, e.ServiceId })
                    .HasName("PET_RESERVATION_SERVICE_PK");

                entity.HasIndex(e => new { e.PetReservationId, e.ServiceId })
                    .HasName("IX_PET_RESERVATION_SERVICE_PK")
                    .IsUnique();

                entity.HasOne(d => d.PetReservation)
                    .WithMany(p => p.PetReservationService)
                    .HasForeignKey(d => d.PetReservationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PRS_PR_FK");

                entity.HasOne(d => d.Service)
                    .WithMany(p => p.PetReservationService)
                    .HasForeignKey(d => d.ServiceId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PRS_SERV_FK");
            });

            modelBuilder.Entity<PetVaccination>(entity => {
                entity.HasKey(e => new { e.VaccinationId, e.PetId })
                    .HasName("PET_VACCINATION_PK");

                entity.HasIndex(e => new { e.VaccinationId, e.PetId })
                    .HasName("IX_PET_VACCINATION_PK")
                    .IsUnique();

                entity.Property(e => e.ExpiryDate).HasColumnType("date");

                entity.HasOne(d => d.Pet)
                    .WithMany(p => p.PetVaccination)
                    .HasForeignKey(d => d.PetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PV_PET_FK");

                entity.HasOne(d => d.Vaccination)
                    .WithMany(p => p.PetVaccination)
                    .HasForeignKey(d => d.VaccinationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("PV_VACC_FK");
            });

            modelBuilder.Entity<Reservation>(entity => {
                entity.HasIndex(e => e.ReservationId)
                    .HasName("IX_RESERVATION_PK")
                    .IsUnique();

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.StartDate).HasColumnType("date");

                entity.Property(e => e.Status)
                    .HasColumnType("numeric(1, 0)")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<ReservationDiscount>(entity => {
                entity.HasKey(e => new { e.DiscountId, e.ReservationId })
                    .HasName("RES_DISC_PK");

                entity.HasIndex(e => new { e.DiscountId, e.ReservationId })
                    .HasName("IX_RES_DISC_PK")
                    .IsUnique();

                entity.HasOne(d => d.Discount)
                    .WithMany(p => p.ReservationDiscount)
                    .HasForeignKey(d => d.DiscountId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RD_DISC_FK");

                entity.HasOne(d => d.Reservation)
                    .WithMany(p => p.ReservationDiscount)
                    .HasForeignKey(d => d.ReservationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("RD_RES_FK");
            });

            modelBuilder.Entity<Run>(entity => {
                entity.HasIndex(e => e.RunId)
                    .HasName("IX_RUN_PK")
                    .IsUnique();

                entity.Property(e => e.Location)
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Size)
                    .IsRequired()
                    .HasMaxLength(1)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasDefaultValueSql("('R')");

                entity.Property(e => e.Status)
                    .HasColumnType("numeric(1, 0)")
                    .HasDefaultValueSql("((1))");
            });

            modelBuilder.Entity<Service>(entity => {
                entity.HasIndex(e => e.ServiceId)
                    .HasName("IX_SERVICE_PK")
                    .IsUnique();

                entity.Property(e => e.ServiceDescription)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<SpecialNotes>(entity => {
                entity.HasKey(e => e.PetId)
                    .HasName("SPECIAL_NOTES_PK");

                entity.HasIndex(e => e.PetId)
                    .HasName("IX_SPECIAL_NOTES_PK")
                    .IsUnique();

                entity.Property(e => e.PetId).ValueGeneratedNever();

                entity.Property(e => e.Notes)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.HasOne(d => d.Pet)
                    .WithOne(p => p.SpecialNotes)
                    .HasForeignKey<SpecialNotes>(d => d.PetId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("SPECIAL_NOTES_PET_FK");
            });

            modelBuilder.Entity<Vaccination>(entity => {
                entity.HasIndex(e => e.VaccinationId)
                    .HasName("IX_VACCINATION_PK")
                    .IsUnique();

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<Veterinarian>(entity => {
                entity.Property(e => e.City)
                    .HasMaxLength(25)
                    .IsUnicode(false);

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.PostalCode)
                    .HasMaxLength(6)
                    .IsUnicode(false)
                    .IsFixedLength();

                entity.Property(e => e.Province)
                    .HasMaxLength(2)
                    .IsUnicode(false)
                    .IsFixedLength()
                    .HasDefaultValueSql("('QC')");

                entity.Property(e => e.Street)
                    .HasMaxLength(40)
                    .IsUnicode(false);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
