using HotelManagementSystem.Services;
using HotelManagementSystem.Validators.Messages;
using System.ComponentModel.DataAnnotations;

namespace HotelManagementSystem.Validators
{
    public class CountryIdAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext { get { return true; } }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var guestService = (IGuestsService)validationContext.GetService(typeof(IGuestsService));

            if (!guestService.IsCountryExist(value?.ToString().Trim()))
            {
                return new ValidationResult(ValidatorConstants.validateCountryId);
            }

            return ValidationResult.Success;
        }
    }
}
