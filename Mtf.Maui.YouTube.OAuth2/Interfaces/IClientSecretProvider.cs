namespace Mtf.Maui.YouTube.OAuth2.Interfaces;

public interface IClientSecretProvider
{
    string ClientId { get; }

    string? ClientSecret { get; }
}
