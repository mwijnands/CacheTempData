using System.ComponentModel.DataAnnotations;

namespace XperiCode.CacheTempData.Sample.Models
{
    public class HomeIndexModel
    {
        [Required, Display(Name = "Some text")]
        public string Text { get; set; }
    }
}