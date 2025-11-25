using System.ComponentModel.DataAnnotations;

namespace TL.Socials.Identity.Startups.Api.Models;

public sealed record RequestUserRegisterViaEmailDto([EmailAddress] string Email);
