using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using HHAPIWebApp.Models;
using Newtonsoft.Json;

namespace HHAPIWebApp
{
    // Класс, предназначенный для работы с API сайта HH.ru
    static public class HHApi
    {
        // Возвращает свойства текущего пользователя HH.ru
        public static Dictionary<String, object> GetUserInfo(string Token, string UserId)
        {
            HttpClient client = new HttpClient();
            if (string.IsNullOrEmpty(Token)) throw new System.ArgumentException("Не задан параметр Token", "Token");
            if (string.IsNullOrEmpty(UserId)) throw new System.ArgumentException("Не задан параметр UserId", "UserId");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            client.DefaultRequestHeaders.Add("User-Agent", UserId + " / 1.0 (alexkmbk@gmail.com)");

            HttpResponseMessage response = client.GetAsync("https://api.hh.ru/me").Result;
            HttpContent content = response.Content;

            // ... Read the string.
            string strres = content.ReadAsStringAsync().Result;

             // ... Display the result.
            if (strres != null && strres.Length >= 50)
            {
                return JsonConvert.DeserializeObject<Dictionary<string, object>>(strres);

            };

            return null;
        }

        // Получение списка вакансий с сервера HH.ru
        public static List<Vacancy> GetFavoriteVacancies(string Token, string UserId, string searchString, bool openOnly=true)
        {
            // если  задана строка отбора, то приводим её к нижему регистру
            string searchStringLow = (searchString==null) ? null : searchString.ToLower();
  
            // ... Use HttpClient.
            HttpClient client = new HttpClient();
            if (string.IsNullOrEmpty(Token)) throw new System.ArgumentException("Не задан параметр Token", "Token");
            if (string.IsNullOrEmpty(UserId)) throw new System.ArgumentException("Не задан параметр UserId", "UserId");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            client.DefaultRequestHeaders.Add("User-Agent", UserId + " / 1.0 (alexkmbk@gmail.com)");

            HttpResponseMessage response;
            HttpContent content;
            string result;

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
                    // Обходим элементы Json массива
                    foreach (var item in a)
                    {
                        // Заполням свойства вакансии из элементов Json массива
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

                        // Применяем отбор если задан
                        // Строка отбора
                        if ((searchString != null) && (!vacancy.AreaName.ToLower().Contains(searchStringLow)))
                        {
                            skip = true;
                          }

                        // openOnly - признак указыващий что нужно вернуть только открытые вакансии
                        if (openOnly&&(vacancy.is_open== false))
                        {
                            skip = true;
                        }

                        // если вакансия не попала в список отфильтрованных - добавляем в коллекцию
                        if (!skip) vacancies.Add(vacancy);
                        
                        vacancy.id = item["id"].ToString();

                    }
                }
                pageNum++;
            }

            // Сортируем результат по региону
            vacancies.Sort((x, y) => x.AreaName.CompareTo(y.AreaName));
            return vacancies;

        }
    }

}
