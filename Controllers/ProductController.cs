using Assignment5.Models.Domain;
using Microsoft.AspNetCore.Mvc;

namespace Assignment5.Controllers
{
    public class ProductController : Controller
    {
        private readonly MVCDemoDbContextcs context;
        private readonly IWebHostEnvironment environment;

        public ProductController(MVCDemoDbContextcs context, IWebHostEnvironment environment)
        {
            this.context = context;
            this.environment = environment;
        }
        [HttpGet]
        public IActionResult Index()
        {
            var data = context.Products.ToList();
            return View(data);
        }
        [HttpGet]
        public IActionResult AddProduct()
        {
            return View();
        }
        [HttpPost]
        public IActionResult AddProDuct(Product model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    string uniqueFileName = UploadImage(model);
                    var data = new Product()
                    {
                        Title = model.Title,
                        Summary = model.Summary,
                        Path = uniqueFileName,
                        Price = model.Price,
                        Number = model.Number,
                    };
                    context.Products.Add(data);
                    context.SaveChanges();
                    TempData["Success"] = "Record Successfully saved!";
                    return RedirectToAction("Index");   
                }
                ModelState.AddModelError(string.Empty, "Model is not valid, please check");
            }
            catch(Exception ex)
            {
                ModelState.AddModelError(String.Empty, ex.Message);
            } 
            return View(model);
        }
        public IActionResult Delete(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }
            else
            {
                var data = context.Products.Where(e => e.Id == id).SingleOrDefault();
                if(data != null)
                {
                    string deleteFromFolder = Path.Combine(environment.WebRootPath, "Content/Product/");
                    string currentImage = Path.Combine(Directory.GetCurrentDirectory(), deleteFromFolder, data.Path);
                    if(currentImage !=null)
                    {
                        if(System.IO.File.Exists(currentImage))
                        {
                            System.IO.File.Delete(currentImage);
                        }
                    }
                    context.Products.Remove(data);
                    context.SaveChanges();
                    TempData["Success"] = "Record Delete!";
                }
            }
            return RedirectToAction("Index");
        }
        [HttpGet]
        public IActionResult Details(int id) 
        {
            if(id == 0)
            {
                return NotFound();
            }
            var data = context.Products.Where(e => e.Id == id).SingleOrDefault();
            return View(data);
        }
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var data = context.Products.Where(e => e.Id == id).SingleOrDefault();
            if(data != null)
            {
                return View(data);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public IActionResult Edit(Product model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var data = context.Products.Where(e=>e.Id == model.Id).SingleOrDefault();
                    string uniqueFileName = string.Empty;
                    if(model.ImagePath != null) {
                        if(data.Path != null) {
                            string filepath = Path.Combine(environment.WebRootPath,"Content/Product",data.Path);
                            if(System.IO.File.Exists(filepath)) {
                                System.IO.File.Delete(filepath);
                            }
                        }
                        uniqueFileName = UploadImage(model);
                    }
                    data.Title = model.Title;
                    data.Summary = model.Summary;
                    data.Price = model.Price;
                    data.Number = model.Number;
                    if(model.ImagePath != null)
                    {
                        data.Path = uniqueFileName;
                    }
                    context.Products.Update(data);
                    context.SaveChanges();
                    TempData["Success"] = "Record Update Successfully !!";
                } 
            }
            catch (Exception ex) {
                ModelState.AddModelError(string.Empty, ex.Message);
            }
            return View(model);
        }
        private string UploadImage(Product model)
        {
            string uniqueFileName = string.Empty;
            if(model.ImagePath != null)
            {
                string uploadFolder = Path.Combine(environment.WebRootPath,"Content/Product/");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImagePath.FileName;
                string filePath = Path.Combine(uploadFolder,uniqueFileName);
                using(var fileStream = new FileStream(filePath,FileMode.Create)) 
                {
                    model.ImagePath.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }
    }
}
