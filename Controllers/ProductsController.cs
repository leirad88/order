using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using acme.Data;
using acme.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace acme.Controllers
{
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly AppDbContext _context;

        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<List<Product>> GetProductsAsync(string token)
        {
            var _httpClient = new HttpClient();
            try
            {
                // Constructing the request URL with a query string
                string requestUrl = $"http://64.23.163.48/products/get_all";
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(jsonResponse);
                    return JsonConvert.DeserializeObject<List<Product>>(jsonResponse);
                }
                else
                {
                    // Handle the case when the order is not found or the API call fails
                    Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return null;
            }
        }
        // GET: Products
        public async Task<Product> GetProductByIdAsync(string id, string token)
        {
            var _httpClient = new HttpClient();
            try
            {
                // Constructing the request URL with a query string
                string requestUrl = $"http://64.23.163.48/products/search_product{id}";
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(jsonResponse);
                    return JsonConvert.DeserializeObject<Product>(jsonResponse);
                }
                else
                {
                    // Handle the case when the order is not found or the API call fails
                    Console.WriteLine($"API call failed with status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine($"Exception occurred: {ex.Message}");
                return null;
            }
        }

        // GET: Orders
        public async Task<IActionResult> Index()
        {
            //var token = HttpContext.Request.Cookies["AccessToken"];
            //var token2 =  await HttpContext.GetTokenAsync("authorization");

            var token = await HttpContext.GetTokenAsync("access_token");

            Console.WriteLine("Token Cookies is " + token);
            var products = await GetProductsAsync(token);
            
            return View(products);

        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var token = await HttpContext.GetTokenAsync("access_token");
            Console.WriteLine("Token is " + token);
            var product = await GetProductByIdAsync(id, token);

            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Pack,Uom,Comments,Price,DepartmentName,Active,DateActive,DateDeactive,Priority")] Product product)
        {
            if (ModelState.IsValid)
            {
                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Name,Pack,Uom,Comments,Price,DepartmentName,Active,DateActive,DateDeactive,Priority")] Product product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Product
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var product = await _context.Product.FindAsync(id);
            if (product != null)
            {
                _context.Product.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(string id)
        {
            return _context.Product.Any(e => e.Id == id);
        }
    }
}
