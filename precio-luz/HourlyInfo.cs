namespace precio_luz;

public class HourlyInfo
{
    public readonly string DateTime;
    public readonly string Price;

    public HourlyInfo(string dateTime, string price)
    {
        DateTime = dateTime;
        Price = price;
    }

    public override string ToString()
    {
        return $"Hora: {DateTime}, Precio: {Price} euro/MWh";
    }
}