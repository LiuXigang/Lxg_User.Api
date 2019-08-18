﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using User.Api.Data;

namespace User.Api.Migrations
{
    [DbContext(typeof(UserContext))]
    [Migration("20190818041722_InitUserContext")]
    partial class InitUserContext
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.2-servicing-10034")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("User.Api.Model.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Address");

                    b.Property<string>("Avatar");

                    b.Property<string>("City");

                    b.Property<int>("CityId");

                    b.Property<string>("Company");

                    b.Property<string>("Email");

                    b.Property<byte>("Gender");

                    b.Property<string>("Name");

                    b.Property<string>("Phone");

                    b.Property<string>("Province");

                    b.Property<string>("ProvinceId");

                    b.Property<string>("Tel");

                    b.Property<string>("Title");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("User.Api.Model.BPfile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AppUserId");

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime");

                    b.Property<string>("FileName");

                    b.Property<string>("FromatFilePath");

                    b.Property<string>("OriginFilePath");

                    b.HasKey("Id");

                    b.ToTable("UserBPFiles");
                });

            modelBuilder.Entity("User.Api.Model.UserProperty", b =>
                {
                    b.Property<string>("Key")
                        .HasMaxLength(100);

                    b.Property<int>("AppUserId");

                    b.Property<string>("Value")
                        .HasMaxLength(100);

                    b.Property<int?>("AppUserId1");

                    b.Property<string>("Text");

                    b.HasKey("Key", "AppUserId", "Value");

                    b.HasAlternateKey("AppUserId", "Key", "Value");

                    b.HasIndex("AppUserId1");

                    b.ToTable("UserProperties");
                });

            modelBuilder.Entity("User.Api.Model.UserTag", b =>
                {
                    b.Property<int>("AppUserId");

                    b.Property<int>("Tag")
                        .HasMaxLength(100);

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("datetime");

                    b.HasKey("AppUserId", "Tag");

                    b.ToTable("UserTags");
                });

            modelBuilder.Entity("User.Api.Model.UserProperty", b =>
                {
                    b.HasOne("User.Api.Model.AppUser")
                        .WithMany("Properties")
                        .HasForeignKey("AppUserId1");
                });
#pragma warning restore 612, 618
        }
    }
}
