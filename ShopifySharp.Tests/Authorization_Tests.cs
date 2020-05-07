using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using ShopifySharp.Enums;
using Xunit;

namespace ShopifySharp.Tests
{
    [Trait("Category", "Authorization")]
    public class Authorization_Tests
    {
        [Fact]
        public async Task Validates_Shop_Urls()
        {
            string validUrl = Utils.MyShopifyUrl;
            string invalidUrl = "https://google.com";

            Assert.True(await AuthorizationService.IsValidShopDomainAsync(validUrl));
            Assert.False(await AuthorizationService.IsValidShopDomainAsync(invalidUrl));
        }

        [Fact]
        public async Task Validates_Shop_Malfunctioned_Urls()
        {
            string invalidUrl = "foo";

            Assert.False(await AuthorizationService.IsValidShopDomainAsync(invalidUrl));
        }

        [Fact]
        public void Builds_Authorization_Urls_With_Enums()
        {
            var scopes = new List<AuthorizationScope>()
            {
                AuthorizationScope.ReadCustomers,
                AuthorizationScope.WriteCustomers
            };
            string redirectUrl = "http://example.com";
            string result = AuthorizationService.BuildAuthorizationUrl(scopes, Utils.MyShopifyUrl, Utils.ApiKey, redirectUrl).ToString();

            Assert.Contains($"/admin/oauth/authorize?", result);
            Assert.Contains($"client_id={Utils.ApiKey}", result);
            Assert.Contains($"scope=read_customers,write_customers", result);
            Assert.Contains($"redirect_uri={redirectUrl}", result);
        }

        [Fact]
        public void Builds_Authorization_Urls_With_Strings()
        {
            string[] scopes = { "read_customers", "write_customers" };
            string redirectUrl = "http://example.com";
            string result = AuthorizationService.BuildAuthorizationUrl(scopes, Utils.MyShopifyUrl, Utils.ApiKey, redirectUrl).ToString();

            Assert.Contains($"/admin/oauth/authorize?", result);
            Assert.Contains($"client_id={Utils.ApiKey}", result);
            Assert.Contains($"scope=read_customers,write_customers", result);
            Assert.Contains($"redirect_uri={redirectUrl}", result);
        }

        [Fact]
        public void Builds_Authorization_Urls_With_Grants_And_State()
        {
            string[] scopes = { "read_customers", "write_customers" };
            string redirectUrl = "http://example.com";
            string[] grants = { "per-user" };
            string state = Guid.NewGuid().ToString();
            string result = AuthorizationService.BuildAuthorizationUrl(scopes, Utils.MyShopifyUrl, Utils.ApiKey, redirectUrl, state, grants).ToString();

            Assert.Contains($"/admin/oauth/authorize?", result);
            Assert.Contains($"client_id={Utils.ApiKey}", result);
            Assert.Contains($"scope=read_customers,write_customers", result);
            Assert.Contains($"redirect_uri={redirectUrl}", result);
            Assert.Contains($"state={state}", result);
            Assert.Contains($"grant_options[]=per-user", result);
        }
    }
}