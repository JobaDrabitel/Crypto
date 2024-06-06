﻿// <auto-generated />
using System;
using FP.Core.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FP.Migrations
{
    [DbContext(typeof(FpDbContext))]
    [Migration("20240115163726_NotifyDate")]
    partial class NotifyDate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.13")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FP.Core.Database.Models.DefiTransactions", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("AdditionPercent")
                        .HasColumnType("numeric");

                    b.Property<int>("DaysWithoutWithdraws")
                        .HasColumnType("integer");

                    b.Property<int>("InvestId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsClosed")
                        .HasColumnType("boolean");

                    b.Property<decimal>("Sum")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("InvestId");

                    b.ToTable("DefiTransactions");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Investment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsEnded")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastAccrualDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int>("MaxPacksCount")
                        .HasColumnType("integer");

                    b.Property<decimal>("MaxSum")
                        .HasColumnType("numeric");

                    b.Property<int>("PacksCount")
                        .HasColumnType("integer");

                    b.Property<int>("PoolId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("TotalAccrual")
                        .HasColumnType("numeric");

                    b.Property<decimal>("TotalSum")
                        .HasColumnType("numeric");

                    b.Property<decimal>("TotalYield")
                        .HasColumnType("numeric");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("PoolId");

                    b.HasIndex("UserId");

                    b.ToTable("Investments");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Notify", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("Date")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("IsRead")
                        .HasColumnType("boolean");

                    b.Property<string>("Message")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Notifications");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Operation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<bool>("IsAgentBalance")
                        .HasColumnType("boolean");

                    b.Property<int>("OperationTypeId")
                        .HasColumnType("integer");

                    b.Property<int?>("PartnerId")
                        .HasColumnType("integer");

                    b.Property<string>("Source")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Sum")
                        .HasColumnType("numeric");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("OperationTypeId");

                    b.HasIndex("PartnerId");

                    b.HasIndex("UserId");

                    b.ToTable("Operations");
                });

            modelBuilder.Entity("FP.Core.Database.Models.OperationType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("OperationTypes");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Pack", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<bool>("ByMaxDuration")
                        .HasColumnType("boolean");

                    b.Property<decimal>("DealSum")
                        .HasColumnType("numeric");

                    b.Property<DateTime>("EndDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("HasLastAccrual")
                        .HasColumnType("boolean");

                    b.Property<int>("InvestmentId")
                        .HasColumnType("integer");

                    b.Property<int>("PackTypeId")
                        .HasColumnType("integer");

                    b.Property<DateTime>("StartDate")
                        .HasColumnType("timestamp with time zone");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.Property<decimal>("Yield")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("InvestmentId");

                    b.HasIndex("PackTypeId");

                    b.HasIndex("UserId");

                    b.ToTable("Packs");
                });

            modelBuilder.Entity("FP.Core.Database.Models.PackType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("ActiveSum")
                        .HasColumnType("numeric");

                    b.Property<string>("Avatar")
                        .HasColumnType("text");

                    b.Property<int>("MaxDuration")
                        .HasColumnType("integer");

                    b.Property<int>("MinDuration")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("TotalIncome")
                        .HasColumnType("numeric");

                    b.Property<decimal>("Yield")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("PackTypes");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Pool", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("ActiveSum")
                        .HasColumnType("numeric");

                    b.Property<string>("Avatar")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int>("MaxPacksCount")
                        .HasColumnType("integer");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("TotalIncome")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.ToTable("Pools");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Referral", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int>("Inline")
                        .HasColumnType("integer");

                    b.Property<int>("RefId")
                        .HasColumnType("integer");

                    b.Property<int>("ReferrerId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("RefId");

                    b.HasIndex("ReferrerId");

                    b.ToTable("Referrals");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Transaction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("DealSum")
                        .HasColumnType("numeric");

                    b.Property<bool>("FromAgent")
                        .HasColumnType("boolean");

                    b.Property<int>("FromUserId")
                        .HasColumnType("integer");

                    b.Property<bool>("IsConfirmed")
                        .HasColumnType("boolean");

                    b.Property<bool>("ToAgent")
                        .HasColumnType("boolean");

                    b.Property<int>("ToUserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("FromUserId");

                    b.HasIndex("ToUserId");

                    b.ToTable("Transactions");
                });

            modelBuilder.Entity("FP.Core.Database.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Avatar")
                        .HasColumnType("text");

                    b.Property<decimal>("BalanceAgent")
                        .HasColumnType("numeric");

                    b.Property<decimal>("BalanceCrypto")
                        .HasColumnType("numeric");

                    b.Property<decimal>("BalanceIncome")
                        .HasColumnType("numeric");

                    b.Property<string>("City")
                        .HasColumnType("text");

                    b.Property<string>("Country")
                        .HasColumnType("text");

                    b.Property<decimal>("CurrentIncome")
                        .HasColumnType("numeric");

                    b.Property<string>("Email")
                        .HasColumnType("text");

                    b.Property<bool>("IsAdmin")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsRegistrationEnded")
                        .HasColumnType("boolean");

                    b.Property<bool>("IsVerified")
                        .HasColumnType("boolean");

                    b.Property<DateTime>("LastActivityTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<decimal>("LinesIncome")
                        .HasColumnType("numeric");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<string>("Nickname")
                        .HasColumnType("text");

                    b.Property<string>("Passwordhash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Phone")
                        .HasColumnType("text");

                    b.Property<int>("Rang")
                        .HasColumnType("integer");

                    b.Property<string>("ReferralCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("ReferrerCode")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<DateTime>("RegistrationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("ShowEmail")
                        .HasColumnType("boolean");

                    b.Property<bool>("ShowPhone")
                        .HasColumnType("boolean");

                    b.Property<bool>("ShowTg")
                        .HasColumnType("boolean");

                    b.Property<string>("Surname")
                        .HasColumnType("text");

                    b.Property<string>("Telegram")
                        .HasColumnType("text");

                    b.Property<long?>("TelegramId")
                        .HasColumnType("bigint");

                    b.Property<int>("TopUpWalletId")
                        .HasColumnType("integer");

                    b.Property<decimal>("TotalIncome")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("TopUpWalletId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("FP.Core.Database.Models.VerificationCode", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("Code")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<bool>("IsActive")
                        .HasColumnType("boolean");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("VerificationCodes");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Wallet", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<string>("WalletAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("WalletSecretKey")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Wallets");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Withdraw", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<bool>("FromAgentBalance")
                        .HasColumnType("boolean");

                    b.Property<DateTime?>("RealizationTime")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<decimal>("Sum")
                        .HasColumnType("numeric");

                    b.Property<int>("UserId")
                        .HasColumnType("integer");

                    b.Property<string>("WalletAddress")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("Withdraws");
                });

            modelBuilder.Entity("FP.Core.Database.Models.DefiTransactions", b =>
                {
                    b.HasOne("FP.Core.Database.Models.Investment", "Investment")
                        .WithMany()
                        .HasForeignKey("InvestId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Investment");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Investment", b =>
                {
                    b.HasOne("FP.Core.Database.Models.Pool", "Pool")
                        .WithMany()
                        .HasForeignKey("PoolId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FP.Core.Database.Models.User", "User")
                        .WithMany("Investments")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Pool");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Notify", b =>
                {
                    b.HasOne("FP.Core.Database.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Operation", b =>
                {
                    b.HasOne("FP.Core.Database.Models.OperationType", "OperationType")
                        .WithMany()
                        .HasForeignKey("OperationTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FP.Core.Database.Models.User", "Partner")
                        .WithMany()
                        .HasForeignKey("PartnerId");

                    b.HasOne("FP.Core.Database.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("OperationType");

                    b.Navigation("Partner");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Pack", b =>
                {
                    b.HasOne("FP.Core.Database.Models.Investment", "Investment")
                        .WithMany("Packs")
                        .HasForeignKey("InvestmentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FP.Core.Database.Models.PackType", "PackType")
                        .WithMany()
                        .HasForeignKey("PackTypeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FP.Core.Database.Models.User", null)
                        .WithMany("Packs")
                        .HasForeignKey("UserId");

                    b.Navigation("Investment");

                    b.Navigation("PackType");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Referral", b =>
                {
                    b.HasOne("FP.Core.Database.Models.User", "Ref")
                        .WithMany("Referral")
                        .HasForeignKey("RefId");

                    b.HasOne("FP.Core.Database.Models.User", "Referrer")
                        .WithMany("ReferrersCollection")
                        .HasForeignKey("ReferrerId");

                    b.Navigation("Ref");

                    b.Navigation("Referrer");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Transaction", b =>
                {
                    b.HasOne("FP.Core.Database.Models.User", "FromUser")
                        .WithMany()
                        .HasForeignKey("FromUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("FP.Core.Database.Models.User", "ToUser")
                        .WithMany()
                        .HasForeignKey("ToUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("FromUser");

                    b.Navigation("ToUser");
                });

            modelBuilder.Entity("FP.Core.Database.Models.User", b =>
                {
                    b.HasOne("FP.Core.Database.Models.Wallet", "TopUpWallet")
                        .WithMany()
                        .HasForeignKey("TopUpWalletId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("TopUpWallet");
                });

            modelBuilder.Entity("FP.Core.Database.Models.VerificationCode", b =>
                {
                    b.HasOne("FP.Core.Database.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId");

                    b.Navigation("User");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Withdraw", b =>
                {
                    b.HasOne("FP.Core.Database.Models.User", "User")
                        .WithMany("Withdraws")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("FP.Core.Database.Models.Investment", b =>
                {
                    b.Navigation("Packs");
                });

            modelBuilder.Entity("FP.Core.Database.Models.User", b =>
                {
                    b.Navigation("Investments");

                    b.Navigation("Packs");

                    b.Navigation("Referral");

                    b.Navigation("ReferrersCollection");

                    b.Navigation("Withdraws");
                });
#pragma warning restore 612, 618
        }
    }
}
