using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Migrations.Builders;
using Microsoft.Data.Entity.Migrations.Operations;

namespace wikia_UnofficialMigrations
{
    public partial class MyFirstMigration : Migration
    {
        public override void Up(MigrationBuilder migration)
        {
            migration.CreateTable(
                name: "Favorite",
                columns: table => new
                {
                    FavoriteId = table.Column(type: "INTEGER", nullable: false),
                    FavoriteName = table.Column(type: "TEXT", nullable: false),
                    Url = table.Column(type: "TEXT", nullable: false),
                    WikiId = table.Column(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Favorite", x => x.FavoriteId);
                });
            migration.CreateTable(
                name: "Wiki",
                columns: table => new
                {
                    Id = table.Column(type: "INTEGER", nullable: false),
                    Url = table.Column(type: "TEXT", nullable: false),
                    WikiId = table.Column(type: "INTEGER", nullable: false),
                    WikiName = table.Column(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Wiki", x => x.Id);
                });
        }

        public override void Down(MigrationBuilder migration)
        {
            migration.DropTable("Favorite");
            migration.DropTable("Wiki");
        }
    }
}
