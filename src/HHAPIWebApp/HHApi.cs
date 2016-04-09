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
        // Возвращает свойства текущего пользователя HH.ru в виде словаря свойств
        // Параметры:
        //      Token - текстовый token, можно получить на сайте hh.ru при подлючении приложения в профиле
        //      UserId - текстовый идентификатор пользователя, можно получить на сайте hh.ru при подлючении приложения в профиле
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

        // Возвращает свойства вакансии в виде словаря свойств
        // Параметры:
        //      Token - текстовый token, можно получить на сайте hh.ru при подлючении приложения в профиле
        //      UserId - текстовый идентификатор пользователя, можно получить на сайте hh.ru при подлючении приложения в профиле
        //      VacancyId - текстовый идентификатор вакансии
        public static Dictionary<String, object> GetVacancyInfo(string Token, string UserId, string VacancyId)
        {
            HttpClient client = new HttpClient();
            if (string.IsNullOrEmpty(Token)) throw new System.ArgumentException("Не задан параметр Token", "Token");
            if (string.IsNullOrEmpty(UserId)) throw new System.ArgumentException("Не задан параметр UserId", "UserId");
            if (string.IsNullOrEmpty(VacancyId)) throw new System.ArgumentException("Не задан параметр VacancyId", "VacancyId");

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
            client.DefaultRequestHeaders.Add("User-Agent", UserId + " / 1.0 (alexkmbk@gmail.com)");

            HttpResponseMessage response = client.GetAsync($"https://api.hh.ru//vacancies/{VacancyId}").Result;
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

        // Возвращает список отобранных вакансий с сервера HH.ru
        // Параметры:
        //      Token - текстовый token, можно получить на сайте hh.ru при подлючении приложения в профиле
        //      UserId - текстовый идентификатор пользователя, можно получить на сайте hh.ru при подлючении приложения в профиле
        //      searchString - строка отбора, если задана, то функция возвращает только те вакансии, в свойствах которых присутствует
        //                      указанный текст отбора
        //      openOnly - если задано в значение true, то функция вернет только открытые вакансии
        //
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
                    break;
                }
                content = response.Content;

                result = content.ReadAsStringAsync().Result;
                if (result == null)
                {
                    throw new System.Exception("Ошибка при получении списка отобранных вакансий в текстовом виде.");
                }
                else
                {
                    JObject json = JObject.Parse(result);
                    JArray a = (JArray)json["items"];
                    // Если количество полученных вакансий равно нулю, значит дошли до предела списка
                    // и поэтому выходим из списка
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
                            vacancy.is_open = (item["archived"].ToString() != "True");
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
                        
                        vacancy.id = item["id"].ToString();

                        // если вакансия не попала в список отфильтрованных - добавляем в коллекцию
                        if (!skip) vacancies.Add(vacancy);


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
