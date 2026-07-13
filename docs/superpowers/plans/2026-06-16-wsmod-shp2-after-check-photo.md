# WsMod_Shp2 After-Check Photo Implementation Plan

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** Add an operator-controlled option that captures and saves one non-blocking `WsMod_Shp2` image immediately after a successful `WsMod_Shp` back-check.

**Architecture:** Follow the existing static `PT_SET` parameter model and WinForms `FrSys` parameter page pattern. Keep the new `WsMod_Shp2` capture separate from `Check2open` and `UpCamBackCheck2()` so it never participates in production OK/NG judgment.

**Tech Stack:** C# WinForms, VisionPro `CogToolBlock` VPP tasks, existing INI helper APIs in `MotionCtrl.MyType`, existing camera APIs in `UI.Class.Cam` and `UI.Class.XT`.

---

## File Map

- `MotionCtrl/MyType.cs`: Add two static parameters, read them from `OTHER_SET`, and write them back to the INI file.
- `UI/FormView/FrSys.Designer.cs`: Add checkbox, path textbox, and path browse button on `tabPage8`.
- `UI/FormView/FrSys.cs`: Load/save the new UI controls and implement folder selection for the save path.
- `UI/Class/UpDownLoad.cs`: Add a non-blocking helper that runs `WsMod_Shp2` after successful `WsMod_Shp` and saves the raw image.
- `docs/superpowers/specs/2026-06-16-wsmod-shp2-after-check-photo-design.md`: Already written design reference; do not modify unless the implementation changes scope.

## Constraints

- Do not change `PT_SET.Check2open`.
- Do not change `UpCamBackCheck2()` behavior.
- Do not let the extra `WsMod_Shp2` capture change the return value of `UpCamBackCheck()`.
- Do not parse `WsMod_Shp2` area values for this feature.
- Do not show blocking dialogs for this extra capture.
- Preserve existing unrelated dirty worktree changes.
- Before editing any target file, save its current diff to `sessions/general/wsmod-shp2-after-check-photo/` so existing user changes can be distinguished from feature changes.
- If a target file already has unrelated user edits, do not create a commit that stages the whole file. Leave changes uncommitted and report the exact feature edits instead, unless the user explicitly approves committing the mixed file.

---

### Task 1: Add PT_SET Parameters

**Files:**
- Modify: `MotionCtrl/MyType.cs`

- [ ] **Step 1: Capture the existing file diff**

Run:

```powershell
New-Item -ItemType Directory -Force sessions\general\wsmod-shp2-after-check-photo | Out-Null
git diff -- MotionCtrl\MyType.cs > sessions\general\wsmod-shp2-after-check-photo\MyType.before.diff
```

Expected: The `.before.diff` file records any pre-existing edits. If it is empty, `MotionCtrl/MyType.cs` was clean before this task.

- [ ] **Step 2: Add static fields near existing back-check photo parameters**

Find the area around the existing fields:

```csharp
public static bool Check2open;  //对于长模组的情况
public static bool bJigDownPhoto;  //夹具闭合后拍照
public static double JigDownPhotoIntervalHours;  //夹具闭合后拍照间隔时间，单位小时，0表示每次触发
```

Add:

```csharp
public static bool bWsModShp2PhotoAfterCheck;  //回检后追加WsMod_Shp2拍照，仅用于调光和存图
public static string WsModShp2PhotoSavePath = "";  //回检后WsMod_Shp2图片保存路径，空则使用默认路径
```

- [ ] **Step 3: Read the fields from `OTHER_SET`**

Find the existing load block around:

```csharp
bAddCapQrcode = inf.ReadBool("OTHER_SET", "bAddCapQrcode", false);
bDwAddCapQrcode = inf.ReadBool("OTHER_SET", "bDwAddCapQrcode", false);
Check2open = inf.ReadBool("OTHER_SET", "Check2open", false);
```

Add directly after `Check2open`:

```csharp
bWsModShp2PhotoAfterCheck = inf.ReadBool("OTHER_SET", "BWSMOD_SHP2_AFTER_CHECK_PHOTO", false);
WsModShp2PhotoSavePath = inf.ReadString("OTHER_SET", "WSMOD_SHP2_AFTER_CHECK_SAVE_PATH", "");
```

- [ ] **Step 4: Write the fields to `OTHER_SET`**

