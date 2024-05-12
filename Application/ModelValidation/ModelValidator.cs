using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.CSharp.Interfaces;

namespace Application.CSharp.ModelValidation
{
    public class ModelValidator : IModelValidator
    {
        public (bool, string?) Validate<T>(T entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var errors = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(entity, new System.ComponentModel.DataAnnotations.ValidationContext(entity), errors, true);

            if (!isValid)
            {
                StringBuilder errorMessage = new StringBuilder();
                foreach (var error in errors)
                {
                    errorMessage.Append(error.ErrorMessage).Append(" ");
                }

                return (isValid, errorMessage.ToString());
            }

            return (true, default);
        }
    }
}
