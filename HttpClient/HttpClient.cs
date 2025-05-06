using budget_tracker;
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

public class RevenueApiClient
{
	private readonly HttpClient _httpClient;
	private readonly Logging logging;

	public RevenueApiClient(HttpClient httpClient, Logging logging)
	{
		_httpClient = httpClient;
		_httpClient.BaseAddress = new Uri("https://shuleone-web-server.prema.co.ke"); // replace with actual API base URL
        this.logging = logging;
    }

	public async Task<bool> PostRevenueAsync(Revenue revenue)
	{
		try
		{
			HttpResponseMessage response = await _httpClient.PostAsJsonAsync("api/Accounting/Revenue", revenue);

			if (response.IsSuccessStatusCode)
			{
				// Optionally read response content
				var result = await response.Content.ReadFromJsonAsync<Revenue>();
                logging.WriteToLog($"Posted Revenue ID: {result?.id}", "Information");
				return true;
			}
			else
			{
				string errorContent = await response.Content.ReadAsStringAsync();
                logging.WriteToLog($"Failed to post revenue: {response.StatusCode} - {errorContent}", "Error");
				return false;
			}
		}
		catch (Exception ex)
		{
            logging.WriteToLog($"Exception when calling Revenue API: {ex.Message}", "Error");
			return false;
		}
	}
}

public class Revenue
{
	public int id { get; set; }
	public decimal amount { get; set; }
	public string paid_by { get; set; }
	public string payment_reference { get; set; }
	public string account_number { get; set; }
	public int? fk_intended_account_number { get; set; }
	public RevenueStatus status { get; set; } = RevenueStatus.Unallocated;
	public DateTime payment_date { get; set; }
	public PaymentMethod payment_method { get; set; }
	public DateTime date_created { get; set; } = DateTime.UtcNow;
	public string recorded_by { get; set; } 
}

public enum RevenueStatus
{
	Unallocated = 0,
	Allocated = 1,
	TransactionPending = 2
}

public enum PaymentMethod
{
	Mpesa = 0,
	CardPDQ = 1,
	BankTransfer = 2,
	InternalTransaction = 3
}