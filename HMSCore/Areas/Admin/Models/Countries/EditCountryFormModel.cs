using HotelManagementSystem.Areas.Admin.Validators;
using HotelManagementSystem.Areas.Admin.Validators.Messages;
using HOtelManagementSystem.Areas.Admin.Validators;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Areas.Admin.Models.Countries
{
    public class EditCountryFormModel
    {
        public string Id { get; set; }

        [Required]
        [CountryNameEdit]
        [MinLength(3, ErrorMessage = ValidatorConstants.minLength)]
        [MaxLength(30, ErrorMessage = ValidatorConstants.maxLength)]
        public string Name { get; set; }
    }
}