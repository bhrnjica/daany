using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Daany;

namespace Unit.Test.DF
{
	public class MockHttpMessageHandler : HttpMessageHandler
	{
		private readonly Dictionary<string, HttpResponseMessage> _responses = new Dictionary<string, HttpResponseMessage>();
		private string _lastUrl;

		public MockHttpMessageHandler When(string url)
		{
			_lastUrl = url;
			_responses[url] = new HttpResponseMessage();
			return this; // Return self to enable method chaining
		}

		public MockHttpMessageHandler Respond(string mediaType, string content)
		{
			if (!_responses.ContainsKey(_lastUrl))
			{
				throw new InvalidOperationException("No URL specified. Call When() first.");
			}

			_responses[_lastUrl].Content = new StringContent(content, System.Text.Encoding.UTF8, mediaType);
			return this;
		}

		protected override async Task<HttpResponseMessage> SendAsync(
			HttpRequestMessage request,
			System.Threading.CancellationToken cancellationToken)
		{
			if (_responses.TryGetValue(request.RequestUri.ToString(), out var response))
			{
				return await Task.FromResult(response);
			}
			return new HttpResponseMessage(System.Net.HttpStatusCode.NotFound);
		}
	}

	// Testable subclass that allows HttpClient injection
	public class TestableDataFrame : DataFrame
	{
		private readonly HttpClient _httpClient;

		public TestableDataFrame(HttpClient httpClient) : base(new object[] { }, new List<string>() { "c" })
		{
			_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
		}

		public new async Task<DataFrame> FromWebAsync(string urlPath, char sep = ',', string[]? names = null,
			string? dformat = null, ColType[]? colTypes = null, int nRows = -1)
		{
			if (string.IsNullOrEmpty(urlPath))
				throw new ArgumentNullException(nameof(urlPath), "Argument should not be null.");

			var lines = new List<string>();
			using (var stream = await _httpClient.GetStreamAsync(urlPath))
			using (StreamReader reader = new StreamReader(stream))
			{
				while (!reader.EndOfStream)
				{
					string line = await reader.ReadLineAsync();
					if (!string.IsNullOrEmpty(line))
						lines.Add(line);
				}
			}

			return FromStrings(lines.ToArray(), sep, names, dformat, colTypes, nRows: nRows);
		}
	}
}
