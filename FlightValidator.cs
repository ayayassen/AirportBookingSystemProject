using FluentValidation;

namespace TrainingProject
{
    public class FlightValidator : AbstractValidator<Flight>
    {
        public FlightValidator()
        {
            RuleFor(flight => flight.FlightNumber)
                .GreaterThanOrEqualTo(0)
                .WithMessage(GetValidationDetails(nameof(Flight.FlightNumber), "int", "Required, FlightNumber must be Int and greater than 0"));

            RuleFor(flight => flight.DepartureCountry)
                .NotEmpty()
                .WithMessage(GetValidationDetails(nameof(Flight.DepartureCountry), "Free Text", "Required"));

            RuleFor(flight => flight.DestinationCountry)
                .NotEmpty()
                .WithMessage(GetValidationDetails(nameof(Flight.DestinationCountry), "Free Text", "Required"));

            RuleFor(flight => flight.DepartureDate)
                .NotEmpty()
                .Must(BeInFutureDate)
                .WithMessage(GetValidationDetails(nameof(Flight.DepartureDate), "Date Time", "Required, Allowed Range (today => future)"));

            RuleFor(flight => flight.DepartureAirport)
                .NotEmpty()
                .WithMessage(GetValidationDetails(nameof(Flight.DepartureAirport), "Free Text", "Required"));

            RuleFor(flight => flight.ArrivalAirport)
                .NotEmpty()
                .WithMessage(GetValidationDetails(nameof(Flight.ArrivalAirport), "Free Text", "Required"));

            RuleForEach(flight => flight.ClassPrices)
                .NotEmpty()
                .Must((flight, classPrice) => flight.ClassPrices.Values.All(price => price > 0))
                .WithMessage(GetValidationDetails(nameof(Flight.ClassPrices), "double>", "Required, Class prices must be greater than 0"));

        }

        private static bool BeInFutureDate(DateTime departureDate)
        {
            return departureDate.Date > DateTime.Today;
        }
        private static string GetValidationDetails(string fieldName, string type, string constraints)
        {
            return $"{fieldName}: \n - Type: {type}\n - Constraint: {constraints}";
        }

    }

}
