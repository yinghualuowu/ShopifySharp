using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using ShopifySharp;
using ShopifySharp.Filters;
using ShopifySharp.Infrastructure;

namespace WpfApp1
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string AccessToken => "";

        public static string MyShopifyUrl => "";

        public MainWindow()
        {
            InitializeComponent();
            Foo();
        }


        public async void Foo()
        {
            await C();
        }

        private async Task C()
        {

            Dictionary<string,string> dic = new Dictionary<string, string>
            {
                {"1","a"},
            };

            var json = Serializer.Serialize(dic);

            var shopService = new ShopifyPaymentsService(MyShopifyUrl, AccessToken);

            //var model = await shopService.(2937970884803);

            //var resp = await shopService.ListAsync(2299988246617);

            //var productserver = new ProductService(MyShopifyUrl,AccessToken);
            //var fromDate = DateTime.UtcNow.AddDays(-15);
            //var y = new ProductVariantService(MyShopifyUrl, AccessToken);
            //var x = new InventoryItemService(MyShopifyUrl, AccessToken);
            //var z = new OrderService(MyShopifyUrl, AccessToken);
            //var order = await z.GetAsync(2322884100194);
            //var orderstr = JsonConvert.SerializeObject(order);
            //var zz = new LocationService(MyShopifyUrl, AccessToken);
            //var zzz = new FulfillmentService(MyShopifyUrl, AccessToken);
            //var image = new ProductImageService(MyShopifyUrl, AccessToken);
            //var abandoned = new CheckoutService(MyShopifyUrl, AccessToken);


            //var ab = await abandoned.ListAsync(new CheckoutListFilter
            //{
            //    CreatedAtMax = new DateTimeOffset(DateTime.Now),
            //    CreatedAtMin = new DateTimeOffset(DateTime.Now.AddDays(-3)),
            //});

            //var abstr = JsonConvert.SerializeObject(ab);
            //   var model = await zz.ListAsync();
            //  var aa = await productserver.GetAsync(4398237581410);
            // var model = await y.GetAsync(31510788636770);

            // var model = await z.GetAsync(2189020692578);
            // model.ShippingAddress.Province = "Minnesota";
            //  model.ShippingAddress.ProvinceCode = "SH";
            //model.ShippingAddress.Address1 = "guandong";
            // model.ShippingAddress.Country = "Australia";
            //model.ShippingAddress.CountryCode = "AU";
            //model.ShippingAddress.Province = "South Australia";
            // model.ShippingAddress.CountryName = "Australia";
            //  await z.UpdateAsync(2189020692578, model);
            //31510767304802
            //33021700407394
            //13801266544738
            //var model = await x.GetAsync(33021700407394);

            ////var productVariant = model.Items.First();

            //model.SKU = "cyt2020";

            //var updated = await x.UpdateAsync(model.Id.Value, model);

            //var x = new FulfillmentService(MyShopifyUrl, AccessToken);
            //var ful = await x.ListAsync(2184475410530);
            //ful.TrackingNumber = "1234567";
            //ful.TrackingNumbers = new List<string> { "1234567", "01234567" };
            //ful.TrackingCompany = "Cyt Shipping Company";

            //ful.Id = 2042118242402;
            //var resp = await x.UpdateAsync(ful.OrderId.Value, 2042118242402, ful);
            //var f = await zzz.CreateAsync(2189467451490, fulfillment);
            //  var gg = await z.GetAsync(2160607264866);
            //await z.CancelAsync(2189020692578);
            //var imageModel =  await image.GetAsync(4398237581410, 13801266544738);
        }
    }
}
