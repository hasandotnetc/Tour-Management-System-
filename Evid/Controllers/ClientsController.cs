using Evid.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Evid.Controllers
{
    public class ClientsController : Controller
    {
        private EvEntities db = new EvEntities();
        public ActionResult Index()
        {
            var clients = db.Clients.Include (x=>x.BookingEnties.Select(b=>b.Spot)).OrderByDescending(x=>x.ClientId).ToList();
            return View(clients);
        }
        public ActionResult AddNewSpot(int?id)
        {
            ViewBag.spots = new SelectList(db.Spots.ToList(), "SpotId", "SpotName", (id != null) ? id.ToString() : "");
            return PartialView("_addNewSpot");
        }
        public ActionResult Create()
        {

            return View();
        }

        [HttpPost]

        public ActionResult Create(clientVM clientVM, int[] spotId)
        {
            if (ModelState.IsValid)
            {
                Client client = new Client()
                {
                    ClientName = clientVM.ClientName,
                    BirthDate = clientVM.BirthDate,
                    Age = clientVM.Age,
                    MaritalStatus = clientVM.MaritalStatus

                };

                HttpPostedFileBase file = clientVM.PictureFile;

                if (file != null)
                {
                    string filePath = Path.Combine("/Images/", Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));

                    file.SaveAs(Server.MapPath(filePath));
                    client.Picture = filePath;

                }

                //Save All Spot From SpotId
                foreach (var item in spotId)
                {
                    BookingEnty bookingEntry = new BookingEnty()
                    {
                        Client = client,
                        ClientId = client.ClientId,
                        SpotId = item,
                    };
                    db.BookingEnties.Add(bookingEntry);

                }
                db.SaveChanges();
                return PartialView("_success");

            }
            return PartialView("_error");

        }

        public ActionResult Edit(int? id)
        {
            Client client = db.Clients.First(x => x.ClientId == id);
            var clientSpot = db.BookingEnties.Where(x => x.ClientId == id).ToList();

            clientVM clientVM = new clientVM()
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                Age = (int)client.Age,
                BirthDate = (DateTime)client.BirthDate,
                Picture = client.Picture,
                MaritalStatus = (bool)client.MaritalStatus
            };
            Session["imgPath"] = client.Picture;

            if (clientSpot.Count() > 0)
            {
                foreach (var item in clientSpot)
                {
                    clientVM.SpotList.Add((int)item.SpotId);
                }
            }
            return View(clientVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(clientVM clientVM, int[] spotId)
        {
            if (ModelState.IsValid)
            {

                Client client = new Client()
                {
                    ClientId = clientVM.ClientId,
                    ClientName = clientVM.ClientName,
                    BirthDate = clientVM.BirthDate,
                    Age = clientVM.Age,
                    MaritalStatus = clientVM.MaritalStatus
                };
                HttpPostedFileBase file = clientVM.PictureFile;
                string filePath = clientVM.Picture;
                if (file != null)
                {
                    filePath = Path.Combine("/Images/", Guid.NewGuid().ToString() + Path.GetExtension(file.FileName));
                    file.SaveAs(Server.MapPath(filePath));
                    client.Picture = filePath;
                }
                else
                {
                    client.Picture = Session["imgPath"].ToString();
                }
                var spotEntry = db.BookingEnties.Where(x => x.ClientId == client.ClientId).ToList();

                foreach (var bookingEntry in spotEntry)
                {
                    db.BookingEnties.Remove(bookingEntry);
                }

                foreach (var item in spotId)
                {
                    BookingEnty bookingEntry = new BookingEnty()
                    {
                        ClientId = client.ClientId,
                        SpotId = item
                    };
                    db.BookingEnties.Add(bookingEntry);
                }
                db.Entry(client).State = EntityState.Modified;
                db.SaveChanges();
                return PartialView("_success");
            }
            return PartialView("_error");
        }
        public ActionResult Delete(int? id)
        {
            Client client = db.Clients.First(x => x.ClientId == id);

            var clientSpots = db.BookingEnties.Where(x => x.ClientId == id).ToList();

            clientVM clientVM = new clientVM()
            {
                ClientId = client.ClientId,
                ClientName = client.ClientName,
                Age =(int) client.Age,
                BirthDate =(DateTime) client.BirthDate,
                Picture = client.Picture,
                MaritalStatus = (bool)client.MaritalStatus
            };
            if (clientSpots.Count > 0)
            {
                foreach (var item in clientSpots)
                {
                    clientVM.SpotList.Add((int)item.SpotId);
                }
            }
            return View(clientVM);
        }

        [HttpPost, ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public ActionResult DoDelete(int id)
        {
            Client client = db.Clients.Find(id);
            if (client == null)
            {
                return HttpNotFound();
            }
            db.Entry(client).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}