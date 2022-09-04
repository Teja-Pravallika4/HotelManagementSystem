
using System.ComponentModel.DataAnnotations;
using HotelManagementSystem.Validators.Messages;
using HotelManagementSystem.Services;

namespace HotelManagementSystem.Validators
{
    public class IdentityCardAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext { get { return true; } }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var guestService = (IGuestsService)validationContext.GetService(typeof(IGuestsService));

            if (guestService.IsIdentityNumberExist(value?.ToString().Trim()))
            {
                return new ValidationResult(ValidatorConstants.validateIdentityId);
            }

            return ValidationResult.Success;
        }
    }
}