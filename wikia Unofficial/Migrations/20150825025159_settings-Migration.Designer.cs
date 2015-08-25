using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Infrastructure;
using wikia_Unofficial.Models;

namespace wikia_UnofficialMigrations
{
    [ContextType(typeof(wikiaModels))]
    partial class settingsMigration
    {
        public override string Id
        {
            get { return "20150825025159_settings-Migration"; }
        }

        public override string ProductVersion
        {
            get { return "7.0.0-beta6-13815"; }
        }

        public override void BuildTargetModel(ModelBuilder builder)
        {
            builder
                .Annotation("ProductVersion", "7.0.0-beta6-13815");

            builder.Entity("wikia_Unofficial.Models.Favorite", b =>
                {
                    b.Property<int>("FavoriteId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("FavoriteName")
                        .Required();

                    b.Property<string>("Url")
                        .Required();

                    b.Property<int>("WikiId");

                    b.Key("FavoriteId");
                });

            builder.Entity("wikia_Unofficial.Models.Setting", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("hasRun");

                    b.Key("Id");
                });

            builder.Entity("wikia_Unofficial.Models.Wiki", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Url")
                        .Required();

                    b.Property<int>("WikiId");

                    b.Property<string>("WikiName")
                        .Required();

                    b.Key("Id");
                });
        }
    }
}
