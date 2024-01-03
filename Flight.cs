using System.ComponentModel.DataAnnotations;
using TrainingProject.General;

public class Flight
{
    [Range(1, 9999, ErrorMessage = "Flight number must be between 1 and 9999.")]
    public int FlightNumber { get; set; }

    [Required(ErrorMessage = "Departure country is required.")]
    public string DepartureCountry { get; set; } = string.Empty;

    [Required(ErrorMessage = "Destination country is required.")]
    public string DestinationCountry { get; set; } = string.Empty;

    [Range(typeof(DateTime), "now", "1/1/2222", ErrorMessage = "Departure date must be between now and 1/1/2222.")]
    public DateTime DepartureDate { get; set; }

    [Required(ErrorMessage = "Departure airport is required.")]
    public string DepartureAirport { get; set; } = string.Empty;

    [Required(ErrorMessage = "Arrival airport is required.")]
    public string ArrivalAirport { get; set; } = string.Empty;

    [Required(ErrorMessage = "Flight class is required.")]
    public FlightClassSelection Class { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Price must be greater than or equal to 0.")]
    public double Price { get; set; }

    public Dictionary<FlightClassSelection, double> ClassPrices { get; set; } = new Dictionary<FlightClassSelection, double>();

}
