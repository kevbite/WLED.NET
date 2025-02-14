namespace Kevsoft.WLED;

public interface IWLedClient
{
    Task<WLedRootResponse> Get();

    Task<StateResponse> GetState();

    Task<InformationResponse> GetInformation();

    Task<string[]> GetEffects();

    Task<string[]> GetPalettes();

    Task Post(WLedRootRequest request);

    Task Post(StateRequest request);
}