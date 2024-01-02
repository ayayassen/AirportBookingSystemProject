using TrainingProject;
using TrainingProject.General;

public class Booking
{
    public int BookingId { get; set; }
    public Flight Flight { get; set; } = new Flight();
    public Passenger Passenger { get; set; } = new Passenger();
    public FlightClassSelection Class { get; set; }

}
