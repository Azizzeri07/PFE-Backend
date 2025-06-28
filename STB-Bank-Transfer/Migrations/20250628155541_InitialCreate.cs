using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace STB_Bank_Transfer.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
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
                    Nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    MotDePasse = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Banquiers", x => x.IdBanquier);
                });

            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    IdClient = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Nom = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MotDePasse = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "text", nullable: false),
                    BanquierId = table.Column<int>(type: "integer", nullable: false),
                    IdCompte = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.IdClient);
                    table.ForeignKey(
                        name: "FK_Clients_Banquiers_BanquierId",
                        column: x => x.BanquierId,
                        principalTable: "Banquiers",
                        principalColumn: "IdBanquier",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Comptes",
                columns: table => new
                {
                    IdCompte = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Solde = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TypeCompte = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    ClientId = table.Column<int>(type: "integer", nullable: false),
                    BanquierId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comptes", x => x.IdCompte);
                    table.ForeignKey(
                        name: "FK_Comptes_Banquiers_BanquierId",
                        column: x => x.BanquierId,
                        principalTable: "Banquiers",
                        principalColumn: "IdBanquier",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comptes_Clients_ClientId",
                        column: x => x.ClientId,
                        principalTable: "Clients",
                        principalColumn: "IdClient",
                        onDelete: ReferentialAction.Cascade);
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
                    CompteId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Operations", x => x.IdOperation);
                    table.ForeignKey(
                        name: "FK_Operations_Comptes_CompteId",
                        column: x => x.CompteId,
                        principalTable: "Comptes",
                        principalColumn: "IdCompte",
                        onDelete: ReferentialAction.Cascade);
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
                    IdCompte = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Virements", x => x.IdVirement);
                    table.ForeignKey(
                        name: "FK_Virements_Comptes_IdCompte",
                        column: x => x.IdCompte,
                        principalTable: "Comptes",
                        principalColumn: "IdCompte",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Banquiers_Email",
                table: "Banquiers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Clients_BanquierId",
                table: "Clients",
                column: "BanquierId");

            migrationBuilder.CreateIndex(
                name: "IX_Clients_IdCompte",
                table: "Clients",
                column: "IdCompte",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Comptes_BanquierId",
                table: "Comptes",
                column: "BanquierId");

            migrationBuilder.CreateIndex(
                name: "IX_Comptes_ClientId",
                table: "Comptes",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_Operations_CompteId",
                table: "Operations",
                column: "CompteId");

            migrationBuilder.CreateIndex(
                name: "IX_Virements_IdCompte",
                table: "Virements",
                column: "IdCompte");

            migrationBuilder.AddForeignKey(
                name: "FK_Clients_Comptes_IdCompte",
                table: "Clients",
                column: "IdCompte",
                principalTable: "Comptes",
                principalColumn: "IdCompte",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Banquiers_BanquierId",
                table: "Clients");

            migrationBuilder.DropForeignKey(
                name: "FK_Comptes_Banquiers_BanquierId",
                table: "Comptes");

            migrationBuilder.DropForeignKey(
                name: "FK_Clients_Comptes_IdCompte",
                table: "Clients");

            migrationBuilder.DropTable(
                name: "Operations");

            migrationBuilder.DropTable(
                name: "Virements");

            migrationBuilder.DropTable(
                name: "Banquiers");

            migrationBuilder.DropTable(
                name: "Comptes");

            migrationBuilder.DropTable(
                name: "Clients");
        }
    }
}
