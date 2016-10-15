using Microsoft.EntityFrameworkCore;
using NetCoreLearning.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreLearning.Repositoy
{
    public class LearningDbContext : DbContext
    {
        public LearningDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Restaurant> Restaurants { get; set; }
    }
}
