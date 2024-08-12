namespace MVCProtectingSecrets.Models
{
    public class AllVehicles
    {
        public int Count { get; set; }
        public string Message { get; set; }
        public string SearchCriteria { get; set; }
        public List<Vehicle> Results { get; set; }
    }
}
