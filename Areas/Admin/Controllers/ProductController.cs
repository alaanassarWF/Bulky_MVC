using Bulky.DataAccess.Data;
using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;



namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        ///declare the field that can only be assigned during the ctor which is defined that field is immutable 
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            //it well go to the database run the commnd eect star from poduct retrieve that and assign the obect
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();  //in unit which object we are working on.
            return View(objProductList);
        }

        public IActionResult Upsert(int? id) //UpateInsert
        {
            //Pass some additional functionalities but before we need to retrive that.
            //restore that in an innumerable of select list item(How can we convert a category to IEnumerable SelectListItem).
            //IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category
            //     .GetAll().Select(u => new SelectListItem
            //     {
            //         Text = u.Name,
            //         Value = u.Id.ToString(),
            //     });
            ////Pass the category list into our view 
            //ViewBag.CategoryList = CategoryList;
            //Pass product
            ProductVM productVM = new ProductVM
            {
                Product = new Product(),
                CategoryList = _unitOfWork.Category
                 .GetAll().Select(u => new SelectListItem
                 {
                     Text = u.Name,
                     Value = u.Id.ToString()
                 })
              
            };
            if (id == null || id == 0 )
            {
                //Craete 
                return View(productVM);
            }
            else
            {
                //Update 
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
                return View(productVM);
            }

        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {       
            
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if(file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName); //to give a random name  for file and preserve the extenstion. 
                    string productPath = Path.Combine(wwwRootPath, @"images\product");  
                    
                    using (var fileStream = new FileStream(Path.Combine(productPath,fileName),FileMode.Create))
                    {
                        file.CopyTo(fileStream);//copy the file in the new location that will added 
                    }
                    //save that in product image URL
                    productVM.Product.ImageUrl = @"\images\product\" + fileName;
                }
                //to add this poduct in the table an save
                _unitOfWork.Product.Add(productVM.Product);
                _unitOfWork.Save();
                TempData["Success"] = "Product created successfully";
                return RedirectToAction("Index");//to redirect the user to different page after processing  
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category
                 .GetAll().Select(u => new SelectListItem
                 {
                     Text = u.Name,
                     Value = u.Id.ToString()
                 });
               return View(productVM);
            };
        }
           


       

        //public IActionResult Edit(int? id)
        //{
        //    if (id == null || id == 0)
        //    {
        //        return NotFound();
        //    }
        //    Product? poductFromDb = _unitOfWork.Product.Get(u => u.Id == id);
        //    //Product? poductFromDb1 = _catlist.Categories.FirstOrDefault(c => c.Id == id);
        //    //Product? poductFromDb2 = _catlist.Categories.Where(u=>u.Id == id).FirstOrDefault();

        //    if (poductFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(poductFromDb);
        //}

        ////Where we will retrieve a object and we have update poduct
        //[HttpPost]
        ////id is populated 
        //public IActionResult Edit(Product obj)
        //{

        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Product.update(obj);
        //        _unitOfWork.Save();
        //        TempData["Success"] = "Product created successfully";
        //        return RedirectToAction("Index");//to redirect the user to different page after processing  
        //    }
        //    return View();


        //}

        //Get Method 
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            Product? poductFromDb = _unitOfWork.Product.Get(u => u.Id == id);

            if (poductFromDb == null)
            {
                return NotFound();
            }
            return View(poductFromDb);
        }

        //Where we will retrieve a object and we have Delete poduct
        [HttpPost, ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
            //To delete poduct we have to find from Db
            Product? obj = _unitOfWork.Product.Get(u => u.Id == id);
            if (obj == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Remove(obj);
            _unitOfWork.Save();
            TempData["Success"] = "Product Deleted successfully";
            return RedirectToAction("Index");//to redirect the user to different page after processing  
        }

    }
}
