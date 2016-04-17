using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;
using HHAPIWebApp;
using NUnitLite;
using System.Net.Http;
using System.Net;

namespace UnitTests
{

    public class FakeResponseHandler : DelegatingHandler
    {
        private readonly Dictionary<Uri, HttpResponseMessage> _FakeResponses = new Dictionary<Uri, HttpResponseMessage>();

        public void AddFakeResponse(Uri uri, HttpResponseMessage responseMessage)
        {
            _FakeResponses.Add(uri, responseMessage);
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, System.Threading.CancellationToken cancellationToken)
        {
            if (_FakeResponses.ContainsKey(request.RequestUri))
            {
                return _FakeResponses[request.RequestUri];
            }
            else
            {
                return new HttpResponseMessage(HttpStatusCode.NotFound) { RequestMessage = request };
            }

        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
#if DNX451
            new AutoRun().Execute(args);
#else
       // new AutoRun().Execute(typeof(Program).GetTypeInfo().Assembly, Console.Out, Console.In, args);
#endif
        }
    }

    [TestFixture]
    public class HHApiUnitTests
    {
        [Test]
        // проверка на то, что будет вызвано исключение если передать не правильные параметры аутентификации
        public void GetFavoriteVacancies_MockClient_Ok()
        {
            var fakeResponseHandler = new FakeResponseHandler();
            fakeResponseHandler.AddFakeResponse(new Uri("https://api.hh.ru/vacancies/favorited"), new HttpResponseMessage(HttpStatusCode.OK));

            HttpClient mockClient = new HttpClient(fakeResponseHandler);

            HHApi hhapi = new HHApi(mockClient);
            //Assert.AreSame(List<Vacancy>,typeof(hhapi.GetFavoriteVacancies("12313", "13131", "12131616")));
        }
    }

}
