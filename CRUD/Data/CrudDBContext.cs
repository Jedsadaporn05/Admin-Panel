using CRUD.Models;
using Microsoft.EntityFrameworkCore;

namespace CRUD.Data
{
    public class CrudDBContext : DbContext
    {
        public CrudDBContext() 
        {
        }
        public CrudDBContext(DbContextOptions<CrudDBContext> options) : base(options) 
        {
        }
        public DbSet<CRUD.Models.Products> Product {  get; set; }

        public DbSet<CRUD.Models.Admins> Admin { get; set; }
    }
}
