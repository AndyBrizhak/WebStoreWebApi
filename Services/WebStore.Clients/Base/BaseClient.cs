using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebStore.Clients.Base
{
    /// <summary>
    /// Class for create Clients
    /// </summary>
    public abstract class BaseClient : IDisposable
    {
        protected readonly string _ServiceAddress;
        protected readonly HttpClient _Client;

        protected BaseClient(IConfiguration Configuration, string ServiceAddress )
        {
            //_Configuration = Configuration;
            _ServiceAddress = ServiceAddress;
            _Client = new HttpClient
            {
                BaseAddress = new Uri(Configuration["WebApiURL"]),
                DefaultRequestHeaders =
                {
                    Accept = { new MediaTypeWithQualityHeaderValue("application/json") }
                }
            };
        }

        protected T Get<T>(string url) => GetAsync<T>(url).Result;

        protected async Task<T> GetAsync<T>(string url)
        {
            var response = await _Client.GetAsync(url);
            return await response
                .EnsureSuccessStatusCode() // Убеждаемся, что в ответ получен 200-ый статусный код.
                .Content             // В ответе есть содержимое с которым можно работать
                .ReadAsAsync<T>(); // Десериализуем данные в нужный тип объекта
        }

        protected HttpResponseMessage Post<T>(string url, T item) => PostAsync(url, item).Result;

        protected async Task<HttpResponseMessage> PostAsync<T>(string url, T item)
        {
            var response = await _Client.PostAsJsonAsync(url, item);
            return response.EnsureSuccessStatusCode();
        }

        protected HttpResponseMessage Put<T>(string url, T item) => PutAsync(url, item).Result;

        protected async Task<HttpResponseMessage> PutAsync<T>(string url, T item)
        {
            var response = await _Client.PutAsJsonAsync(url, item);
            return response.EnsureSuccessStatusCode();
        }

        protected HttpResponseMessage Delete(string url) => DeleteAsync(url).Result;

        protected async Task<HttpResponseMessage> DeleteAsync(string url)
        {
            var response = await _Client.DeleteAsync(url);
            return response;
        }

        #region IDisposable

        //~BaseClient() => Dispose(false);

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            // Здесь можно выполнить освобождение неуправляемых ресурсов
            if (disposing)
            {
                // Здесь можно выполнить освобождение управляемых ресурсов
                _Client.Dispose();
            }
        }

        #endregion
    }
}
