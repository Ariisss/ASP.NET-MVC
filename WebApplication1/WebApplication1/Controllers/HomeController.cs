using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {

        private oopEntities1 db = new oopEntities1();

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Login()
        {
            
            if (Session != null && Session["UserId"] != null)
            {
                return RedirectToAction("Dashboard");
            }


            return View();
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult userLogin(String Email, String Password)
        {

            if (string.IsNullOrEmpty(Email) || string.IsNullOrEmpty(Password))
            {
                ViewBag.ErrorMessage = "Both email and password required";
                return View("Login");
            }

            var user = db.Users.SingleOrDefault(u => u.email == Email && u.hashed_pass == Password);

            if (user != null)
            {
                Session["UserId"] = user.user_id;

                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.ErrorMessage = "Invalid email or password";
                return View("Login");
            }

        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Dashboard()
        {

            if (Session != null && Session["UserId"] != null)
            {
                int? userId = (int?)Session["UserId"];
                //User user = db.Users.FirstOrDefault(u => u.user_id == userId);
                User user = db.Users.FirstOrDefault(u => u.user_id == userId);
                var role = user.Role.role_name;

                var layoutModel = new LayoutModel
                {
                    UserId = user.user_id,
                    UserName = user.FullName,
                    RoleName = user.Role.role_name
                };

                if (user != null)
                {
                    if (role == "Employee")
                    {
                        return RedirectToAction("EmployeeDashboard", layoutModel);
                    }
                    else if (role == "Manager")
                    {
                        return RedirectToAction("ManagerDashboard", layoutModel);
                    }
                    else if (role == "Admin")
                    {
                        return RedirectToAction("AdminDashboard", layoutModel);
                    }
                }
            }

            return RedirectToAction("Login");
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult AdminDashboard(LayoutModel layoutModel)
        {

            if(Session != null && Session["UserId"] != null)
            {
                var retModel = new viewModel();


                List<User> users = db.Users.Where(u => u.role_id != 3).ToList();

                retModel.Users = users;
                retModel.LayoutModel = layoutModel;

                return View(retModel);
            }

            return RedirectToAction("Dashboard");

        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult ManagerDashboard(LayoutModel layoutModel)
        {

            if(Session != null && Session["UserId"] != null)
            {
                var retModel = new viewModel();



                List<Order> orders = db.Orders.Where(u => u.created_by == layoutModel.UserId).ToList();

                retModel.Orders = orders;
                retModel.LayoutModel = layoutModel;

                return View(retModel);
            }

            return RedirectToAction("Dashboard");
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult EmployeeDashboard(LayoutModel layoutModel)
        {
            if (Session != null && Session["UserId"] != null)
            {
                var retModel = new viewModel();

                List<Order> orders = db.Orders.Where(u => u.assigned_to == layoutModel.UserId).ToList();

                retModel.Orders = orders;
                retModel.LayoutModel = layoutModel;

                return View(retModel);
            }

            return RedirectToAction("Dashboard");
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Register()
        {

            if (Session != null && Session["UserId"] != null)
            {
                return View();
            }

                return RedirectToAction("Dashboard");
        }

        [HttpPost]
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult registerSave(string firstName, string lastName, string Email, string Password, int userType)
        {

            if (Session != null && Session["UserId"] != null)
            {
                if (ModelState.IsValid)
                {

                    User newUser = new User
                    {
                        user_name = Email,
                        hashed_pass = Password,
                        first_name = firstName,
                        last_name = lastName,
                        email = Email,
                        role_id = userType
                    };


                    db.Users.Add(newUser);
                    db.SaveChanges();


                    return RedirectToAction("Dashboard");
                }

                return View("Register", new User { first_name = firstName, last_name = lastName, email = Email, hashed_pass = Password });
            }

            return RedirectToAction("Dashboard");

        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult Logout()
        {

            if (Session != null && Session["UserId"] != null)
            {
                Session.Remove("UserId");
            }

            this.Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            this.Response.Cache.SetNoStore();

            return RedirectToAction("Login");
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult AddTask()
        {
            if (Session != null && Session["UserId"] != null)
            {
                int? userId = (int?)Session["UserId"];
                User user = db.Users.FirstOrDefault(u => u.user_id == userId);
                string userName = user.user_name;
                string roleName = user.Role.role_name;
                var retModel = new viewModel();

                var layoutModel = new LayoutModel
                {
                    UserId = userId,
                    UserName = userName,
                    RoleName = roleName

                };

                var employees = db.Users.Where(u => u.role_id == 1).ToList();
                var managerName = db.Users.FirstOrDefault(u => u.user_id == userId).FullName;

                SelectList employeeList = new SelectList(employees, "user_id", "FullName");

                ViewBag.Employees = employeeList;
                ViewBag.Manager = managerName;

                retModel.LayoutModel = layoutModel;

                return View(retModel);
            }

            return RedirectToAction("Dashboard");
        }

        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult saveTask(string orderName, string Description, string createdBy, int assignedTo, string dateCreated, string dueDate)
        {

            if (Session != null && Session["UserId"] != null)
            {
                if (ModelState.IsValid)
                {
                    DateTime createdate = DateTime.Parse(dateCreated).Date;
                    DateTime duedate = DateTime.Parse(dueDate).Date;
                    var createdUser = db.Users.FirstOrDefault(u => (u.first_name + " " + u.last_name) == createdBy);

                    Order order = new Order
                    {
                        order_name = orderName,
                        description = Description,
                        created_by = createdUser.user_id,
                        assigned_to = assignedTo,
                        date_created = createdate,
                        due_date = duedate,
                        status = 0
                    };

                    db.Orders.Add(order);
                    db.SaveChanges();

                    return RedirectToAction("Dashboard");
                }


                return View("addTask");
            }

            return RedirectToAction("Dashboard");
            
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult FinishTask(int orderId)
        {

            var order = db.Orders.FirstOrDefault(u => u.order_id == orderId);
            var user = db.Users.FirstOrDefault(u => u.user_id == order.created_by);

            if (order != null)
            {
                order.status = 1;
                db.SaveChanges();
            }

            return RedirectToAction("ManagerDashboard", user);
        }
        [OutputCache(NoStore = true, Duration = 0, VaryByParam = "None")]
        public ActionResult DeleteTask(int orderId)
        {
            var order = db.Orders.FirstOrDefault(u => u.order_id == orderId);
            var user = db.Users.FirstOrDefault(u => u.user_id == order.created_by);

            if(order != null)
            {
                db.Orders.Remove(order);
                db.SaveChanges();
            }

            return RedirectToAction("ManagerDashboard", user);
        }

    }
}