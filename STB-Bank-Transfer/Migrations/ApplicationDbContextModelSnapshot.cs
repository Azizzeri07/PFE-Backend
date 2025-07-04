﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using STB_Bank_Transfer.Data;

#nullable disable

namespace STB_Bank_Transfer.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.6")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("STB_Bank_Transfer.Models.Banquier", b =>
                {
                    b.Property<int>("IdBanquier")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdBanquier"));

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MotDePasse")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("IdBanquier");

                    b.ToTable("Banquiers");
                });

            modelBuilder.Entity("STB_Bank_Transfer.Models.Client", b =>
                {
                    b.Property<int>("IdClient")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdClient"));

                    b.Property<int>("BanquierId")
                        .HasColumnType("integer");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("IdCompte")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("MotDePasse")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Nom")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("IdClient");

                    b.HasIndex("BanquierId");

                    b.HasIndex("IdCompte")
                        .IsUnique();

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("STB_Bank_Transfer.Models.Compte", b =>
                {
                    b.Property<string>("IdCompte")
                        .HasColumnType("text");

                    b.Property<decimal>("Solde")
                        .HasColumnType("numeric");

                    b.Property<string>("TypeCompte")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("IdCompte");

                    b.ToTable("Comptes");
                });

            modelBuilder.Entity("STB_Bank_Transfer.Models.Operation", b =>
                {
                    b.Property<int>("IdOperation")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdOperation"));

                    b.Property<string>("CompteIdCompte")
                        .HasColumnType("text");

                    b.Property<DateTime>("DateOperation")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Libelle")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Montant")
                        .HasColumnType("numeric");

                    b.Property<string>("NumCompte")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("TypeOperation")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("IdOperation");

                    b.HasIndex("CompteIdCompte");

                    b.ToTable("Operations");
                });

            modelBuilder.Entity("STB_Bank_Transfer.Models.Virement", b =>
                {
                    b.Property<int>("IdVirement")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("IdVirement"));

                    b.Property<int?>("BanquierIdBanquier")
                        .HasColumnType("integer");

                    b.Property<int?>("ClientIdClient")
                        .HasColumnType("integer");

                    b.Property<DateTime>("DateCreation")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("DateValidation")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("Montant")
                        .HasColumnType("numeric");

                    b.Property<string>("Motif")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NumCompteDestination")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("NumCompteSource")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("RaisonRejet")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("Statut")
                        .HasColumnType("integer");

                    b.HasKey("IdVirement");

                    b.HasIndex("BanquierIdBanquier");

                    b.HasIndex("ClientIdClient");

                    b.ToTable("Virements");
                });

            modelBuilder.Entity("STB_Bank_Transfer.Models.Client", b =>
                {
                    b.HasOne("STB_Bank_Transfer.Models.Banquier", "Banquier")
                        .WithMany("Clients")
                        .HasForeignKey("BanquierId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("STB_Bank_Transfer.Models.Compte", "Compte")
                        .WithOne()
                        .HasForeignKey("STB_Bank_Transfer.Models.Client", "IdCompte")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Banquier");

                    b.Navigation("Compte");
                });

            modelBuilder.Entity("STB_Bank_Transfer.Models.Operation", b =>
                {
                    b.HasOne("STB_Bank_Transfer.Models.Compte", null)
                        .WithMany("HistoriqueOperations")
                        .HasForeignKey("CompteIdCompte");
                });

            modelBuilder.Entity("STB_Bank_Transfer.Models.Virement", b =>
                {
                    b.HasOne("STB_Bank_Transfer.Models.Banquier", null)
                        .WithMany("VirementsEnAttente")
                        .HasForeignKey("BanquierIdBanquier");

                    b.HasOne("STB_Bank_Transfer.Models.Client", null)
                        .WithMany("Virements")
                        .HasForeignKey("ClientIdClient");
                });

            modelBuilder.Entity("STB_Bank_Transfer.Models.Banquier", b =>
                {
                    b.Navigation("Clients");

                    b.Navigation("VirementsEnAttente");
                });

            modelBuilder.Entity("STB_Bank_Transfer.Models.Client", b =>
                {
                    b.Navigation("Virements");
                });

            modelBuilder.Entity("STB_Bank_Transfer.Models.Compte", b =>
                {
                    b.Navigation("HistoriqueOperations");
                });
#pragma warning restore 612, 618
        }
    }
}
