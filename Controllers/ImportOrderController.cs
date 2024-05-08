using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using acme.Data;
using acme.Models;
using acme.Services;
using static System.Net.Mime.MediaTypeNames;
using System.Globalization;
using iTextSharp.text.pdf.parser;

namespace acme.Controllers
{
    public class ImportOrderController : Controller
    {
        private readonly AppDbContext _context;
        readonly IFileUploadService _bufferedFileUploadService;
        public ImportOrderController(AppDbContext context, IFileUploadService bufferedFileUploadService)
        {
            _bufferedFileUploadService = bufferedFileUploadService;
            _context = context;
        }

        // GET: ImportOrder
        public async Task<IActionResult> Index()
        {
            return View(await _context.ImportedOrder.ToListAsync());
        }
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }
            List<string> textlines = new List<string>();
            Console.WriteLine(file.Name);
            Console.WriteLine(file.FileName);

            using (iTextSharp.text.pdf.PdfReader reader = new iTextSharp.text.pdf.PdfReader(file.OpenReadStream()))
            {
                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    string text1 = iTextSharp.text.pdf.parser.PdfTextExtractor.GetTextFromPage(reader, page);

                    foreach (var text in text1.Split('\n'))
                    {

                        textlines.Add(text);
                        Console.WriteLine(text);
                    }


                }
            }
            var order = ParseOrderFromText(textlines);
            _context.ImportedOrder.Add(order);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", order);
        }
        // GET: ImportOrder/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var importedOrder = await _context.ImportedOrder.Include(c=>c.Items).Include(d=>d.ShippingAddress).Include(f=>f.BillingAddress)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (importedOrder == null)
            {
                return NotFound();
            }

            return View(importedOrder);
        }

        // GET: ImportOrder/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ImportOrder/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OrderNumber,OrderDate,Status,ShippingMethod,RequestedDeliveryDate,Subtotal,ShippingHandling,Tax,Total")] ImportedOrder importedOrder)
        {
            if (ModelState.IsValid)
            {
                _context.Add(importedOrder);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(importedOrder);
        }

        // GET: ImportOrder/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var importedOrder = await _context.ImportedOrder.FindAsync(id);
            if (importedOrder == null)
            {
                return NotFound();
            }
            return View(importedOrder);
        }

        // POST: ImportOrder/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,OrderNumber,OrderDate,Status,ShippingMethod,RequestedDeliveryDate,Subtotal,ShippingHandling,Tax,Total")] ImportedOrder importedOrder)
        {
            if (id != importedOrder.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(importedOrder);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ImportedOrderExists(importedOrder.Id))
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
            return View(importedOrder);
        }

        // GET: ImportOrder/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var importedOrder = await _context.ImportedOrder
                .FirstOrDefaultAsync(m => m.Id == id);
            if (importedOrder == null)
            {
                return NotFound();
            }

            return View(importedOrder);
        }

        // POST: ImportOrder/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var importedOrder = await _context.ImportedOrder.FindAsync(id);
            if (importedOrder != null)
            {
                _context.ImportedOrder.Remove(importedOrder);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ImportedOrderExists(string id)
        {
            return _context.ImportedOrder.Any(e => e.Id == id);
        }
        public static ImportedOrder ParseOrderFromText(List<string> lines)
        {
            var order = new ImportedOrder();
            order.Id = Guid.NewGuid().ToString();
            order.ShippingAddress = new Address() { Id = Guid.NewGuid().ToString() };
            order.BillingAddress = new Address() { Id = Guid.NewGuid().ToString() };
            order.Items = new List<ImportedOrderItem>();
            int i = 0;
            order.BillingAddress.FullAddress = lines[4] + "\n" + lines[7] + "\n" + lines[8] + ", " + lines[11] + "\n" + lines[13];
            order.ShippingAddress.FullAddress = lines[18] + "\n" + lines[20] + "\n" + lines[21] + "\n" + lines[23] + "\n" + lines[24];
            order.RequestedDeliveryDate = DateTime.ParseExact(lines[22], "M/d/yyyy", CultureInfo.InvariantCulture);
            order.ShippingMethod = lines[17];
            order.OrderNumber = lines[0].Split('#')[1].Trim();
            order.OrderDate = DateTime.ParseExact(lines[2].Split(new[] { "Order Date:" }, StringSplitOptions.None)[1].Split(' ')[1], "M/d/yyyy", CultureInfo.InvariantCulture);
            order.Subtotal = decimal.Parse(lines[3].Split('$')[1].Trim());
            order.Status = lines[5].Split(':')[1].Trim();
            order.ShippingHandling = decimal.Parse(lines[6].Split('$')[1].Trim());
            order.Tax = decimal.Parse(lines[9].Split('$')[1].Trim());
            order.Total = decimal.Parse(lines[12].Split('$')[1].Trim());
            order.PO = lines[14].Split(':')[1].Trim();
            foreach (var line in lines)
            {
                
               if (line.Contains("Distribution #:"))
                {
                   var currentItem = new ImportedOrderItem();
                    currentItem.Id = Guid.NewGuid().ToString();
                    currentItem.DistributionNumber = line.Split(':')[1].Trim();
                    currentItem.UnitType = lines[i - 1];
                    currentItem.CustomerNumber = lines[i - 2].Split(':')[1].Trim();
                    currentItem.UnitPrice = decimal.Parse(lines[i - 3].Split(' ')[0].Replace("$", "").Trim());
                    currentItem.Quantity = int.Parse(lines[i - 3].Split(' ')[1].Trim());
                    currentItem.Subtotal = decimal.Parse(lines[i - 3].Split(' ')[2].Replace("$", "").Trim());
                    currentItem.Description = lines[i - 4];

                    order.Items.Add(currentItem);
                    
                }
                
                i++;
            }

            return order;
        }
    }

}

