using KBE.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace KBE.Components.SQL
{
    public class KanjiWordContext : DbContext
    {
        public DbSet<KanjiWord> KANJIWORD { get; set; }

        public string DbPath { get; }

        public KanjiWordContext(string path)
        {
            DbPath = path;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<KanjiWord>().Property(x => x.Radicals).HasColumnName("Radical");
        }
    }
}
