using Task10.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Task10.ViewModels;
using System.Data.Entity;


namespace ExamTask.Controllers
{
    public class StockController : Controller
    {
        ExamEntities db = new ExamEntities();


        // Constructor without dependency injection
        public StockController()
        {
        }

        //Action to display stock report
        public ActionResult StockReport()
        {

            var user = Session["UserID"];
            if (user != null)
            {
                var stockReport = db.Database.SqlQuery<StockReportViewModel>("EXEC GetStockReport").ToList();
                return View("StockReport", stockReport);
            }
            else
            {
                return RedirectToAction("Login", "Home");
            }

        }
    }
}




//Stored Procedure inside database

//create procedure getstockreport
//as
//begin
//select 

//p.name as productname,

//sum(case when s.type = 'in' then s.qty else 0 end) as totalstock,

//sum(case when s.type = 'in' then s.qty else 0 end) - sum(case when s.type = 'out' then s.qty else 0 end) - isnull(sum(o.quantity), 0) as remstock,

//case 
//when sum(case when s.type = 'in' then s.qty else 0 end) - sum(case when s.type = 'out' then s.qty else 0 end) - isnull(sum(o.quantity), 0) < p.threshold 
//then 'yes' 
//else 'no' 
//end as islowkey

//from 
//products p
//join 
//stocks s on p.productid = s.itemid
//left join 
//orderitems o on p.productid = o.itemid
//where 
//p.type = 'purchasable'
//group by 
//p.productid, p.name, p.threshold
//end



