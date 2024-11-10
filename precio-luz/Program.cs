using precio_luz;

class Program
{
    static async Task Main(string[] args)
    {
        var calculator = new ElectricityPriceCalculator( new HttpClient());
        try
        {
            var hourlyPrice = await calculator.GetElectricityPriceAsync();

            Console.WriteLine("Precios de la luz por hora en la Península:");
            foreach (var (dateTime, price) in hourlyPrice)
            {
                Console.WriteLine($"Hora: {dateTime}, Precio: {price} euro/MWh");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}