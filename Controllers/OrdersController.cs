using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using acme.Data;
using acme.Models;
using Microsoft.AspNetCore.Authentication;
using Newtonsoft.Json;

namespace acme.Controllers
{
    public class OrdersController : Controller
    {
        private readonly AppDbContext _context;

        public OrdersController(AppDbContext context)
        {
            _context = context;
        }

        // Method to retrieve an order by ID using a query string parameter
        public async Task<Order> GetOrderByIdAsync(int orderId, string token)
        {
            var _httpClient = new HttpClient();
            try
            {
                // Constructing the request URL with a query string
                string requestUrl = $"http://64.23.163.48/orders/search_order_by_id?order_id={orderId}";
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.GetAsync(requestUrl);

                if (response.IsSuccessStatusCode)
                {
                    string jsonResponse = await response.Content.ReadAsStringAsync();
                    Console.WriteLine(jsonResponse);
                    return JsonConvert.DeserializeObject<Order>(jsonResponse);
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
            var order = await GetOrderByIdAsync(int.Parse("1"), token);
            List<Order> Orders = new List<Order>();
            Orders.Add(order);
            return View(Orders);

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
            var orders = await GetOrderByIdAsync(int.Parse(id), token);

            if (orders == null)
            {
                return NotFound();
            }

            return View(orders);
        }

        // GET: Orders/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OrderId")] Order order)
        {
            if (ModelState.IsValid)
            {
                _context.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(order);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OrderId")] Order order)
        {
            if (id != order.OrderId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(order);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(order.OrderId))
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
            return View(order);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var order = await _context.Orders
                .FirstOrDefaultAsync(m => m.OrderId == id);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderId == id);
        }
    }
}
