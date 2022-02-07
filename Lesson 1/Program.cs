using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Lesson_1
{
    public class Program
    {
        private static readonly List<ResponseModel> ResultList = new();

        static async Task Main(string[] args)
        {
            AppFilesChk();
            var tasks = new List<Task>();
            var reslist = new List<ResponseModel>();
            for (int i = 4; i <= 13; i++)
            {
                tasks.Add(GetResponse(i));
            }

            await Task.WhenAll(tasks);
            
            foreach (var r in ResultList.OrderBy(x =>x?.InsertDate))
            {
                await File.AppendAllLinesAsync(
                    "result.txt",
                    new string[] { r.UserId.ToString(), r.Id.ToString(), r.Title, r.Body,r.InsertDate?.ToString("o") });
                await File.AppendAllTextAsync("result.txt", "\n");
            }

        }


        static  void AppFilesChk()
        {
            var history = Path.Combine(Directory.GetCurrentDirectory(), "result.txt");
            if (!File.Exists(history))
            {
                using (StreamWriter sr = File.CreateText(history)) { };
            }

        }

        static async Task GetResponse(int i)
        {
            var client = new HttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync($"https://jsonplaceholder.typicode.com/posts/{i}");
                response.EnsureSuccessStatusCode();
                var responseBody = await response.Content.ReadFromJsonAsync<ResponseModel>();
                if (responseBody != null)
                {
                    responseBody.InsertDate = DateTime.Now;
                    ResultList.Add(responseBody);
                    Console.WriteLine($"Got response from {responseBody.Id}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"\nException Caught for id:{i}");
                Console.WriteLine("Message :{0} ", e.Message);
            }

        }


    }
}