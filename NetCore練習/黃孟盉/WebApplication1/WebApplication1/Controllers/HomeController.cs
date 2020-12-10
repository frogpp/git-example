using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using WebApplication1.Models;
using static WebApplication1.Startup;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly IConfiguration _config;
        private readonly ISample _transient;
        private readonly ISample _scoped;
        private readonly ISample _singleton;
        public Config configModel { get; set; }

        public HomeController(
            ITransient transient,
            IScoped scoped,
            ISingleton singleton,
            IConfiguration config,
            IOptions<Config> memberConfig)
        {
            _transient = transient;
            _scoped = scoped;
            _singleton = singleton;
            _config = config;
            configModel = memberConfig.Value;
        }
        public string Index()
        {
            var configValue = _config.GetValue<string>("ConfigValue");
            var url = _config.GetValue<string>("DemoUrl:Google");
            return $"{configModel.Name},{configModel.PhoneNumber.Tel},{configModel.PhoneNumber.Phone},{configModel.Address}.{configModel.Email},{configModel.Age},{configModel.IsActive}\r\n{configValue}\r\n{url}";
        }
        //public IActionResult Index()
        //{
        //    ViewBag.TransientId = _transient.Id;
        //    ViewBag.TransientHashCode = _transient.GetHashCode();

        //    ViewBag.ScopedId = _scoped.Id;
        //    ViewBag.ScopedHashCode = _scoped.GetHashCode();

        //    ViewBag.SingletonId = _singleton.Id;
        //    ViewBag.SingletonHashCode = _singleton.GetHashCode();


        //    return View();
        //}

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
