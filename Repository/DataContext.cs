using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Shopping_Tutorial.Models;

namespace Shopping_Tutorial.Repository
{
	public class DataContext : IdentityDbContext<AppUserModel>
	{

		public DataContext(DbContextOptions options) : base(options) { }
		public DataContext(DbContextOptions<DbContext> options) : base(options) 
		{
		}
		public DbSet<BrandModel> Brands { get; set; }
		public DbSet<ProductModel> Products { get; set; }	
		public DbSet<CategoryModel> Categories { get; set; }
		public DbSet<OrderModel> Orders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }


    }
}