Find the existing save block around:

```csharp
inf.WriteBool("OTHER_SET", "bAddCapQrcode", bAddCapQrcode, ref ischange, true, filename);
inf.WriteBool("OTHER_SET", "bDwAddCapQrcode", bDwAddCapQrcode, ref ischange, true, filename);
inf.WriteBool("OTHER_SET", "Check2open", Check2open, ref ischange, true, filename);
```

Add directly after `Check2open`:

```csharp
inf.WriteBool("OTHER_SET", "BWSMOD_SHP2_AFTER_CHECK_PHOTO", bWsModShp2PhotoAfterCheck, ref ischange, true, filename);
inf.WriteString("OTHER_SET", "WSMOD_SHP2_AFTER_CHECK_SAVE_PATH", WsModShp2PhotoSavePath ?? "", ref ischange, true, filename);
```

- [ ] **Step 5: Build-check the parameter file**

Run:

```powershell
dotnet build MTLAssemble\CPMPro.csproj
```

Expected: The build may fail because this legacy solution depends on local VS/Framework setup, but there should be no new compile errors in `MotionCtrl/MyType.cs`. If `dotnet build` is not the project’s valid build command, use the existing Visual Studio/MSBuild command available in this workspace and record the result.

- [ ] **Step 6: Review feature diff**

Run:

```powershell
git diff -- MotionCtrl\MyType.cs
```

Expected: The new diff contains only the two new parameter fields plus their INI read/write lines, in addition to any already-recorded baseline changes from `MyType.before.diff`.

- [ ] **Step 7: Commit parameter storage only if the file was clean before this task**

Run:

```powershell
if ((Get-Content sessions\general\wsmod-shp2-after-check-photo\MyType.before.diff -Raw).Length -eq 0) {
    git add MotionCtrl\MyType.cs
    git commit -m "feat: add wsmod shp2 after-check photo parameters"
} else {
    Write-Host "MotionCtrl\MyType.cs had pre-existing edits; skip commit and report mixed diff."
}
```

Expected: If the file was clean, commit includes only `MotionCtrl/MyType.cs`. If it was dirty, no commit is made.

---

### Task 2: Add Parameter Page UI

**Files:**
- Modify: `UI/FormView/FrSys.Designer.cs`
- Modify: `UI/FormView/FrSys.cs`

- [ ] **Step 1: Capture existing file diffs**

Run:

```powershell
New-Item -ItemType Directory -Force sessions\general\wsmod-shp2-after-check-photo | Out-Null
git diff -- UI\FormView\FrSys.Designer.cs > sessions\general\wsmod-shp2-after-check-photo\FrSys.Designer.before.diff
git diff -- UI\FormView\FrSys.cs > sessions\general\wsmod-shp2-after-check-photo\FrSys.before.diff
```

Expected: The `.before.diff` files record any pre-existing edits. Empty files mean those source files were clean before this task.

- [ ] **Step 2: Add control fields in `FrSys.Designer.cs`**

Near the existing final field declarations:

```csharp
private System.Windows.Forms.NumericUpDown nudJigDownPhotoIntervalHours;
private System.Windows.Forms.Label lblJigDownPhotoIntervalHours;
private System.Windows.Forms.CheckBox ckJigDownPhoto;
```

Add:

```csharp
private System.Windows.Forms.CheckBox ckWsModShp2PhotoAfterCheck;
private System.Windows.Forms.TextBox txtWsModShp2PhotoSavePath;
private System.Windows.Forms.Button btnSelectWsModShp2PhotoSavePath;
private System.Windows.Forms.Label lblWsModShp2PhotoSavePath;
```

- [ ] **Step 3: Instantiate controls in `InitializeComponent()`**

Near the existing `ckJigDownPhoto` component creation:

```csharp
this.nudJigDownPhotoIntervalHours = new System.Windows.Forms.NumericUpDown();
this.lblJigDownPhotoIntervalHours = new System.Windows.Forms.Label();
this.ckJigDownPhoto = new System.Windows.Forms.CheckBox();
```

Add:

```csharp
this.ckWsModShp2PhotoAfterCheck = new System.Windows.Forms.CheckBox();
this.txtWsModShp2PhotoSavePath = new System.Windows.Forms.TextBox();
this.btnSelectWsModShp2PhotoSavePath = new System.Windows.Forms.Button();
this.lblWsModShp2PhotoSavePath = new System.Windows.Forms.Label();
```

