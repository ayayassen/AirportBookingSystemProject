namespace TrainingProject
{
    public class Passenger
    {
        public string Name { get; set; }

        public List<Booking> BookFlight(Booking booking, List<Booking> allBookings)
        {
            Booking bookingFlight = new Booking
            {
                BookingId = booking.BookingId,
                Passenger = new Passenger { Name = booking.Passenger.Name },
                Flight = booking.Flight,
                Class = booking.Flight.Class,
            };

            allBookings.Add(bookingFlight);
            DisplayBookings(allBookings);
            return allBookings;
        }

        private static void DisplayBookings(List<Booking> filteredBookings)
        {
            foreach (var booking in filteredBookings)
            {
                Console.WriteLine("Booking Details : ");
                Console.WriteLine($"BookingId: {booking.BookingId}, Passenger: {booking.Passenger.Name}, Flight: {booking.Flight.FlightNumber}, Class: {booking.Class}\n");

            }
        }

        public List<Flight> SearchForAvailableFlights(List<Flight> flights, Flight flightToSearch)
        {
            var searchResults = flights.Where(f =>
                   f.ClassPrices.ContainsKey(flightToSearch.Class) &&
                   f.ClassPrices[flightToSearch.Class] <= flightToSearch.Price &&
                   f.DepartureCountry == flightToSearch.DepartureCountry &&
                   f.DestinationCountry == flightToSearch.DestinationCountry && 
                   f.DepartureDate.Date >= flightToSearch.DepartureDate.Date &&
                   f.DepartureAirport == flightToSearch.DepartureAirport &&
                   f.ArrivalAirport == flightToSearch.ArrivalAirport
                ).ToList();
            
            if (searchResults.Count > 0)
            {
                Console.WriteLine($"Available Flights = {searchResults.Count} : \n");
                Console.WriteLine($"For Class = {flightToSearch.Class}");
                foreach (Flight flight in searchResults)
                {
                    flight.Class = flightToSearch.Class;
                    Console.WriteLine($"FlightNum = {flight.FlightNumber}, DepartureCountry = {flight.DepartureCountry} , DestinationCountry = {flight.DestinationCountry} ," +
                        $"DepartureDate = {flight.DepartureDate} , DepartureAirport = {flight.DepartureAirport}, ArrivalAirport = {flight.ArrivalAirport} " +
                        $", Price = {flight.ClassPrices[flightToSearch.Class]}");
                    Console.WriteLine("\n");
                }
            }
            else Console.WriteLine("No available Flights");

            return searchResults;
        }

        public Flight SearchForAvailableFlightsByNum(List<Flight> searchFlights, int flightNum)
        {
            Flight selectedFlight = searchFlights.FirstOrDefault(flight => flight.FlightNumber.Equals(flightNum));
            if (selectedFlight == null)
            {
                Console.WriteLine($"Invalid Flight Number. Please enter a valid Flight Number {flightNum}.");
                return null;

            }
            else
            {
                return selectedFlight;
            }
        }

        public void ViewPersonalBookings(List<Booking> bookings, string passengerName)
        {
            List<Booking> foundBookings = bookings.FindAll(booking => string.Equals(booking.Passenger.Name, passengerName, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (!foundBookings.Any())
            {
                Console.WriteLine($"No bookings found for passenger {passengerName}.");
            }

            foreach (var book in foundBookings)
            {
                string flightNumber = book.Flight.FlightNumber.ToString();
                string passenger = book.Passenger.Name;

                Console.WriteLine($"BookingId = {book.BookingId}, Passenger = {passenger}, FlightNum = {flightNumber}, FlightClass = {book.Class}");
            }
        }

        public bool CancelBooking(List<Booking> bookings, string passengerName, int bookId)
        {
            bool isExist = bookings.Any(booking => string.Equals(booking.Passenger.Name, passengerName, StringComparison.InvariantCultureIgnoreCase)
            && booking.BookingId == bookId);

            if (!isExist)
            {
                Console.WriteLine("No bookings with this passenger name and booking ID are found.");
                return false;
            }

            bookings.RemoveAll(booking => string.Equals(booking.Passenger.Name, passengerName, StringComparison.InvariantCultureIgnoreCase)
            && booking.BookingId == bookId);
            Console.WriteLine($"Flight Booking with the Id = '{bookId}' is canceled.");
            return true;
        }

        public List<Booking>? ModifyABooking(List<Booking> bookings, Booking toUpdateBooking)
        {
            var newbooking = bookings.Find(booking => booking.BookingId == toUpdateBooking.BookingId && String.Equals(booking.Passenger.Name, toUpdateBooking.Passenger.Name, StringComparison.InvariantCultureIgnoreCase));
            if (newbooking == null)
            {
                return null;
            }
            newbooking.Flight = toUpdateBooking.Flight;
            newbooking.Class = toUpdateBooking.Class;

            return bookings;
        }
    }
}
