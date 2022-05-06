namespace HRD.Models
{
    /// <summary>
    /// A reservation response, returned by ReserveRoom
    /// </summary>
    public class ReservationResponse
    {
        public ReservationResult Status { get; set; }
        public int ReservationId { get; set; }
    }
}
