﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ShireBank.Server.Database;

#nullable disable

namespace ShireBank.Server.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20220405155924_Init")]
    partial class Init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.3");

            modelBuilder.Entity("ShireBank.Server.Models.Account", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<float>("Balance")
                        .HasColumnType("REAL");

                    b.Property<float>("DebtLimit")
                        .HasColumnType("REAL");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsClosed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<int>("Version")
                        .IsConcurrencyToken()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0);

                    b.HasKey("Id");

                    b.HasIndex("FirstName", "LastName")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("ShireBank.Server.Models.Transaction", b =>
                {
                    b.Property<uint>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<uint>("AccountId")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Balance")
                        .HasColumnType("REAL");

                    b.Property<DateTime>("Timestamp")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT")
                        .HasDefaultValueSql("STRFTIME('%Y-%m-%d %H:%M:%f', 'NOW')");

                    b.Property<int>("Type")
                        .HasColumnType("INTEGER");

                    b.Property<float>("Value")
                        .HasColumnType("REAL");

                    b.HasKey("Id");

                    b.HasIndex("AccountId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("ShireBank.Server.Models.Transaction", b =>
                {
                    b.HasOne("ShireBank.Server.Models.Account", null)
                        .WithMany("Transactions")
                        .HasForeignKey("AccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("ShireBank.Server.Models.Account", b =>
                {
                    b.Navigation("Transactions");
                });
#pragma warning restore 612, 618
        }
    }
}