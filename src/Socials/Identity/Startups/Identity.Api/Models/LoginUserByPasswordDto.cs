using System.ComponentModel.DataAnnotations;

namespace TL.Socials.Identity.Startups.Api.Models;

public sealed record LoginUserByPasswordDto([EmailAddress] string Email, string Password);