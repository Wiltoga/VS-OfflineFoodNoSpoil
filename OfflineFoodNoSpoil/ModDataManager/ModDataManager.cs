using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

internal class ModDataManager : IModDataManager
{
    private readonly ICoreServerAPI server;
    private readonly IItemPerishService itemPerishService;

    public ModDataManager()
    {
        server = Scope.Inject<ICoreServerAPI>();
        itemPerishService = Scope.Inject<IItemPerishService>();
    }

    public Dictionary<string, ModData>? TryGetModData(IPlayer player, IInventory inventory, ItemSlot slot, ItemPerishMapping[] mappings)
    {
        var saveGameData = TryGetModDataFromSaveData(player, inventory, slot);
        
        if (saveGameData is not null)
        {
            return saveGameData;
        }

        var legacyData = TryGetModDataFromMappings(mappings);

        return legacyData;
    }

    public void SaveModData(IPlayer player, IInventory inventory, ItemSlot slot, Dictionary<string, ModData> data)
    {
        string uniqueId = $"{player.PlayerUID}:{inventory.InventoryID}:{inventory.GetSlotId(slot)}";

        server.WorldManager.SaveGame.StoreData(uniqueId, data);
    }

    private Dictionary<string, ModData>? TryGetModDataFromSaveData(IPlayer player, IInventory inventory, ItemSlot slot)
    {
        string uniqueId = $"{player.PlayerUID}:{inventory.InventoryID}:{inventory.GetSlotId(slot)}";

        return server.WorldManager.SaveGame.GetData<Dictionary<string, ModData>>(uniqueId);
    }

    /// <summary>
    /// Method used to fetch old data that was saved in the item attributes
    /// </summary>
    /// <param name="mappings"></param>
    /// <returns></returns>
    private Dictionary<string, ModData>? TryGetModDataFromMappings(ItemPerishMapping[] mappings)
    {
        var result = mappings.Where(mapping => mapping.OldModData.DisconnectTotalHours.HasValue && mapping.OldModData.DisconnectFreshHours.HasValue).ToDictionary(
            mapping => mapping.Key,
            mapping => new ModData
            {
                DisconnectTotalHours = (float)mapping.OldModData.DisconnectTotalHours!,
                DisconnectFreshHours = (float)mapping.OldModData.DisconnectFreshHours!,
            });

        // cleanup of legacy data
        foreach (var oldData in mappings.Select(mapping => mapping.OldModData).Where(oldData => oldData.Exists))
        {
            oldData.DeleteData();
        }

        if (result.Keys.Count > 0)
        {
            return result;
        }
        else
        {
            return null;
        }
    }
}
