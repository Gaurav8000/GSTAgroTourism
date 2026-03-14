using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace AgroClassLib.FarmOwner
{

    #region Gaurav
    public class ServiceManagement
    {
        public int UserId { get; set; }

        // ===== Food Service =====


        public string FoodServiceCode { get; set; }
        public string MealTypeCode { get; set; }
        public string MealName { get; set; }
        public string FoodStyleCode { get; set; }
        public string FoodStyleName { get; set; }

        // ===== Farm =====
        [Required(ErrorMessage = "Please select Farmhouse")]
        public string FarmhouseCode { get; set; }
        public string FarmhouseName { get; set; }
        public string FarmownerCode { get; set; }

        // ===== Activity Service =====
        public string ActivityCode { get; set; }
        public string ActivityName { get; set; }
        public string Duration { get; set; }
        public decimal Price { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public string Description { get; set; }

        // ===== Common =====
        public string ImageFile { get; set; }
        public bool IsActive { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        /////////
        ///public string RoomCode { get; set; }

        public string RoomType { get; set; }
        public string RoomCode { get; set; }

        public int Capacity { get; set; }

        [Required(ErrorMessage = "Please select Room Type")]
        public string RoomTypeCode { get; set; }

        public string RoomTypeName { get; set; }


        [Required(ErrorMessage = "Please enter Room Name")]
        public string RoomName { get; set; }

        public int NumberOfGuests { get; set; }

        public decimal PricePerNight { get; set; }



    }
    #endregion

    #region LoginRS

    public class LoginRS
    {
        [Required(ErrorMessage = "Please Enter Email")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Please Enter Password")]
        public string UserPassword { get; set; }

        [Required(ErrorMessage = "Please Enter new Password")]
        public string newPassword { get; set; }

    }
    #endregion
}
