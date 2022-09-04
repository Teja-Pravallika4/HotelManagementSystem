using HotelManagementSystem.Services;
using HotelManagementSystem.Validators.Messages;
using System.ComponentModel.DataAnnotations;

namespace HMSCore.Validators
{
    public class RankIdAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext { get { return true; } }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var guestService = (IGuestsService)validationContext.GetService(typeof(IGuestsService));

            if (!guestService.IsRankExist(value?.ToString().Trim()))
            {
                return new ValidationResult(ValidatorConstants.validateRankId);
            }

            return ValidationResult.Success;
        }
    }
}