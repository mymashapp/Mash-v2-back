﻿// <auto-generated />
using System;
using Aimo.Data.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Aimo.Data.Migrations
{
    [DbContext(typeof(EfDataContext))]
    [Migration("20220425070449_user_updated_props_nullable")]
    partial class user_updated_props_nullable
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Aimo.Domain.Cards.Card", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Address1")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Address2")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Address3")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CardType")
                        .HasColumnType("int");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<string>("City")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Country")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedAtUtc")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateUtc")
                        .HasColumnType("datetime2");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNo")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PictureUrl")
                        .HasColumnType("nvarchar(max)");

                    b.Property<float?>("Rating")
                        .HasColumnType("real");

                    b.Property<int?>("ReviewCount")
                        .HasColumnType("int");

                    b.Property<string>("State")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UpdatedBy")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ZipCode")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("Card", (string)null);
                });

            modelBuilder.Entity("Aimo.Domain.Cards.CardPicture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CardId")
                        .HasColumnType("int");

                    b.Property<DateTime?>("CreatedAtUtc")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<string>("PictureUrl")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UpdatedBy")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.ToTable("CardPicture", (string)null);
                });

            modelBuilder.Entity("Aimo.Domain.Cards.SwipeGroup", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AgeFrom")
                        .HasColumnType("int");

                    b.Property<int>("AgeTo")
                        .HasColumnType("int");

                    b.Property<int>("Gender")
                        .HasColumnType("int");

                    b.Property<int>("GroupType")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("SwipeGroup", (string)null);
                });

            modelBuilder.Entity("Aimo.Domain.Cards.SwipeGroupInterest", b =>
                {
                    b.Property<int>("InterestId")
                        .HasColumnType("int");

                    b.Property<int>("SwipeGroupId")
                        .HasColumnType("int");

                    b.HasKey("InterestId", "SwipeGroupId");

                    b.HasIndex("SwipeGroupId");

                    b.ToTable("SwipeGroupInterest", (string)null);
                });

            modelBuilder.Entity("Aimo.Domain.Cards.SwipeHistory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("CardId")
                        .HasColumnType("int");

                    b.Property<DateTime>("SeenAtUtc")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<int>("SwipeType")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("CardId");

                    b.HasIndex("UserId");

                    b.ToTable("SwipeHistory", (string)null);
                });

            modelBuilder.Entity("Aimo.Domain.Categories.CardSubCategory", b =>
                {
                    b.Property<int>("CardId")
                        .HasColumnType("int");

                    b.Property<int>("SubCategoryId")
                        .HasColumnType("int");

                    b.HasKey("CardId", "SubCategoryId");

                    b.HasIndex("SubCategoryId");

                    b.ToTable("CardSubCategory");
                });

            modelBuilder.Entity("Aimo.Domain.Categories.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<DateTime?>("CreatedAtUtc")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UpdatedBy")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Category", (string)null);
                });

            modelBuilder.Entity("Aimo.Domain.Categories.SubCategory", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Alias")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("CategoryId")
                        .HasColumnType("int");

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("CategoryId");

                    b.ToTable("SubCategory", (string)null);
                });

            modelBuilder.Entity("Aimo.Domain.Interests.Interest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("DisplayOrder")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.ToTable("Interest");
                });

            modelBuilder.Entity("Aimo.Domain.Users.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Bio")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("CreatedAtUtc")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("datetime2")
                        .HasDefaultValueSql("GETUTCDATE()");

                    b.Property<int>("CreatedBy")
                        .HasColumnType("int");

                    b.Property<DateTime>("DateOfBirth")
                        .HasColumnType("datetime2");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("Gender")
                        .HasColumnType("int");

                    b.Property<int?>("Height")
                        .HasColumnType("int");

                    b.Property<bool>("IsActive")
                        .HasColumnType("bit");

                    b.Property<bool>("IsDeleted")
                        .HasColumnType("bit");

                    b.Property<bool>("IsNew")
                        .HasColumnType("bit");

                    b.Property<string>("Location")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(250)
                        .HasColumnType("nvarchar(250)");

                    b.Property<int>("PreferenceAgeFrom")
                        .HasColumnType("int");

                    b.Property<int>("PreferenceAgeTo")
                        .HasColumnType("int");

                    b.Property<int?>("PreferenceGender")
                        .HasColumnType("int");

                    b.Property<int?>("PreferenceGroupOf")
                        .HasColumnType("int");

                    b.Property<string>("Uid")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("University")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("UpdatedAtUtc")
                        .HasColumnType("datetime2");

                    b.Property<int?>("UpdatedBy")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("Uid")
                        .IsUnique();

                    b.ToTable("User", (string)null);
                });

            modelBuilder.Entity("Aimo.Domain.Users.Entities.UserInterest", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("InterestId")
                        .HasColumnType("int");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("InterestId");

                    b.HasIndex("UserId");

                    b.ToTable("UserInterest");
                });

            modelBuilder.Entity("Aimo.Domain.Users.Entities.UserPicture", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("PictureType")
                        .HasColumnType("int");

                    b.Property<string>("Url")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("UserId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("UserPicture");
                });

            modelBuilder.Entity("Aimo.Domain.Cards.Card", b =>
                {
                    b.HasOne("Aimo.Domain.Categories.Category", "Category")
                        .WithMany()
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Aimo.Domain.Cards.CardPicture", b =>
                {
                    b.HasOne("Aimo.Domain.Cards.Card", "Card")
                        .WithMany("CardPictures")
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");
                });

            modelBuilder.Entity("Aimo.Domain.Cards.SwipeGroupInterest", b =>
                {
                    b.HasOne("Aimo.Domain.Interests.Interest", "Interest")
                        .WithMany()
                        .HasForeignKey("InterestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Aimo.Domain.Cards.SwipeGroup", "SwipeGroup")
                        .WithMany()
                        .HasForeignKey("SwipeGroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Interest");

                    b.Navigation("SwipeGroup");
                });

            modelBuilder.Entity("Aimo.Domain.Cards.SwipeHistory", b =>
                {
                    b.HasOne("Aimo.Domain.Cards.Card", "Card")
                        .WithMany()
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Aimo.Domain.Users.Entities.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");

                    b.Navigation("User");
                });

            modelBuilder.Entity("Aimo.Domain.Categories.CardSubCategory", b =>
                {
                    b.HasOne("Aimo.Domain.Cards.Card", "Card")
                        .WithMany()
                        .HasForeignKey("CardId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Aimo.Domain.Categories.SubCategory", "SubCategory")
                        .WithMany()
                        .HasForeignKey("SubCategoryId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Card");

                    b.Navigation("SubCategory");
                });

            modelBuilder.Entity("Aimo.Domain.Categories.SubCategory", b =>
                {
                    b.HasOne("Aimo.Domain.Categories.Category", "Category")
                        .WithMany("SubCategories")
                        .HasForeignKey("CategoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("Aimo.Domain.Users.Entities.UserInterest", b =>
                {
                    b.HasOne("Aimo.Domain.Interests.Interest", null)
                        .WithMany()
                        .HasForeignKey("InterestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Aimo.Domain.Users.Entities.User", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Aimo.Domain.Users.Entities.UserPicture", b =>
                {
                    b.HasOne("Aimo.Domain.Users.Entities.User", null)
                        .WithMany("Pictures")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Aimo.Domain.Cards.Card", b =>
                {
                    b.Navigation("CardPictures");
                });

            modelBuilder.Entity("Aimo.Domain.Categories.Category", b =>
                {
                    b.Navigation("SubCategories");
                });

            modelBuilder.Entity("Aimo.Domain.Users.Entities.User", b =>
                {
                    b.Navigation("Pictures");
                });
#pragma warning restore 612, 618
        }
    }
}
