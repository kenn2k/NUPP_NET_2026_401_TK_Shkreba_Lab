namespace OnlineStore.App.External;

/// <summary>
/// Contact DTO received from the external JSONPlaceholder reference API.
/// </summary>
public sealed record ExternalUser(int Id, string Name, string Email);
