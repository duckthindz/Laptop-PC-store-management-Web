using System.ComponentModel.DataAnnotations;

namespace Shopping_Tutorial.Models.ViewModels
{
    public class RoleModel
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Làm ơn nhập Name")]
        public string Name { get; set; }
    }
}
