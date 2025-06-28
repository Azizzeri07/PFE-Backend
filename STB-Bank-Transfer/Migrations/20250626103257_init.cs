using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace STB_Bank_Transfer.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Banquiers",
                columns: table => new
                {
                    IdBanquier = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    MotDePasse = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banquiers", x => x.IdBanquier);
                });

            migrationBuilder.CreateTable(
                name: "Comptes",
                columns: table => new
                {
                    IdCompte = table.Column<string>(type: "text", nullable: false),
                    Solde = table.Column<decimal>(type: "numeric", nullable: false),
                    TypeCompte = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comptes", x => x.IdCompte);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    IdClient = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    MotDePasse = table.Column<string>(type: "text", nullable: false),
                    BanquierIdBanquier = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.IdClient);
                    table.ForeignKey(
                        name: "FK_Clients_Banquiers_BanquierIdBanquier",
                        column: x => x.BanquierIdBanquier,
                        principalTable: "Banquiers",
                        principalColumn: "IdBanquier");
                });

            migrationBuilder.CreateTable(
                name: "Operations",
                columns: table => new
                {
                    IdOperation = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumCompte = table.Column<string>(type: "text", nullable: false),
                    Montant = table.Column<decimal>(type: "numeric", nullable: false),
                    DateOperation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Libelle = table.Column<string>(type: "text", nullable: false),
                    TypeOperation = table.Column<string>(type: "text", nullable: false),
                    CompteIdCompte = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.IdOperation);
                    table.ForeignKey(
                        name: "FK_Operations_Comptes_CompteIdCompte",
                        column: x => x.CompteIdCompte,
                        principalTable: "Comptes",
                        principalColumn: "IdCompte");
                });

            migrationBuilder.CreateTable(
                name: "Virements",
                columns: table => new
                {
                    IdVirement = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NumCompteSource = table.Column<string>(type: "text", nullable: false),
                    NumCompteDestination = table.Column<string>(type: "text", nullable: false),
                    Montant = table.Column<decimal>(type: "numeric", nullable: false),
                    Motif = table.Column<string>(type: "text", nullable: false),
                    DateCreation = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DateValidation = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Statut = table.Column<int>(type: "integer", nullable: false),
                    RaisonRejet = table.Column<string>(type: "text", nullable: false),
                    BanquierIdBanquier = table.Column<int>(type: "integer", nullable: true),
                    ClientIdClient = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Virements", x => x.IdVirement);
                    table.ForeignKey(
                        name: "FK_Virements_Banquiers_BanquierIdBanquier",
                        column: x => x.BanquierIdBanquier,
                        principalTable: "Banquiers",
                        principalColumn: "IdBanquier");
                    table.ForeignKey(
                        name: "FK_Virements_Clients_ClientIdClient",
                        column: x => x.ClientIdClient,
                        principalTable: "Clients",
                        principalColumn: "IdClient");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Clients_BanquierIdBanquier",
                table: "Clients",
                column: "BanquierIdBanquier");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_CompteIdCompte",
                table: "Operations",
                column: "CompteIdCompte");

            migrationBuilder.CreateIndex(
                name: "IX_Virements_BanquierIdBanquier",
                table: "Virements",
                column: "BanquierIdBanquier");

            migrationBuilder.CreateIndex(
                name: "IX_Virements_ClientIdClient",
                table: "Virements",
                column: "ClientIdClient");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "Virements");

            migrationBuilder.DropTable(
                name: "Comptes");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Banquiers");
        }
    }
}
