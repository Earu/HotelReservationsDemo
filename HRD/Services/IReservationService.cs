using HRD.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRD.Services
{
    public interface IReservationService
    {
        Task<List<HotelRoom>> GetRooms();
        Task<List<Reservation>> GetUserReservations(int userId);
        Task<ReservationResult> ValidateReservationInputs(int roomId, DateTime startDate, DateTime endDate);
        Task<Reservation> GetReservationAsync(int reservationId);
        Task<bool> UpdateReservationAsync(int userId, Reservation reservation);
        Task<bool> DeleteReservationAsync(int userId, int reservationId);
        Task<ReservationResponse> ReserveRoomAsync(Reservation reservation);
    }
}
