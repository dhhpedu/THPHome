﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using THPCore.Extensions;
using THPHome.Data;
using THPHome.Entities.Utils;
using THPHome.Models.Args.Utils;
using THPIdentity.Constants;
using WebUI.Foundations;
using WebUI.Models.Blocks;

namespace THPHome.Controllers;

public class ToolController(ApplicationDbContext context) : BaseController(context)
{
    private readonly string _baseUrl = "https://dhhp.edu.vn/";
    // Generates a shortened URL and saves it to the database
    [HttpPost("create-short-link")]
    public async Task<IActionResult> CreateShortLink([FromBody] CreateShortLinkArgs args)
    {
        if (string.IsNullOrWhiteSpace(args.OriginalUrl)) return BadRequest("Original url is required!");
        var shortenedCode = GenerateShortenedCode(args.OriginalUrl);
        var shortenedUrl = _baseUrl + shortenedCode;

        // Save the mapping of original URL to shortened URL in the database
        var urlShortener = new UrlShortener
        {
            OriginalUrl = args.OriginalUrl,
            ShortenedUrl = shortenedUrl,
            CreatedDate = DateTime.Now,
            CreatedBy = User.GetUserName(),
            UTM = args.UTM
        };

        await _context.UrlShorteners.AddAsync(urlShortener);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            data = new
            {
                shortenedUrl
            }
        });
    }

    // Retrieves the original URL from the database using the shortened code
    [Route("/u/{shortenedCode}")]
    public async Task<IActionResult> GetOriginalUrl(string shortenedCode)
    {
        var urlShortener = await _context.UrlShorteners
            .Where(u => u.ShortenedUrl.EndsWith(shortenedCode))
            .FirstOrDefaultAsync();
        if (urlShortener is null) return NotFound();
        urlShortener.ViewCount++;
        urlShortener.LastVisitedDate = DateTime.Now;
        _context.UrlShorteners.Update(urlShortener);
        await _context.SaveChangesAsync();
        return Redirect(urlShortener.OriginalUrl);
    }

    [HttpPost("delete-short-link/{id}")]
    public async Task<IActionResult> DeleteShortLink([FromRoute] Guid id)
    {
        if (!User.IsInRole(RoleName.ADMIN)) return Unauthorized();
        var urlShortener = await _context.UrlShorteners.FindAsync(id);
        if (urlShortener is null) return NotFound();
        _context.UrlShorteners.Remove(urlShortener);
        await _context.SaveChangesAsync();
        return Ok();
    }

    // Helper method to generate a short code for the URL
    private static string GenerateShortenedCode(string originalUrl)
    {
        var hash = originalUrl.GetHashCode();
        return Convert.ToBase64String(BitConverter.GetBytes(hash)).TrimEnd('=')[..6]; // First 6 chars of base64 encoding
    }

    public class OldSponsorBlock
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }
        [JsonPropertyName("logos")]
        public List<string>? Logos { get; set; }
    }

    [HttpGet("mirate-block"), AllowAnonymous]
    public async Task<IActionResult> MigrateBlockAsync()
    {
        var sponsorBlock = await _context.Blocks.FirstOrDefaultAsync(x => x.NormalizedName == nameof(SponsorBlock));
        if (sponsorBlock is null) return NotFound();
        var blockData = await _context.PostBlocks.Where(x => x.BlockId == sponsorBlock.Id).ToListAsync();
        if (!blockData.Any())
        {
            return Ok("Ko co gi");
        }
        foreach (var item in blockData)
        {
            try
            {
                if (string.IsNullOrEmpty(item.Data)) continue;
                var dataObject = JsonConvert.DeserializeObject<OldSponsorBlock>(item.Data);
                if (dataObject is null || dataObject.Logos is null) continue;
                var newDataObjectItem = new List<SponsorItem>();
                foreach (var item1 in dataObject.Logos)
                {
                    newDataObjectItem.Add(new SponsorItem
                    {
                        Logo = item1,
                        Link = "#"
                    });
                }
                var newDataObject = new SponsorBlock
                {
                    AutoPlay = 1000,
                    ClassName = "container",
                    Speed = 1000,
                    Id = item.Id,
                    Sponsors = newDataObjectItem
                };
                item.Data = JsonConvert.SerializeObject(newDataObject);
                _context.PostBlocks.Update(item);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
        await _context.SaveChangesAsync();
        return Ok();
        
    }
}
