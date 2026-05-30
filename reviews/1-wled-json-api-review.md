# WLED JSON API coverage review

Compared against the current WLED JSON API documentation at <https://kno.wled.ge/interfaces/json-api/> on 2026-05-30.

Overall, the library is in good shape: the major documented endpoints are present (`/json`, `/json/state`, `/json/info`, `/json/si`, `/json/nodes`, `/json/eff`, `/json/pal`, `/json/fxdata`, `/json/net`, `/json/live`, `/json/cfg`), and the strongly typed command/value objects are a good fit for the "hard to misuse" goal. The review below focuses on gaps or behavior that does not line up with the docs.

## Review comments

### 1. High - "selected segments" intent methods currently target segment 0

**Docs:** The API says `seg` can be either an object or an array. When `seg` is an array and a segment object omits `id`, the ID is inferred from the object's array position. The docs' selected-segment examples use the object form, e.g. `{"seg":{"fx":"r"}}`, for applying to all selected segments.

**Code:** `WLedClient.SetColor`, `SetEffect`, and `SetPalette` say "selected segments when no id is given" in `IWLedClient.cs`, but they all call `SingleSegment(...)`, which always serializes `seg` as a one-element array (`WLedClient.cs:114-124`, `WLedClient.cs:272-277`). The tests also assert that the no-id form is `seg: [{ ... }]` (`IntentMethodTests.cs:47-56`, `IntentMethodTests.cs:80-85`).

**Why it matters:** Per the docs, `{"seg":[{"fx":42}]}` infers `id:0`, so the ergonomic methods likely update segment 0 rather than the selected segments. This is a behavior bug and contradicts the public XML comments.

**Suggested fix:** Model the `seg` request as a union (`SegmentRequest` object or `SegmentRequest[]`) and have no-id intent methods emit the object form. Keep explicit `segmentId` calls as an array with `id`.

### 2. High - Transition values are limited to `byte`, but the API allows `0..65535`

**Docs:** State `transition` and one-shot `tt` are documented as `0 to 65535`, in 100 ms units.

**Code:** `StateResponse.Transition` is `byte` (`StateResponse.cs:20-21`), `StateRequest.Transition` and `TransientTransition` are `byte?` (`StateRequest.cs:16-26`), and `StateUpdate.ToTransitionUnits` clamps to `byte.MaxValue` (`StateUpdate.cs:134-147`). The fluent comments also say the max is 25.5s (`StateUpdate.cs:35`), but the documented API supports roughly 109 minutes.

**Why it matters:** The client cannot represent or send valid WLED transition values above 255. It will also deserialize valid responses incorrectly/fail if WLED returns a transition above 255.

**Suggested fix:** Change transition fields and conversion helpers to `ushort`/`ushort?`, clamp to `ushort.MaxValue`, and update tests to include a value above 255 units.

### 3. Medium - Effect/palette selector does not support documented ranged random syntax

**Docs:** The examples include ranged random palette selection, e.g. `{"seg":[{"id":2,"pal":"5~10r"}]}`. Presets already support similar range syntax.

**Code:** `Selector` supports only id, next, previous, and random (`Selector.cs:10-38`), and `SelectorJsonConverter` rejects anything except `"~"`, `"~-"`, and `"r"` (`SelectorJsonConverter.cs:13-21`). `PresetSelector` has `RandomInRange`, but effect/palette selection does not (`PresetSelector.cs:40-45`).

**Why it matters:** A documented WLED command cannot be expressed with the strongly typed API, and JSON responses/payloads containing that token would fail to deserialize.

**Suggested fix:** Extend `Selector` with a ranged random value (and possibly a range validator), serialize it as `"{from}~{to}r"`, and parse that token in `SelectorJsonConverter`.

### 4. Medium - `/json/palx` is listed in the API routes but is not modeled

**Docs:** The API route list includes `/json/palx` alongside `/json/pal` and `/json/cfg`.

**Code:** There is support for palette names via `/json/pal` (`WLedClient.cs:81-82`), but no client method or model for `/json/palx`.

