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
    [Migration("20241007100036_Dbv1")]
    partial class Dbv1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.33")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

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
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Note")
                        .IsRequired()
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
                        .IsRequired()
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
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("User");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.GroupModel", b =>
                {
                    b.HasOne("API_Flight_Altar.Model.User", "User")
                        .WithMany("groups")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.TypeDoc", b =>
                {
                    b.HasOne("API_Flight_Altar.Model.User", "User")
                        .WithMany("typeDocs")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("API_Flight_Altar_ThucTap.Model.GroupModel", b =>
                {
                    b.Navigation("group_Users");
                });

            modelBuilder.Entity("API_Flight_Altar.Model.User", b =>
                {
                    b.Navigation("group_Users");

                    b.Navigation("groups");

                    b.Navigation("typeDocs");
                });
#pragma warning restore 612, 618
        }
    }
}
