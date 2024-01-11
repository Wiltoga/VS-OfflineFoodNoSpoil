using OfflineFoodNoSpoil;
using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace Wiltoga
{
    public class OfflineFoodNoSpoil : ModSystem
    {
        public static string DataOldFoodSpoil => "freshness";
        public static ICoreServerAPI Server { get; private set; } = default!;
        private string SettingsFile => $"{Mod.Info.ModID}.json";

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Server;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            Server = api;
            // loading to trigger the file creation if it doesn't exist yet
            LoadSettings();
            Server.Event.PlayerJoin += Event_PlayerJoin;
        }

        private static List<(FloatArrayAttribute Fresh, DoubleAttribute LastUpdateHours)> ExtractFreshnessAttributes(ItemStack stack, Settings settings)
        {
            var attributes = new List<(FloatArrayAttribute, DoubleAttribute)>();
            try
            {
                var freshAttribute = (stack.Attributes["transitionstate"] as ITreeAttribute)?["freshHours"] as FloatArrayAttribute;
                var lastUpdatedTotalHours = (stack.Attributes["transitionstate"] as ITreeAttribute)?["lastUpdatedTotalHours"] as DoubleAttribute;
                if (freshAttribute is not null && lastUpdatedTotalHours is not null)
                    attributes.Add((freshAttribute, lastUpdatedTotalHours));
            }
            catch (Exception e)
            {
                if (settings.UseLogs)
                    Server.Logger.Error(e);
            }

            var content = stack.Attributes["contents"] as ITreeAttribute;
            if (content is not null)
            {
                foreach (var set in content.Values)
                {
                    try
                    {
                        var contentStack = set as ItemstackAttribute;
                        var freshAttribute = (contentStack?.value.Attributes["transitionstate"] as ITreeAttribute)?["freshHours"] as FloatArrayAttribute;
                        var lastUpdatedTotalHours = (contentStack?.value.Attributes["transitionstate"] as ITreeAttribute)?["lastUpdatedTotalHours"] as DoubleAttribute;
                        if (freshAttribute is not null && lastUpdatedTotalHours is not null)
                            attributes.Add((freshAttribute, lastUpdatedTotalHours));
                    }
                    catch (Exception e)
                    {
                        if (settings.UseLogs)
                            Server.Logger.Error(e);
                    }
                }
            }

            return attributes;
        }

        private void Event_PlayerJoin(IServerPlayer byPlayer)
        {
            var settings = LoadSettings();
            try
            {
                if (settings.UseLogs)
                    Server.Logger.Debug($"Player {byPlayer.PlayerName} joined");
                foreach (var inventory in byPlayer.InventoryManager.Inventories.Values)
                {
                    if (inventory is null)
                        continue;
                    if (settings.UseLogs)
                        Server.Logger.Debug($"Scanning inventory {inventory.ClassName} {inventory.InventoryID}");
                    if (inventory.ClassName != "hotbar" && inventory.ClassName != "backpack")
                        continue;
                    foreach (var slot in inventory)
                    {
                        if (slot.Itemstack is not null)
                        {
                            var stack = slot.Itemstack;
                            if (settings.UseLogs)
                                Server.Logger.Debug($"Scanning slot {inventory.GetSlotId(slot)} {stack.GetName()}");
                            var attributes = ExtractFreshnessAttributes(stack, settings);
                            if (settings.UseLogs)
                                Server.Logger.Debug($"Found {attributes.Count} freshness attributes");
                            foreach (var attributeSet in attributes)
                            {
                                var skip = (float)(Server.World.Calendar.TotalHours - attributeSet.LastUpdateHours.value);
                                skip *= 1 - settings.FoodSpoilMultiplier;
                                if (settings.UseLogs)
                                    Server.Logger.Debug("Skip time : " + skip);
                                for (int i = 0; i < attributeSet.Fresh.value.Length; i++)
                                {
                                    if (settings.UseLogs)
                                        Server.Logger.Debug("attribute before : " + attributeSet.Fresh.value[i]);
                                    attributeSet.Fresh.value[i] += skip;
                                    if (settings.UseLogs)
                                        Server.Logger.Debug("attribute after : " + attributeSet.Fresh.value[i]);
                                }
                            }
                            if (attributes.Any())
                                slot.MarkDirty();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                if (settings.UseLogs)
                    Server.Logger.Error(e);
            }
        }

        private Settings LoadSettings()
        {
            try
            {
                var settings = Server.LoadModConfig<Settings>(SettingsFile);
                if (settings is null)
                {
                    Server.StoreModConfig(Settings.Default, SettingsFile);
                    return Settings.Default;
                }
                if (settings.FoodSpoilMultiplier < 0 || settings.FoodSpoilMultiplier > 1)
                {
                    settings.FoodSpoilMultiplier = Math.Clamp(settings.FoodSpoilMultiplier, 0, 1);
                    Server.StoreModConfig(settings, SettingsFile);
                }
                return settings;
            }
            catch
            {
                Server.StoreModConfig(Settings.Default, SettingsFile);
                return Settings.Default;
            }
        }
    }
}