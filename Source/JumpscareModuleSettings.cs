namespace Celeste.Mod.Jumpscare;

public class JumpscareModuleSettings : EverestModuleSettings
{
    public bool PhotosensitiveMode {get; set; } = false;
    public bool AllowWithGoldenBerry { get; set; } = true;

    [SettingNumberInput]
    public int JumpscareOdds { get; set; } = 1000;

    [SettingNumberInput]
    public int JumpscareOddsWithGoldenBerry { get; set; } = 10;
}