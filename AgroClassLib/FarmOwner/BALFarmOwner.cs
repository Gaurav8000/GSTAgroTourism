using Helper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using AgroClassLib;
using System.Data.SqlClient;

namespace AgroClassLib.FarmOwner
{
    public class BALFarmOwner : MSSQL
    {
        // =====================================================
        // FETCH TABLE LIST
        // =====================================================
        public async Task<List<ServiceManagement>> StayServicesRoomsTable()
        {
            try
            {
                Dictionary<string, string> dc = new Dictionary<string, string>();
                dc.Add("@Flag", "FetchFood");

                DataSet ds = await ExecuteStoreProcedureReturnDS("SPFarmowner", dc);
                List<ServiceManagement> list = new List<ServiceManagement>();

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        ServiceManagement obj = new ServiceManagement();

                        obj.FoodServiceCode = row["FoodServiceCode"]?.ToString();

                        // CODE (if needed later)
                        obj.MealTypeCode = row["MealTypeCode"]?.ToString();
                        obj.FoodStyleCode = row["FoodStyleCode"]?.ToString();

                        // DISPLAY NAME (for table)
                        obj.MealName = row["MealName"]?.ToString();
                        obj.FoodStyleName = row["FoodStyleName"]?.ToString();

                        obj.StartTime = row["StartTime"]?.ToString();
                        obj.EndTime = row["EndTime"]?.ToString();

                        list.Add(obj);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching food table data.", ex);
            }
        }

        // =====================================================
        // FETCH SINGLE RECORD FOR EDIT
        // =====================================================
        public async Task<ServiceManagement> FetchFood(string FoodServiceCode)
        {
            try
            {
                ServiceManagement obj = new ServiceManagement();

                Dictionary<string, string> dc = new Dictionary<string, string>();
                dc.Add("@Flag", "FetchFoodByUser");
                dc.Add("@FoodServiceCode", FoodServiceCode);
                using (SqlDataReader dr = await ExecuteStoreProcedureReturnDR("SPFarmowner", dc))
                {
                    if (dr != null && await dr.ReadAsync())
                    {
                        obj.FoodServiceCode = dr["FoodServiceCode"]?.ToString();
                        obj.MealTypeCode = dr["MealTypeCode"]?.ToString();
                        obj.FoodStyleCode = dr["FoodStyleCode"]?.ToString();
                        obj.StartTime = dr["StartTime"]?.ToString();
                        obj.EndTime = dr["EndTime"]?.ToString();
                        obj.FarmhouseCode = dr["FarmhouseCode"]?.ToString();
                        obj.FarmhouseName = dr["FarmhouseName"]?.ToString();
                        obj.ImageFile = dr["ImagePath"]?.ToString();
                    }
                }
                return obj;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching food details.", ex);
            }
        }

        // =====================================================
        // FETCH MEAL TYPE DROPDOWN
        // =====================================================
        public async Task<List<ServiceManagement>> FetchFoodTypeList()
        {
            try
            {
                Dictionary<string, string> dc = new Dictionary<string, string>();
                dc.Add("@Flag", "FetchFoodTypeList");

                DataSet ds = await ExecuteStoreProcedureReturnDS("SPFarmowner", dc);
                List<ServiceManagement> list = new List<ServiceManagement>();

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        ServiceManagement obj = new ServiceManagement();

                        obj.MealTypeCode = row["MealTypeCode"]?.ToString();
                        obj.MealName = row["MealName"]?.ToString();

                        list.Add(obj);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching food type list.", ex);
            }
        }

        // =====================================================
        // FETCH FOOD STYLE DROPDOWN
        // =====================================================
        public async Task<List<ServiceManagement>> FetchFoodStyleList()
        {
            try
            {
                Dictionary<string, string> dc = new Dictionary<string, string>();
                dc.Add("@Flag", "FetchFoodStyleList");

                DataSet ds = await ExecuteStoreProcedureReturnDS("SPFarmowner", dc);
                List<ServiceManagement> list = new List<ServiceManagement>();

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        ServiceManagement obj = new ServiceManagement();

                        obj.FoodStyleCode = row["FoodStyleCode"]?.ToString();
                        obj.FoodStyleName = row["FoodStyleName"]?.ToString();
                        list.Add(obj);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching food style list.", ex);
            }
        }
        public async Task<List<ServiceManagement>> FetchUserFarms(ServiceManagement ser)
        {
            try
            {
                Dictionary<string, string> dc = new Dictionary<string, string>();
                dc.Add("@Flag", "FetchUserFarms");
                dc.Add("@FarmOwnerCode", ser.FarmownerCode);

                DataSet ds = await ExecuteStoreProcedureReturnDS("SPFarmowner", dc);

                List<ServiceManagement> list = new List<ServiceManagement>();

                if (ds != null && ds.Tables.Count > 0)
                {
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        ServiceManagement obj = new ServiceManagement
                        {
                            FarmhouseCode = row["FarmHousecode"]?.ToString(),
                            FarmhouseName = row["FarmHouseName"]?.ToString()
                        };

                        list.Add(obj);
                    }
                }

                return list;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while fetching farms.", ex);
            }
        }
        public async Task UpdateFoodServiceTable(ServiceManagement obj)
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();

