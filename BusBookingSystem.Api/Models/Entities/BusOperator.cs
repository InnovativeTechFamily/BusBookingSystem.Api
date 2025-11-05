namespace BusBookingSystem.Api.Models.Entities
{
    public class BusOperator
    {
        public string OperatorId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public decimal Rating { get; set; }
        public int TotalReviews { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<Bus> Buses { get; set; }
    }




}
