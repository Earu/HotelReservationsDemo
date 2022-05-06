using HRD.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Threading.Tasks;

namespace HRD.Services
{
    /// <summary>
    /// The service class for handling reservations its related functions
    /// 
    /// DISCLAIMER: 
    /// Instead of using prepared SQL statements this could have used Entity Framework, 
    /// however considering the size of the project I did not think it was worth it
    /// </summary>
    public class ReservationService : IReservationService
    {
        private const int MINIMUM_DAYS_BEFORE_RESERVATION = 30;
        private const int MAXIMUM_RESERVATION_LENGTH = 3;
        private const int MINIMUM_RESERVATION_LENGTH = 1;

        private readonly IDatabaseService Database;
        private readonly ILogger<ReservationService> Logger;

        public ReservationService(IDatabaseService database, ILogger<ReservationService> logger)
        {
            this.Database = database;
            this.Logger = logger;
        }

        /// <summary>
        /// Gets all the rooms, optionally can be filtered 
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<List<HotelRoom>> GetRooms()
        {
            List<HotelRoom> rooms = new();

            using (SQLiteCommand command = await this .Database.CreateCommandAsync())
            {
                command.CommandText = "SELECT Id, Name FROM Rooms;";
                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        rooms.Add(new HotelRoom
                        {
                            Id = reader.GetInt32(0),
                            Name = reader.GetString(1),
                        });
                    }
                }
            }

