using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCProtectingSecrets.Data;
using MVCProtectingSecrets.Models;
using System.Diagnostics;
using System.Security.Claims;

namespace MVCProtectingSecrets.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;
    private readonly TelemetryClient _telemetryClient;
    private readonly ApplicationDbContext _context;

    public HomeController(ILogger<HomeController> logger, TelemetryClient telemetryClient
                            , IConfiguration configuration, ApplicationDbContext context)
    {
        _logger = logger;
        _telemetryClient = telemetryClient;
        _configuration = configuration;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        _telemetryClient.TrackPageView("Home/Index");
        var storageAccountName = _configuration["StorageDetails:ImagesAccountName"];
        var storageContainerName = _configuration["StorageDetails:ImagesContainerName"];
        var storageAccountSASToken = _configuration["StorageDetails:ImagesSASToken"];
        var databaseInfo = _configuration["ConnectionStrings:DefaultConnection"];

        _telemetryClient.TrackTrace($"Storage Account Name: {storageAccountName}");
        _telemetryClient.TrackTrace($"Storage Container Name: {storageContainerName}");
        _telemetryClient.TrackTrace($"Storage Account SAS: {storageAccountSASToken}");
        _telemetryClient.TrackTrace($"Database Info: {databaseInfo}");

        //NOTE: Users that aren't logged in shouldn't see any storage details
        //      so we'll set these to empty strings and update if the user is logged in
        ViewBag.StorageAccountName = string.Empty;
        ViewBag.StorageContainerName = string.Empty;
        ViewBag.StorageAccountSASToken = string.Empty;
        ViewBag.ShowStorage = false;

        var images = new List<ImageDetail>();

        //NOTE: just simulating user-based images, for brevity just using any user (all users see all images)
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userEmail = User.FindFirstValue(ClaimTypes.Email);
        if (!string.IsNullOrWhiteSpace(userId))
        {
            _telemetryClient.TrackTrace($"User is logged in: {userId} | {userEmail}");
            ViewBag.ShowStorage = true;
            ViewBag.StorageAccountName = storageAccountName;
            ViewBag.StorageContainerName = storageContainerName;
            ViewBag.StorageAccountSASToken = storageAccountSASToken;
            images = await GetImages(storageAccountName, storageContainerName, storageAccountSASToken);
        }

        return View(images);
    }

    

    public async Task<IActionResult> Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public async Task<IActionResult> Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    //note: this is just for testing purposes, clearly not recommended for production
    public async Task<IActionResult> MigrateDatabase()
    {
        if (_context?.Database != null)
        {
            _context.Database.Migrate();
        }
        return RedirectToAction("Index");
    }

    private async Task<List<ImageDetail>> GetImages(string accountName, string containerName, string sasToken)
    {
        var images = new List<ImageDetail>();

        if (_context?.Database != null)
        {
            images = await _context.ImageDetails.OrderBy(x => x.FileName).ToListAsync();
        }

        foreach (var image in images)
        {
            image.ImageFullPath = $"https://{accountName}.blob.core.windows.net/{containerName}/{image.FileName}?{sasToken}";
        }

        return images;
    }
}
