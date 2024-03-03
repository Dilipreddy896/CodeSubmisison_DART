using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using System.IO;
using System.Net.Http;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpClient();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateTime.Now.AddDays(index),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.MapGet("/getdata", () =>
{
    string strDates = File.ReadAllText("./App_Data/data.txt");
   var lstDates =  strDates.Split("\r\n");
    List<DateTime> lstdates = new List<DateTime>();
    if(lstDates.Length>0)
    foreach (string date in lstDates)
    {
        if(date != string.Empty)
        {
                DateTime myDate;
               
               if(DateTime.TryParse(date, out myDate))
                {
                    lstdates.Add(DateTime.Parse(date));
                    HttpClient client = new HttpClient();
                    GetNasaAPIDate(DateTime.Parse(date));
                    //return product;
                }
        }
    }
    return lstdates;
})
.WithName("GetData");


      async void GetNasaAPIDate(DateTime dateVal)
        {
            //HttpClientFactory _clientFactory = new HttpClientFactory();
            HttpClient client = new HttpClient();
    
            client.BaseAddress = new Uri("https://api.nasa.gov/planetary");
            //client.DefaultRequestHeaders.Add("api-key", "DEMO_KEY");
            // send request
            var builder = new UriBuilder(client.BaseAddress)
            {
                Path = "/apod", //api_key=DEMO_KEY&",
                Query = "?date=" + dateVal.ToString("yyyy-MM-dd") + "&api_key=DEMO_KEY",
            };

            var request = new HttpRequestMessage
            {
                RequestUri = builder.Uri,
                Method = HttpMethod.Get,
               //Headers.Add("api_key", "1234");

            };

            request.Headers.Add("Accept", "text/html");
            request.Headers.Add("Access-Control-Allow-Origin", "*");

           // request.Headers.Add("Content-Type","application/json");
        var response = await client.SendAsync(request);
           // File.WriteAllBytes($@"C:\ExtractedDataFiles\img,html", response.Content.());
            if (response.IsSuccessStatusCode)
            {
                response.Content.ReadAsStringAsync().Wait();
                var type = response.Content;
            }
            // response.EnsureSuccessStatusCode();
            //return response.ToString();
        }

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}