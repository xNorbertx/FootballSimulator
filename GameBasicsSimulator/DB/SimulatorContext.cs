using System;
using GameBasicsSimulator.Model;
using Microsoft.EntityFrameworkCore;

namespace GameBasicsSimulator.DB
{
    public class SimulatorContext : DbContext
    {
        public SimulatorContext(DbContextOptions<SimulatorContext> options)
            : base(options)
        {

        }

        public DbSet<Team> Teams { get; set; }
        public DbSet<MatchTeam> MatchTeams { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MatchTeam>()
                .HasKey(mt => new { mt.MatchId, mt.TeamId });
            modelBuilder.Entity<MatchTeam>()
                .HasOne(mt => mt.Team)
                .WithMany(t => t.MatchTeams)
                .HasForeignKey(mt => mt.TeamId);
            modelBuilder.Entity<MatchTeam>()
                .HasOne(mt => mt.Match)
                .WithMany(m => m.MatchTeams)
                .HasForeignKey(mt => mt.MatchId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Goal>(goal =>
            {
                goal.HasOne(g => g.Team)
                    .WithMany(t => t.Goals)
                    .HasForeignKey(g => g.TeamId);

                goal.HasOne(g => g.Opponent)
                    .WithMany(t => t.GoalsConceded)
                    .HasForeignKey(g => g.OpponentId);

                goal.HasOne(g => g.Match)
                    .WithMany(m => m.Goals)
                    .HasForeignKey(g => g.MatchId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
