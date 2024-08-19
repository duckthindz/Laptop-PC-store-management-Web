namespace Shopping_Tutorial.Models
{
	public class CartItemModel
	{
		public int ProductId { get; set; }
		public string ProductName { get; set; }
		public int Quantity		{ get; set; }
		public decimal Price { get; set; }
		public string Images { get; set; }
		public decimal Total {
			get { return Quantity * Price; }
		}
		public CartItemModel() 
		{

		}
		public CartItemModel(ProductModel product)
		{
			ProductId = product.Id;
			ProductName = product.Name;
			Price = product.Price;
			Quantity = 1;
			Images = product.Images;
			
		}
	}
}
