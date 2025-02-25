using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CRUD.Data;
using CRUD.Models;
using System.Drawing;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace CRUD.Controllers
{
    public class AdminController : Controller
    {

        private readonly CrudDBContext _context;

        public AdminController(CrudDBContext context)
        {
            _context = context;
        }
        public IActionResult Index1()
        {
            var products = _context.Product.ToList();
            return View(products);
        }

        public IActionResult Login()
        {
            return View();
        }

        // POST: Login
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            
            // ตรวจสอบข้อมูล Admin
            var admin = _context.Admin.FirstOrDefault(a => a.Username == username && a.Password == password);

            if (admin == null)
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }

            // บันทึกข้อมูลการเข้าสู่ระบบใน Session
            HttpContext.Session.SetString("Username", admin.Username);
            HttpContext.Session.SetString("Role", admin.Admin);

            return RedirectToAction("Index", "Admin"); // เปลี่ยนไปยังหน้าจัดการข้อมูล CRUD
        }

        // Action สำหรับการจัดการข้อมูล CRUD
        public IActionResult Index2()
        {
            // ตรวจสอบสิทธิ์ Admin
            var role = HttpContext.Session.GetString("Role");

            if (role != "Admin")
            {
                return RedirectToAction("Login");
            }

            // ดำเนินการแสดงหน้าจัดการข้อมูล CRUD (หน้า Admin)
            var products = _context.Product.ToList();
            return View(products);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index1");
        }

        // GET: Admin
        public async Task<IActionResult> Index(int ID)
        {
            return View(await _context.Product.ToListAsync());
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View();
        }

        public async Task<byte[]> ConvertToByteArray(IFormFile imageFile)
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await imageFile.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }

            return null;
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int ProductID, string Name, string Description, float Price, [FromForm] IFormFile imageFile, Products products)
        {

            ModelState.Remove("Image");

            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await imageFile.CopyToAsync(memoryStream);
                        products.Image = memoryStream.ToArray(); //จัดเก็บไฟล์
                    }
                }

                // แปลงรูปภาพที่ได้รับจากฟอร์มเป็น byte array
                if (imageFile != null)
                {
                    products.Image = await ConvertToByteArray(imageFile);
                }

                _context.Add(products);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Product.FindAsync(id);
            if (products == null)
            {
                return NotFound();
            }
            return View(products);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int ProductID, string Name, string Description, float Price, [FromForm] IFormFile imageFile, Products products)
        {
			ModelState.Remove("Image");

			if (ProductID != products.ProductID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (imageFile != null)
                    {
                        products.Image = await ConvertToByteArray(imageFile);
                    }
                    else
                    {
                        // เก็บค่ารูปภาพเดิมหากไม่ได้อัพโหลดรูปใหม่
                        var existingProduct = await _context.Product.AsNoTracking().FirstOrDefaultAsync(p => p.ProductID == ProductID);
                        products.Image = existingProduct.Image;
                    }

                    _context.Update(products);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductsExists(products.ProductID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(products);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var products = await _context.Product
                .FirstOrDefaultAsync(m => m.ProductID == id);
            if (products == null)
            {
                return NotFound();
            }

            return View(products);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var products = await _context.Product.FindAsync(id);
            if (products != null)
            {
                _context.Product.Remove(products);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductsExists(int id)
        {
            return _context.Product.Any(e => e.ProductID == id);
        }
    }
}