            return rooms;
        }

        /// <summary>
        /// Gets all the reservations for a user
        /// </summary>
        /// <param name="userId">The technical id of the user</param>
        /// <returns>The user reservations</returns>
        public async Task<List<Reservation>> GetUserReservations(int userId)
        {
            List<Reservation> reservations = new();

            using (SQLiteCommand command = await this.Database.CreateCommandAsync())
            {
                command.CommandText = "SELECT RoomId, StartDate, EndDate, UserId, Id FROM Reservations WHERE UserId = @userid;";
                command.Parameters.AddWithValue("@userid", userId);

                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        reservations.Add(new Reservation
                        {
                            RoomId = reader.GetInt32(0),
                            StartDate = reader.GetDateTime(1),
                            EndDate = reader.GetDateTime(2),
                            UserId = reader.GetInt32(3),
                            Id = reader.GetInt32(4),
                        });
                    }
                }
            }

            return reservations;
        }

        /// <summary>
        /// Attempts to get the reservation for the specified reservation id
        /// </summary>
        /// <param name="reservationId">The technical id of the reservation</param>
        /// <returns>The reservation if any was found</returns>
        public async Task<Reservation> GetReservationAsync(int reservationId)
        {
            using (SQLiteCommand command = await this.Database.CreateCommandAsync())
            {
                command.CommandText = "SELECT RoomId, StartDate, EndDate, UserId, Id FROM Reservations WHERE ReservationId = @reservationid;";
                command.Parameters.AddWithValue("@reservationid", reservationId);

                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    if (!reader.HasRows) return null;

                    await reader.ReadAsync();

                    // if first column is null then we could not find anything
                    if (reader.IsDBNull(0)) return null;

                    return new Reservation
                    {
                        RoomId = reader.GetInt32(0),
                        StartDate = reader.GetDateTime(1),
                        EndDate = reader.GetDateTime(2),
                        UserId = reader.GetInt32(3),
                        Id = reader.GetInt32(4),
                    };
                }
            }
        }

        /// <summary>
        /// Deletes a reservation
        /// </summary>
        /// <param name="reservationId">The technical id of the reservation</param>
        /// <returns>Was the deletion successful</returns>
        public async Task<bool> DeleteReservationAsync(int userId, int reservationId)
        {
            using (SQLiteCommand command = await this.Database.CreateCommandAsync())
            {
                command.CommandText = "DELETE FROM Reservations ReservationId = @reservationid AND UserId = @userid;";
                command.Parameters.AddWithValue("@reservationid", reservationId);
                command.Parameters.AddWithValue("@userid", userId);

                return await command.ExecuteNonQueryAsync() >= 1;
            }
        }

        /// <summary>
        /// Updates an existing reservation
        /// </summary>
        /// <param name="userId">The technical id of the user making the update</param>
        /// <param name="reservation">The concerned reservation</param>
        /// <returns>Was the update successful?</returns>
        public async Task<bool> UpdateReservationAsync(int userId, Reservation reservation)
        {
            using (SQLiteCommand command = await this.Database.CreateCommandAsync())
            {
                command.CommandText = "UPDATE Reservations SET RoomId = @roomid, StartDate = @startdate, EndDate = @enddate WHERE Id = @reservationid AND UserId = @userid;";
                command.Parameters.AddWithValue("@roomid", reservation.RoomId);
                command.Parameters.AddWithValue("@startdate", reservation.StartDate);
                command.Parameters.AddWithValue("@enddate", reservation.EndDate);
                command.Parameters.AddWithValue("@reservationid", reservation.Id);
                command.Parameters.AddWithValue("@userid", userId);

                return await command.ExecuteNonQueryAsync() >= 1; // this should always be 1, but in production if theres a bug that duplicates reservations you never know
            }
        }

        /// <summary>
        /// Gets whether a room is available or not
        /// </summary>
        /// <param name="roomId">The technical id of the room</param>
        /// <param name="startDate">The start date for the reservation</param>
        /// <param name="endDate">The end date for the reservation</param>
        /// <returns>Is the room available</returns>
        public async Task<bool> IsRoomAvailable(int roomId, long startDate, long endDate)
        {
            using (SQLiteCommand command = await this.Database.CreateCommandAsync())
            {
                command.CommandText = "SELECT Rooms.Id "
                    + "FROM Reservations, Rooms "
                    + "WHERE Reservations.RoomId = Rooms.Id "
                    + "AND ((StartDate < @enddate AND StartDate >= @startdate) OR(EndDate < @enddate AND EndDate >= @startdate)) "
                    + "AND ((@startdate < EndDate AND @startdate >= StartDate) OR(@enddate < EndDate AND @enddate >= StartDate));";

                command.Parameters.AddWithValue("@startdate", startDate);
                command.Parameters.AddWithValue("@enddate", endDate);

                using (DbDataReader reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        if (reader.GetInt32(0) == roomId) return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// Validate the inputs according to the business rules for a reservation
        /// </summary>
        /// <param name="roomId">The technical id of the room</param>
        /// <param name="startDate">The start date of the reservation</param>
        /// <param name="endDate">The end date of the reservation</param>
        /// <returns>The result, error, valid, etc...</returns>
        private async Task<ReservationResult> ValidateReservationInputs(int roomId, long startDate, long endDate)
        {
            if (endDate < startDate) return ReservationResult.InvalidReservationDates; // end cant be before start
            if (startDate > endDate) return ReservationResult.InvalidReservationDates; // start cant be before end

            if (startDate < DateTime.Today.AddDays(MINIMUM_DAYS_BEFORE_RESERVATION).Ticks) return ReservationResult.ReservationTooEarly; // we cant reserve less than 30 days before occupying the room
            if (new TimeSpan(endDate - startDate).TotalDays > MAXIMUM_RESERVATION_LENGTH) return ReservationResult.ReservationTooLong; // we cant reserve for longer than 3 days

            bool isAvailable = await this.IsRoomAvailable(roomId, startDate, endDate); // check whether there is any reservations that would make the room unavailable
            if (!isAvailable) return ReservationResult.RoomNotAvailable;

            List<HotelRoom> existingRooms = await this.GetRooms();
            bool roomExists = existingRooms.Any(room => room.Id == roomId);
            if (!roomExists) return ReservationResult.RoomDoesNotExist;

            return ReservationResult.Success;
        }

        /// <summary>
        /// Reserves a room for a specified duration
        /// </summary>
        /// <param name="roomId">The technical id of the room</param>
        /// <param name="startDate">The start date of the reservation</param>
        /// <param name="endDate">The end date of the reservation</param>
        /// <returns>The result, error, valid, etc...</returns>
        public async Task<ReservationResponse> ReserveRoomAsync(Reservation reservation)
        {
            try
            {
                ReservationResult validationResult = await this.ValidateReservationInputs(reservation.RoomId, reservation.StartDate.Ticks, reservation.EndDate.Ticks);
                if (validationResult != ReservationResult.Success)
                {
                    return new ReservationResponse
                    {
                        ReservationId = -1,
                        Status = validationResult,
                    };
                }

                DateTime sanitizedStartDate = reservation.StartDate.Date;
                DateTime sanitizedEndDate = reservation.EndDate.Date;

                // if the reservation length is under the minimum length, standardize it to the minimum
                if (sanitizedEndDate < sanitizedStartDate.AddDays(MINIMUM_RESERVATION_LENGTH))
                    sanitizedEndDate = sanitizedStartDate.AddDays(MINIMUM_RESERVATION_LENGTH);

                using (SQLiteCommand command = await this.Database.CreateCommandAsync())
                {
                    command.CommandText = "INSERT INTO Reservations (RoomId, StartDate, EndDate, UserId) VALUES (@roomid, @startdate, @enddate, @userid);";
                    command.Parameters.AddWithValue("@roomid", reservation.RoomId);
                    command.Parameters.AddWithValue("@startdate", sanitizedStartDate);
                    command.Parameters.AddWithValue("@enddate", sanitizedEndDate);
                    command.Parameters.AddWithValue("@userid", reservation.UserId);

                    if (await command.ExecuteNonQueryAsync() == 1) // make sure the database was updated with the new reservation
                    {
                        return new ReservationResponse
                        {
                            ReservationId = (int)this.Database.LastInsertedId,
                            Status = ReservationResult.Success,
                        };

                    }
                }
            }
            catch (Exception ex)
            {
                this.Logger.LogError(ex.ToString());
            }

            return new ReservationResponse
            {
                ReservationId = -1,
                Status = ReservationResult.TechnicalError,
            };
        }
    }
}
