using System.ComponentModel.DataAnnotations;

namespace Shopping_Tutorial.Models
{
	public class UserModel
	{
		public string Id { get; set; }
		[Required(ErrorMessage ="Làm ơn nhập UerName")]
		public string UserName { get; set; }
		[Required(ErrorMessage ="Làm ơn nhập Email"), EmailAddress]
		public string Email { get; set; }
		[DataType(DataType.Password),Required(ErrorMessage ="Làm ơn nhập Password")]
		public string Password { get; set; }
	}
}
