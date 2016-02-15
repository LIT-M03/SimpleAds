using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SimpleAds.Data;
using SimpleAds.Models;

namespace SimpleAds.Controllers
{
    public class AdsController : Controller
    {
        public ActionResult Index()
        {
            var manager = new SimpleAdManager(SimpleAds.Properties.Settings.Default.ConnectionString);
            return View(manager.GetAdsWithSingleImages());
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult New(Ad ad, HttpPostedFileBase[] images)
        {
            var manager = new SimpleAdManager(SimpleAds.Properties.Settings.Default.ConnectionString);
            ad.Images = images.Where(i => i != null).Select(i =>
            {
                Guid g = Guid.NewGuid();
                string fileName = g + ".jpg";
                i.SaveAs(Server.MapPath("~/Uploads/" + fileName));
                return fileName;
            });
            manager.AddAd(ad);

            //3,6,10

            List<string> adIds;
            if (Request.Cookies["adIds"] != null)
            {
                adIds = Request.Cookies["adIds"].Value.Split(',').ToList();
            }
            else
            {
                adIds = new List<string>();
            }

            adIds.Add(ad.Id.ToString());
            var cookie = new HttpCookie("adIds", String.Join(",", adIds));
            Response.Cookies.Add(cookie);

            return RedirectToAction("Index");

        }

        public ActionResult Show(int id)
        {
            var manager = new SimpleAdManager(SimpleAds.Properties.Settings.Default.ConnectionString);
            Ad ad = manager.GetAdById(id);
            var viewModel = new ShowAdViewModel
            {
                Ad = ad
            };
            if (Request.Cookies["adIds"] != null)
            {
                List<string> adIds = Request.Cookies["adIds"].Value.Split(',').ToList();
                viewModel.ShowDeleteButton = adIds.Any(a => ad.Id.ToString() == a);
            }
            return View(viewModel);
        }

    }


}
