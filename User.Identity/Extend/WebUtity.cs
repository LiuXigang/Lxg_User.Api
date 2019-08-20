using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace User.Identity.Extend
{
    public class WebUtity
    {
        private static readonly double _timeSpan = 10;
        /// <summary>
        /// HttpClient实现Post请求
        /// </summary>
        public static async Task<string> PostAsync(string url, Dictionary<string, string> data)
        {
            //设置HttpClientHandler的AutomaticDecompression
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            //创建HttpClient（注意传入HttpClientHandler）
            using (var http = new HttpClient(handler))
            {
                //使用FormUrlEncodedContent做HttpContent
                var content = new FormUrlEncodedContent(data);

                http.Timeout = TimeSpan.FromSeconds(_timeSpan);

                //await异步等待回应
                var response = await http.PostAsync(url, content);
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                return await response.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// HttpClient实现Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public static async Task<string> PostAsync(string url, object data)
        {
            //设置HttpClientHandler的AutomaticDecompression
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };
            //创建HttpClient（注意传入HttpClientHandler）
            using (var http = new HttpClient(handler))
            {
                var inputJson = Object2JsonString(data);
                var requestContent = new StringContent(inputJson, Encoding.UTF8, "application/json");
                http.Timeout = TimeSpan.FromSeconds(_timeSpan);

                //await异步等待回应
                var response = await http.PostAsync(url, requestContent);
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();
                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                return await response.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// HttpClient实现Get请求
        /// </summary>
        public static async Task<string> GetAsync(string url)
        {
            //创建HttpClient（注意传入HttpClientHandler）
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip };

            using (var http = new HttpClient(handler))
            {
                http.Timeout = TimeSpan.FromSeconds(_timeSpan);
                //await异步等待回应
                var response = await http.GetAsync(url);
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();

                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                return await response.Content.ReadAsStringAsync();
            }
        }

        /// <summary>
        /// HttpClient实现异步Get请求
        /// </summary>
        public static async Task<string> GetAsync(string url, Dictionary<string, string> headers = null)
        {
            //创建HttpClient（注意传入HttpClientHandler）
            var handler = new HttpClientHandler() { AutomaticDecompression = DecompressionMethods.GZip, UseCookies = false };

            using (var http = new HttpClient(handler))
            {
                var message = new HttpRequestMessage(HttpMethod.Get, url);
                if (headers != null)
                {
                    foreach (var header in headers)
                    {
                        message.Headers.Add(header.Key, header.Value);
                    }
                }

                http.Timeout = TimeSpan.FromSeconds(_timeSpan);

                //await异步等待回应
                var response = await http.SendAsync(message);
                //确保HTTP成功状态值
                response.EnsureSuccessStatusCode();

                //await异步读取最后的JSON（注意此时gzip已经被自动解压缩了，因为上面的AutomaticDecompression = DecompressionMethods.GZip）
                return await response.Content.ReadAsStringAsync();
            }
        }

        private static string Object2JsonString(object obj)
        {
            return obj is null ? JsonConvert.SerializeObject(obj) : "";
        }
        private static TObj JsonString2Object<TObj>(string str)
        {
            return JsonConvert.DeserializeObject<TObj>(str, new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
        }

    }
}
