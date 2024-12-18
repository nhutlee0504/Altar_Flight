﻿// <auto-generated />
using System;
using API_Flight_Altar.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API_Flight_Altar_ThucTap.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20241017132626_Dbv5")]
    partial class Dbv5
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.33")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.DocFlight", b =>
                {
                    b.Property<int>("IdDocument")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdDocument"), 1L, 1);

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("DocumentName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("FilePath")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("FlightId")
                        .HasColumnType("int");

                    b.Property<string>("Signature")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("UpdatedTime")
                        .HasColumnType("datetime2");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.Property<double?>("Version")
                        .HasColumnType("float");

                    b.HasKey("IdDocument");

                    b.HasIndex("FlightId");

                    b.HasIndex("UserId");

                    b.ToTable("documents");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.Flight", b =>
                {
                    b.Property<int>("IdFlight")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdFlight"), 1L, 1);

                    b.Property<DateTime>("DateTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("FlightNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PointOfLoading")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PointOfUnloading")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Signature")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TimeEnd")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TimeStart")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("IdFlight");

                    b.HasIndex("UserId");

                    b.ToTable("flights");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.Group_Type", b =>
                {
                    b.Property<int>("IdGT")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdGT"), 1L, 1);

                    b.Property<int>("IdGroup")
                        .HasColumnType("int");

                    b.Property<int>("IdPermission")
                        .HasColumnType("int");

                    b.Property<int>("IdType")
                        .HasColumnType("int");

                    b.HasKey("IdGT");

                    b.HasIndex("IdGroup");

                    b.HasIndex("IdPermission");

                    b.HasIndex("IdType");

                    b.ToTable("group_Types");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.Group_User", b =>
                {
                    b.Property<int>("IdGU")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdGU"), 1L, 1);

                    b.Property<int>("GroupID")
                        .HasColumnType("int");

                    b.Property<int>("UserID")
                        .HasColumnType("int");

                    b.HasKey("IdGU");

                    b.HasIndex("GroupID");

                    b.HasIndex("UserID");

                    b.ToTable("group_Users");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.GroupModel", b =>
                {
                    b.Property<int>("IdGroup")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdGroup"), 1L, 1);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("GroupName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("IdGroup");

                    b.HasIndex("UserId");

                    b.ToTable("groups");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.Permission", b =>
                {
                    b.Property<int>("idPermission")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("idPermission"), 1L, 1);

                    b.Property<string>("PermissionName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.HasKey("idPermission");

                    b.ToTable("permissions");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.TypeDoc", b =>
                {
                    b.Property<int>("IdTypeDoc")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdTypeDoc"), 1L, 1);

                    b.Property<DateTime>("CreatedDate")
                        .HasColumnType("datetime2");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TypeName")
                        .IsRequired()
                        .HasColumnType("varchar(50)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("IdTypeDoc");

                    b.HasIndex("UserId");

                    b.ToTable("typeDocs");
                });

            modelBuilder.Entity("API_Flight_Altar.Model.User", b =>
                {
                    b.Property<int>("IdUser")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("IdUser"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Phone")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("IdUser");

                    b.ToTable("users");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.DocFlight", b =>
                {
                    b.HasOne("API_Flight_Altar_ThucTap.Model.Flight", "Flight")
                        .WithMany("documents")
                        .HasForeignKey("FlightId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("API_Flight_Altar.Model.User", "User")
                        .WithMany("documents")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Flight");

                    b.Navigation("User");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.Flight", b =>
                {
                    b.HasOne("API_Flight_Altar.Model.User", "User")
                        .WithMany("flights")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.Group_Type", b =>
                {
                    b.HasOne("API_Flight_Altar_ThucTap.Model.GroupModel", "GroupModel")
                        .WithMany("group_Types")
                        .HasForeignKey("IdGroup")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API_Flight_Altar_ThucTap.Model.Permission", "Permission")
                        .WithMany("group_Types")
                        .HasForeignKey("IdPermission")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("API_Flight_Altar_ThucTap.Model.TypeDoc", "TypeDoc")
                        .WithMany("group_Types")
                        .HasForeignKey("IdType")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("GroupModel");

                    b.Navigation("Permission");

                    b.Navigation("TypeDoc");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.Group_User", b =>
                {
                    b.HasOne("API_Flight_Altar_ThucTap.Model.GroupModel", "Group")
                        .WithMany("group_Users")
                        .HasForeignKey("GroupID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API_Flight_Altar.Model.User", "User")
                        .WithMany("group_Users")
                        .HasForeignKey("UserID")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.GroupModel", b =>
                {
                    b.HasOne("API_Flight_Altar.Model.User", "User")
                        .WithMany("groups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.TypeDoc", b =>
                {
                    b.HasOne("API_Flight_Altar.Model.User", "User")
                        .WithMany("typeDocs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.Flight", b =>
                {
                    b.Navigation("documents");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.GroupModel", b =>
                {
                    b.Navigation("group_Types");

                    b.Navigation("group_Users");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.Permission", b =>
                {
                    b.Navigation("group_Types");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.TypeDoc", b =>
                {
                    b.Navigation("group_Types");
                });

            modelBuilder.Entity("API_Flight_Altar.Model.User", b =>
                {
                    b.Navigation("documents");

                    b.Navigation("flights");

                    b.Navigation("group_Users");

                    b.Navigation("groups");

                    b.Navigation("typeDocs");
                });
#pragma warning restore 612, 618
        }
    }
}
