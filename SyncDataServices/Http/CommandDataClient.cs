using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using PlatformService.Dtos;

namespace PlatformService.SyncDataServices.Http
{
    public class CommandDataClient: ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public CommandDataClient(HttpClient httpClient, IConfiguration config)
        {
            _httpClient = httpClient;
            _config = config;
        }

        public async Task SendPlatformToCommand(PlatformReadDto platformReadDto)
        {
            var httpConent = new StringContent(
                JsonSerializer.Serialize(platformReadDto), Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync($"{_config["CommandService"]}", httpConent);

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("--> sync post to command service success");
            }
            else
            {
                Console.WriteLine("--> sync post to command service failed");
            }
        }
    }
}