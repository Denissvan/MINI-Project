# WsMod_Shp2 After-Check Photo Design

## Goal

Add an operator-controlled option that captures one extra `WsMod_Shp2` photo immediately after the normal `WsMod_Shp` back-check photo.

The extra photo is only for lighting setup and image saving. It must not change the existing `WsMod_Shp` OK/NG decision and must not reuse the current `Check2open` second-check logic.

## Current Behavior

The normal upper-camera back-check runs `WsMod_Shp` through `UpCamBackCheck()`. Its result is used for placement and connector-area checks.

`Check2open` currently enables `UpCamBackCheck2()`, which runs `WsMod_Shp2`, offsets the photo position by `xt.DwCapQrCodeoffset`, parses area data from the VPP message, and can mark the back-check as failed. This is production judgment logic, so it is not suitable for the new light-adjustment photo.

## New Parameters

Add two product/system parameters under `OTHER_SET`:

- `BWSMOD_SHP2_AFTER_CHECK_PHOTO`: enables the extra `WsMod_Shp2` capture after `WsMod_Shp`.
- `WSMOD_SHP2_AFTER_CHECK_SAVE_PATH`: optional image save directory.

If the save path is empty, the program uses a default directory under the current product image folder:

```text
<current product path>\image\<CamUp name>\WSMOD_SHP2_LIGHT
```

## UI

Add controls on the last parameter page, `FrSys.tabPage8`:

- Checkbox: `回检后追加WsMod_Shp2拍照`
- Text box: configured save directory
- Button: `选择路径`

The checkbox reads from and writes to `PT_SET.bWsModShp2PhotoAfterCheck`. The text box reads from and writes to `PT_SET.WsModShp2PhotoSavePath`.

## Runtime Flow

Inside `UpCamBackCheck()`, after `WsMod_Shp` has captured successfully, check the new enable flag.

When enabled, call a new helper such as `CaptureWsModShp2PhotoOnly(...)`:

1. Use the same base back-check position as the completed `WsMod_Shp` capture.
2. Run `xt.UpCam(..., CONST.WsModUpFw2, ...)`.
3. Save the resulting original image to the configured path or default path.
4. Log success or failure.
5. Return without affecting the `WsMod_Shp` result.

The helper must not parse area values, must not compare against `LeftArea3` through `Area4`, and must not change `FeedBackWs`, barcode state, or placement result.

## Failure Handling

Because this extra photo is for lighting setup and image collection, its failure should not block production.

If `WsMod_Shp2` capture fails:

- Record an error or warning message with the flow name and return code.
- Do not retry automatically.
- Do not show a blocking operator dialog.
- Do not change the return value of `UpCamBackCheck()`.

If the configured save path is invalid or cannot be written:

- Attempt to create the directory.
- If creation or saving fails, log the exception.
- Continue the main back-check result unchanged.

## Scope

In scope:

- Parameter storage in `PT_SET`.
- Last parameter page UI.
- Extra `WsMod_Shp2` capture after successful `WsMod_Shp`.
- Image saving to configured or default path.
- Non-blocking logging on extra-capture failures.

Out of scope:

- Changing `Check2open` behavior.
- Changing `UpCamBackCheck2()` judgment logic.
- Changing the `WsMod_Shp` result or existing barcode back-check behavior.
- Editing the VPP files themselves.

## Verification

Manual verification should cover:

- With the new checkbox off, back-check behavior and image saving remain unchanged.
- With the checkbox on, each successful `WsMod_Shp` back-check is followed by one `WsMod_Shp2` capture.
- Extra `WsMod_Shp2` image files appear in the configured directory.
- Missing or failing `WsMod_Shp2` logs an error but does not fail the original `WsMod_Shp` back-check.
- Existing `Check2open` behavior remains unchanged when enabled.
