using System;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;
using ShopifySharp.Fluent;
using ShopifySharp.Fluent.Properties;

namespace ShopifySharp.Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var scriptTag = new ScriptTagBuilder()
                .AddSrc("https://example.com")
                .MakePropertyNull(ScriptTagProperty.DisplayScope)
                .RemoveProperty(ScriptTagProperty.DisplayScope);

            var json = JsonConvert.SerializeObject(scriptTag.ToDictionary());

            
            Console.WriteLine(json);

            //
            // var product = new FluentProduct()
            //     .AddSrc("https://example.js")
            //     .MakePropertyNull(ScriptTagProperty.Event)
            //     .RemoveProperty(ScriptTagProperty.Event);
        }
    }
}