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
            Server.Event.PlayerJoin += Event_PlayerJoin;
        }

        private static List<(FloatArrayAttribute Fresh, DoubleAttribute LastUpdateHours)> ExtractFreshnessAttributes(ItemStack stack)
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
                Server?.Logger.Error(e);
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
                        Server?.Logger.Error(e);
                    }
                }
            }

            return attributes;
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
                                for (int i = 0; i < attributeSet.Fresh.value.Length; i++)
                                    attributeSet.Fresh.value[i] += (float)(hours - attributeSet.LastUpdateHours.value);
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