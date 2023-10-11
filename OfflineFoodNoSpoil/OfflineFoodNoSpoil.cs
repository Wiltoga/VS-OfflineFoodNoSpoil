using System;
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
        public static ICoreServerAPI Server { get; private set; }

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

        private void Event_PlayerDisconnect(IServerPlayer byPlayer)
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
                        var freshAttribute = (stack.Attributes["transitionstate"] as ITreeAttribute)?["freshHours"] as FloatArrayAttribute;
                        if (freshAttribute is null)
                            continue;
                        var currentFreshness = freshAttribute.value[0];
                        Server.WorldManager.SaveGame.StoreData($"{byPlayer.PlayerUID}.{slot.Inventory.InventoryID}.{slot.Inventory.GetSlotId(slot)}.{DataOldFoodSpoil}", currentFreshness);
                        freshAttribute.value[0] = float.MaxValue;
                        slot.MarkDirty();
                    }
                }
            }
        }

        private void Event_PlayerJoin(IServerPlayer byPlayer)
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
                        var freshAttribute = (stack.Attributes["transitionstate"] as ITreeAttribute)?["freshHours"] as FloatArrayAttribute;
                        var createdTotalHours = (stack.Attributes["transitionstate"] as ITreeAttribute)?["createdTotalHours"] as DoubleAttribute;
                        var lastUpdatedTotalHours = (stack.Attributes["transitionstate"] as ITreeAttribute)?["lastUpdatedTotalHours"] as DoubleAttribute;
                        if (freshAttribute is null || createdTotalHours is null || lastUpdatedTotalHours is null)
                            continue;
                        var hours = Server.World.Calendar.TotalHours;
                        var savedFreshness = Server.WorldManager.SaveGame.GetData($"{byPlayer.PlayerUID}.{slot.Inventory.InventoryID}.{slot.Inventory.GetSlotId(slot)}.{DataOldFoodSpoil}", float.MinValue);
                        if (savedFreshness >= 0)
                        {
                            createdTotalHours.value = lastUpdatedTotalHours.value = hours;
                            freshAttribute.value[0] = savedFreshness;
                            slot.MarkDirty();
                        }
                    }
                }
            }
        }
    }
}