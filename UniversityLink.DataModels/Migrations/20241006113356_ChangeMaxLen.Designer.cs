﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using UniversityLink.DataModels;

#nullable disable

namespace UniversityLink.DataModels.Migrations
{
    [DbContext(typeof(LinkContext))]
    [Migration("20241006113356_ChangeMaxLen")]
    partial class ChangeMaxLen
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.6");

            modelBuilder.Entity("UniversityLink.DataModels.CategoryModel", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("varchar(32)");

                    b.Property<string>("Description")
                        .HasColumnType("varchar(32)");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasColumnType("varchar(16)");

                    b.Property<int>("Index")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(32)");

                    b.HasKey("Key");

                    b.HasIndex("Index");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("UniversityLink.DataModels.LinkModel", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("varchar(32)");

                    b.Property<string>("CategoryModelKey")
                        .HasColumnType("varchar(32)");

                    b.Property<string>("Description")
                        .HasColumnType("varchar(32)");

                    b.Property<string>("Icon")
                        .IsRequired()
                        .HasColumnType("varchar(32)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("varchar(32)");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("varchar(64)");

                    b.HasKey("Key");

                    b.HasIndex("CategoryModelKey");

                    b.ToTable("Links");
                });

            modelBuilder.Entity("UniversityLink.DataModels.UserModel", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(32)");

                    b.Property<string>("ClassName")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("varchar(2)");

                    b.Property<string>("Identity")
                        .IsRequired()
                        .HasColumnType("varchar(20)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("varchar(32)");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("UniversityLink.DataModels.LinkModel", b =>
                {
                    b.HasOne("UniversityLink.DataModels.CategoryModel", null)
                        .WithMany("Links")
                        .HasForeignKey("CategoryModelKey");
                });

            modelBuilder.Entity("UniversityLink.DataModels.CategoryModel", b =>
                {
                    b.Navigation("Links");
                });
#pragma warning restore 612, 618
        }
    }
}
