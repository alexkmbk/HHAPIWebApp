using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using HHAPIWebApp.Models;

namespace HHAPIWebApp
{
    static public class HHApi
    {

        public static string GetUserInfo(string Token, string UserId)
        {
            HttpClient client = new HttpClient();
            if (string.IsNullOrEmpty(Token)) throw new System.ArgumentException("Не задан параметр Token", "Token");
            if (string.IsNullOrEmpty(UserId)) throw new System.ArgumentException("Не задан параметр UserId", "UserId");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            client.DefaultRequestHeaders.Add("User-Agent", UserId + " / 1.0 (alexkmbk@gmail.com)");

            HttpResponseMessage response = client.GetAsync("https://api.hh.ru/me").Result;
            HttpContent content = response.Content;

            // ... Read the string.
            string result = content.ReadAsStringAsync().Result;

            // ... Display the result.
            if (result != null &&
            result.Length >= 50)
            {
                return result;
            }

            return "";
        }

        public static List<Vacancy> GetFavoriteVacancies(string Token, string UserId, string searchString, bool openOnly=true)
        {
            string searchStringLow = (searchString==null) ? null : searchString.ToLower();
            // ... Target page.


            // ... Use HttpClient.
            HttpClient client = new HttpClient();
            if (string.IsNullOrEmpty(Token)) throw new System.ArgumentException("Не задан параметр Token", "Token");
            if (string.IsNullOrEmpty(UserId)) throw new System.ArgumentException("Не задан параметр UserId", "UserId");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            client.DefaultRequestHeaders.Add("User-Agent", UserId + " / 1.0 (alexkmbk@gmail.com)");

            HttpResponseMessage response = client.GetAsync("https://api.hh.ru/me").Result;
            HttpContent content = response.Content;

            // ... Read the string.
            string result = content.ReadAsStringAsync().Result;

            // ... Display the result.
            if (result != null &&
            result.Length >= 50)
            {
                Console.WriteLine(result);
            }

            //JArray items = new JArray();
            List<Vacancy> vacancies = new List<Vacancy>();

            int pageNum = 0;
            while (true)
            {
                try
                {
                    response = client.GetAsync($"https://api.hh.ru/vacancies/favorited?per_page=100&page={pageNum}").Result;
                }
                catch (Exception e)
                {
                    Console.WriteLine("Exception while execute Get https://api.hh.ru/vacancies/favorited: " + e.Message);
                    return null;
                }

                if (!response.IsSuccessStatusCode)
                {
                    // problems handling here
                    Console.WriteLine(
                        "Error occurred, the status code is: {0}",
                        response.StatusCode
                    );
                    break;
                }
                content = response.Content;

                // ... Read the string.
                result = content.ReadAsStringAsync().Result;

                // ... Display the result.
                if (result == null)
                {
                    Console.WriteLine("Ошибка при получении списка отобранных вакансий в текстовом виде.");
                    return null;
                }
                else
                {
                    JObject json = JObject.Parse(result);
                    JArray a = (JArray)json["items"];
                    if (a.Count == 0)
                    {
                        break;
                    }
                    foreach (var item in a)
                    {
                        Vacancy vacancy = new Vacancy();
                        vacancy.name = item["name"].ToString();
                        try
                        {
                            JObject address = (JObject)item["employer"];
                            vacancy.EmployerName = address["name"].ToString();
                        }
                        catch
                        {
                            vacancy.EmployerName = "";
                        }
                        try
                        {
                            JObject address = (JObject)item["area"];
                            vacancy.AreaName = address["name"].ToString();
                        }
                        catch
                        {
                            vacancy.EmployerName = "";
                        }
                        try
                        {
                            JObject address = (JObject)item["address"];
                            vacancy.raw_adress = address["raw"].ToString();
                        }
                        catch
                        {
                            vacancy.raw_adress = "";
                        }
                        try
                        {
                            JObject type = (JObject)item["type"];
                            vacancy.is_open = (type["id"].ToString() == "open") ? true : false;
                        }
                        catch
                        {
                            vacancy.is_open = false;
                        }
                        vacancy.url = item["alternate_url"].ToString();

                        bool skip = false;

                        if ((searchString != null) && (!vacancy.AreaName.ToLower().Contains(searchStringLow)))
                        {
                            skip = true;
                          }

                        if (openOnly&&(vacancy.is_open== false))
                        {
                            skip = true;
                        }

                        if (!skip) vacancies.Add(vacancy);
                        
                        vacancy.id = item["id"].ToString();

                    }
                }
                pageNum++;
            }

            vacancies.Sort((x, y) => x.AreaName.CompareTo(y.AreaName));
            return vacancies;

        }
    }

}
