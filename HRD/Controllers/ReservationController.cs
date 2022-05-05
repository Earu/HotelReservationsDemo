using HRD.Models;
using HRD.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HRD.Controllers
{
    /// <summary>
    /// DISCLAIMER:
    /// Some of the methods here would typically use the [Authorize] attribute to guarantee that the user is logged in when accessing
    /// the page, or API endpoint, however here since the login is "home-made", its a bit different. So for simplicity's sake I'm checking in 
    /// the methods themselves.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly ILogger<ReservationController> Logger;
        private readonly IReservationService Reservations;
        private readonly ISessionService Sessions;

        public ReservationController(IReservationService reservations, ISessionService sessions,  ILogger<ReservationController> logger)
        {
            this.Logger = logger;
            this.Reservations = reservations;
            this.Sessions = sessions;
        }

        /// <summary>
        /// Gets all the rooms available in the hotel
        /// </summary>
        /// <returns>The rooms</returns>
        [HttpGet]
        [Route("/Rooms")]
        public async Task<List<HotelRoom>> GetRooms()
        {
            this.Logger.LogInformation("/Rooms");

            try
            {
                return await this.Reservations.GetRooms();
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.ToString());
                this.Response.StatusCode = StatusCodes.Status500InternalServerError;

                return null;
            }
        }

        /// <summary>
        /// Gets all the reservations made by the specified user
        /// </summary>
        /// <param name="userId">The technical id of the user</param>
        /// <returns>The reservations</returns>
        [HttpGet]
        [Route("/Reservations")]
        public async Task<List<Reservation>> GetReservations(int userId)
        {
            this.Logger.LogInformation($"/Reservations => userid: {userId}");

            try
            {
                return await this.Reservations.GetUserReservations(userId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.ToString());
                this.Response.StatusCode = StatusCodes.Status500InternalServerError;

                return null;
            }
        }

        /// <summary>
        /// Updates a user's reservation
        /// </summary>
        /// <param name="reservation">The updated reservation</param>
        /// <returns>Was the update successful</returns>
        [HttpPost]
        [Route("/UpdateReservation")]
        public async Task<bool> UpdateReservation(Reservation reservation)
        {
            string token = this.Request.Headers["Authorization"];
            this.Logger.LogInformation($"/UpdateReservation => reservationid: {reservation.Id}");

            try
            {
                if (!this.Sessions.TryGetUserSession(token, out int userId)) return false;

                return await this.Reservations.UpdateReservationAsync(userId, reservation);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.ToString());
                this.Response.StatusCode = StatusCodes.Status500InternalServerError;

                return false;
            }
        }

        /// <summary>
        /// Deletes a user's reservation
        /// </summary>
        /// <param name="reservationId">The technical id of the reservation</param>
        /// <returns>Was the deletion successful</returns>
        [HttpDelete]
        [Route("/DeleteReservation")]
        public async Task<bool> DeleteReservation(int reservationId)
        {
            string token = this.Request.Headers["Authorization"];
            this.Logger.LogInformation($"/DeleteReservation => reservationid: {reservationId}");

            try
            {
                if (!this.Sessions.TryGetUserSession(token, out int userId)) return false;

                return await this.Reservations.DeleteReservationAsync(userId, reservationId);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.ToString());
                this.Response.StatusCode = StatusCodes.Status500InternalServerError;

                return false;
            }
        }

        /// <summary>
        /// Checks whether a room is available to be reserved
        /// </summary>
        /// <param name="roomId">The technical id of the room</param>
        /// <param name="startDate">The start date for a reservation</param>
        /// <param name="endDate">The end date for a reservation</param>
        /// <returns>Is the room available for the duration?</returns>
        [HttpGet]
        [Route("/IsRoomAvailable")]
        public async Task<bool> IsRoomAvailable(int roomId, long startDate, long endDate)
        {
            this.Logger.LogInformation("/IsRoomAvailable");

            try
            {
                return await this.Reservations.IsRoomAvailable(roomId, startDate, endDate);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.ToString());
                this.Response.StatusCode = StatusCodes.Status500InternalServerError;

                return false;
            }
        }

        /// <summary>
        /// Reserves the room selected by the user
        /// 
        /// DISCLAIMER:
        /// Because I'm following the REST principles, this is a POST request, 
        /// it could have been anything else
        /// </summary>
        /// <param name="reservation">The reservation made by the user</param>
        /// <returns>Is the reservation successful?</returns>
        [HttpPost]
        [Route("/ReserveRoom")]
        public async Task<ReservationResult> ReserveRoom(Reservation reservation)
        {
            string token = this.Request.Headers["Authorization"];
            this.Logger.LogInformation("/ReserverRoom");

            try
            {
                if (!this.Sessions.TryGetUserSession(token, out int userId)) return ReservationResult.TechnicalError; // might as well not tell the user anything useful in case of a security breach

                return await this.Reservations.ReserveRoomAsync(reservation);
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.ToString());
                this.Response.StatusCode = StatusCodes.Status500InternalServerError;

                return ReservationResult.TechnicalError;
            }
        }
    }
}
