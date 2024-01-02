namespace TrainingProject
{
    public class FlightEqualityComparer : IEqualityComparer<Flight>
    {
        public bool Equals(Flight flightA, Flight flightB)
        {
            if (ReferenceEquals(flightA, flightB))
                return true;

            if (ReferenceEquals(flightA, null) || ReferenceEquals(flightB, null))
                return false;

            return flightA.FlightNumber == flightB.FlightNumber &&
                   flightA.DepartureDate == flightB.DepartureDate &&
                   flightA.DepartureCountry == flightB.DepartureCountry &&
                   flightA.DestinationCountry == flightB.DestinationCountry &&
                   flightA.DepartureAirport == flightB.DepartureAirport &&
                   flightA.ArrivalAirport == flightB.ArrivalAirport &&
                   flightA.Class == flightB.Class &&
                   flightA.Price == flightB.Price &&
                   DictionaryEquals(flightA.ClassPrices, flightB.ClassPrices);
        }

        public int GetHashCode(Flight flight)
        {
            if (ReferenceEquals(flight, null))
                return 0;

            return flight.FlightNumber.GetHashCode();
        }
        private bool DictionaryEquals<TKey, TValue>(Dictionary<TKey, TValue> flightA, Dictionary<TKey, TValue> flightB)
        {
            if (ReferenceEquals(flightA, flightB))
                return true;

            if (ReferenceEquals(flightA, null) || ReferenceEquals(flightB, null))
                return false;

            if (flightA.Count != flightB.Count)
                return false;

            foreach (var pair in flightA)
            {
                if (!flightB.TryGetValue(pair.Key, out var value) || !EqualityComparer<TValue>.Default.Equals(pair.Value, value))
                    return false;
            }

            return true;
        }
    }

}
