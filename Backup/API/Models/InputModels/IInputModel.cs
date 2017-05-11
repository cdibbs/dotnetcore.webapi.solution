using Data;

namespace API.Models.InputModels
{
    public interface IInputModel<T> where T: IEntity
    {
        long Id { get; set; }
    }
}