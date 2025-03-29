using System.ComponentModel.DataAnnotations;

namespace TL.Socials.Identity.Startups.Api.Models;

public sealed record RegisterUserViaEmailDto([EmailAddress] string Email, string Password, string Token);