using Kevsoft.WLED;

// Point this at your own device's address.
var client = new WLedClient("http://wled-office-computer-wled/");

// --- Simple intent methods -------------------------------------------------

await client.TurnOn();
await client.SetBrightness(200);
await client.SetColor(RgbColor.FromHex("FFAA00"));   // warm orange
await client.SetEffect(9);                            // "Rainbow"
await client.SetPalette(11);                          // "Rainbow"

// --- Reading state ---------------------------------------------------------

var info = await client.GetInformation();
Console.WriteLine($"Connected to {info.Name} running WLED {info.VersionName} with {info.Leds.Count} LEDs.");

var state = await client.GetState();
Console.WriteLine($"Power: {(state.On ? "on" : "off")}, brightness: {state.Brightness}");

// --- Sparse, fluent state updates -----------------------------------------

await client.UpdateState(update => update
    .TurnOn()
    .Brightness(128)
    .Segment(0, segment => segment
        .Effect(0)
        .Color(RgbColor.FromHex("0066FF"))));

// --- Individual LED control ------------------------------------------------

await client.SetIndividualLeds(0, leds => leds
    .Set(0, RgbColor.FromHex("FF0000"))
    .Set(1, RgbColor.FromHex("00FF00"))
    .Set(2, RgbColor.FromHex("0000FF")));

// --- Presets & playlists ---------------------------------------------------

var presets = await client.GetPresets();
foreach (var (id, preset) in presets)
{
    Console.WriteLine($"Preset {id}: {preset.Name}");
}

await client.StartPlaylist(playlist => playlist
    .Add(1, TimeSpan.FromSeconds(10))
    .Add(2, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(1))
    .Repeat(3));

// --- Discovering other devices --------------------------------------------

foreach (var node in await client.GetNodes())
{
    Console.WriteLine($"Found node {node.Name} at {node.IpAddress}");
}

await client.TurnOff();
