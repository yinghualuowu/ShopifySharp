using Newtonsoft.Json.Linq;
using ShopifySharp.Enums;
using ShopifySharp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ShopifySharp
{
    public static class AuthorizationService
    {
        /// <summary>
        /// Determines if an incoming webhook request is authentic.
        /// </summary>
        /// <param name="requestHeaders">The request's headers.</param>
        /// <param name="requestBody">The body of the request.</param>
        /// <param name="shopifySecretKey">Your app's secret key.</param>
        /// <returns>A boolean indicating whether the webhook is authentic or not.</returns>
        public static bool IsAuthenticWebhook(HttpRequestHeaders requestHeaders, string requestBody, string shopifySecretKey)
        {
            var hmacHeaderValue = requestHeaders.FirstOrDefault(kvp => kvp.Key.Equals("X-Shopify-Hmac-SHA256", StringComparison.OrdinalIgnoreCase)).Value.FirstOrDefault();

            if (string.IsNullOrEmpty(hmacHeaderValue))
            {
                return false;
            }

            //Compute a hash from the apiKey and the request body
            string hmacHeader = hmacHeaderValue;
            HMACSHA256 hmac = new HMACSHA256(Encoding.UTF8.GetBytes(shopifySecretKey));
            string hash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(requestBody)));

            //Webhook is valid if computed hash matches the header hash
            return hash == hmacHeader;
        }

        /// <summary>
        /// A convenience function that tries to ensure that a given URL is a valid Shopify domain. It does this by making a HEAD request to the given domain, and returns true if the response contains an X-ShopId header.
        ///
        /// **Warning**: a domain could fake the response header, which would cause this method to return true.
        ///
        /// **Warning**: this method of validation is not officially supported by Shopify and could break at any time.
        /// </summary>
        /// <param name="url">The URL of the shop to check.</param>
        /// <returns>A boolean indicating whether the URL is valid.</returns>
        public static async Task<bool> IsValidShopDomainAsync(string url)
        {
            var uri = ShopifyService.BuildShopUri(url, false);

            using (var handler = new HttpClientHandler
            {
                AllowAutoRedirect = false
            })
            using (var client = new HttpClient(handler))
            using (var msg = new HttpRequestMessage(HttpMethod.Head, uri))
            {
                var version = (typeof(AuthorizationService)).GetTypeInfo().Assembly.GetName().Version;
                msg.Headers.Add("User-Agent", $"ShopifySharp v{version} (https://github.com/nozzlegear/shopifysharp)");

                try
                {
                    var response = await client.SendAsync(msg);

                    return response.Headers.Any(h => h.Key.Equals("X-ShopId", StringComparison.OrdinalIgnoreCase));
                }
                catch (HttpRequestException)
                {
                    return false;
                }
            }
        }

        /// <summary>
        /// Builds an authorization URL for Shopify OAuth integration.
        /// </summary>
        /// <param name="scopes">An array of <see cref="ShopifyAuthorizationScope"/> — the permissions that your app needs to run.</param>
        /// <param name="myShopifyUrl">The shop's *.myshopify.com URL.</param>
        /// <param name="shopifyApiKey">Your app's public API key.</param>
        /// <param name="redirectUrl">URL to redirect the user to after integration.</param>
        /// <param name="state">An optional, random string value provided by your application which is unique for each authorization request. During the OAuth callback phase, your application should check that this value matches the one you provided to this method.</param>
        /// <param name="grants">Requested grant types, which will change the type of access token granted upon OAuth completion. Only known grant type is "per-user", which will give an access token restricted to the permissions of the user accepting OAuth integration and will expire when that user logs out. Leave the grants array empty or null to receive a full access token that doesn't expire.</param>
        /// <returns>The authorization url.</returns>
        public static Uri BuildAuthorizationUrl(IEnumerable<AuthorizationScope> scopes, string myShopifyUrl, string shopifyApiKey, string redirectUrl, string state = null, IEnumerable<string> grants = null)
        {
            return BuildAuthorizationUrl(scopes.Select(s => s.ToSerializedString()), myShopifyUrl, shopifyApiKey, redirectUrl, state, grants);
        }

        /// <summary>
        /// Builds an authorization URL for Shopify OAuth integration.
        /// </summary>
        /// <param name="scopes">An array of Shopify permission strings, e.g. 'read_orders' or 'write_script_tags'. These are the permissions that your app needs to run.</param>
        /// <param name="myShopifyUrl">The shop's *.myshopify.com URL.</param>
        /// <param name="shopifyApiKey">Your app's public API key.</param>
        /// <param name="redirectUrl">URL to redirect the user to after integration.</param>
        /// <param name="state">An optional, random string value provided by your application which is unique for each authorization request. During the OAuth callback phase, your application should check that this value matches the one you provided to this method.</param>
        /// <param name="grants">Requested grant types, which will change the type of access token granted upon OAuth completion. Only known grant type is "per-user", which will give an access token restricted to the permissions of the user accepting OAuth integration and will expire when that user logs out. Leave the grants array empty or null to receive a full access token that doesn't expire.</param>
        /// <returns>The authorization url.</returns>
        public static Uri BuildAuthorizationUrl(IEnumerable<string> scopes, string myShopifyUrl, string shopifyApiKey, string redirectUrl, string state = null, IEnumerable<string> grants = null)
        {
            //Prepare a uri builder for the shop URL
            var builder = new UriBuilder(ShopifyService.BuildShopUri(myShopifyUrl, false));

            //Build the querystring
            var qs = new List<KeyValuePair<string, string>>()
            {
                new KeyValuePair<string, string>("client_id", shopifyApiKey),
                new KeyValuePair<string, string>("scope", string.Join(",", scopes)),
                new KeyValuePair<string, string>("redirect_uri", redirectUrl),
            };

            if (string.IsNullOrEmpty(state) == false)
            {
                qs.Add(new KeyValuePair<string, string>("state", state));
            }

            if (grants != null && grants.Count() > 0)
            {
                foreach (var grant in grants)
                {
                    qs.Add(new KeyValuePair<string, string>("grant_options[]", grant));
                }
            }

            builder.Path = "admin/oauth/authorize";
            builder.Query = string.Join("&", qs.Select(s => $"{s.Key}={s.Value}"));

            return builder.Uri;
        }

        /// <summary>
        /// Authorizes an application installation, generating an access token for the given shop.
        /// </summary>
        /// <param name="code">The authorization code generated by Shopify, which should be a parameter named 'code' on the request querystring.</param>
        /// <param name="myShopifyUrl">The store's *.myshopify.com URL, which should be a paramter named 'shop' on the request querystring.</param>
        /// <param name="shopifyApiKey">Your app's public API key.</param>
        /// <param name="shopifySecretKey">Your app's secret key.</param>
        /// <returns>The shop access token.</returns>
        public static async Task<string> Authorize(string code, string myShopifyUrl, string shopifyApiKey, string shopifySecretKey)
        {
            return (await AuthorizeWithResult(code, myShopifyUrl, shopifyApiKey, shopifySecretKey)).AccessToken;
        }

        /// <summary>
        /// Authorizes an application installation, generating an access token for the given shop.
        /// </summary>
        /// <param name="code">The authorization code generated by Shopify, which should be a parameter named 'code' on the request querystring.</param>
        /// <param name="myShopifyUrl">The store's *.myshopify.com URL, which should be a paramter named 'shop' on the request querystring.</param>
        /// <param name="shopifyApiKey">Your app's public API key.</param>
        /// <param name="shopifySecretKey">Your app's secret key.</param>
        /// <returns>The authorization result.</returns>
        public static async Task<AuthorizationResult> AuthorizeWithResult(string code, string myShopifyUrl, string shopifyApiKey, string shopifySecretKey)
        {
            var ub = new UriBuilder(ShopifyService.BuildShopUri(myShopifyUrl, false))
            {
                Path = "admin/oauth/access_token"
            };
            var content = new JsonContent(new
            {
                client_id = shopifyApiKey,
                client_secret = shopifySecretKey,
                code,
            });

            using (var client = new HttpClient())
            using (var msg = new CloneableRequestMessage(ub.Uri, HttpMethod.Post, content))
            {
                var request = client.SendAsync(msg);
                var response = await request;
                var rawDataString = await response.Content.ReadAsStringAsync();

                ShopifyService.CheckResponseExceptions(response, rawDataString);

                var json = JToken.Parse(rawDataString);
                return new AuthorizationResult(json.Value<string>("access_token"), json.Value<string>("scope").Split(','));
            }
        }
    }
}
