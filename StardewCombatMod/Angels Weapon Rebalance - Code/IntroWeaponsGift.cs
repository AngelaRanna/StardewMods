using System;
using Harmony;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace StardewCombatMod
{
    public class IntroWeaponsGift : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += this.dayStarted;
            //helper.Events.Player.Warped += this.adventureGuildEntry;

            // Make sure to get the monitor set up for debugging prints
            CheckEventPostfixPatch.Initialize(this.Monitor);

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkForEvents)),
                postfix: new HarmonyMethod(typeof(CheckEventPostfixPatch), nameof(CheckEventPostfixPatch.checkForEvents_postfix))
                );
        }

        private void dayStarted(object sender, DayStartedEventArgs e)
        {
            // If we haven't received the mail already, and the player has been to the mines
            if (!(Game1.player.mailReceived.Contains("WeaponRebalanceIntro")) && Game1.player.deepestMineLevel > 1)
            {
                // You've got mail!
                Game1.mailbox.Add("WeaponRebalanceIntro");
            }
        }

        // DEPRECATED -- using the Harmony postfix patcher instead. Just keeping this around in case things break in 1.5 or we get a better way to play asset load chicken.
        //private void adventureGuildEntry(object sender, WarpedEventArgs e)
        //{
        //    // If the player entered the guild, they've seen the mail, this event hasn't played, and the player has enough free inventory to receive the free lewt
        //    if (e.NewLocation.name == "AdventureGuild" && Game1.player.mailReceived.Contains("WeaponRebalanceIntro") && !Game1.player.eventsSeen.Contains(68940000) 
        //        && Game1.player.freeSpotsInInventory() >= 2 && !Game1.eventUp && e.NewLocation.currentEvent == null)
        //    {
        //        // Play the Gil event
        //        e.NewLocation.startEvent(new StardewValley.Event(Game1.content.LoadString("Data\\Events\\WeaponRebalanceEvents:WeaponRebalanceIntroWeaponsGift"), 68940000, Game1.player));

        //        // Give the player a carving knife and a femur
        //        Game1.player.addItemToInventory(new StardewValley.Tools.MeleeWeapon(16));
        //        Game1.player.addItemToInventory(new StardewValley.Tools.MeleeWeapon(31));
        //    }
        //}
    }

    public class CheckEventPostfixPatch
    {
        private static IMonitor Monitor;

        public static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        public static void checkForEvents_postfix(GameLocation __instance)
        {
            try
            {
                // If the player entered the guild, they've seen the mail, this event hasn't played, and the player has enough free inventory to receive the free lewt
                if (__instance.name == "AdventureGuild" && Game1.player.mailReceived.Contains("WeaponRebalanceIntro") && !Game1.player.eventsSeen.Contains(68940000)
                    && Game1.player.freeSpotsInInventory() >= 2 && !Game1.eventUp && __instance.currentEvent == null)
                {
                    // Play the Gil event
                    __instance.startEvent(new StardewValley.Event(Game1.content.LoadString("Data\\Events\\WeaponRebalanceEvents:WeaponRebalanceIntroWeaponsGift"), 68940000, Game1.player));

                    // Give the player a carving knife and a femur
                    Game1.player.addItemToInventory(new StardewValley.Tools.MeleeWeapon(16));
                    Game1.player.addItemToInventory(new StardewValley.Tools.MeleeWeapon(31));
                }
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(checkForEvents_postfix)}:\n{ex}", LogLevel.Error);
            }
        }
    }
}