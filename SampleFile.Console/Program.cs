using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SampleFile.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Delay(5000);
            SendFile();
        }

        public static void SendFile()
        {
            using HttpClient httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("http://localhost:55490/");

            using var form = new MultipartFormDataContent();

            using var fs = File.OpenRead(@"D:\Develop\LerningSolution\LerningSolution\README.md");
            var streamContent = new StreamContent(fs);
            var fileContent = new ByteArrayContent(File.ReadAllBytes(@"D:\Develop\LerningSolution\LerningSolution\README.md"));

            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            // "file" parameter name should be the same as the server side input parameter name
            form.Add(fileContent, "file", Path.GetFileName(fs.Name));
            HttpResponseMessage response =  httpClient.PostAsync("WeatherForecast/SaveFile", form).Result;
        }
    }

}
