using System;

namespace HRD.Models
{
    /// <summary>
    /// A room reservation made by a user
    /// </summary>
    public class Reservation
    {
        public int RoomId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int UserId { get; set; }
        public int Id { get; set; }
    }
}
