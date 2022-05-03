namespace HRD.Models
{
    public class Reservation
    {
        public int RoomId { get; set; }
        public long StartDate { get; set; }
        public long EndDate { get; set; }
        public int UserId { get; set; }
        public int Id { get; set; }
    }
}
