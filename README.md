# HotelReservationDemo

*Challenge proposed by the company ALTEN*

## Environment
The application comes packaged with `swagger` which generates real-time docs for the API endpoints and also lets you experiment with them. Although it does it in a simple manner, for modifying headers you probably will require software such as `Postman`.

- The default endpoint for swagger locally is: http://localhost:62059/swagger/index.html
- The default endpoint for the API is: http://localhost:62059

You can start the webserver by debugging with using `Visual Studio` or the `dotnet core CLI`.

## Usage
The API already provides a room that can be booked in the hotel, there are currently no way to add more rooms, this could be added in a further version.

- ### Booking the room
	1) POST request to ```/Authentication/SignUp``` OR ```/Authentication/SignIn```
	2) GET request to ```/Reservation/Rooms```

		*note that __1)__ and __2)__ can be inverted as ```/Reservation/Rooms``` doesn't require an authentication*

	3) GET request to ```/Reservation/IsRoomAvailable```
	4) POST request to ```/Reservation/ReserveRoom```

- ### Modifying a reservation
	1) POST request to ```/Authentication/SignUp``` OR ```/Authentication/SignIn```
	2) GET request to ```/Reservation/Reservations```
	3) POST request to ```/Reservation/UpdateReservation```

- ### Cancelling a reservation
	1) POST request to ```/Authentication/SignUp``` OR ```/Authentication/SignIn```
	2) GET request to ```/Reservation/Reservations```
	3) POST request to ```/Reservation/DeleteReservation```

## Endpoints

### Authentication
- **[POST]** ```/Authentication/SignUp```
	- Attempts to register a new user in the system
	- Expected request:
		```json
		{
			"userName": "username", // the user's username
			"password": "password" // the user's password
		}
		```
	- Expected response:
		- Success:
			```json
			{
				"sessionToken": "5fgjp5&8^_:x25c$jmkdbb7kwdp61ja^", // the session token generated for your connection/session
				"userId": 8 // your user's technical id
			}
			```
		- Failure: no data
- **[POST]** ```/Authentication/SignIn```
	- Attempts to sign in an existing user to the system
	- Expected request:
		```json
		{
			"userName": "username", // the user's username
			"password": "password" // the user's password
		}
		```
	- Expected response:
		- Success:
			```json
			{
				"sessionToken": "5fgjp5&8^_:x25c$jmkdbb7kwdp61ja^", // the session token generated for your connection/session
				"userId": 8 // your user's technical id
			}
			```
		- Failure: no data

### Reservation
- **[GET]** ```/Reservation/Rooms```
	- Gets a list of all the rooms that exist in the hotel
	- Expected answer:
		```json
		[
			{
				"id": 1,
				"name": "ROOM A"
			},
			{
				"id": 2,
				"name": "ROOM B"
			},
			{
				"id": 3,
				"name": "ROOM C"
			}
		]
		```
	- Failure:
		```json
		[]
		```
- **[GET]** *(PROTECTED)* ```/Reservation/Reservations?userId=8```
	- Gets a list of all the reservations made by a user
	- Expected headers:
		- `Authorization`: The user's session token
	- Expected parameters:
		- `userid`: The technical id of the user
	- Expected answer:
		```json
		[
			{
				"roomId": 1, // the technical id of the room
				"startDate": "2022-06-08T02:00:00", // start date of the reservation, universal format
				"endDate": "2022-06-10T02:00:00", // end date of the reservation, universal format
				"userId": 8, // the technical id of the user making the reservation
				"id": 2 // the thechnical id of the reservation
			}
		]
		```
	- Failure:
		```json
		[]
		```
- **[POST]** *(PROTECTED)* ```/Reservation/UpdateReservation```
	- Updates an existing reservation
	- Expected headers:
		- `Authorization`: The user's session token
	- Expected request:
		```json
		{
			"roomId": 1, // the technical id of the room
			"startDate": "2022-06-08T00:00:00Z", // start date of the reservation, universal format
			"endDate": "2022-06-10T00:00:00Z", // end date of the reservation, universal format
			"userId": 8, // the technical id of the user making the reservation
		}
		```
	- Expected answer:
		- Success:
			```json
				true
			```
		- Failure:
			```json
				false
			```

- **[DELETE]** *(PROTECTED)* ```/Reservation/DeleteReservation?reservationId=1```
	- Deletes an existing reservation
	- Expected headers:
		- `Authorization`: The user's session token
	- Expected parameters:
		- `reservationId`: The technical id of the reservation
	- Expected answer:
		- Success:
			```json
				true
			```
		- Failure:
			```json
				false
			```
- **[GET]** ```/Reservation/IsRoomAvailable?roomId=1&startDate=2022-06-10T00:00:00Z&endDate=2022-06-11T00:00:00Z```
	- Verifies if a reservation for a room is possible during a specific time range
	- Expected parameters:
		- `roomId`: The technical id of the room to check availability for
		- `startDate`: The start date at which to check the availability for
		- `endDate`: The end date at which to check the availability for
	- Expected answer:
		- Success:
			```json
				true
			```
		- Failure:
			```json
				false
			```
- **[POST]** *(PROTECTED)* ```/Reservation/ReserveRoom```
	- Attempts to make a reservation
	- Expected headers:
		- `Authorization`: The user's session token
	- Expected request:
		```json
		{
			"roomId": 1, // the technical id of the room
			"startDate": "2022-06-08T00:00:00Z", // start date of the reservation, universal format
			"endDate": "2022-06-10T00:00:00Z", // end date of the reservation, universal format
			"userId": 8, // the technical id of the user making the reservation
		}
		```
	- Expected answer:
		- Success:
			```json
				{

					"status": 1, // the status of the reservation
					// Unknown = 0
					// Success = 1
					// TechnicalError = 2
					// RoomNotAvailable = 3
					// RoomDoesNotExist = 4
					// InvalidReservationDates = 5
					// ReservationTooEarly = 6
					// ReservationTooLong = 7
					"reservationId": 2 // the technical id of the reservation
				}
			```
		- Failure:
			```json
				{

					"status": 2, // the status of the reservation
					// Unknown = 0
					// Success = 1
					// TechnicalError = 2
					// RoomNotAvailable = 3
					// RoomDoesNotExist = 4
					// InvalidReservationDates = 5
					// ReservationTooEarly = 6
					// ReservationTooLong = 7
					"reservationId": -1 // invalid technical id
				}
			```
