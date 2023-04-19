using ImageShare.Data;

namespace ImageShare.Web.Models
{
    public class ViewModel
    {
        public Image Image { get; set; }
        public bool WrongPassword { get; set; }
        public bool ShowPic { get; set; }
    }
}
