using Microsoft.EntityFrameworkCore;
 
namespace FlashCards.Models
{
    public class Context : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public Context(DbContextOptions options) : base(options) { }

	 public DbSet<User> User {get;set;}
	 public DbSet<Group> Group {get;set;}
	 public DbSet<Card> Card {get;set;}

    }
}