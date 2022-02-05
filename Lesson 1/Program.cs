using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;

namespace Lesson_1
{
    class Program
    {
        static readonly HttpClient client = new HttpClient();
        static async Task Main(string[] args)
        {
            AppFilesChk();
            for (int i = 4; i <= 13; i++)
            {
                await GetResponse(i);
            }
            
        }

        public static void AppFilesChk()
        {
            var history = Path.Combine(Directory.GetCurrentDirectory(), "result.txt");
            if (!File.Exists(history))
            {
                using (StreamWriter sr = File.CreateText(history)) { };
            }

        }

        static async Task GetResponse(int i)
        {

            try
            {
                HttpResponseMessage response = await client.GetAsync($"https://jsonplaceholder.typicode.com/posts/{i}");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadFromJsonAsync<ResponseModel>();
                await File.AppendAllLinesAsync(
                    "result.txt",
                    new string[] { responseBody.userId.ToString(),responseBody.id.ToString(), responseBody.title,responseBody.body});
                await File.AppendAllTextAsync("result.txt","\n");
          
                Console.WriteLine($"Got response from {responseBody.id}");
            }
            catch (HttpRequestException e)
            {
                await File.AppendAllLinesAsync(
                    "result.txt",
                    new string[] {$"ERROR:{e.Message}, id:{i}" });
                await File.AppendAllTextAsync("result.txt", "\n");
                Console.WriteLine($"\nException Caught for id:{i}");
                Console.WriteLine("Message :{0} ", e.Message);
            }

        }
    }
}
