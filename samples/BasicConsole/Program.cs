using Kevsoft.WLED;

var client = new WLedClient("http://wled-office-computer-wle/");
var wLedRootResponse = await client.Get();

wLedRootResponse.State.On = true;

await client.Post(wLedRootResponse.State);