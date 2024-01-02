namespace TrainingProject.General
{
    public enum UserType
    {
        exit = 0,
        passenger = 1,
        manager = 2,
    }

    public enum PassengerSelection
    {
        exit = 0,
        bookFlight = 1,
        searchForAvailableFlights = 2,
        manageBookings = 3,
        showMainMenu = 4
    }

    public enum FlightClassSelection
    {
        economy = 0,
        business = 1,
        firstClass = 2,
    }

    public enum PassengerBookingsManageSelection
    {
        exit = 0,
        cancelABooking = 1,
        modifyABooking = 2,
        viewPersonalBookings = 3,
        backToPassengerMenu = 4,
    }

    public enum ManagerBookingsManageSelection
    {
        exit = 0,
        filterBooking = 1,
        importNewAvailableFlights = 2,
        backToPassengerMenu = 3, 
    }

}
