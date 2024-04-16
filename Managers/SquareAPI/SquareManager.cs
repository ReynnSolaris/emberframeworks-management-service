using Square;
using Square.Apis;
using Square.Authentication;
using Square.Exceptions;
using Square.Http;
using Square.Utilities;
using Square.Models;
using System;
using Environment = System.Environment;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;

namespace EmberFrameworksService.Managers.SquareAPI
{
    public class SquareManager
    {
        IConfiguration configuration;
        public SquareManager(IConfiguration config)
        {
            configuration = config;
        }

        public SquareClient createClient()
        {
            bool isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
            System.Diagnostics.Debug.WriteLine("ACCESS_TOKEN: " + (isDevelopment ? configuration["SQUARE_ACCESS_TOKEN_DEV"] : configuration["SQUARE_ACCESS_TOKEN"]));
            SquareClient client = new SquareClient.Builder()
                .Environment(isDevelopment ? Square.Environment.Sandbox : Square.Environment.Production)
                .BearerAuthCredentials(new BearerAuthModel.Builder(isDevelopment ? configuration["SQUARE_ACCESS_TOKEN_DEV"] : configuration["SQUARE_ACCESS_TOKEN"]).Build())
                .Build();
            return client;
        }

        public async Task<ListCatalogResponse> GetCatalog()
        {
            SquareClient client = createClient();
            ICatalogApi catalog = client.CatalogApi;
            try
            {
                ListCatalogResponse v = await catalog.ListCatalogAsync();
                return v;
            } catch (ApiException e)
            {
                throw e;
            }
        }
    }
}
