using Data;
using API.Models.InputModels;

namespace API.Validators
{
    /// <summary>
    /// The thought is to follow something like the error handling model, here:
    /// http://www.infoworld.com/article/2994111/application-architecture/how-to-handle-errors-in-web-api.html
    /// Our custom validation exceptions would be trapped in a non-500-error way (lol) and
    /// bubble up more sensibly to the user interface.
    /// </summary>
    public interface IValidator<T> where T: BaseEntity
    {
        /// <summary>
        /// Throws a custom exception whenever a bad input is received.
        /// </summary>
        /// <param name="input">User input to validate.</param>
        void Validate(IInputModel<T> input);
    }
}
