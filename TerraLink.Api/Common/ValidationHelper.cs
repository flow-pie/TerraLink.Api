using System.ComponentModel.DataAnnotations;

namespace TerraLink.Api.Common
{
    //A typed error body for validation errors.

    public record ErrorResponse(string Message, List<string> Details);

    public static class ValidationHelper
    {
        public static bool TryValidate<T>(T dto, out List<ValidationResult> errors) 
        where T : notnull
        {
            var validationContext =  new ValidationContext(dto);
            errors = new List<ValidationResult>();
            return Validator.TryValidateObject(dto, validationContext, errors, validateAllProperties: true);
        }

        public static ErrorResponse ToErrorResponse(this List<ValidationResult> errors) =>
        new (string.Join("; ", errors.Select(e => e.ErrorMessage)), errors.SelectMany(e => e.MemberNames).ToList());
    }
}