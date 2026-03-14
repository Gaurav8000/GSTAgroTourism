using AgroClassLib.FarmOwner;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
public class FarmOwnerController : Controller
{
    BALFarmOwner objbal = new BALFarmOwner();
    public ActionResult Index()
    {
        return View();
    }


    [HttpGet]
    public ActionResult Login()
    {
        return View();
    }
    [HttpPost] //login 

    public async Task<ActionResult> Login(LoginRS model)
    {

        DataSet ds = await objbal.Login(model);

        if (ds.Tables[0].Rows.Count > 0)
        {
            // OWNER
            Session["UserId"] = ds.Tables[0].Rows[0]["UserId"];
            Session["Email"] = ds.Tables[0].Rows[0]["Email"];
            Session["OwnerCode"] = ds.Tables[0].Rows[0]["FarmOwnerCode"];
            Session["OwnerName"] = ds.Tables[0].Rows[0]["FullName"].ToString();

            return RedirectToAction("ShowFoodServicesMealsTable", "FarmOwner");
        }
        else if (ds.Tables.Count > 1 && ds.Tables[1].Rows.Count > 0)
        {
            // VISITOR
            Session["UserId"] = ds.Tables[1].Rows[0]["UserId"];
            Session["Email"] = ds.Tables[1].Rows[0]["Email"];
            Session["VisitorCode"] = ds.Tables[1].Rows[0]["VisitorCode"];

            return RedirectToAction("Dashboard", "Visitor");
        }
        else
        {
            ViewBag.Message = "Invalid Email or Password";
            return View("Index");
        }
    }
    // ==============================
    // SHOW TABLE
    // =====

    // =========================
    public async Task<ActionResult> ShowFoodServicesMealsTable()
    {
      string  FarmOwnerCode = Session["OwnerCode"].ToString();
        await LoadDropdowns();
        ViewBag.ActiveTab = "Food";
        var list = await objbal.FoodServiceTable(FarmOwnerCode);
        return View(list);
    }
    // ==============================
    // MODAL (ADD / EDIT)
    // ==============================
    public async Task<ActionResult> FoodServiceModal(string id)
    {
        ServiceManagement model = new ServiceManagement();

        if (!string.IsNullOrEmpty(id))
        {
            model = await objbal.FetchFood(id);

            if (model == null)
                return HttpNotFound();
        }

        await LoadDropdowns(model);

        return PartialView("_FoodServiceModal", model);
    }

    // ==============================
    // SAVE OR UPDATE
    // ==============================
    [HttpPost]
    public async Task<ActionResult> SaveorEditFood(ServiceManagement model, HttpPostedFileBase ImageUpload)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Model state invalid" });
            }

            if (ImageUpload != null && ImageUpload.ContentLength > 0)
            {
                string fileName = Path.GetFileName(ImageUpload.FileName);

                string path = Server.MapPath("~/Content/img/");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string fullPath = Path.Combine(path, fileName);

                ImageUpload.SaveAs(fullPath);

                model.ImageFile = "~/Content/img/" + fileName;
            }

            if (string.IsNullOrEmpty(model.FoodServiceCode))
            {
                await objbal.SaveFoodServiceTable(model);
            }
            else
            {
                await objbal.UpdateFoodServiceTable(model);
            }

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message,
                stack = ex.StackTrace
            });
        }
    }

    // ==============================
    // LOAD DROPDOWNS
    // ==============================
    private async Task LoadDropdowns(ServiceManagement model = null)
    {
      string  FarmOwnerCode = Session["OwnerCode"].ToString();

        var mealList = await objbal.FetchFoodTypeList();
        var foodList = await objbal.FetchFoodStyleList();

        ServiceManagement ser = new ServiceManagement();
        ser.FarmownerCode = FarmOwnerCode;

        var farmList = await objbal.FetchUserFarms(ser);

        ViewBag.MealTypeList = new SelectList(
            mealList,
            "MealTypeCode",
            "MealName",
            model?.MealTypeCode
        );

        ViewBag.FoodTypeList = new SelectList(
            foodList,
            "FoodStyleCode",
            "FoodStyleName",
            model?.FoodStyleCode
        );

        ViewBag.FarmList = new SelectList(
            farmList,
            "FarmhouseCode",
            "FarmhouseName",
            model?.FarmhouseCode
        );
        var roomTypeList = await objbal.FetchRoomTypeList();

        ViewBag.RoomTypeList = new SelectList(
            roomTypeList,
            "RoomTypeCode",
            "RoomTypeName",
            model?.RoomTypeCode
        );
    }

    //////////////////////////////Meal End//////////////////////////////
   

    // ==============================
    // SHOW ROOM TABLE
    // ==============================
    public async Task<ActionResult> ShowRoomTable()
    {
        string FarmOwnerCode = Session["OwnerCode"].ToString();
        ViewBag.ActiveTab = "Room";
        await LoadDropdowns();
        var list = await objbal.FetchRoomTable(FarmOwnerCode);
        return View(list);
    }

    // ==============================
    // ROOM MODAL (ADD / EDIT)
    // ==============================
    public async Task<ActionResult> RoomModal(string id)
    {
        ServiceManagement model = new ServiceManagement();

        if (!string.IsNullOrEmpty(id))
        {
            model = await objbal.FetchRoom(id);

            if (model == null)
                return HttpNotFound();
        }

        await LoadRoomDropdown(model);

        return PartialView("RoomModal", model);
    }


    // ==============================
    // SAVE OR UPDATE ROOM
    // ==============================
    [HttpPost]
    [ValidateAntiForgeryToken]
    //Add and Edit Room Controller
    public async Task<ActionResult> SaveRoom(ServiceManagement model, HttpPostedFileBase ImageUpload)
    {
        try
        {
            if (ImageUpload != null && ImageUpload.ContentLength > 0)
            {
                string fileName = Guid.NewGuid().ToString() +
                                  System.IO.Path.GetExtension(ImageUpload.FileName);

                string path = Server.MapPath("~/Content/img/");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string fullPath = Path.Combine(path, fileName);

                ImageUpload.SaveAs(fullPath);

                model.ImageFile = "~/Content/img/" + fileName;
            }

            if (string.IsNullOrEmpty(model.RoomCode))
            {
                await objbal.SaveRoom(model);
            }
            else
            {
                await objbal.UpdateRoom(model);
            }

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    private async Task LoadRoomDropdown(ServiceManagement model = null)
    {
        var roomTypeList = await objbal.FetchRoomTypeList();

        ViewBag.RoomTypeList = new SelectList(
            roomTypeList,
            "RoomTypeCode",
            "RoomTypeName",
            model?.RoomTypeCode
        );
    }
    public async Task<ActionResult> FoodServiceView(string id)
    {
        ServiceManagement model = new ServiceManagement();

        if (!string.IsNullOrEmpty(id))
        {
            model = await objbal.FetchFood(id);

            if (model == null)
                return HttpNotFound();
        }

        await LoadDropdowns(model);

        return PartialView("FoodServiceView", model);
    }
    public async Task<ActionResult> RoomModalView(string id)
    {
        ServiceManagement model = new ServiceManagement();

        if (!string.IsNullOrEmpty(id))
        {
            model = await objbal.FetchRoom(id);

            if (model == null)
                return HttpNotFound();
        }

        await LoadDropdowns(model);

        return PartialView("RoomModalView", model);
    }
}