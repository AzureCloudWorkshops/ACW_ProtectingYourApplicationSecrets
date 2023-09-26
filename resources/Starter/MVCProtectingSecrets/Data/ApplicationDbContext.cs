using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MVCProtectingSecrets.Models;

namespace MVCProtectingSecrets.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public DbSet<ImageDetail> ImageDetails { get; set; }

        public ApplicationDbContext() : base() { 
            //purposefully blank
        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            //purposefully blank
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var imageCreatedDate = new DateTime(2023, 07, 21);
            //requires that you have uploaded the images to storage with the exact names:
            modelBuilder.Entity<ImageDetail>(x => x.HasData(
                new ImageDetail() { Id = 1, AltText="Two fawns under a tree in the shade near a firepit"
                                        , Description="Two fawns enjoying the shade during the heat of summer"
                                        , FileName="fawns.jpg"},
                new ImageDetail() { Id = 2, AltText="Todd Stashwick portraying Captain Liam Shaw with an expression of pain on his face and the subtitle for the spoken line: I'm just a dipshit from Chicago"
                                        , Description="Todd Stashwick gives an emmy worthy performance of PTSD in Star Trek Picard Season three episode four"
                                        , FileName="chicago.jpg"}
            ));
            base.OnModelCreating(modelBuilder);
        }
    }
}