using Microsoft.Xna.Framework;
using Monocle;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;


namespace Celeste.Mod.Jumpscare;

public class JumpscareModule : EverestModule {

    public static bool renderJumpscare = false;
    public static MTexture jumpscareImage;
    public static int timer = 0;
    
    public static JumpscareModule Instance;
    
    public JumpscareModule() {
        Instance = this;
    }


    public override Type SettingsType => typeof(JumpscareModuleSettings);
    public static JumpscareModuleSettings Settings => (JumpscareModuleSettings) Instance._Settings;

    public override Type SessionType => typeof(JumpscareModuleSession);
    public static JumpscareModuleSession Session => (JumpscareModuleSession) Instance._Session;

    public override Type SaveDataType => typeof(JumpscareModuleSaveData);
    public static JumpscareModuleSaveData SaveData => (JumpscareModuleSaveData) Instance._SaveData;
    
    public override void Load() {
        On.Celeste.Level.LoadLevel += LoadImages;
        On.Celeste.Player.Die += sillyfunction;
        On.Celeste.Level.Render += sillyfunction2;
    }

    private void sillyfunction2(On.Celeste.Level.orig_Render orig, Level self)
    {
        orig(self);
        if (renderJumpscare) {
            if (timer == 0) {
                renderJumpscare = false;
            }

            Draw.SpriteBatch.Begin(SpriteSortMode.Deferred,BlendState.AlphaBlend,SamplerState.PointWrap,DepthStencilState.None,RasterizerState.CullNone,null,Engine.ScreenMatrix);
            Draw.Rect(0f, 0f, 1920f, 1080f, Color.Black);

            if (timer > 20) {
                float scale = 1f / (timer - 20);
                jumpscareImage.Draw(
                    new Vector2(960f - (960f * scale), 540f - (540f * scale)), 
                    new Vector2(0, 0), Color.White, 
                    new Vector2(1f/(timer-20), 1f/(timer-20)));
            } else if ((timer / 2 % 2 != 0) || Settings.PhotosensitiveMode) {
                jumpscareImage.Draw(new Vector2(0, 0));
            }
            Draw.SpriteBatch.End();

            timer--;
        }
    }

    private PlayerDeadBody sillyfunction(On.Celeste.Player.orig_Die orig, Player self, Vector2 direction, bool evenIfInvincible, bool registerDeathInStats)
    {
        Jumpscare(self);
        return orig(self, direction, evenIfInvincible, registerDeathInStats);
    }

    public override void Initialize() {
    }

    public override void LoadContent(bool firstLoad) {
    }

    public override void Unload() {
        On.Celeste.Level.LoadLevel -= LoadImages;
        On.Celeste.Player.Die -= sillyfunction;
        On.Celeste.Level.Render -= sillyfunction2;
    }

    private void LoadImages(On.Celeste.Level.orig_LoadLevel orig, Level self, Player.IntroTypes playerIntro, bool isFromLoader) {
        orig(self, playerIntro, isFromLoader);
        jumpscareImage = GFX.Game["dumb/jumpscare"];
    }

    

    private static void Jumpscare(Player self) {
        bool isCarryingGolden = self.Leader.Followers.Any(follower => follower.Entity is Strawberry { Golden: true, Winged: false });
        if (isCarryingGolden && !Settings.AllowWithGoldenBerry)
            { return; }

        if (isCarryingGolden ? Calc.Random.Chance(1f / Settings.JumpscareOddsWithGoldenBerry) 
            : Calc.Random.Chance(1f / Settings.JumpscareOdds)
        ) {
            self.Play("event:/CodeNat/jumpscareAudio");
            timer = 30;
            renderJumpscare = true;
        }
    }
}