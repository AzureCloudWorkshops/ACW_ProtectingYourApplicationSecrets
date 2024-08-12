using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MVCProtectingSecrets.Models
{
    public class Vehicle
    {
        [Display(Name = "MakeId")]
        public int Make_ID { get; set; }

        [Display(Name = "Make")]
        public string Make_Name { get; set; }

        [Display(Name = "ModelId")]
        public int Model_ID { get; set; }

        [Display(Name = "Model")]
        public string Model_Name { get; set; }
    }
}
