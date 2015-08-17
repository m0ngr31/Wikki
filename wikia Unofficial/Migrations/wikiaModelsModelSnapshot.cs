using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Infrastructure;
using wikia_Unofficial.Models;

namespace wikia_UnofficialMigrations
{
    [ContextType(typeof(wikiaModels))]
    partial class wikiaModelsModelSnapshot : ModelSnapshot
    {
        public override void BuildModel(ModelBuilder builder)
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
