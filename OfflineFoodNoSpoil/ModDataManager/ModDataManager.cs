using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

internal sealed class ModDataManager : IModDataManager, IDisposable
{
    private readonly ICoreServerAPI server;
    private readonly IModLogger logger;
    private const string StorageKey = "Wiltoga.OfflineFoodNoSpoil.PlayerData";
    private Dictionary<string, Dictionary<string, ModData>>? globalModDataCache;
    private bool requiresSave = false;

    private Dictionary<string, Dictionary<string, ModData>> GlobalModData
    {
        get
        {
            if (globalModDataCache is null)
            {
                logger.Debug("No data loaded, fetching save file");
                var json = server.WorldManager.SaveGame.GetData<string>(StorageKey);
                globalModDataCache = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, ModData>>?>(json ?? "null");
                if (globalModDataCache is null)
                {
                    logger.Debug("No data found");
                    globalModDataCache = [];
                }
                else
                {
                    logger.Debug($"{globalModDataCache.Keys.Count} entries loaded");
                }
            }
            else
            {
                logger.Debug("Loading data from cache");
            }
            return globalModDataCache;
        }
    }

    public ModDataManager()
    {
        server = Scope.Inject<ICoreServerAPI>();
        logger = Scope.Inject<IModLogger>();
    }

    public Dictionary<string, ModData>? TryGetModData(IInventory inventory, ItemSlot slot, IEnumerable<ItemPerishEntry> entries)
    {
        var saveGameData = TryGetModDataFromSaveData(inventory, slot);
        
        if (saveGameData is not null)
        {
            return saveGameData;
        }

        var legacyData = TryGetModDataFromEntries(entries);

        return legacyData;
    }

    public void SaveModData(IInventory inventory, ItemSlot slot, Dictionary<string, ModData> data)
    {
        string uniqueId = $"{inventory.InventoryID}[{inventory.GetSlotId(slot)}]";
        logger.Debug($"Saving data entry with id {uniqueId}");

        GlobalModData[uniqueId] = data;
        requiresSave = true;
    }

    private Dictionary<string, ModData>? TryGetModDataFromSaveData(IInventory inventory, ItemSlot slot)
    {
        string uniqueId = $"{inventory.InventoryID}[{inventory.GetSlotId(slot)}]";
        logger.Debug($"Retrieving data entry with id {uniqueId}");

        GlobalModData.TryGetValue(uniqueId, out var data);
        GlobalModData.Remove(uniqueId);

        return data;
    }

    /// <summary>
    /// Method used to fetch old data that was saved in the item attributes
    /// </summary>
    /// <param name="entrys"></param>
    /// <returns></returns>
    private Dictionary<string, ModData>? TryGetModDataFromEntries(IEnumerable<ItemPerishEntry> entrys)
    {
        var result = entrys.Where(entry => entry.OldModData.DisconnectTotalHours.HasValue).ToDictionary(
            entry => entry.Key,
            entry => new ModData
            {
                DisconnectTotalHours = (float)entry.OldModData.DisconnectTotalHours!,
            });

        // cleanup of legacy data
        foreach (var oldData in entrys.Select(entry => entry.OldModData).Where(oldData => oldData.Exists))
        {
            oldData.DeleteData();
        }

        if (result.Keys.Count > 0)
        {
            logger.Debug($"Legacy data found in attributes");
            return result;
        }
        else
        {
            logger.Debug($"No legacy data found in attributes");
            return null;
        }
    }

    public void Dispose()
    {
        if (globalModDataCache is not null)
        {
            if (requiresSave)
            {
                logger.Debug($"Saving cache to save file");
                server.WorldManager.SaveGame.StoreData(StorageKey, JsonSerializer.Serialize(globalModDataCache));
            }
            else
            {
                logger.Debug($"Cache not altered, no save required");
            }
        }
    }
}
