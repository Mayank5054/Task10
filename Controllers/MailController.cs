using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace Task10.Controllers
{
    public class MailController : Controller
    {
        // GET: Mail
        [HttpPost]
        public ActionResult Index(string data)
        {
            string email = Session["Email"].ToString();
            var fromAddress = new MailAddress("mayanksheladiya4448@gmail.com", "Mayank Sheladiya");
            var toAddress = new MailAddress(email);
            const string fromPassword = "reah fqwr azvj dbdz";

            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromAddress.Address, fromPassword)
            };
            using (var message = new MailMessage(fromAddress, toAddress)
            {
                Subject = "Receipt For Order",
                Body = data,
                IsBodyHtml = true // Set to true if the body is HTML
            })
            {
                smtp.Send(message);
            }
            return Json(new { status="success",message="Email Has Been Sent"},JsonRequestBehavior.AllowGet);
        }
    }
}