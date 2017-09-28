using API.Models.ViewModels;
using Data.Repositories.ReadOnly;
using System.ComponentModel.DataAnnotations.Schema;
using Data.Models;

namespace API.Models
{
    public class PersonViewModel : IViewModel<V_MyView, string>
    {
        public string Id { get; set; }

        public string Type { get; set; }
    }
}