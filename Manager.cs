using CsvHelper;
using System.Globalization;
using TrainingProject.General;

namespace TrainingProject
{
    public class Manager
    {
        public void FilterBookings(List<Booking> bookings, int flightNumber, double maxPrice, string? departureCountry, string? destinationCountry, DateTime departureDate, string? departureAirport, string? arrivalAirport, string? passengerName, FlightClassSelection flightClass)
        {
            var filteredBookings = bookings
                .Where(b => (flightNumber == 0 || b.Flight.FlightNumber == flightNumber) &&
                (maxPrice == 0 || b.Flight.ClassPrices.ContainsKey(b.Class) && b.Flight.ClassPrices[b.Class] <= maxPrice) &&
                (string.IsNullOrWhiteSpace(departureCountry) || b.Flight.DepartureCountry.Equals(departureCountry, StringComparison.InvariantCultureIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(destinationCountry) || b.Flight.DestinationCountry.Equals(destinationCountry, StringComparison.InvariantCultureIgnoreCase)) &&
                (departureDate == default || b.Flight.DepartureDate <= departureDate) &&
                (string.IsNullOrWhiteSpace(departureAirport) || b.Flight.DepartureAirport.Equals(departureAirport, StringComparison.InvariantCultureIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(arrivalAirport) || b.Flight.ArrivalAirport.Equals(arrivalAirport, StringComparison.InvariantCultureIgnoreCase)) &&
                (string.IsNullOrWhiteSpace(passengerName) || b.Passenger.Name.Equals(passengerName, StringComparison.InvariantCultureIgnoreCase)) &&
                (flightClass == 0 || b.Class == flightClass))
                .ToList();
            DisplayBookings(filteredBookings);
        }

        private static void DisplayBookings(List<Booking> filteredBookings)
        {
            var groupedBookings = filteredBookings.GroupBy(booking => booking.Flight.FlightNumber);

            foreach (var group in groupedBookings)
            {
                Console.WriteLine("\n");
                foreach (var booking in group)
                {
                    Console.WriteLine($"BookingId: {booking.BookingId}, Passenger: {booking.Passenger.Name}, Flight: {group.Key}, Class: {booking.Class}");
                }
                var firstBooking = group.First();
                Console.WriteLine($"\nFlightNum = {firstBooking.Flight.FlightNumber}, DepartureCountry = {firstBooking.Flight.DepartureCountry} , DestinationCountry = {firstBooking.Flight.DestinationCountry} ," +
                     $" DepartureDate = {firstBooking.Flight.DepartureDate.Date} , \nDepartureAirport = {firstBooking.Flight.DepartureAirport}, ArrivalAirport = {firstBooking.Flight.ArrivalAirport} ");

                foreach (var kvp in firstBooking.Flight.ClassPrices)
                {
                    Console.WriteLine($"Class: {kvp.Key}, Price: {kvp.Value}");
                }
                Console.WriteLine("\n");
            }
        }

        public List<Flight> ImportNewAvailableFlights(string flightsFilePath, List<Flight> flights, List<Flight> newAvailableFlights)
        {
            try
            {
                var uniqueFlights = newAvailableFlights.Except(flights, new FlightEqualityComparer()).ToList();

                if (!uniqueFlights.Any())
                {
                    Console.WriteLine("No new unique flights to import.");
                }
                else
                {
                    try
                    {
                        using (var writer = new StreamWriter(flightsFilePath, true))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            var fileInfo = new FileInfo(flightsFilePath);

                            if (!(fileInfo.Length > 0))
                            {
                                csv.WriteField("FlightNumber");
                                csv.WriteField("DepartureCountry");
                                csv.WriteField("DestinationCountry");
                                csv.WriteField("DepartureDate");
                                csv.WriteField("DepartureAirport");
                                csv.WriteField("ArrivalAirport");
                                csv.WriteField("economy");
                                csv.WriteField("business");
                                csv.WriteField("firstClass");
                                csv.NextRecord();
                            }

                            foreach (var flight in uniqueFlights)
                            {
                                csv.WriteField(flight.FlightNumber);
                                csv.WriteField(flight.DepartureCountry);
                                csv.WriteField(flight.DestinationCountry);
                                csv.WriteField(flight.DepartureDate.Date.ToString("yyyy-MM-dd"));
                                csv.WriteField(flight.DepartureAirport);
                                csv.WriteField(flight.ArrivalAirport);
                                csv.WriteField(flight.ClassPrices.GetValueOrDefault(FlightClassSelection.economy, 0.0));
                                csv.WriteField(flight.ClassPrices.GetValueOrDefault(FlightClassSelection.business, 0.0));
                                csv.WriteField(flight.ClassPrices.GetValueOrDefault(FlightClassSelection.firstClass, 0.0));
                                csv.NextRecord();
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error writing to CSV file: {ex.Message}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to CSV file: {ex.Message}");
            }


            return flights;
        }
    }
}
