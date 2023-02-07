using System.ComponentModel.DataAnnotations;

namespace CurrencyViewer.Models
{
    internal class FavoriteCurrency
    {
        [Key]
        public string Code { get; set; }
        public string Name { get; set; }
    }
}
