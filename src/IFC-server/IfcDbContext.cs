namespace IFC4
{
	using Microsoft.EntityFrameworkCore;

	public class IfcDbContext : DbContext
	{
		public IfcDbContext(DbContextOptions<IfcDbContext> options): base(options)
		{
		
		}
 
		public DbSet<IfcProduct> Products { get; set; }
 
	}

}