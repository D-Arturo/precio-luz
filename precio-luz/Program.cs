using precio_luz;

class Program
{
    static async Task Main(string[] args)
    {
        var calculator = new ElectricityPriceCalculator( new HttpClient());
        try
        {
            var hourlyInfo = await calculator.GetElectricityPriceAsync();

            Console.WriteLine("Precios de la luz por hora en la Península:");
            foreach (var hourInfo in hourlyInfo)
            {
                Console.WriteLine(hourInfo.ToString());
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}