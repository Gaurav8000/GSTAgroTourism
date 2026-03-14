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
        //login
        public async Task<DataSet> Login(LoginRS model)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("@Flag", "LOGIN");
            param.Add("@Email", model.UserEmail);
            param.Add("@Password", model.UserPassword);

            return await ExecuteStoreProcedureReturnDS("SPFarmowner", param);
        }
        // =====================================================
        // FETCH TABLE LIST
        // =====================================================
        public async Task<List<ServiceManagement>> FoodServiceTable(string FarmOwnerCode)
        {
            try
            {
                Dictionary<string, string> dc = new Dictionary<string, string>();
                dc.Add("@Flag", "FetchFood");
                dc.Add("@FarmOwnerCode", FarmOwnerCode);


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
                        obj.FarmhouseCode = row["FarmhouseCode"]?.ToString();

                        // DISPLAY NAME (for table)
                        obj.MealName = row["MealName"]?.ToString();
                        obj.FoodStyleName = row["FoodStyleName"]?.ToString();
                        obj.StartTime = TimeSpan.Parse(row["StartTime"].ToString());
                        obj.EndTime = TimeSpan.Parse(row["EndTime"].ToString());
                        obj.FarmhouseName = row["FarmhouseName"]?.ToString();

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
                        obj.MealName = dr["MealName"]?.ToString();
                        obj.FoodStyleCode = dr["FoodStyleCode"]?.ToString();
                        obj.FoodStyleName = dr["FoodStyleName"]?.ToString();
                        obj.StartTime = TimeSpan.Parse(dr["StartTime"].ToString());
                        obj.EndTime = TimeSpan.Parse(dr["EndTime"].ToString());
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
            dc.Add("@StartTime", obj.StartTime.ToString(@"hh\:mm\:ss"));
            dc.Add("@EndTime", obj.EndTime.ToString(@"hh\:mm\:ss"));
            dc.Add("@ImagePath", obj.ImageFile ?? "");

            await ExecuteNonQuery("SPFarmOwner", dc);
        }


        // ===========================================
        // FETCH ROOMS TABLE
        // ===========================================
        public async Task<List<ServiceManagement>> FetchRoomTable(string FarmhouseCode)
        {
            Dictionary<string, string> dc = new Dictionary<string, string>();

            dc.Add("@Flag", "FetchRooms");
            dc.Add("@FarmhouseCode", FarmhouseCode);


            DataSet ds = await ExecuteStoreProcedureReturnDS("SPFarmOwner", dc);

            List<ServiceManagement> list = new List<ServiceManagement>();

            if (ds != null && ds.Tables.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    ServiceManagement obj = new ServiceManagement();

                    obj.RoomName = row["RoomName"]?.ToString();
                    obj.RoomCode = row["FarmRoomCode"]?.ToString();
                    obj.RoomType = row["RoomTypeName"]?.ToString();
                    obj.FarmhouseName = row["FarmHouseName"]?.ToString();

                    if (row["NumberOfGuests"] != DBNull.Value)
                        obj.Capacity = Convert.ToInt32(row["NumberOfGuests"]);

                    if (row["PricePerNight"] != DBNull.Value)
                        obj.Price = Convert.ToDecimal(row["PricePerNight"]);

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

            dc.Add("@Flag", "FetchRoomsById");
            dc.Add("@FarmRoomCode", RoomCode);

            using (SqlDataReader dr = await ExecuteStoreProcedureReturnDR("SPFarmOwner", dc))
            {
                if (dr != null && await dr.ReadAsync())
                {
                    obj.RoomCode = dr["FarmRoomCode"]?.ToString();
                    obj.RoomType = dr["RoomTypeName"]?.ToString();
                    obj.RoomTypeCode = dr["RoomTypeCode"]?.ToString();
                    obj.RoomName = dr["RoomName"]?.ToString();
                    obj.RoomTypeName= dr["RoomTypeName"]?.ToString();
                    obj.FarmhouseName = dr["FarmHouseName"]?.ToString();
                    obj.Capacity = Convert.ToInt32(dr["NumberOfGuests"]);
                    obj.Price = Convert.ToDecimal(dr["PricePerNight"]);
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
            dc.Add("RoomTypeCode", obj.RoomTypeCode);
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
            dc.Add("@FarmRoomCode", obj.RoomCode);
            dc.Add("@RoomName", obj.RoomName);
            dc.Add("RoomTypeCode", obj.RoomTypeCode);
            dc.Add("@NumberOfGuests", obj.Capacity.ToString());
            dc.Add("@PricePerNight", obj.Price.ToString());
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