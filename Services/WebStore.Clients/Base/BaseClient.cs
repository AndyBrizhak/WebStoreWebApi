using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;

namespace WebStore.Clients.Base
{
    public abstract class BaseClient
    {
        //private readonly IConfiguration _Configuration;
        private readonly string _ServiceAddres;
        protected readonly HttpClient _Client;

        protected BaseClient(IConfiguration Configuration, string ServiceAddres )
        {
            //_Configuration = Configuration;
            _ServiceAddres = ServiceAddres;
            _Client = new HttpClient
            {
                BaseAddress = new Uri(Configuration["WebApiURL"]),
                DefaultRequestHeaders =
                {
                    Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
                }
            };
        }
    }
}
