namespace HRD.Models
{
    /// <summary>
    /// The different possible results for a reservation request
    /// </summary>
    public enum ReservationResult
    {
        Unknown,
        Success,
        TechnicalError,
        RoomNotAvailable,
        RoomDoesNotExist,
        InvalidReservationDates,
        ReservationTooEarly,
        ReservationTooLong,
    }
}