//public FileUploadController(
//}
//public IActionResult Index()
//{
//    return View();
//}



//    // Read the file and convert it into a list of lists representing rows and columns
//    var csvData = new List<List<string>>();

//    using (var stream = new StreamReader(file.OpenReadStream(), Encoding.UTF8))
//    {
//        using (TextFieldParser parser = new TextFieldParser(stream))
//        {
//            parser.TextFieldType = FieldType.Delimited;
//            parser.SetDelimiters(",");
//            parser.HasFieldsEnclosedInQuotes = true; // This ensures that commas inside quotes are not treated as delimiter

//            while (!parser.EndOfData)
//            {
//                // Read fields on the current line
//                string[] fields = parser.ReadFields();
//                // fields[0] will be 'first name', fields[1] will be 'last name', etc.

//                var list = new List<string>();
//                foreach (var field in fields)
//                {

//                    list.Add(field);

//                }
//                csvData.Add(list);

//            }
//        }
//    }
//    // Get the file's original name
//    var fileName = Path.GetFileName(file.FileName);

//    // Get the current directory and combine it with the desired sub-directory for uploads
//    var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "tmp");

//    // Check if the 'uploads' directory exists, and if not, create it
//    if (!Directory.Exists(uploadsFolderPath))
//    {
//        Directory.CreateDirectory(uploadsFolderPath);
//    }

//    // Combine the uploads folder path with the file name
//    var savePath = Path.Combine(uploadsFolderPath, fileName);

//    // Save the file to the server
//    try
//    {

//        var jsonString = JsonConvert.SerializeObject(csvData);
//        System.IO.File.WriteAllText(savePath, jsonString);
//    }
//    catch (JsonException ex)
//    {
//        // Handle JSON-specific errors
//    }
//    catch (Exception ex)
//    {
//        // Handle general errors
//    }


//    TempData["path"] = savePath;
//    TempData.Keep("path"); // Keep TempData for the next request

//    var csvFile = new CsvFile();
//    csvFile.Sender = new Sender();
//    csvFile.columnMap = new Dictionary<string, int>();
//    csvFile.cxsvData = csvData;
//    // Pass the data to the view

//    var json = JsonConvert.SerializeObject(csvFile);

//    // Store the updated form data back to TempData
//    TempData["csvFile"] = json;
//    TempData.Keep("csvFile"); // Keep TempData for the next request

//    return View("CreateSender", new Sender());

//}