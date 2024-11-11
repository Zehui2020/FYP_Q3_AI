using UnityEngine.Audio;
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

[System.Serializable]
public class Sound
{
    public enum SoundName
    {
        Walk1,
        Walk2, 
        Walk3, 
        Walk4,

        Axe1,
        Axe2,
        Axe3,

        Sword1,
        Sword2,
        Sword3,
        Sword4,

        Dagger1,
        Dagger2,
        Dagger3,
        Dagger4,

        Dash,
        Land,
        Jump,
        DoubleJump,

        ParryHit,
        Parry,

        Roll,
        LedgeGrab,

        PlungeAir,
        PlungeLand,

        Hurt,
        Die,

        SlimeAttackStart,
        SlimeAttackEnd,
        SlimeDie,
        SlimeHurt,
        SlimeJump,
        SlimeLand,
        SlimeTeleport,

        SkeletonWalk,
        SkeletonHurt,
        SkeletonDie,
        SkeletonLungeStart,
        SkeletonLungeEnd,
        SkeletonAttack1,
        SkeletonAttack2,

        ScorpionAttack,
        ScorpionDie,
        ScorpionHurt,
        ScorpionWalk,

        UndeadLoop,
        UndeadAttack,

        BloodArts,
        DivineBlessing,
        FreezingOrb,
        ProjectileThrow,
        HeatWave,
        MolotovCocktail,
        PoisonKnives,
        RabidExecution,
        Ravage,
        Shatter,
        Shred,
        StoneSkin,
        StoneSkinEnd,
        HealthPotion,
        ProtectionShieldActivate,
        ProtectionShieldActive,
        ProtectionShieldDeactivate,
        AbilityCooldownReset,

        Burn,
        Bleed,
        Poison,
        Static,
        Stunned,
        Frozen,
        Dazed,

        SettingsOpenClick,
        AbilityClick,
        DialogHover,
        DialogClick,
        AbilitySelect,

        ChestOpen,
        Error,
        PickUp,
        HeartBeat,
        Haste,

        Map,
        TeleportStart,
        TeleportEnd,
        PortalActivate,
        PortalIdle,
        MapClose,

        BottleOSurprise,
        DynaMight,
        FrazzledWire,
        NRGBar,

        Heal,
        DoorIdle,
        DoorChime,
        Dialog,

        BossShockwave,

        BossP1Summon,
        BossP1Knife,
        BossP1PunchCharge,
        BossP1PunchRelease,
        BossP1Jump,
        BossP1Land,

        MainMenuHover,
        MainMenuClick,
        SettingsHover,
        SettingsClick,
        SettingsSlide,

    }
    public SoundName name;

    public AudioClip clip;

    public AudioMixerGroup mixerGroup;

    [Range(0.1f, 3f)]
    public float pitch = 1;

    [Range(0f, 10f)]
    public float volume = 1;

    [HideInInspector]
    public AudioSource source;

    public bool loop;
    public bool createSource = true;
    public bool playOnAwake = false;
    public bool doNotFade = false;

    public Coroutine fadeRoutine;

    public IEnumerator FadeSoundRoutine(bool fadeIn, float duration, float targetVolume)
    {
        if (source == null)
        {
            fadeRoutine = null;
            yield break;
        }

        float time = 0f;
        float startVolume = source.volume;

        while (time < duration)
        {
            time += Time.unscaledDeltaTime;
            if (!fadeIn)
                source.volume = Mathf.Lerp(startVolume, targetVolume, time / duration);
            else
                source.volume = Mathf.Lerp(0, targetVolume, time / duration);
            yield return null;
        }

        if (!fadeIn)
        {
            source.Stop();
            source.volume = 0;
        }
        else
        {
            source.Play();
            source.volume = targetVolume;
        }

        fadeRoutine = null;

        yield break;
    }
}