using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using ShelterServiceClient.Exceptions;
using ShelterServiceClient.Utilities;
using ShelterServiceClient.ViewModels.Requests;
using ShelterServiceClient.ViewModels.Responses;
using System.Text;

namespace ShelterServiceClient
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IHttpRequests _httpRequests;
        private readonly string shelterApiUrl;

        public Worker(ILogger<Worker> logger,
                      IHttpRequests httpRequests,
                      IOptions<ShelterServiceConfigurationOptions> options)
        {
            _logger = logger;
            _httpRequests = httpRequests;
            shelterApiUrl = options.Value.ShelterApi.ServerUrl;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    var companies = await GetCompaniesListAsync().ConfigureAwait(false);

                    if (!companies.Any(x => x.Phone.ToLower().Contains(DateTimeOffset.Now.Date.ToString("dd.MM.yyyy")))) 
                        await CreateCompanyAsync().ConfigureAwait(false);
                }
                catch (UserUnauthorizedException)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                        _logger.LogInformation("User unauthorized exception at: {time}", DateTimeOffset.Now);
                }
                catch (UserForbidException)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                        _logger.LogInformation("User forbiden exception at: {time}", DateTimeOffset.Now);
                }
                catch (BadRequestException)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                        _logger.LogInformation("bad request at: {time}", DateTimeOffset.Now);
                }
                catch (UnprocessableEntityException)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                        _logger.LogInformation("Unprocessable entity at: {time}", DateTimeOffset.Now);
                }
                catch (UnknownPostException)
                {
                    if (_logger.IsEnabled(LogLevel.Information))
                        _logger.LogInformation("Unknown post at: {time}", DateTimeOffset.Now);
                }

                await Task.Delay(5000, stoppingToken);
            }
        }

        private async Task<IEnumerable<CompanyViewModel>> GetCompaniesListAsync()
        {
            var request = new RequestViewModel()
            {
                Operation = "READ_LIST"
            };

            string requestBody = JsonConvert.SerializeObject(request);
            HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            var response = await _httpRequests.PostAsync($"{shelterApiUrl}/companies", null, content, null).ConfigureAwait(false); //TODO: �������� �����
            string responseBody = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

            return JsonConvert.DeserializeObject<IEnumerable<CompanyViewModel>>(responseBody) ?? throw new UnknownPostException();
        }

        private async Task CreateCompanyAsync()
        {
            var request = new RequestViewModel()
            {
                Operation = "CREATE",
                Body = new CreateCompanyViewModel()
                {
                    Inn = "1234567890",
                    Name = "Test",
                    ParentCompanyId = null,
                    Phone = DateTimeOffset.Now.Date.ToString("dd.MM.yyyy")
                }
            };

            string requestBody = JsonConvert.SerializeObject(request);
            HttpContent content = new StringContent(requestBody, Encoding.UTF8, "application/json");
            await _httpRequests.PostAsync($"{shelterApiUrl}/companies", null, content, null).ConfigureAwait(false); //TODO: �������� �����
        }
    }
}
