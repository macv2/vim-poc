using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebApp.Client;

namespace WebApp.Pages.LocalDbClient;

public class LocalDbPageModel(IConfiguration configuration, IHttpClientFactory httpClientFactory) : PageModel
{
	public List<ProductModel> ProductList { get; set; } = new();
	[BindProperty]
	public string ProductName { get; set; }
	[BindProperty]
	public decimal ProductValue { get; set; }
    [BindProperty]
    public int ProductQuantity { get; set; }

    public async Task<IActionResult> OnGet()
    {
		try
		{
            var client = new ApiClient(httpClientFactory, configuration);

            var response = await client.Get<ProductListResponse>($"{configuration.GetValue<string>("ApiUrl")}products/GetProducts");

            if (response.IsSuccess)
            {
                foreach (var item in response.Products)
                {
                    ProductList.Add(new ProductModel { Name = item.Name, Value = item.Price });
                }
            }
        }
		catch (Exception)
		{

		}
        return Page();
    }

 //   public async Task<IActionResult> OnGetGetProductList()
	//{

	//}

	public async Task<IActionResult> OnPostSaveProduct()
	{
		try
		{
			var client = new ApiClient(httpClientFactory, configuration);

			var request = new AddProductRequest
			{
				Name = ProductName,
				Price = ProductValue,
				Quantity = ProductQuantity
			};

			var response = await client.Post<AddProductRequest, AddProductResponse>($"{ configuration.GetValue<string>("ApiUrl")}products/AddProduct", request);
		}
		catch (Exception)
		{

		}
        return Page();
    }

    public class ProductListResponse : ResponseBase
	{
		public List<Product> Products { get; set; }
    }

	public class AddProductRequest
	{
		public string? Name { get; set; }	
		public decimal Price { get; set; }
		public int Quantity { get; set; }
	}

	public class AddProductResponse : ResponseBase
    {
		public string Message { get; set; }
	}

    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}
