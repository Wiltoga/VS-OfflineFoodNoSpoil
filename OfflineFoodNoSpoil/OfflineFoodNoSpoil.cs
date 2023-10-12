using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Schema;
using Vintagestory.API.Client;
using Vintagestory.API.Common;
using Vintagestory.API.Config;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace Wiltoga
{
    public class OfflineFoodNoSpoil : ModSystem
    {
        public static string DataOldFoodSpoil => "freshness";
        public static ICoreServerAPI? Server { get; private set; }

        public override bool ShouldLoad(EnumAppSide forSide)
        {
            return forSide == EnumAppSide.Server;
        }

        public override void StartServerSide(ICoreServerAPI api)
        {
            Server = api;
            Server.Event.PlayerDisconnect += Event_PlayerDisconnect;
            Server.Event.PlayerJoin += Event_PlayerJoin;
        }

        private static List<(FloatArrayAttribute Fresh, DoubleAttribute LastUpdateHours, string? Index)> ExtractFreshnessAttributes(ItemStack stack)
        {
            var attributes = new List<(FloatArrayAttribute, DoubleAttribute, string?)>();
            try
            {
                var freshAttribute = (stack.Attributes["transitionstate"] as ITreeAttribute)?["freshHours"] as FloatArrayAttribute;
                var lastUpdatedTotalHours = (stack.Attributes["transitionstate"] as ITreeAttribute)?["lastUpdatedTotalHours"] as DoubleAttribute;
                if (freshAttribute is not null && lastUpdatedTotalHours is not null)
                    attributes.Add((freshAttribute, lastUpdatedTotalHours, null));
            }
            catch (Exception e)
            {
                Server?.Logger.Error(e);
            }

            var content = stack.Attributes["contents"] as ITreeAttribute;
            if (content is not null)
            {
                foreach (var set in content)
                {
                    try
                    {
                        var contentStack = set.Value as ItemstackAttribute;
                        var freshAttribute = (contentStack?.value.Attributes["transitionstate"] as ITreeAttribute)?["freshHours"] as FloatArrayAttribute;
                        var lastUpdatedTotalHours = (contentStack?.value.Attributes["transitionstate"] as ITreeAttribute)?["lastUpdatedTotalHours"] as DoubleAttribute;
                        if (freshAttribute is not null && lastUpdatedTotalHours is not null)
                            attributes.Add((freshAttribute, lastUpdatedTotalHours, set.Key));
                    }
                    catch (Exception e)
                    {
                        Server?.Logger.Error(e);
                    }
                }
            }

            return attributes;
        }

        private void Event_PlayerDisconnect(IServerPlayer byPlayer)
        {
            try
            {
                if (Server is null)
                    return;
                foreach (var inventory in byPlayer.InventoryManager.Inventories.Values)
                {
                    if (inventory is null)
                        continue;
                    if (inventory.ClassName != "hotbar" && inventory.ClassName != "backpack")
                        continue;
                    foreach (var slot in inventory)
                    {
                        if (slot.Itemstack is not null)
                        {
                            var stack = slot.Itemstack;
                            var attributes = ExtractFreshnessAttributes(stack);
                            foreach (var attributeSet in attributes)
                            {
                                var suffix = attributeSet.Index is not null ? $".{attributeSet.Index}" : "";
                                var currentFreshness = attributeSet.Fresh.value;
                                Server.WorldManager.SaveGame.StoreData($"{byPlayer.PlayerUID}.{slot.Inventory.InventoryID}.{slot.Inventory.GetSlotId(slot)}{suffix}.{DataOldFoodSpoil}", currentFreshness);
                            }
                            slot.MarkDirty();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Server?.Logger.Error(e);
            }
        }

        private void Event_PlayerJoin(IServerPlayer byPlayer)
        {
            if (Server is null)
                return;
            try
            {
                foreach (var inventory in byPlayer.InventoryManager.Inventories.Values)
                {
                    if (inventory is null)
                        continue;
                    if (inventory.ClassName != "hotbar" && inventory.ClassName != "backpack")
                        continue;
                    foreach (var slot in inventory)
                    {
                        if (slot.Itemstack is not null)
                        {
                            var stack = slot.Itemstack;
                            var attributes = ExtractFreshnessAttributes(stack);
                            var hours = Server.World.Calendar.TotalHours;
                            foreach (var attributeSet in attributes)
                            {
                                var suffix = attributeSet.Index is not null ? $".{attributeSet.Index}" : "";
                                var savedFreshness = Server.WorldManager.SaveGame.GetData<float[]?>($"{byPlayer.PlayerUID}.{slot.Inventory.InventoryID}.{slot.Inventory.GetSlotId(slot)}{suffix}.{DataOldFoodSpoil}", null);
                                if (savedFreshness is not null)
                                {
                                    for (int i = 0; i < savedFreshness.Length; i++)
                                        savedFreshness[i] += (float)(hours - attributeSet.LastUpdateHours.value);
                                    attributeSet.Fresh.value = savedFreshness;
                                }
                            }
                            slot.MarkDirty();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Server?.Logger.Error(e);
            }
        }
    }
}