﻿using DatingApp.API.Data;
using DatingApp.API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp.API.Controllers;

public class BuggyController : BaseApiController
{
    private readonly AppDbContext _context;

    public BuggyController(AppDbContext context)
    {
        _context = context;
    }

    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetSecret()
        => "secret text";

    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {
        var thing = _context.Users.Find(-1);

        if (thing == null)
            return NotFound();

        return thing;
    }

    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    {
        var thing = _context.Users.Find(-1);
        var thingToReturn = thing!.ToString();
        return thingToReturn!;
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
        => BadRequest("Bad Request error");


}