- [ ] **Step 4: Add controls to the last parameter page group**

Find where `groupBox75` adds the jig-down-photo controls:

```csharp
this.groupBox75.Controls.Add(this.nudJigDownPhotoIntervalHours);
this.groupBox75.Controls.Add(this.lblJigDownPhotoIntervalHours);
this.groupBox75.Controls.Add(this.ckJigDownPhoto);
```

Add the new controls to the same group:

```csharp
this.groupBox75.Controls.Add(this.ckWsModShp2PhotoAfterCheck);
this.groupBox75.Controls.Add(this.txtWsModShp2PhotoSavePath);
this.groupBox75.Controls.Add(this.btnSelectWsModShp2PhotoSavePath);
this.groupBox75.Controls.Add(this.lblWsModShp2PhotoSavePath);
```

- [ ] **Step 5: Configure the new controls**

Place this near the existing `ckJigDownPhoto` and interval control property assignments:

```csharp
// ckWsModShp2PhotoAfterCheck
this.ckWsModShp2PhotoAfterCheck.AutoSize = true;
this.ckWsModShp2PhotoAfterCheck.Font = new System.Drawing.Font("微软雅黑", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
this.ckWsModShp2PhotoAfterCheck.Location = new System.Drawing.Point(20, 118);
this.ckWsModShp2PhotoAfterCheck.Name = "ckWsModShp2PhotoAfterCheck";
this.ckWsModShp2PhotoAfterCheck.Size = new System.Drawing.Size(230, 29);
this.ckWsModShp2PhotoAfterCheck.TabIndex = 150;
this.ckWsModShp2PhotoAfterCheck.Text = "回检后追加WsMod_Shp2拍照";
this.ckWsModShp2PhotoAfterCheck.UseVisualStyleBackColor = true;

// lblWsModShp2PhotoSavePath
this.lblWsModShp2PhotoSavePath.AutoSize = true;
this.lblWsModShp2PhotoSavePath.Font = new System.Drawing.Font("微软雅黑", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
this.lblWsModShp2PhotoSavePath.Location = new System.Drawing.Point(20, 158);
this.lblWsModShp2PhotoSavePath.Name = "lblWsModShp2PhotoSavePath";
this.lblWsModShp2PhotoSavePath.Size = new System.Drawing.Size(74, 21);
this.lblWsModShp2PhotoSavePath.TabIndex = 151;
this.lblWsModShp2PhotoSavePath.Text = "保存路径";

// txtWsModShp2PhotoSavePath
this.txtWsModShp2PhotoSavePath.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
this.txtWsModShp2PhotoSavePath.Location = new System.Drawing.Point(100, 154);
this.txtWsModShp2PhotoSavePath.Name = "txtWsModShp2PhotoSavePath";
this.txtWsModShp2PhotoSavePath.Size = new System.Drawing.Size(360, 27);
this.txtWsModShp2PhotoSavePath.TabIndex = 152;

// btnSelectWsModShp2PhotoSavePath
this.btnSelectWsModShp2PhotoSavePath.Font = new System.Drawing.Font("微软雅黑", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
this.btnSelectWsModShp2PhotoSavePath.Location = new System.Drawing.Point(470, 153);
this.btnSelectWsModShp2PhotoSavePath.Name = "btnSelectWsModShp2PhotoSavePath";
this.btnSelectWsModShp2PhotoSavePath.Size = new System.Drawing.Size(88, 30);
this.btnSelectWsModShp2PhotoSavePath.TabIndex = 153;
this.btnSelectWsModShp2PhotoSavePath.Text = "选择路径";
this.btnSelectWsModShp2PhotoSavePath.UseVisualStyleBackColor = true;
this.btnSelectWsModShp2PhotoSavePath.Click += new System.EventHandler(this.btnSelectWsModShp2PhotoSavePath_Click);
```

If `groupBox75` is too short for the new controls, increase only that group’s `Size.Height` enough to show the controls without overlap. Do not rearrange unrelated controls.

- [ ] **Step 6: Load UI values in `FrSys.cs`**

Find the parameter load block around:

