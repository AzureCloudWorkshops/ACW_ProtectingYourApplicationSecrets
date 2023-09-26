using System.ComponentModel.DataAnnotations.Schema;

namespace MVCProtectingSecrets.Models
{
    public class ImageDetail
    {
        public int Id { get; set; }
        public string FileName { get; set; }
        public string Description { get; set; }
        public string AltText { get; set; }

        [NotMapped]
        public string ImageFullPath { get; set; }
    }
}
