using System.Data;
using System.Net;
using Core;
using Core.Models;
using Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace WebSite.Controllers;

public class ApiController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly Settings _settings;
    private readonly ILogger _logger;
    private readonly IPostService _postService;

    public ApiController(
        IUserService userService,
        Settings settings,
        IPostService postService,
        ILogger<ApiController> logger)
    {
        _logger = logger;
        _postService = postService;
        _settings = settings;
        _userService = userService;
    }
        
    [HttpGet]
    [Route("api")]
    public async Task<ActionResult<string>> GetApiVersion() => await Task.FromResult($"//devdigest API v{_settings.Version}");

    [HttpGet]
    [Route("api/categories")]
    public async Task<IActionResult> GetCategories()
    {
        var categories = await _postService.GetCategories();

        return Ok(categories);
    }

    [HttpPost]
    [Route("api/publications")]
    public async Task<IActionResult> AddPublication(CreatePostRequest request)
    {
        var userId = await _userService.GetUserId(request.Key);

        if (userId == null)
        {
            _logger.LogWarning($"Somebody tried to login with this key: `{request.Key}`. Text: `{request.Comment}`");

            return Forbid();
        }

        try
        {
            var (post, url) = await _postService.Create(request, userId.Value);
            
            return Created(url, post);
        }
        catch (DuplicateNameException ex)
        {
            _logger.LogError(ex, "Publication with this url already exist");
                
            return StatusCode((int) HttpStatusCode.Conflict, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while creating new publication");
                
            return BadRequest(ex.Message);
        }
    }
}