**Why it matters:** If `/json/palx` is the custom palette read/write endpoint, the current README's "full JSON API" capability story is overstated for palette manipulation.

**Suggested fix:** Confirm the exact `/json/palx` wire format from firmware/source or companion libraries, then add typed models and client methods. If it is intentionally unsupported, document it explicitly in the feature matrix.

### 5. Medium - `info.sensor` draft API is not preserved or exposed

**Docs:** The JSON API page includes a draft Sensors API where `info.sensor` is an array of sensor objects. The object shape is intentionally extensible (`type`, `n`, `val`, `unit`, `error`, timing fields, bounds, uncertainty, model, etc.).

**Code:** `InformationResponse` ends at `ip` and has no `sensor` property or extension data (`InformationResponse.cs:151-155`).

**Why it matters:** Devices with usermods exposing sensor data will silently lose that information. Because this is read-only data, exposing it would not risk invalid writes.

**Suggested fix:** Add `InformationResponse.Sensors` as an array of a flexible `SensorResponse` type with `JsonElement? Value`, typed common metadata, and `JsonExtensionData` for usermod-specific fields.

### 6. Medium - Config modeling is intentionally lossless, but not ergonomic enough for "hard to misuse"

**Docs:** `/json/cfg` exposes many configuration sections. The project goal is not just field mapping, but ergonomic modeling that prevents invalid requests.

**Code:** `DeviceConfig` uses typed section shells plus `JsonExtensionData` for almost everything (`DeviceConfig.cs:7-100`). This is safe for round-tripping and partial writes, and the network/access-point guard is good, but only `id.name` is modeled.

**Why it matters:** Callers still have to hand-build raw `JsonElement` values for most configuration changes. That is safe for data preservation, but not yet "impossible to send the wrong information" for config.

**Suggested fix:** Incrementally model high-value, low-risk sections with small value objects/enums (identity, sync/UDP, MQTT enablement, brightness defaults, LED count/bus read views), keeping `JsonExtensionData` as an escape hatch.

### 7. Low - Color temperature range is stricter than the docs' forward-compat guidance

**Docs:** The CCT section first describes `0..255` or `1900..10091`, then says integrations should expect Kelvin values in a broader future-compatible range (`1000..16000`, and later `1000..20000 K`) and prefer preserving the range received from WLED.

**Code:** `ColorTemperature.Kelvin` only allows `1900..10091` (`ColorTemperature.cs:11-38`). Reading is permissive (`FromWire` treats any value above 255 as Kelvin), but callers cannot intentionally send a broader Kelvin value back.

**Why it matters:** Newer firmware or hardware-specific CCT setups could legitimately use values outside the current constructor's limits, making the typed API more restrictive than WLED.

**Suggested fix:** Consider widening the constructor to the docs' forward-compatible range, or add a clearly named escape hatch such as `ColorTemperature.KelvinUnchecked(int)` if strict validation is still preferred by default.

### 8. Low - Random color slots are not represented

**Docs:** Segment `col` can be color arrays, hex strings, and the docs mention random color slots using `"r"` (noting this is future/soon behavior).

**Code:** `ColorJsonConverter` reads all strings as hex (`ColorJsonConverter.cs:11-14`) and writes only numeric arrays (`ColorJsonConverter.cs:41-52`). There is no `Random` color slot representation.

**Why it matters:** Once WLED supports random color slots in stable firmware, the current `Color` model will reject that documented token and callers cannot request random colors through the strongly typed API.

**Suggested fix:** Track this as a future compatibility item. If implementing now, introduce a separate `ColorSlot` union (`Rgb`, `Rgbw`, `Random`) rather than weakening the existing `Color` type.

## Positive notes

- Individual LED control matches the docs well: RGB writes use compact hex strings, RGBW uses arrays, and large updates are split into sequential requests rather than sent in parallel.
- Effect metadata parsing correctly accounts for missing vs empty sections and filters `RSVD` / `-` while preserving original effect IDs.
- Presets/playlists are modeled much more ergonomically than the raw WLED JSON and align with the documented parallel arrays.
- The config API's `JsonExtensionData` approach is a safe baseline because it avoids dropping firmware-specific or future settings.