```csharp
ckJigDownPhoto.Checked = PT_SET.bJigDownPhoto;
nudJigDownPhotoIntervalHours.Value = (decimal)Math.Max(0, Math.Min((double)nudJigDownPhotoIntervalHours.Maximum, PT_SET.JigDownPhotoIntervalHours));
rbtn_DwAddCapQrcodeEn.Checked = PT_SET.bDwAddCapQrcode;
```

Add:

```csharp
ckWsModShp2PhotoAfterCheck.Checked = PT_SET.bWsModShp2PhotoAfterCheck;
txtWsModShp2PhotoSavePath.Text = PT_SET.WsModShp2PhotoSavePath ?? "";
```

- [ ] **Step 7: Save UI values in `FrSys.cs`**

Find the save block around:

```csharp
PT_SET.Check2open = rbtn_check2open.Checked;
PT_SET.bJigDownPhoto = ckJigDownPhoto.Checked;
PT_SET.JigDownPhotoIntervalHours = (double)nudJigDownPhotoIntervalHours.Value;
```

Add:

```csharp
PT_SET.bWsModShp2PhotoAfterCheck = ckWsModShp2PhotoAfterCheck.Checked;
PT_SET.WsModShp2PhotoSavePath = txtWsModShp2PhotoSavePath.Text.Trim();
```

- [ ] **Step 8: Add folder picker handler in `FrSys.cs`**

Add this method inside the `FrSys` class:

```csharp
private void btnSelectWsModShp2PhotoSavePath_Click(object sender, EventArgs e)
{
    using (FolderBrowserDialog dialog = new FolderBrowserDialog())
    {
        dialog.Description = "选择WsMod_Shp2附加拍照图片保存路径";
        if (!string.IsNullOrWhiteSpace(txtWsModShp2PhotoSavePath.Text) && Directory.Exists(txtWsModShp2PhotoSavePath.Text))
        {
            dialog.SelectedPath = txtWsModShp2PhotoSavePath.Text;
        }

        if (dialog.ShowDialog() == DialogResult.OK)
        {
            txtWsModShp2PhotoSavePath.Text = dialog.SelectedPath;
        }
    }
}
```

If `System.IO` is not already imported at the top of `FrSys.cs`, add:

```csharp
using System.IO;
```

- [ ] **Step 9: Compile-check UI changes**

Run:

```powershell
dotnet build MTLAssemble\CPMPro.csproj
```

Expected: No new compile errors for `FrSys.Designer.cs` or `FrSys.cs`. If the command is not valid for this legacy project, use the project’s established MSBuild command and record the result.

- [ ] **Step 10: Review feature diff**

Run:

```powershell
git diff -- UI\FormView\FrSys.Designer.cs UI\FormView\FrSys.cs
```

Expected: The new diff contains only the new controls, load/save wiring, and folder picker handler, in addition to any already-recorded baseline changes from the two `.before.diff` files.

- [ ] **Step 11: Commit UI changes only if both files were clean before this task**

Run:

```powershell
$designerDirty = (Get-Content sessions\general\wsmod-shp2-after-check-photo\FrSys.Designer.before.diff -Raw).Length -ne 0
$codeDirty = (Get-Content sessions\general\wsmod-shp2-after-check-photo\FrSys.before.diff -Raw).Length -ne 0
if (-not $designerDirty -and -not $codeDirty) {
    git add UI\FormView\FrSys.Designer.cs UI\FormView\FrSys.cs
    git commit -m "feat: add wsmod shp2 photo settings UI"
} else {
    Write-Host "FrSys files had pre-existing edits; skip commit and report mixed diff."
}
```

Expected: If both files were clean, commit includes only the two `FrSys` files. If either file was dirty, no commit is made.

---

### Task 3: Add Non-Blocking WsMod_Shp2 Capture

**Files:**
- Modify: `UI/Class/UpDownLoad.cs`

- [ ] **Step 1: Capture the existing file diff**

Run:

```powershell
New-Item -ItemType Directory -Force sessions\general\wsmod-shp2-after-check-photo | Out-Null
git diff -- UI\Class\UpDownLoad.cs > sessions\general\wsmod-shp2-after-check-photo\UpDownLoad.before.diff
```

Expected: The `.before.diff` file records any pre-existing edits. If it is empty, `UI/Class/UpDownLoad.cs` was clean before this task.

