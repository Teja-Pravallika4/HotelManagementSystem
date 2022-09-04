
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using HotelManagementSystem.Services;
using HotelManagementSystem.Validators.Messages;

namespace HotelManagementSystem.Validators
{

    public class RankNameForEditAttribute : ValidationAttribute
    {
        public override bool RequiresValidationContext { get { return true; } }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var guestRankService = (IGuestRanksService)validationContext.GetService(typeof(IGuestRanksService));

            var httpAccesor = (IHttpContextAccessor)validationContext.GetService(typeof(IHttpContextAccessor));

            string rankId = httpAccesor.HttpContext.Request.RouteValues.Values.Last().ToString();

            if (guestRankService.IsNameExistWhenEdit(value?.ToString().Trim(), rankId))
            {
                return new ValidationResult(ValidatorConstants.validateRankNameErrMsg);
            }

            return ValidationResult.Success;
        }
    }
}