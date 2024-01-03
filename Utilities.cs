using TrainingProject.General;
using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;

namespace TrainingProject
{
    internal class Utilities
    {

        private static string flightsFilePath = "C:\\Users\\fts\\source\\repos\\AirportSystemProject\\FlightFile.csv";
        private static string bookingsFilePath = "C:\\Users\\fts\\source\\repos\\AirportSystemProject\\BookingsFile.csv";

        private static List<Flight> flights = new();
        private static List<Booking> bookings = new();

        private static List<String> errors = new();

        private static Passenger passenger = new Passenger();
        private static Manager manager = new Manager();

        private static string passengerName = "";

        public static void LoadData()
        {
            flights = LoadFlightsFromCsvFile(flightsFilePath);
            bookings = LoadBookingsFromCsvFile(bookingsFilePath);
        }

        public static List<Flight> LoadFlightsFromCsvFile(string filePath)
        {
            var flights = new List<Flight>();

            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Delimiter = ",",
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                HasHeaderRecord = true
            };

            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, csvConfiguration))
                {
                    csv.Read();
                    csv.ReadHeader();

                    while (csv.Read())
                    {
                        var flight = new Flight();

                        if (!string.IsNullOrWhiteSpace(csv.GetField(nameof(flight.FlightNumber))))
                        {
                            if (int.TryParse(csv.GetField("FlightNumber"), out var flightNumber))
                            {
                                flight.FlightNumber = flightNumber;
                            }
                            else
                            {
                                errors.Add($"Invalid FlightNumber value: {csv.GetField("FlightNumber")}");
                            }
                        }

                        if (string.IsNullOrWhiteSpace(csv.GetField(nameof(flight.DepartureCountry))))
                        {
                            errors.Add("Invalid DepartureCountry, DepartureCountry must not be empty");
                        }
                        else
                        {
                            flight.DepartureCountry = csv.GetField("DepartureCountry");
                        }

                        if (string.IsNullOrWhiteSpace(csv.GetField(nameof(flight.DestinationCountry))))
                        {
                            errors.Add("Invalid DestinationCountry value, DestinationCountry must not be empty");
                        }
                        else
                        {
                            flight.DestinationCountry = csv.GetField("DestinationCountry");
                        }

                        string departureDateString = csv.GetField(nameof(flight.DepartureDate));
                        if (string.IsNullOrWhiteSpace(departureDateString))
                        {
                            errors.Add("DepartureDate is empty");
                        }
                        else
                        {
                            if (DateTime.TryParse(departureDateString, out DateTime departureDate))
                            {
                                flight.DepartureDate = departureDate.Date;
                            }
                            else
                            {
                                errors.Add($"Invalid DepartureDate value: {departureDateString}");
                            }
                        }

                        if (string.IsNullOrWhiteSpace(csv.GetField(nameof(flight.DepartureAirport))))
                        {
                            errors.Add("Invalid DepartureAirport value, DepartureAirport must not be empty");
                        }
                        else
                        {
                            flight.DepartureAirport = csv.GetField("DepartureAirport");
                        }

                        if (string.IsNullOrWhiteSpace(nameof(flight.ArrivalAirport)))
                        {
                            errors.Add("Invalid ArrivalAirport value, ArrivalAirport must not be empty");
                        }
                        else
                        {
                            flight.ArrivalAirport = csv.GetField("ArrivalAirport");
                        }

                        if (!string.IsNullOrWhiteSpace(csv.GetField("economy")))
                        {
                            if (double.TryParse(csv.GetField("economy"), out var economyValue))
                            {
                                flight.ClassPrices.Add(FlightClassSelection.economy, economyValue);
                            }
                            else
                            {
                                errors.Add($"Invalid economy value: {csv.GetField("economy")}");
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(csv.GetField("business")))
                        {
                            if (double.TryParse(csv.GetField("business"), out var economyValue))
                            {
                                flight.ClassPrices.Add(FlightClassSelection.business, economyValue);
                            }
                            else
                            {
                                errors.Add($"Invalid business value: {csv.GetField("business")}");
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(csv.GetField("firstClass")))
                        {
                            if (double.TryParse(csv.GetField("firstClass"), out var economyValue))
                            {
                                flight.ClassPrices.Add(FlightClassSelection.firstClass, economyValue);
                            }
                            else
                            {
                                errors.Add($"Invalid firstClass value: {csv.GetField("firstClass")}");
                            }
                        }
                        var flightValidator = new FlightValidator().Validate(flight);

                        if (flightValidator.IsValid)
                        {
                            flights.Add(flight);
                            continue;
                        }

                        flightValidator.Errors.ForEach(error => { Console.WriteLine(error); });

                        errors.ForEach(error => Console.WriteLine(error));
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV file: {ex.Message}");
            }

            return flights;
        }

        public static List<Booking> LoadBookingsFromCsvFile(string filePath)
        {
            try
            {
                using (var reader = new StreamReader(filePath))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    csv.Read();
                    csv.ReadHeader();

                    var bookings = new List<Booking>();

                    while (csv.Read())
                    {
                        var booking = new Booking
                        {
                            BookingId = csv.GetField<int>("BookId"),
                            Passenger = new Passenger { Name = csv.GetField("PassengerName") },
                            Class = Enum.Parse<FlightClassSelection>(csv.GetField("Class"), true)
                        };

                        int flightNum;
                        if (int.TryParse(csv.GetField("FlightNum"), out flightNum))
                        {
                            booking.Flight = passenger.SearchForAvailableFlightsByNum(flights, flightNum);
                        }

                        bookings.Add(booking);
                    }

                    return bookings;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading CSV file: {ex.Message}");
                return new List<Booking>();
            }
        }

        public static void SaveBookingsToFile(List<Booking> bookings, string filePath)
        {
            try
            {
                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture)))
                {
                    csv.WriteField("BookId");
                    csv.WriteField("PassengerName");
                    csv.WriteField("FlightNum");
                    csv.WriteField("Class");
                    csv.NextRecord();

                    foreach (var booking in bookings)
                    {
                        csv.WriteField(booking.BookingId);
                        csv.WriteField(booking.Passenger.Name);
                        csv.WriteField(booking.Flight.FlightNumber);
                        csv.WriteField(booking.Class.ToString());
                        csv.NextRecord();
                    }
                }

                Console.WriteLine("Bookings have been successfully saved to the file.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving bookings to file: {ex.Message}");
            }
        }

        internal static void DeleteBookingFromFile(string filePath, int bookingId)
        {
            try
            {
                var existingBookings = LoadBookingsFromCsvFile(filePath);

                var bookingToRemove = existingBookings.FirstOrDefault(b => b.BookingId == bookingId);

                if (bookingToRemove == null)
                {
                    Console.WriteLine($"Booking with ID {bookingId} not found in the file.");
                }
                else
                {
                    existingBookings.Remove(bookingToRemove);
                    SaveBookingsToFile(existingBookings, bookingsFilePath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting booking from file: {ex.Message}");
            }
        }

        internal static void PassengerOrManagerMenu()
        {
            Console.ResetColor();
            Console.Clear();
            Console.WriteLine("********************");
            Console.WriteLine("* Enter the User Type *");
            Console.WriteLine("********************");

            Console.WriteLine("1: Passenger ");
            Console.WriteLine("2: Manager ");
            Console.WriteLine("0: Close application");

            var exit = false;

            do
            {
                Console.Write("Your selection: ");

                if (Enum.TryParse(Console.ReadLine(), out UserType userType))
                {
                    switch (userType)
                    {
                        case UserType.passenger:
                            ShowMainMenuForPassenger();
                            break;
                        case UserType.manager:
                            ShowMainMenuForManager();
                            break;
                        case UserType.exit:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("\nInvalid selection. Please try again.");
                            break;
                    }
                }
            } while (!exit);
        }

        private static void ShowMainMenuForManager()
        {
            var exit = false;
            do
            {
                Console.WriteLine("********************");
                Console.WriteLine("* Select an action *");
                Console.WriteLine("********************");

                Console.WriteLine("1: Filter For A Flight");
                Console.WriteLine("2: Import New Available Flights");
                Console.WriteLine("3: Back to Passenger Or Manager Menu");
                Console.WriteLine("0: Close application");

                Console.Write("Your selection: ");

                if (Enum.TryParse(Console.ReadLine(), out ManagerBookingsManageSelection managerBookingsManageSelection))
                {
                    switch (managerBookingsManageSelection)
                    {
                        case ManagerBookingsManageSelection.filterBooking:
                            FilterBookings();
                            break;
                        case ManagerBookingsManageSelection.importNewAvailableFlights:
                            ImportNewAvailableFlights();
                            break;
                        case ManagerBookingsManageSelection.backToPassengerMenu:
                            PassengerOrManagerMenu();
                            break;
                        case ManagerBookingsManageSelection.exit:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("\nInvalid selection. Please try again.");
                            Console.Write("Your selection: ");
                            break;
                    }
                }
            } while (!exit);
        }

        private static void ImportNewAvailableFlights()
        {
            Console.WriteLine("Enter the File Path");
            var newFlightsFilePath = Console.ReadLine();
            if (newFlightsFilePath == null)
            {
                Console.WriteLine("File Path Cannot be null");
            }
            var newAvailableFlights = LoadFlightsFromCsvFile(newFlightsFilePath);
            manager.ImportNewAvailableFlights(flightsFilePath, flights, newAvailableFlights);
            LoadFlightsFromCsvFile(flightsFilePath);
        }

        private static void FilterBookings()
        {
            int flightNumber;
            double maxPrice;

            Console.WriteLine("\nFilter Bookings:");

            var validFlightNumber = false;
            do
            {
                Console.Write("Enter Flight Number (0 for all): ");
                if (int.TryParse(Console.ReadLine(), out flightNumber) && flightNumber >= 0)
                {
                    validFlightNumber = true;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number for Max Price.");
                }
            } while (!validFlightNumber);

            var validPrice = false;
            do
            {
                Console.Write("Enter Max Price (0 for any): ");
                if (double.TryParse(Console.ReadLine(), out maxPrice) && maxPrice >= 0)
                {
                    validPrice = true;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number for Max Price.");
                }
            } while (!validPrice);

            Console.Write("Enter Departure Country (leave empty for any): ");
            var departureCountry = Console.ReadLine();

            Console.Write("Enter Destination Country (leave empty for any): ");
            var destinationCountry = Console.ReadLine();


            Console.Write("Enter Departure Date (yyyy-MM-dd format, leave empty for any): ");
            var departureDateInput = Console.ReadLine();
            var departureDate = DateTime.TryParse(departureDateInput, out var parsedDate) ? parsedDate : default(DateTime);

            Console.Write("Enter Departure Airport (leave empty for any): ");
            var departureAirport = Console.ReadLine();

            Console.Write("Enter Arrival Airport (leave empty for any): ");
            var arrivalAirport = Console.ReadLine();

            Console.Write("Enter Passenger Name (leave empty for any): ");
            var passengerName = Console.ReadLine();

            Console.Write("Enter Class (0 for any, 1 for Economy, 2 for Business, 3 for First Class): ");
            var flightClass = Enum.Parse<FlightClassSelection>(Console.ReadLine());

            manager.FilterBookings(bookings, flightNumber, maxPrice, departureCountry, destinationCountry, departureDate, departureAirport, arrivalAirport, passengerName, flightClass);
        }

        internal static void ShowMainMenuForPassenger()
        {
            Console.WriteLine("********************");
            Console.WriteLine("* Enter Your Name *");
            Console.WriteLine("********************");

            passengerName = Console.ReadLine();
            passenger.Name = passengerName;

            Console.WriteLine("********************");
            Console.WriteLine($"* Welcome {passengerName} *");

            var exit = false;
            do
            {
                Console.WriteLine("* Select an action *");
                Console.WriteLine("********************");
                Console.WriteLine("1: Book a Flight");
                Console.WriteLine("2: Search for Available Flights");
                Console.WriteLine("3: Manage Bookings");
                Console.WriteLine("4: Back to Passenger Or Manager Menu");

                Console.Write("Your selection: ");

                if (Enum.TryParse(Console.ReadLine(), out PassengerSelection passengerSelection))
                {
                    switch (passengerSelection)
                    {
                        case PassengerSelection.bookFlight:
                            BookingFlight(flights, bookings);
                            break;
                        case PassengerSelection.searchForAvailableFlights:
                            SearchForAvailableFlights();
                            break;
                        case PassengerSelection.manageBookings:
                            PassengerBookingsManageOpetions();
                            break;
                        case PassengerSelection.showMainMenu:
                            PassengerOrManagerMenu();
                            break;
                        case PassengerSelection.exit:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("\nInvalid selection. Please try again.");
                            Console.Write("Your selection: ");
                            break;
                    }
                }
            } while (!exit);
        }

        private static void PassengerBookingsManageOpetions()
        {
            var exit = false;
            do
            {
                Console.WriteLine("********************");
                Console.WriteLine("* Select an action *");
                Console.WriteLine("********************");

                Console.WriteLine("1: Cancel A Booking");
                Console.WriteLine("2: Modify A booking");
                Console.WriteLine("3: View Personal Bookings");
                Console.WriteLine("4: Back to Passenger Menu");

                Console.Write("Your selection: ");

                if (Enum.TryParse(Console.ReadLine(), out PassengerBookingsManageSelection passengerSelection))
                {
                    switch (passengerSelection)
                    {
                        case PassengerBookingsManageSelection.cancelABooking:
                            CancelBooking();
                            break;
                        case PassengerBookingsManageSelection.modifyABooking:
                            ModifyBooking();
                            break;
                        case PassengerBookingsManageSelection.viewPersonalBookings:
                            ViewPersonalBookings();
                            break;
                        case PassengerBookingsManageSelection.backToPassengerMenu:
                            ShowMainMenuForPassenger();
                            break;
                        case PassengerBookingsManageSelection.exit:
                            exit = true;
                            break;
                        default:
                            Console.WriteLine("\nInvalid selection. Please try again.");
                            Console.Write("Your selection: ");
                            break;
                    }
                }
            } while (!exit);
        }
        private static void ModifyBooking()
        {
            Console.WriteLine("Enter the bookId you want to Modify");
            var bookingId = Console.ReadLine();
            var newFlight = new Flight();

            if (string.IsNullOrEmpty(bookingId))
            {
                Console.WriteLine("Null value entered. Please try again.");
                ModifyBooking();
                return;
            }

            var bookingToUpdate = bookings.FirstOrDefault(booking => String.Equals(booking.BookingId.ToString(), bookingId, StringComparison.InvariantCultureIgnoreCase) &&
            String.Equals(booking.Passenger.Name, passenger.Name, StringComparison.InvariantCultureIgnoreCase));

            if (bookingToUpdate == null)
            {
                Console.WriteLine($"Non-existing booking with this id for {passenger.Name}. Please try again.");
                ModifyBooking();
                return;
            }

            Console.WriteLine($" let's modify your booking with Id = {bookingId}");

            var validFlightNum = false;
            do
            {
                Console.Write("Enter new Flight Number (leave 0 to keep the current Flight): ");
                if (int.TryParse(Console.ReadLine(), out int newFlightNum))
                {
                    if (newFlightNum >= 0)
                    {
                        validFlightNum = true;
                        if (newFlightNum != 0)
                        {
                            newFlight = passenger.SearchForAvailableFlightsByNum(flights, newFlightNum);

                            if (newFlight == null)
                            {
                                Console.WriteLine("Non-existing Flight with this Num. Please try again.");
                            }
                            else
                            {
                                bookingToUpdate.Flight = newFlight;
                            }

                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid FlightNum. Please enter a non-negative number.");
                    }
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number for Flight.");
                }
            } while (!validFlightNum);

            Console.Write("Enter the Flight Class : - leave empty to keep current ");

            Console.WriteLine("Select New Flight Class:");
            Console.WriteLine("1: Economy");
            Console.WriteLine("2: Business");
            Console.WriteLine("3: First Class");

            var flightClass = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(flightClass))
            {
                var validFlightClass = false;
                do
                {
                    if (!Enum.TryParse(flightClass, out FlightClassSelection flightClassSelection))
                    {
                        Console.WriteLine("Invalid input Flight Class Cannot be Empty. Please try again.");
                    }
                    else
                    {
                        validFlightClass = true;
                        bookingToUpdate.Class = flightClassSelection;
                    }
                } while (!validFlightClass);
            }

            bookingToUpdate.Passenger.Name = passenger.Name;
            bookings = passenger.ModifyABooking(bookings, bookingToUpdate);
            SaveBookingsToFile(bookings, bookingsFilePath);
            LoadData();
            PassengerBookingsManageOpetions();
        }

        private static void CancelBooking()
        {
            Console.WriteLine("Enter the bookId you want to cancel");

            if (int.TryParse(Console.ReadLine(), out int bookId))
            {
                var isCanceled = passenger.CancelBooking(bookings, passenger.Name, bookId);

                if (isCanceled)
                {
                    DeleteBookingFromFile(bookingsFilePath, bookId);
                    LoadData();
                    PassengerBookingsManageOpetions();
                }
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid bookId as an integer.");
            }
        }

        private static void ViewPersonalBookings()
        {
            passenger.ViewPersonalBookings(bookings, passenger.Name);
        }

        private static void BookingFlight(List<Flight> flights, List<Booking> bookings)
        {
            var booking = new Booking();
            var searchFlights = SearchForAvailableFlights();

            Console.WriteLine("Enter The Flight Number you want to book ");

            if (int.TryParse(Console.ReadLine(), out int flightNum))
            {
                Flight selectedFlight = passenger.SearchForAvailableFlightsByNum(searchFlights, flightNum);

                if (selectedFlight == null)
                {
                    Console.WriteLine($"Flight with ID {flightNum} not found.");
                }
                else
                {
                    booking.Flight = selectedFlight;
                    booking.Class = selectedFlight.Class;
                }

                booking.Passenger.Name = passengerName;
                booking.BookingId = GenerateRandomBookingId();

                bookings = passenger.BookFlight(booking, bookings);

                if (bookings.Count > 0)
                {
                    Console.WriteLine($"Available bookings = {bookings.Count} : \n");

                    foreach (Booking book in bookings)
                    {
                        Console.WriteLine($"BookingId = {book.BookingId}, Passenger = {book.Passenger.Name}, FlightNum = {book.Flight.FlightNumber}, FlightClass = {book.Class}");

                        if (book.Flight.ClassPrices.ContainsKey(book.Class))
                        {
                            Console.WriteLine($"Class = {book.Class}");
                        }
                        else
                        {
                            Console.WriteLine($"Class = {book.Class} - Class not available");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("No bookings available.");
                }

                SaveBookingsToFile(bookings, bookingsFilePath);
                LoadData();
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid Flight Number as an integer.");
            }
        }

        private static int GenerateRandomBookingId()
        {
            var random = new Random();
            return random.Next(1000, 9999);
        }

        private static List<Flight> SearchForAvailableFlights()
        {
            var flightToSearch = new Flight();

            Console.WriteLine("Search for Available Flights:");

            var validPrice = false;
            do
            {
                Console.Write("Enter Flight Price: ");
                if (double.TryParse(Console.ReadLine(), out var maxPrice) && maxPrice > 0)
                {
                    validPrice = true;
                    flightToSearch.Price = maxPrice;
                }
                else
                {
                    Console.WriteLine("Invalid input. Please enter a valid number for Max Price.");
                }
            } while (!validPrice);

            var validDepartureCountry = false;
            do
            {
                Console.Write("Departure Country: ");
                var departureCountry = Console.ReadLine();
                if (string.IsNullOrEmpty(departureCountry))
                {
                    Console.WriteLine("Invalid input Departure Country Cannot be Empty. Please try again.");
                }
                else
                {
                    validDepartureCountry = true;
                    flightToSearch.DepartureCountry = departureCountry;
                }
            } while (!validDepartureCountry);

            var validDestinationCountry = false;
            do
            {
                Console.Write("Destination Country: ");
                var destinationCountry = Console.ReadLine();
                if (string.IsNullOrEmpty(destinationCountry))
                {
                    Console.WriteLine("Invalid input Destination Country Cannot be Empty. Please try again.");
                }
                else
                {
                    validDestinationCountry = true;
                    flightToSearch.DestinationCountry = destinationCountry;
                }
            } while (!validDestinationCountry);

            Console.Write("Departure Date (yyyy-MM-dd): ");
            while (!DateTime.TryParseExact(Console.ReadLine(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime departureDate))
            {
                Console.WriteLine("Invalid date format. Please enter a valid date in the format yyyy-MM-dd.");
            }

            var validDepartureAirport = false;
            do
            {
                Console.Write("Departure Airport: ");
                var departureAirport = Console.ReadLine();
                if (string.IsNullOrEmpty(departureAirport))
                {
                    Console.WriteLine("Invalid input Departure Airport Cannot be Empty. Please try again.");
                }
                else
                {
                    validDepartureAirport = true;
                    flightToSearch.DepartureAirport = departureAirport;
                }
            } while (!validDepartureAirport);

            var validArrivalAirport = false;
            do
            {
                Console.Write("Arrival Airport: ");
                var arrivalAirport = Console.ReadLine();
                if (string.IsNullOrEmpty(arrivalAirport))
                {
                    Console.WriteLine("Invalid input arrival Airport Cannot be Empty. Please try again.");
                }
                else
                {
                    validArrivalAirport = true;
                    flightToSearch.ArrivalAirport = arrivalAirport;
                }
            } while (!validArrivalAirport);

            var validFlightClass = false;
            do
            {
                Console.WriteLine("********************");
                Console.WriteLine("* Select Flight Class *");
                Console.WriteLine("********************");

                Console.WriteLine("1: Economy");
                Console.WriteLine("2: Business");
                Console.WriteLine("3: First Class");

                if (!Enum.TryParse(Console.ReadLine(), out FlightClassSelection flightClassSelection))
                {
                    Console.WriteLine("Invalid input Flight Class Cannot be Empty. Please try again.");
                }
                else
                {
                    validFlightClass = true;
                    flightToSearch.Class = flightClassSelection;
                }
            } while (!validFlightClass);

            return passenger.SearchForAvailableFlights(flights, flightToSearch);

        }

    }
}