- [ ] **Step 2: Add helper to resolve the save directory**

Add this private method inside the `UpDownLoad` class near the back-check helper methods:

```csharp
private string GetWsModShp2PhotoSavePath(Cam cam)
{
    string configuredPath = PT_SET.WsModShp2PhotoSavePath;
    if (!string.IsNullOrWhiteSpace(configuredPath))
    {
        return configuredPath;
    }

    string camName = cam == null ? "CamUp" : cam.mName;
    return Path.Combine(VAR.gsys_set.GetCurProductPath, "image", camName, "WSMOD_SHP2_LIGHT");
}
```

If `System.IO` is not already imported at the top of `UpDownLoad.cs`, add:

```csharp
using System.IO;
```

- [ ] **Step 3: Add non-blocking capture helper**

Add this private method inside the `UpDownLoad` class near `UpCamBackCheck()`:

```csharp
private void CaptureWsModShp2PhotoOnly(ref bool bquit, XT xt, ST_XY posModUpcam)
{
    if (!PT_SET.bWsModShp2PhotoAfterCheck)
    {
        return;
    }

    if (xt == null || xt.upcam == null)
    {
        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} WsMod_Shp2附加拍照跳过，吸头或上相机为空", disc));
        return;
    }

    try
    {
        VisionOutPutData resData = new VisionOutPutData();
        EM_RES res = xt.UpCam(ref bquit, posModUpcam, CONST.WsModUpFw2, ref resData, Demo);
        if (res != EM_RES.OK)
        {
            VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} WsMod_Shp2附加拍照失败，流程:{1}, 返回:{2}", xt.disc, CONST.WsModUpFw2, res));
            return;
        }

        string savePath = GetWsModShp2PhotoSavePath(xt.upcam);
        Directory.CreateDirectory(savePath);
        xt.upcam.SaveOriginImage(xt.upcam.curTask.Image, savePath, string.Format("{0}_{1}_{2}.jpg",
            DateTime.Now.ToString("yyyyMMdd"),
            DateTime.Now.ToString("HHmmss_fff"),
            CONST.WsModUpFw2));

        VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0} WsMod_Shp2附加拍照完成，图片路径:{1}", xt.disc, savePath));
    }
    catch (Exception ex)
    {
        VAR.msg.AddMsg(Msg.EM_MSGTYPE.ERR, string.Format("{0} WsMod_Shp2附加拍照异常:{1}", xt.disc, ex.Message));
    }
}
```

Do not return `EM_RES` from this helper. It is intentionally non-blocking.

- [ ] **Step 4: Call the helper after successful `WsMod_Shp` capture**

In `UpCamBackCheck()`, find:

```csharp
VAR.msg.AddMsg(Msg.EM_MSGTYPE.NOR, string.Format("{0}上相机拍照完成，数据X:{1},Y:{2},Z:{3}", xt.disc, ResData.PosMM.x, ResData.PosMM.y, ResData.PosMM.a));
```

Add immediately after it:

```csharp
CaptureWsModShp2PhotoOnly(ref bquit, xt, pos_mod_upcam);
```

This placement means:

- `WsMod_Shp2` runs only after `WsMod_Shp` capture returned OK.
- Existing area parsing and judgment still use `WsMod_Shp` data from `ResData`.
- The extra capture does not change the method’s return path.

- [ ] **Step 5: Confirm `curTask` side effect does not break existing `WsMod_Shp` logic**

After adding the helper call, inspect the rest of `UpCamBackCheck()`. If any later code reads `upcam.curTask.ResData` expecting `WsMod_Shp`, convert those reads to use the already-copied `ResData` variable before the helper runs.

Specifically, change this pattern after the helper call:

```csharp
date = upcam.curTask.ResData.Message.Split(',');
```

to:

```csharp
date = ResData.Message.Split(',');
```

And change this pattern:

```csharp
if (xt.upcam.curTask.ResData.bOK)
```

to:

```csharp
if (ResData.bOK)
```

Keep image saving for original back-check failures using the existing `upcam.curTask.Image` only if it still refers to the desired image. If it now refers to `WsMod_Shp2`, save `ResData.OutputImg` through `SaveOriginImage()` instead:

