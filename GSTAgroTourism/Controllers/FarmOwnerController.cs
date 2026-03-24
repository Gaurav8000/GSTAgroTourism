using AgroClassLib.FarmOwner;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
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

            return RedirectToAction("ShowFoodServicesMealsTableGS", "FarmOwner");
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
    #region Gaurav
    // ==============================
    // SHOW TABLE
    // ==============================
    public async Task<ActionResult> ShowFoodServicesMealsTableGS()
    {
        string FarmOwnerCode = Session["OwnerCode"].ToString();
        await LoadDropdownsGS();
        ViewBag.ActiveTab = "Food";
        var list = await objbal.FoodServiceTableGS(FarmOwnerCode);
        return View(list);
    }
    // ==============================
    // MODAL (ADD / EDIT)
    // ==============================
    public async Task<ActionResult> FoodServiceModalGS(string id)
    {
        FoodServiceVM model = new FoodServiceVM();

        if (!string.IsNullOrEmpty(id))
        {
            var data = await objbal.FetchFoodGS(id);

            model.FoodServiceCode = data.FoodServiceCode;
            model.MealTypeCode = data.MealTypeCode;
            model.FoodStyleCode = data.FoodStyleCode;
            model.FarmhouseCode = data.FarmhouseCode;
            model.StartTime = data.StartTime;
            model.EndTime = data.EndTime;
            model.ImageFile = data.ImageFile;
        }

        await LoadDropdownsGS();
        return PartialView("FoodServiceModalGS", model);
    }

    // ==============================
    // SAVE OR UPDATE
    // ==============================
    [HttpPost]
    public async Task<ActionResult> SaveorEditFoodGS(FoodServiceVM model, HttpPostedFileBase ImageUpload)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Model state invalid" });
            }

            ServiceManagement obj = new ServiceManagement();

            obj.FoodServiceCode = model.FoodServiceCode;
            obj.MealTypeCode = model.MealTypeCode;
            obj.FoodStyleCode = model.FoodStyleCode;
            obj.FarmhouseCode = model.FarmhouseCode;
            obj.StartTime = model.StartTime;
            obj.EndTime = model.EndTime;
            obj.ImageFile = model.ImageFile;

            if (ImageUpload != null && ImageUpload.ContentLength > 0)
            {
                string fileName = Path.GetFileName(ImageUpload.FileName);

                string path = Server.MapPath("~/Content/img/");

                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                string fullPath = Path.Combine(path, fileName);

                ImageUpload.SaveAs(fullPath);

                obj.ImageFile = "~/Content/img/" + fileName;
            }

            if (string.IsNullOrEmpty(model.FoodServiceCode))
            {
                await objbal.SaveFoodServiceTableGS(obj);
            }
            else
            {
                await objbal.UpdateFoodServiceTableGS(obj);
            }

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new
            {
                success = false,
                message = ex.Message
            });
        }
    }


    // ==============================
    // LOAD DROPDOWNS
    // ==============================
    private async Task LoadDropdownsGS(ServiceManagement model = null)
    {
        string FarmOwnerCode = Session["OwnerCode"].ToString();

        var mealList = await objbal.FetchFoodTypeListGS();
        var foodList = await objbal.FetchFoodStyleListGS();

        ServiceManagement ser = new ServiceManagement();
        ser.FarmownerCode = FarmOwnerCode;

        var farmList = await objbal.FetchUserFarmsGS(ser);

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
        var roomTypeList = await objbal.FetchRoomTypeListGS();

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

    public async Task<ActionResult> ShowRoomTableGS()
    {
        string FarmOwnerCode = Session["OwnerCode"].ToString();
        ViewBag.ActiveTab = "Room";
        await LoadDropdownsGS();
        var list = await objbal.FetchRoomTableGS(FarmOwnerCode);
        return View(list);
    }

    // ==============================
    // ROOM MODAL (ADD / EDIT)
    // ==============================
    public async Task<ActionResult> RoomModalGS(string id)
    {
        ServiceManagement model = new ServiceManagement();

        if (!string.IsNullOrEmpty(id))
        {
            model = await objbal.FetchRoomGS(id);

            if (model == null)
                return HttpNotFound();
        }

        await LoadRoomDropdownGS(model);

        return PartialView("RoomModalGS", model);
    }


    // ==============================
    // SAVE OR UPDATE ROOM
    // ==============================
    [HttpPost]
    [ValidateAntiForgeryToken]
    //Add and Edit Room Controller
    public async Task<ActionResult> SaveRoomGS(ServiceManagement model, HttpPostedFileBase ImageUpload)
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
                await objbal.SaveRoomGS(model);
            }
            else
            {
                await objbal.UpdateRoomGS(model);
            }

            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    private async Task LoadRoomDropdownGS(ServiceManagement model = null)
    {
        var roomTypeList = await objbal.FetchRoomTypeListGS();

        ViewBag.RoomTypeList = new SelectList(
            roomTypeList,
            "RoomTypeCode",
            "RoomTypeName",
            model?.RoomTypeCode
        );
    }
    public async Task<ActionResult> FoodServiceViewGS(string id)
    {
        ServiceManagement model = new ServiceManagement();

        if (!string.IsNullOrEmpty(id))
        {
            model = await objbal.FetchFoodGS(id);

            if (model == null)
                return HttpNotFound();
        }

        await LoadDropdownsGS(model);

        return PartialView("FoodServiceViewGS", model);
    }
    public async Task<ActionResult> RoomModalViewGS(string id)
    {
        ServiceManagement model = new ServiceManagement();

        if (!string.IsNullOrEmpty(id))
        {
            model = await objbal.FetchRoomGS(id);

            if (model == null)
                return HttpNotFound();
        }

        await LoadDropdownsGS(model);

        return PartialView("RoomModalViewGS", model);
    }
    #endregion
}
