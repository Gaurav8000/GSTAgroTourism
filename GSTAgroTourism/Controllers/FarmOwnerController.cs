using AgroClassLib.FarmOwner;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

public class FarmOwnerController : Controller
{
    BALFarmOwner objbal = new BALFarmOwner();

    // ==============================
    // SHOW TABLE
    // ==============================
    public async Task<ActionResult> ShowFoodServicesMealsTable()
    {
        var list = await objbal.StayServicesRoomsTable();
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
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> SaveorEditFood(ServiceManagement model, HttpPostedFileBase ImageUpload)
    {
        if (!ModelState.IsValid)
        {
            await LoadDropdowns(model);
            return PartialView("_FoodServiceModal", model);
        }

        // ==============================
        // IMAGE UPLOAD
        // ==============================
        if (ImageUpload != null && ImageUpload.ContentLength > 0)
        {
            string fileName = Guid.NewGuid().ToString() +
                              System.IO.Path.GetExtension(ImageUpload.FileName);

            string path = Server.MapPath("~/Content/img/");

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            string fullPath = System.IO.Path.Combine(path, fileName);

            ImageUpload.SaveAs(fullPath);

            model.ImageFile = "~/Content/img/" + fileName;
        }

        // ==============================
        // INSERT OR UPDATE
        // ==============================
        if (string.IsNullOrEmpty(model.FoodServiceCode))
        {
            await objbal.SaveFoodServiceTable(model);
        }
        else
        {
            await objbal.UpdateFoodServiceTable(model);
        }

        // AJAX SUCCESS RESPONSE
        return Json(new { success = true });
    }

    // ==============================
    // LOAD DROPDOWNS
    // ==============================
    private async Task LoadDropdowns(ServiceManagement model = null)
    {
        string farmOwnerCode = "FW001"; // from session normally

        var mealList = await objbal.FetchFoodTypeList();
        var foodList = await objbal.FetchFoodStyleList();

        ServiceManagement ser = new ServiceManagement();
        ser.FarmownerCode = farmOwnerCode;

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
    }

    //////////////////////////////Meal End//////////////////////////////
    public async Task<ActionResult> ShowActivityTable()
{
    var list = await objbal.FetchActivityTable();

    return View(list);
}
    public async Task<ActionResult> ActivityModal(string id)
    {
        ServiceManagement model = new ServiceManagement();

        if (!string.IsNullOrEmpty(id))
        {
            model = await objbal.FetchActivity(id);
        }

        await LoadDropdowns(model);

        return PartialView("ActivityModal", model);
    }
    [HttpPost]
    public async Task<ActionResult> SaveActivity(ServiceManagement model,
HttpPostedFileBase ImageUpload)
    {
        if (ImageUpload != null)
        {
            string fileName = Guid.NewGuid().ToString()
            + System.IO.Path.GetExtension(ImageUpload.FileName);

            string path = Server.MapPath("~/Content/img/");

            ImageUpload.SaveAs(System.IO.Path.Combine(path, fileName));

            model.ImageFile = "~/Content/img/" + fileName;
        }

        if (string.IsNullOrEmpty(model.ActivityCode))
        {
            await objbal.SaveActivity(model);
        }
        else
        {
            await objbal.UpdateActivity(model);
        }

        return Json(new { success = true });
    }


    // ==============================
    // SHOW ROOM TABLE
    // ==============================
    public async Task<ActionResult> ShowRoomTable()
    {
        var list = await objbal.FetchRoomTable();
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

        await LoadDropdowns(model);

        return PartialView("_RoomModal", model);
    }


    // ==============================
    // SAVE OR UPDATE ROOM
    // ==============================
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> SaveRoom(ServiceManagement model,
    HttpPostedFileBase ImageUpload)
    {

        if (ImageUpload != null && ImageUpload.ContentLength > 0)
        {

            string fileName = Guid.NewGuid().ToString()
            + System.IO.Path.GetExtension(ImageUpload.FileName);

            string path = Server.MapPath("~/Content/img/");

            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);

            string fullPath = System.IO.Path.Combine(path, fileName);

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

}

