using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CRUD.Models
{
    public class Products
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public float Price { get; set; }
        [Column(TypeName = "VARBINARY(MAX)")]
        public byte[] Image { get; set; }
    }

    public class Admins
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Admin { get; set; }

        //Username : Admin
        //Password : 12345
        //Role : Admin
    }
}
