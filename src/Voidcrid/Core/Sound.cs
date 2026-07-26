using System;
using RoR2;
using UnityEngine;

namespace Voidcrid
{
    /// <summary>
    /// Soundbank loading and playback for the vanilla Wwise events Voidcrid borrows.
    ///
    /// Voidcrid plays sounds it does not own: the skill states post VoidSurvivor events by name,
    /// and several borrowed effect prefabs carry their own EffectComponent.soundName that RoR2's
    /// EffectManager emits on spawn. Those events only resolve if the bank holding them is
    /// resident, and RoR2 loads character banks per run, based on who is actually present. Playing
    /// Acrid without a VoidSurvivor in the run therefore leaves every borrowed event unresolvable.
    /// </summary>
    internal static class Sound
    {
        // Banks holding events Voidcrid borrows but does not own.
        //
        // char_VoidSurvivor  — every event the skill states post by name (LaserBeam, Entropy,
        //                      VoidEscape).
        // char_captain       — Play_captain_drone_zap, baked into VoidJailerCaptureTracer.prefab,
        //                      which NullBeam uses as its BulletAttack tracer.
        //
        // Deliberately absent: Play_item_void_chainLightning (VoidSurvivorBeamImpactCorrupt) and
        // Play_voidJailer_death_vortex_explode (VoidJailerDeathBombExplosion) both live in
        // Global.bnk, which is always resident. Verified against the generated .txt bank
        // manifests in StreamingAssets/Audio/GeneratedSoundBanks/Windows.
        private static readonly string[] borrowedSoundbanks =
        {
            "char_VoidSurvivor",
            "char_captain"
        };

        /// <summary>
        /// Queues the borrowed banks for loading once the Wwise engine exists.
        ///
        /// The deferral is not optional. AkBankManager caches a handle per bank name and only
        /// calls DoLoadBank when that handle's refcount is still zero, but it increments the
        /// refcount unconditionally. Requesting a bank before AkSoundEngine is initialized
        /// therefore fails, logs nothing (AkBankManager suppresses the warning when the engine is
        /// down), and leaves a cached handle at refcount 1 that no later request will ever retry —
        /// permanently poisoning that bank name for the session. BepInEx Awake runs roughly a
        /// hundred log lines before "Sound engine initialized successfully", so loading from there
        /// does exactly that. RoR2Application.onLoad runs well after engine init.
        /// </summary>
        internal static void RequestBorrowedSoundbanks()
        {
            RoR2Application.onLoad += LoadBorrowedSoundbanks;
        }

        private static void LoadBorrowedSoundbanks()
        {
            foreach (string bank in borrowedSoundbanks)
            {
                AkBankManager.LoadBankAsync(bank, OnSoundbankLoaded);
            }
        }

        private static void OnSoundbankLoaded(uint bankId, IntPtr inMemoryBankPtr, AKRESULT loadResult, object cookie)
        {
            if (loadResult == AKRESULT.AK_Success)
            {
                Log.LogInfo($"Loaded borrowed soundbank (id {bankId}).");
            }
            else
            {
                Log.LogError($"Borrowed soundbank failed to load: {loadResult}. Sounds from it will not play.");
            }
        }

        /// <summary>
        /// Wrapper around <see cref="Util.PlaySound"/> that reports events Wwise refused to post.
        /// PostEvent returns AK_INVALID_PLAYING_ID (0) when the sound engine has never heard of the
        /// event, which in practice means the soundbank holding it was not loaded. Nothing anywhere
        /// logs that — the failure is silent in the most literal sense — so it is worth a line.
        /// A non-zero id means the event posted and any silence is downstream (routing, attenuation,
        /// a Wwise state gate), not a missing bank.
        /// </summary>
        internal static uint Play(string soundString, GameObject gameObject)
        {
            uint playingId = Util.PlaySound(soundString, gameObject);

            if (playingId == 0)
            {
                Log.LogWarning($"Wwise would not post '{soundString}' (invalid playing id) — its soundbank is most likely not loaded.");
            }

            return playingId;
        }
    }
}
