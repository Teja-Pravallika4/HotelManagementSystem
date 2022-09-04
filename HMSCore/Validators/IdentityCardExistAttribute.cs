using HotelManagementSystem.Services;
using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelManagementSystem.Validators.Messages;

namespace HotelManagementSystem.Validators
{
    public class IdentityCardExistAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext { get { return true; } }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var guestService = (IGuestsService)validationContext.GetService(typeof(IGuestsService));

            if (!guestService.IsIdentityNumberExist(value?.ToString().Trim()))
            {
                return new ValidationResult(ValidatorConstants.validateIdentityIdIsExist);
            }

            return ValidationResult.Success;
        }
    }
}