using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OzonReturnsManager1.Models;

namespace OzonReturnsManager1.Services
{
    public class ReturnsApiClient
    {
        private readonly string _baseUrl = "http://95.84.154.33:7780";
        private readonly string _token;
        private readonly HttpClient _httpClient;

        public ReturnsApiClient(string token)
        {
            _token = token;
            _httpClient = new HttpClient();
            // Токен передается как есть, без префикса "Bearer "
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", token);
            // Content-Type добавляется автоматически при создании StringContent
        }

        /// <summary>
        /// Получить возвраты от покупателей
        /// </summary>
        public async Task<List<ReturnRecord>> GetCustomerReturnsAsync(
            DateTime dateFrom,
            DateTime dateTo,
            int? orgType,
            string ourStatus)
        {
            var requestUrl = $"{_baseUrl}/api/arm/returns/getV2";

            var requestBody = new
            {
                changeMomentFrom = dateFrom.ToString("yyyy-MM-dd"),
                changeMomentTo = dateTo.ToString("yyyy-MM-dd"),
                org_type = orgType.HasValue ? orgType.Value.ToString() : null,
                our_status = ourStatus
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, content);
            
            // Получаем подробную информацию об ошибке
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"HTTP {(int)response.StatusCode} {response.ReasonPhrase} - {responseContent}");
            }

            var result = JsonConvert.DeserializeObject<GetReturnsResponse>(responseContent);

            if (result?.Status != "ok" || result.Items == null)
            {
                return new List<ReturnRecord>();
            }

            var records = new List<ReturnRecord>();
            foreach (var item in result.Items)
            {
                records.Add(new ReturnRecord
                {
                    Id = item.Id,
                    Date = DateTime.Parse(item.ChangeMoment),
                    Type = "Возврат",
                    ShopName = item.ShopName,
                    OfferId = item.OfferId,
                    OurStatus = item.OurStatus,
                    OrgType = item.OrgType,
                    OzonReturnId = item.OzonReturnId.ToString(),
                    PostingNumber = item.PostingNumber,
                    Sku = item.Sku.ToString(),
                    Name = item.Name,
                    Quantity = item.Quantity
                });
            }

            return records;
        }

        /// <summary>
        /// Получить вывоз со склада Озон
        /// </summary>
        public async Task<List<ReturnRecord>> GetStockRemovalsAsync(
            DateTime dateFrom,
            DateTime dateTo,
            int? orgType,
            string ourStatus)
        {
            var requestUrl = $"{_baseUrl}/api/arm/from_stock/getRecords";

            var requestBody = new
            {
                return_completion_at = new
                {
                    from = dateFrom.ToString("yyyy-MM-dd"),
                    to = dateTo.ToString("yyyy-MM-dd")
                },
                return_state = "Завершено",
                our_status = ourStatus
            };

            var json = JsonConvert.SerializeObject(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(requestUrl, content);
            
            // Получаем подробную информацию об ошибке
            var responseContent = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                throw new HttpRequestException(
                    $"HTTP {(int)response.StatusCode} {response.ReasonPhrase} - {responseContent}");
            }

            var result = JsonConvert.DeserializeObject<GetStockRemovalsResponse>(responseContent);

            if (result?.Status != "ok" || result.Data == null)
            {
                return new List<ReturnRecord>();
            }

            var records = new List<ReturnRecord>();
            foreach (var item in result.Data)
            {
                records.Add(new ReturnRecord
                {
                    Id = item.Id,
                    Date = DateTime.Parse(item.ReturnCompletionAt),
                    Type = "Вывоз",
                    ShopName = item.ShopName,
                    OfferId = item.OfferId,
                    OurStatus = item.OurStatus,
                    OrgType = item.OrgType,
                    ReturnId = item.ReturnId,
                    BoxId = item.BoxId,
                    BoxState = item.BoxState,
                    ReturnState = item.ReturnState
                });
            }

            return records;
        }
    }

    // Ответ API для возвратов
    public class GetReturnsResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("items")]
        public List<ReturnItem> Items { get; set; }
    }

    public class ReturnItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("shop_name")]
        public string ShopName { get; set; }

        [JsonProperty("ozon_return_id")]
        public int OzonReturnId { get; set; }

        [JsonProperty("offer_id")]
        public string OfferId { get; set; }

        [JsonProperty("sku")]
        public int Sku { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("posting_number")]
        public string PostingNumber { get; set; }

        [JsonProperty("our_status")]
        public string OurStatus { get; set; }

        [JsonProperty("org_type")]
        public int OrgType { get; set; }

        [JsonProperty("change_moment")]
        public string ChangeMoment { get; set; }
    }

    // Ответ API для вывоза
    public class GetStockRemovalsResponse
    {
        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("data")]
        public List<StockRemovalItem> Data { get; set; }
    }

    public class StockRemovalItem
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("offer_id")]
        public string OfferId { get; set; }

        [JsonProperty("shop_name")]
        public string ShopName { get; set; }

        [JsonProperty("org_type")]
        public int OrgType { get; set; }

        [JsonProperty("our_status")]
        public string OurStatus { get; set; }

        [JsonProperty("box_id")]
        public string BoxId { get; set; }

        [JsonProperty("return_id")]
        public string ReturnId { get; set; }

        [JsonProperty("box_state")]
        public string BoxState { get; set; }

        [JsonProperty("return_state")]
        public string ReturnState { get; set; }

        [JsonProperty("return_completion_at")]
        public string ReturnCompletionAt { get; set; }
    }
}
