using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;
using HHAPIWebApp;
using NUnitLite;

namespace IntegrationTests
{
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
    public class HHApiIntegrationTests
    {
        [Test]
        // проверка на то, что будет вызвано исключение если передать не правильные параметры аутентификации
        public void GetUserInfo_EmptyParams_ThrowsException()
        {
            HHApi hhapi = new HHApi();
            Assert.Catch<AuthException>(() => hhapi.GetFavoriteVacancies("12313", "13131", "12131616"));
        }
    }

}