```csharp
upcam.SaveOriginImage(ResData.OutputImg, string.Format("{0}\\image\\{1}\\BACK", VAR.gsys_set.GetCurProductPath, upcam.mName), string.Format("{0}{1}.jpg",
    DateTime.Now.ToString("yyyyMMdd"), DateTime.Now.ToString("HHmmss_fff")));
```

- [ ] **Step 6: Compile-check runtime changes**

Run:

```powershell
dotnet build MTLAssemble\CPMPro.csproj
```

Expected: No new compile errors for `UpDownLoad.cs`. If `dotnet build` is not valid, use the established MSBuild command and record the result.

- [ ] **Step 7: Review feature diff**

Run:

```powershell
git diff -- UI\Class\UpDownLoad.cs
```

Expected: The new diff contains only the helper methods, the helper call, and any necessary `ResData` substitutions caused by the extra capture, in addition to any already-recorded baseline changes from `UpDownLoad.before.diff`.

- [ ] **Step 8: Commit runtime changes only if the file was clean before this task**

Run:

```powershell
if ((Get-Content sessions\general\wsmod-shp2-after-check-photo\UpDownLoad.before.diff -Raw).Length -eq 0) {
    git add UI\Class\UpDownLoad.cs
    git commit -m "feat: capture wsmod shp2 image after back-check"
} else {
    Write-Host "UI\Class\UpDownLoad.cs had pre-existing edits; skip commit and report mixed diff."
}
```

Expected: If the file was clean, commit includes only `UI/Class/UpDownLoad.cs`. If it was dirty, no commit is made.

---

### Task 4: End-to-End Verification

**Files:**
- Verify only; no planned edits.

- [ ] **Step 1: Confirm no unrelated files are staged**

Run:

```powershell
git status --short
```

Expected: Existing unrelated dirty files may remain, but staged changes should be empty after the task commits.

- [ ] **Step 2: Verify parameters persist**

Manual test in the application:

1. Open the parameter page last tab.
2. Check `回检后追加WsMod_Shp2拍照`.
3. Select a save directory.
4. Save parameters.
5. Restart the application or reload parameters.
6. Confirm the checkbox and path are restored.

Expected: INI contains:

```ini
[OTHER_SET]
BWSMOD_SHP2_AFTER_CHECK_PHOTO=True
WSMOD_SHP2_AFTER_CHECK_SAVE_PATH=<selected path>
```

- [ ] **Step 3: Verify disabled behavior**

Manual test:

1. Uncheck the new option.
2. Run a normal back-check that calls `WsMod_Shp`.

Expected:

- No `WsMod_Shp2附加拍照完成` log appears.
- No new image appears in `WSMOD_SHP2_LIGHT`.
- Existing `WsMod_Shp` judgment is unchanged.

- [ ] **Step 4: Verify enabled behavior**

Manual test:

1. Check the new option.
2. Set a writable save directory.
3. Run a normal back-check that calls `WsMod_Shp`.

Expected:

- `WsMod_Shp` completes and its normal judgment continues.
- One `WsMod_Shp2` capture runs immediately afterward.
- Log contains `WsMod_Shp2附加拍照完成`.
- One JPG appears in the configured directory.

- [ ] **Step 5: Verify missing or failing `WsMod_Shp2` is non-blocking**

Manual test on a controlled setup:

1. Temporarily use a product/camera setup without `WsMod_Shp2.vpp`, or make `WsMod_Shp2` fail in a safe test environment.
2. Run a normal `WsMod_Shp` back-check with the new option enabled.

Expected:

- Log contains `WsMod_Shp2附加拍照失败`.
- `UpCamBackCheck()` still returns based on the original `WsMod_Shp` result.
- No operator blocking dialog appears from the extra capture.

- [ ] **Step 6: Verify `Check2open` remains independent**

Manual test:

1. Enable `Check2open`.
2. Enable the new option.
3. Run a safe back-check cycle.

Expected:

- Existing `UpCamBackCheck2()` behavior still runs only where it previously ran.
- New extra photo still does not parse area values or affect judgment.
- Logs distinguish the extra photo with `WsMod_Shp2附加拍照`.

- [ ] **Step 7: Final commit or report**

Run:

```powershell
git log --oneline -4
git status --short
```

Expected: Recent commits show parameter storage, UI, runtime capture, plus the design/plan commits. Report any remaining dirty files that were present before this work and were not touched.
