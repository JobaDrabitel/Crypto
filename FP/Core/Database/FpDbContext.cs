using FP.Core.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace FP.Core.Database;

public class FpDbContext : DbContext
{

    public DbSet<User> Users { get; set; }
    public DbSet<Pack> Packs { get; set; }
    public DbSet<PackType> PackTypes { get; set; }
    public DbSet<Wallet> Wallets { get; set; }
    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Withdraw> Withdraws { get; set; }
    public DbSet<Investment?> Investments { get; set; }
    public DbSet<Pool> Pools { get; set; }
    public DbSet<Referral> Referrals { get; set; }
    public DbSet<VerificationCode> VerificationCodes { get; set; }
	public DbSet<OperationType> OperationTypes { get; set; }
	public DbSet<Operation> Operations { get; set; }
	public DbSet<Notify> Notifications { get; set; }
	public DbSet<Promocode> Promocodes { get; set; }
	public DbSet<DefiTransactions> DefiTransactions { get; set; }

	public FpDbContext(DbContextOptions<FpDbContext> dbContextOptions) : base(dbContextOptions) { }

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{

	}
	
	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<Referral>()
			.HasOne(r => r.Referrer)
			.WithMany(e => e.ReferrersCollection)
			.HasForeignKey(r => r.ReferrerId)
			.IsRequired(false);

		modelBuilder.Entity<Referral>()
			.HasOne(r => r.Ref)
			.WithMany(e => e.Referral)
			.HasForeignKey(r => r.RefId)
			.IsRequired(false);
	}
}
