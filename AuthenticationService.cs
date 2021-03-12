using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using JWT.Algorithms;
using JWT.Builder;
using System.Collections.Generic;

namespace AzFunctionsJwtAuth
{
    /// <summary>
    ///     Service class for performing authentication.
    /// </summary>
    public class AuthenticationService
    {
        private readonly TokenIssuer _tokenIssuer;

        /// <summary>
        ///     Injection constructor.
        /// </summary>
        /// <param name="tokenIssuer">DI injected token issuer singleton.</param>
        public AuthenticationService(TokenIssuer tokenIssuer)
        {
            _tokenIssuer = tokenIssuer;
        }

        [FunctionName("Authenticate")]
        public async Task<IActionResult> Authenticate(
            // https://stackoverflow.com/a/52748884/116051
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "auth")] HttpRequest req,
            ILogger log)
        {
            // Perform custom authentication here
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var tokenResult = await GoogleTokenHelper.TokenIsValid(requestBody);

            if (!tokenResult.IsValid)
            {
                return new UnauthorizedResult();
            }

            var creds = new Credentials() { User = tokenResult.Email, Password = "" };

            var jwt = new { token = _tokenIssuer.IssueTokenForUser(creds) };

            return new JsonResult(jwt);
        }
    }


    public struct GoogleTokenResult
    {
        public GoogleTokenResult(bool isValid, string email = null)
        {
            IsValid = isValid;
            Email = email;
        }

        public bool IsValid;
        public string? Email;
    }

    public class GoogleTokenHelper
    {
        private static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static async Task<GoogleTokenResult> TokenIsValid(string token)
        {
            var claims = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret("DztHe0MiWFqIpXBBsUBLgZ50")
                    .Decode<IDictionary<string, object>>(token);

            var unixExpTime = Convert.ToDouble(claims["exp"]);

            var expire = UnixTimeStampToDateTime(unixExpTime);



            string baseUrl = "https://www.googleapis.com/oauth2/v3/tokeninfo?id_token=";

            try
            {

                using (HttpClient client = new HttpClient())
                {
                    using (HttpResponseMessage res = await client.GetAsync(baseUrl + token))
                    {
                        using (HttpContent content = res.Content)
                        {
                            var data = await content.ReadAsStringAsync();

                            if (data != null)
                            {

                                Console.WriteLine("data------------{0}", data);


                                if (data.Contains("Invalid Value"))
                                {
                                    return new GoogleTokenResult(false);
                                }

                                return new GoogleTokenResult(true, "get_email_from_data");
                            }
                            else
                            {
                                Console.WriteLine("NO Data----------");
                                return new GoogleTokenResult(false);
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception Hit------------");
                Console.WriteLine(exception);
                return new GoogleTokenResult(false);
            }

        }
    }

    public class Credentials
    {
        public string User { get; set; }
        public string Password { get; set; }
    }
}