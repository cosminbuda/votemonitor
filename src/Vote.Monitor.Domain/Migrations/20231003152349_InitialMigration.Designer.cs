﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using Vote.Monitor.Domain.DataContext;

#nullable disable

namespace Vote.Monitor.Domain.Migrations
{
    [DbContext(typeof(PollingStationDbContext))]
    [Migration("20231003152349_InitialMigration")]
    partial class InitialMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.11")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("PollingStationTag", b =>
                {
                    b.Property<int>("PollingStationId")
                        .HasColumnType("integer");

                    b.Property<int>("TagId")
                        .HasColumnType("integer");

                    b.HasKey("PollingStationId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("PollingStationTag");
                });

            modelBuilder.Entity("Vote.Monitor.Core.Models.PollingStationEf", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Address")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.ToTable("PollingStations");
                });

            modelBuilder.Entity("Vote.Monitor.Core.Models.TagEf", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Key")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("PollingStationTag", b =>
                {
                    b.HasOne("Vote.Monitor.Core.Models.PollingStationEf", null)
                        .WithMany()
                        .HasForeignKey("PollingStationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Vote.Monitor.Core.Models.TagEf", null)
                        .WithMany()
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}