            dc.Add("@Flag", "UpdateFoodServieTable");
            dc.Add("@FoodServiceCode", obj.FoodServiceCode);
            dc.Add("@MealTypeCode", obj.MealTypeCode);
            dc.Add("@FoodStyleCode", obj.FoodStyleCode);
            dc.Add("@StartTime", obj.StartTime.ToString());
            dc.Add("@EndTime", obj.EndTime.ToString());
            dc.Add("@ImagePath", obj.ImageFile);
            await ExecuteNonQuery("SPFarmOwner", dc);
        }
        public async Task SaveFoodServiceTable(ServiceManagement obj)
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();

            dc.Add("@Flag", "InsertFoodServiceTable");
            dc.Add("@Farmhousecode", obj.FarmhouseCode);
            dc.Add("@MealTypeCode", obj.MealTypeCode);
            dc.Add("@FoodStyleCode", obj.FoodStyleCode);
            dc.Add("@StartTime", obj.StartTime.ToString());
            dc.Add("@EndTime", obj.EndTime.ToString());
            dc.Add("@ImagePath", obj.ImageFile);
            await ExecuteNonQuery("SPFarmOwner", dc);
        }
        /// <summary>
        /// ///////////////////////////////activity////////////////////////////////////////
        /// </summary>
        /// <returns></returns>
        public async Task<List<ServiceManagement>> FetchActivityTable()
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();

            dc.Add("@Flag", "FetchActivity");

            DataSet ds = await ExecuteStoreProcedureReturnDS("SPFarmOwner", dc);

            List<ServiceManagement> list = new List<ServiceManagement>();

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    ServiceManagement obj = new ServiceManagement();

                    obj.ActivityCode = row["ActivityCode"].ToString();
                    obj.FarmhouseCode = row["FarmhouseCode"].ToString();
                    obj.ActivityName = row["ActivityName"].ToString();
                    obj.Duration = row["Duration"].ToString();
                    obj.Price = Convert.ToDecimal(row["Price"]);
                    obj.StartDate = row["StartDate"].ToString();
                    obj.EndDate = row["EndDate"].ToString();
                    obj.ImageFile = row["ImagePath"].ToString();
                    obj.IsActive = Convert.ToBoolean(row["IsActive"]);
                    obj.Description = row["Description"].ToString();

                    list.Add(obj);
                }
            }

            return list;
        }

        public async Task<ServiceManagement> FetchActivity(string ActivityCode)
        {
            ServiceManagement obj = new ServiceManagement();

            Dictionary<string, string> dc = new Dictionary<string, string>();

            dc.Add("@Flag", "FetchActivityByCode");
            dc.Add("@ActivityCode", ActivityCode);

            using (SqlDataReader dr = await ExecuteStoreProcedureReturnDR("SPFarmOwner", dc))
            {
                if (dr != null && await dr.ReadAsync())
                {
                    obj.ActivityCode = dr["ActivityCode"].ToString();
                    obj.FarmhouseCode = dr["FarmhouseCode"].ToString();
                    obj.ActivityName = dr["ActivityName"].ToString();
                    obj.Duration = dr["Duration"].ToString();
                    obj.Price = Convert.ToDecimal(dr["Price"]);
                    obj.StartDate = dr["StartDate"].ToString();
                    obj.EndDate = dr["EndDate"].ToString();
                    obj.ImageFile = dr["ImagePath"].ToString();
                    obj.Description = dr["Description"].ToString();
                }
            }

            return obj;
        }
        public async Task SaveActivity(ServiceManagement obj)
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();

            dc.Add("@Flag", "InsertActivity");
            dc.Add("@FarmhouseCode", obj.FarmhouseCode);
            dc.Add("@ActivityName", obj.ActivityName);
            dc.Add("@Duration", obj.Duration);
            dc.Add("@Price", obj.Price.ToString());
            dc.Add("@StartDate", obj.StartDate);
            dc.Add("@EndDate", obj.EndDate);
            dc.Add("@ImagePath", obj.ImageFile);
            dc.Add("@Description", obj.Description);

            await ExecuteNonQuery("SPFarmOwner", dc);
        }
        public async Task UpdateActivity(ServiceManagement obj)
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();

            dc.Add("@Flag", "UpdateActivity");
            dc.Add("@ActivityCode", obj.ActivityCode);
            dc.Add("@FarmhouseCode", obj.FarmhouseCode);
            dc.Add("@ActivityName", obj.ActivityName);
            dc.Add("@Duration", obj.Duration);
            dc.Add("@Price", obj.Price.ToString());
            dc.Add("@StartDate", obj.StartDate);
            dc.Add("@EndDate", obj.EndDate);
            dc.Add("@ImagePath", obj.ImageFile);
            dc.Add("@Description", obj.Description);

            await ExecuteNonQuery("SPFarmOwner", dc);
        }

      

        // ===========================================
        // FETCH ROOMS TABLE
        // ===========================================
        public async Task<List<ServiceManagement>> FetchRoomTable()
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();

            dc.Add("@Flag", "FetchRooms");

            DataSet ds = await ExecuteStoreProcedureReturnDS("SPFarmOwner", dc);

            List<ServiceManagement> list = new List<ServiceManagement>();

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    ServiceManagement obj = new ServiceManagement();

                    obj.RoomName = row["RoomName"]?.ToString();
                    obj.RoomType = row["RoomType"]?.ToString();

                    if (row["Capacity"] != DBNull.Value)
                        obj.Capacity = Convert.ToInt32(row["Capacity"]);

                    if (row["Price"] != DBNull.Value)
                        obj.Price = Convert.ToDecimal(row["Price"]);

                    obj.ImageFile = row["ImagePath"]?.ToString();

                    list.Add(obj);
                }
            }

            return list;
        }


        // ===========================================
        // FETCH SINGLE ROOM (FOR EDIT)
        // ===========================================
        public async Task<ServiceManagement> FetchRoom(string RoomCode)
        {
            ServiceManagement obj = new ServiceManagement();

            Dictionary<string, string> dc = new Dictionary<string, string>();

            dc.Add("@Flag", "FetchRoomByCode");
            dc.Add("@RoomCode", RoomCode);

            using (SqlDataReader dr = await ExecuteStoreProcedureReturnDR("SPFarmOwner", dc))
            {
                if (dr != null && await dr.ReadAsync())
                {
                    obj.RoomCode = dr["RoomCode"]?.ToString();
                    obj.RoomType = dr["RoomType"]?.ToString();

                    if (dr["Capacity"] != DBNull.Value)
                        obj.Capacity = Convert.ToInt32(dr["Capacity"]);

                    if (dr["Price"] != DBNull.Value)
                        obj.Price = Convert.ToDecimal(dr["Price"]);

                    obj.ImageFile = dr["ImagePath"]?.ToString();
                }
            }

            return obj;
        }


        // ===========================================
        // INSERT ROOM
        // ===========================================
        public async Task SaveRoom(ServiceManagement obj)
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();

            dc.Add("@Flag", "InsertRoom");
            dc.Add("@FarmhouseCode", obj.FarmhouseCode);
            dc.Add("@RoomType", obj.RoomType);
            dc.Add("@Capacity", obj.Capacity.ToString());
            dc.Add("@Price", obj.Price.ToString());
            dc.Add("@ImagePath", obj.ImageFile);

            await ExecuteNonQuery("SPFarmOwner", dc);
        }


        // ===========================================
        // UPDATE ROOM
        // ===========================================
        public async Task UpdateRoom(ServiceManagement obj)
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();

            dc.Add("@Flag", "UpdateRoom");
            dc.Add("@RoomCode", obj.RoomCode);
            dc.Add("@RoomType", obj.RoomType);
            dc.Add("@Capacity", obj.Capacity.ToString());
            dc.Add("@Price", obj.Price.ToString());
            dc.Add("@ImagePath", obj.ImageFile);

            await ExecuteNonQuery("SPFarmOwner", dc);
        }
        // ===========================================
        // FETCH ROOM TYPE LIST
        // ===========================================
        public async Task<List<ServiceManagement>> FetchRoomTypeList()
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();

            dc.Add("@Flag", "FetchRoomTypeList");

            DataSet ds = await ExecuteStoreProcedureReturnDS("SPFarmOwner", dc);

            List<ServiceManagement> list = new List<ServiceManagement>();

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    ServiceManagement obj = new ServiceManagement();

                    obj.RoomTypeCode = row["RoomTypeCode"]?.ToString();
                    obj.RoomTypeName = row["RoomTypeName"]?.ToString();

                    list.Add(obj);
                }
            }

            return list;
        }

    }
}