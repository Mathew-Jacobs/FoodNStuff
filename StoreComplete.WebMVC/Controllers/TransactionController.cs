using PagedList;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using StoreComplete.WebMVC.Models;

namespace StoreComplete.WebMVC.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transaction
        public ActionResult Index(string sortOrder, string currentFilter, string searchString, int? page)
        {
            ViewBag.CurrentSort = sortOrder;
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.FirstSortParm = sortOrder == "First" ? "first_desc" : "First";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }
            ViewBag.CurrentFilter = searchString;

            var transactions = from s in db.Transactions
                           select s;

            if (!String.IsNullOrEmpty(searchString))
            {
                transactions = transactions.Where(s => s.Customer.FirstName.Contains(searchString) || s.Customer.LastName.Contains(searchString)
                    || s.Product.ProductName.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    transactions = transactions.OrderByDescending(s => s.Customer.LastName);
                    break;
                case "First":
                    transactions = transactions.OrderBy(s => s.Product.ProductName);
                    break;
                case "first_desc":
                    transactions = transactions.OrderByDescending(s => s.Product.ProductName);
                    break;
                default:
                    transactions = transactions.OrderBy(s => s.Customer.LastName);
                    break;
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(transactions.ToPagedList(pageNumber, pageSize));
        }

        //var transactions = db.Transactions.Include(t => t.Customer).Include(t => t.Product);
        //    return View(transactions.ToList());
        // GET: Transaction/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transaction/Create
        public ActionResult Create()
        {
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName");
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName");
            return View();
        }

        // POST: Transaction/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TransactionID,CustomerID,ProductID,AmmountPurchased")] Transaction transaction)
        {
            GetProductByID(transaction);
            if (ModelState.IsValid)
            {
                db.Transactions.Add(transaction);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", transaction.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", transaction.ProductID);
            return View(transaction);
        }

        private Product GetProductByID(Transaction transaction)
        {
            //If the product doesn't exist, 
            if (transaction.ProductID == null)
            {
                //we need to throw an exception.
                throw new Exception();
            }

            //When we find the product in the db, we need to store it in a variable.
            var product = db.Products.Find(transaction.ProductID);

            //If the product doesn't exist we need to 
            if (product == null)
            {
                //throw an exception.
                throw new Exception();
            }

            //Update the product quantity by calling the method.
            UpdateProductQuantity(product, 1);  //The quantity to update will need to be whatever is passed in.

            //Get the product out of the function
            return product;
        }

        private void UpdateProductQuantity(Product product, int quantityPurchased)
        {
            //Access the quantity property and decrement it.
            product.Quantity -= quantityPurchased;
        }

        // GET: Transaction/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", transaction.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", transaction.ProductID);
            return View(transaction);
        }

        // POST: Transaction/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TransactionID,CustomerID,ProductID,AmmountPurchased")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "CustomerID", "FirstName", transaction.CustomerID);
            ViewBag.ProductID = new SelectList(db.Products, "ProductID", "ProductName", transaction.ProductID);
            return View(transaction);
        }

        // GET: Transaction/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
