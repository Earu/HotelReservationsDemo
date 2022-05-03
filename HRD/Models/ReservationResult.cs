namespace HRD.Models
{
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
