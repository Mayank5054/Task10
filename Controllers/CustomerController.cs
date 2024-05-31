using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task10.Models;

namespace Task10.Controllers
{
    public class CustomerController : Controller
    {
        ExamEntities _db = new ExamEntities();

        public ActionResult Index()
        {
            return View();
        }


        public ActionResult Details()
        {

            string role = (string)Session["Type"];

            var data = _db.Orders.ToList();
            string email = Convert.ToString(Session["Email"]);
            User u = _db.Users.FirstOrDefault(x => x.Email == email);
            if (role == "admin")
            {
                List<sp_orderDetails_Result> details = _db.sp_orderDetails(null).ToList();
                return Json(details, JsonRequestBehavior.AllowGet);
            }
            else
            {
                List<sp_orderDetails_Result> details = _db.sp_orderDetails(u.UserId).ToList();
                return Json(details, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult RefundHistory()
        {
            return View();
        }
        public ActionResult RefundHistoryDetails()
        {



            int totalRecord = 0;

            string search = Request.Form.GetValues("search[value]")[0];

            string draw = Request.Form.GetValues("draw")[0];
            string order = "0";

            if (Request.Form.GetValues("order[0][column]")?[0] != null)
            {
                order = Request.Form.GetValues("order[0][column]")[0];
            }
            string orderDir = "asc";
            if (Request.Form.GetValues("order[0][dir]")?[0] != null)
            {
                orderDir = Request.Form.GetValues("order[0][dir]")[0];
            }
            int startRec = Convert.ToInt32(Request.Form.GetValues("start")[0]);
            int pageSize = Convert.ToInt32(Request.Form.GetValues("length")[0]);
            string startDateString = Request["startDate"];
            string endDateString = Request["endDate"];



            DateTime startDate;
            DateTime endDate;


            var data = _db.OrderRefunds.ToList();
            string email = Convert.ToString(Session["Email"]);
            User u = _db.Users.FirstOrDefault(x => x.Email == email);
            Order o = _db.Orders.FirstOrDefault(x => x.CustomerId == u.UserId);
            List<sp_orderRefund_Result> refund;
            string role = (string)Session["Role"];

            if (role == "admin")
            {
                refund = _db.sp_orderRefund(null, null).ToList();

            }
            else
            {
                refund = _db.sp_orderRefund(u.UserId, 0).ToList();
                int IsDelete = 0;

            }


            // if end date is null then get current date 

            if (DateTime.TryParse(startDateString, out startDate))
            {
                if (!DateTime.TryParse(endDateString, out endDate))
                {
                    endDate = DateTime.Now;
                }
                refund = refund.Where(x => x.DateOfOrder >= startDate && x.DateOfOrder <= endDate).ToList();
            }

            totalRecord = refund.Count();


            if (!string.IsNullOrEmpty(search) && !string.IsNullOrWhiteSpace(search))
            {
                refund = (from t in refund
                          where t.OrderType.ToString().ToLower().Contains(search.ToString().ToLower())
                          //|| t.Name.ToString().ToLower().Contains(search.ToString().ToLower())
                          //|| t.ItemType.ToString().ToLower().Contains(search.ToString().ToLower())
                          //|| t.TaxPercentage.ToString().ToLower().Contains(search.ToString().ToLower())
                          //|| t.Discount.ToString().ToLower().Contains(search.ToString().ToLower())
                          || t.TotalAmount.ToString().ToLower().Contains(search.ToString().ToLower())
                          || t.DateOfOrder.HasValue && t.DateOfOrder.Value.ToString("dd/MM/yyyy").ToLower().Contains(search.ToString().ToLower())
                          || t.RefundAmount.ToString().ToLower().Contains(search.ToString().ToLower())
                          || t.Status.ToString().ToLower().Contains(search.ToString().ToLower())
                          select t).ToList();
            }

            refund = this.SortByColumnWithOrder(order, orderDir, refund);


            int recFilter = refund.Count;

            refund = refund.Skip(startRec).Take(pageSize).ToList();





            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DateFormatString = "dd/MM/yyyy"
            };


            var result = new
            {
                draw = Convert.ToInt32(draw),
                recordsTotal = totalRecord,
                recordsFiltered = recFilter,
                data = refund
            };
            string resultJson = JsonConvert.SerializeObject(result, settings);
            return Content(resultJson, "application/json");



        }


        private List<sp_orderRefund_Result> SortByColumnWithOrder(string order, string orderDir, List<sp_orderRefund_Result> data)
        {

            List<sp_orderRefund_Result> lst = new List<sp_orderRefund_Result>();
            try
            {
                // Sorting   
                switch (order)
                {
                    case "0":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.OrderType).ToList() : data.OrderBy(p => p.OrderType).ToList();
                        break;
                    case "1":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.TotalAmount).ToList() : data.OrderBy(p => p.TotalAmount).ToList();
                        break;
                    case "2":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.DateOfOrder).ToList() : data.OrderBy(p => p.DateOfOrder).ToList();
                        break;
                    case "3":

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.Items).ToList() : data.OrderBy(p => p.Items).ToList();
                        break;

                    default:

                        lst = orderDir.Equals("DESC", StringComparison.CurrentCultureIgnoreCase) ? data.OrderByDescending(p => p.OrderType).ToList() : data.OrderBy(p => p.OrderType).ToList();
                        break;
                }
            }
            catch (Exception ex)
            {

                Console.Write(ex);
            }

            return lst;
        }





        public ActionResult RequestForRefund(int? id)
        {

            string email = Convert.ToString(Session["Email"]);
            User u = _db.Users.FirstOrDefault(x => x.Email == email);

            Order o = _db.Orders.FirstOrDefault(x => x.OrderId == id);
            OrderItem item = _db.OrderItems.FirstOrDefault(x => x.OrderId == id);

            List<OrderRefund> orderRefund = _db.OrderRefunds.Where(x => x.OrderId == id).ToList();
            foreach (OrderRefund orderRefundItem in orderRefund)
            {
                if (orderRefund != null && orderRefundItem.Status == "refunded")
                {
                    return Json(new { success = false }, JsonRequestBehavior.AllowGet);
                }
            }


            OrderRefund refund = new OrderRefund();
            refund.OrderId = o.OrderId;
            refund.Status = "Applied";
            refund.OrderItemId = item.ItemID;
            refund.CreatedOn = DateTime.Now;
            refund.UpdatedOn = DateTime.Now;
            refund.IsDeleted = 0;
            refund.CreatedBy = u.UserId;
            refund.Amount = 0;
            refund.Percentage = 0;
            _db.OrderRefunds.Add(refund);
            _db.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }


        public ActionResult DeleteRequest(int? id)
        {
            OrderRefund or = _db.OrderRefunds.FirstOrDefault(x => x.OrderRefundId == id);
            or.IsDeleted = 1;

            _db.OrderRefunds.AddOrUpdate(or);
            _db.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }
    }
}