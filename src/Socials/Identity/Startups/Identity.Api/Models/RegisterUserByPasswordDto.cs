using System.ComponentModel.DataAnnotations;

namespace TL.Socials.Identity.Startups.Api.Models;

public sealed record RegisterUserByPasswordDto([EmailAddress] string Email, string Password);