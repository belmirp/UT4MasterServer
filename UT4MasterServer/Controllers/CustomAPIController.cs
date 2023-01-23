
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text;
using UT4MasterServer.Models;
using UT4MasterServer.Other;
using UT4MasterServer.Services;

namespace UT4MasterServer.Controllers;

public sealed class CustomAPIController : JsonAPIController
{
	private readonly IOptions<ApplicationSettings> configuration;
	private readonly AccountService accountService;

	public CustomAPIController(ILogger<UnrealTournamentMatchmakingController> logger, IOptions<ApplicationSettings> configuration, AccountService accountService) : base(logger)
	{
		this.configuration = configuration;
		this.accountService = accountService;
	}

	[HttpGet("api/ShowMyInfo")]
	public IActionResult ShowMyInfo()
	{
		var ip = GetClientIP(configuration);

		StringBuilder sb = new StringBuilder();

		sb.AppendLine($"Direct IP Address: {HttpContext.Connection.RemoteIpAddress}");
		sb.AppendLine($"Guessed IP Address: {ip}");

		sb.AppendLine();
		sb.AppendLine($"HTTP Method: {HttpContext.Request.Method}");
		sb.AppendLine($"Display URL: {HttpContext.Request.GetDisplayUrl()}");

		sb.AppendLine();
		sb.AppendLine("Query URL Parameters:");
		foreach (var parameter in HttpContext.Request.Query)
		{
			sb.AppendLine($"{parameter.Key}={parameter.Value}");
		}

		sb.AppendLine();
		sb.AppendLine("Request Headers:");
		foreach (var header in HttpContext.Request.Headers)
		{
			foreach (var headerInstance in header.Value)
			{
				sb.AppendLine($"{header.Key}: {headerInstance}");
			}
		}

		return Content(sb.ToString());
	}

	// www.epicgames.com/id/login?noHostRedirect=true
	[HttpGet("id/login")]
	public IActionResult EpicLoginPage()
	{
		return Redirect($"https://{configuration.Value.WebsiteDomain}/Login");
	}
	[HttpGet("id/api/redirect")]
	public IActionResult EpicGetAuthCodePage()
	{
		return Redirect($"https://{configuration.Value.WebsiteDomain}/Login");
	}

	[HttpGet("api/playerCard/{id}")]
	public async Task<IActionResult> GetPlayerCard(string id)
	{
		// Epic doesn't require any authentication to serve player card, there is nothing sensitive here
		var account = await accountService.GetAccountAsync(EpicID.FromString(id));
		if (account == null)
		{
			return NotFound();
		}

		return Json(account);
	}
}
