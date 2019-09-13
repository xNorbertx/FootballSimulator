using Microsoft.EntityFrameworkCore;
using Simulator.Core.Model;

namespace Simulator.Infrastructure.DB
{
    public class SimulatorContext : DbContext
    {
        public SimulatorContext(DbContextOptions<SimulatorContext> options)
            : base(options)
        {}

        public DbSet<League> Leagues { get; set; }
        public DbSet<MatchDay> MatchDays { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Match>()
                .HasOne(x => x.TeamOne)
                .WithMany(x => x.HomeMatches)
                .HasForeignKey(x => x.TeamOneId);

            modelBuilder.Entity<Match>()
                .HasOne(x => x.TeamTwo)
                .WithMany(x => x.AwayMatches)
                .HasForeignKey(x => x.TeamTwoId);
        }
    }
}
