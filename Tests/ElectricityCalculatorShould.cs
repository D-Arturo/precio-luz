using System.Net;
using Moq;
using Moq.Protected;
using precio_luz;

namespace Tests;

public class ElectricityPriceCalculatorShould
{
    private Mock<HttpMessageHandler> _mockHttpMessageHandler;
    private HttpClient _httpClient;
    private ElectricityPriceCalculator _calculator;

    public ElectricityPriceCalculatorShould()
    {
        _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        _httpClient = new HttpClient(_mockHttpMessageHandler.Object);
        _calculator = new ElectricityPriceCalculator(_httpClient);
    }
    
    [Fact]
    public async Task ReturnsEmptyListWhenNoDataAvailable()
    {
        // Arrange
        var jsonResponse = @"{
            ""indicator"": {
                ""values"": []
            }
        }";

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var result = await _calculator.GetElectricityPriceAsync();

        // Assert
        Assert.Equal(1, result.Count);
        Assert.Equal("No disponible", result[0].DateTime);
        Assert.Equal("No disponible", result[0].Price);
    }
    
    [Fact]
    public async Task ThrowHttpExceptionWhenApiCallFails()
    {
        // Arrange
        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("API call failed"));

        // Act & Assert
        await Assert.ThrowsAsync<HttpRequestException>(async () =>
        {
            await _calculator.GetElectricityPriceAsync();
        });
    }

    
    [Fact]
    public async Task ReturnCorrectDataWhenInfoAvailableViaApiCall()
    {
        // Arrange
        var jsonResponse = @"{
            ""indicator"": {
                ""values"": [
                    {
                        ""datetime"": ""2024-11-10T12:00:00.000+01:00"",
                        ""value"": 100.5,
                        ""geo_name"": ""Península""
                    },
                    {
                        ""datetime"": ""2024-11-10T13:00:00.000+01:00"",
                        ""value"": 110.2,
                        ""geo_name"": ""Península""
                    }
                ]
            }
        }";

        _mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(jsonResponse)
            });

        // Act
        var result = await _calculator.GetElectricityPriceAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("12:00", result[0].DateTime);
        Assert.Equal("100,5", result[0].Price);
        Assert.Equal("13:00", result[1].DateTime);
        Assert.Equal("110,2", result[1].Price);
    }
}