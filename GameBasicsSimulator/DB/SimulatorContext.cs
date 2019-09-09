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

        public DbSet<League> Leagues { get; set; }
        public DbSet<MatchDay> MatchDays { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Goal> Goals { get; set; }
        public DbSet<Card> Cards { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
        }
    }
}
