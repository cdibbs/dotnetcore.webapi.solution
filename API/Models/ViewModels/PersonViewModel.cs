using API.Models.ViewModels;
using Data.Repositories.ReadOnly;
using System.ComponentModel.DataAnnotations.Schema;

namespace API.Models
{
    public class PersonViewModel : IViewModel<V_Population, string>
    {
        [Column("UserId")]
        public string Id { get; set; }

        public string Username { get; set; }

        public string Title { get; set; }
        public string Department { get; set; }
        public string Email { get; set; }
        public string First { get; set; }
        public string Middle { get; set; }
        public string Last { get; set; }
    }
}