using ImageShare.Data;
using ImageShare.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Text.Json;

namespace ImageShare.Web.Controllers
{
    public class HomeController : Controller
    {
        private string _connectionString = @"Data Source=.\sqlexpress;Initial Catalog=ImageShare; Integrated Security=true;";

        private IWebHostEnvironment _webHostEnvironment;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Upload(IFormFile image, string password)
        {
            var fileName = $"{Guid.NewGuid()}-{image.FileName}";
            var filePath = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", fileName);
            using var fs = new FileStream(filePath, FileMode.CreateNew);
            image.CopyTo(fs);

            var mgr = new Manager(_connectionString);
            var img = new Image
            {
                Password = password,
                FileName = fileName,
            };
            mgr.Add(img);
            var vm = new ViewModel
            {
                Image = img
            };
            return View(vm);
        }
        public IActionResult ViewImage(int id, string password)
        {
            var mgr = new Manager(_connectionString);
            var vm = new ViewModel();
            vm.Image = mgr.GetImageById(id);
            List<int> list = HttpContext.Session.Get<List<int>>("ids");
            if(list != null)
            {
                if (list.Contains(id))
                {
                    vm.ShowPic = true;
                    mgr.IncrementViews(vm.Image.Views, id);
                    vm.Image.Views++;
                    return View(vm);
                }
            }
            if (password != null)
            {
                if(password == vm.Image.Password)
                {
                    if(list == null)
                    {
                        list = new List<int>();
                    }
                    list.Add(id);
                    vm.ShowPic = true;
                    mgr.IncrementViews(vm.Image.Views, id);
                    vm.Image.Views++;
                    HttpContext.Session.Set("ids", list);
                }
                else
                {
                    vm.WrongPassword = true;
                }
            }
            return View(vm);
        }
    }

    public static class SessionExtensions
    {
        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize(value));
        }
        public static T Get<T>(this ISession session, string key)
        {
            string value = session.GetString(key);

            return value == null ? default(T) :
                JsonSerializer.Deserialize<T>(value);
        }
    }
}




































