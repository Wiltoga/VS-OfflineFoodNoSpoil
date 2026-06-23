using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Vintagestory.API.Common;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil;

internal sealed class ModDataManager : IModDataManager, IDisposable
{
    private readonly ISaveGame saveGame;
    private readonly IModLogger logger;
    /// <summary>
    /// Key of the global mod data in the savefile
    /// </summary>
    internal const string StorageKey = "Wiltoga.OfflineFoodNoSpoil.PlayerData";

    /// <summary>
    /// Format of the key of one inventory slot in the global data
    /// </summary>
    /// <remarks>
    /// First parameter is the inventory id, second is the id of the slot in the inventory
    /// </remarks>
    private const string SlotKeyFormat = "{0}[{1}]";
    private Dictionary<string, Dictionary<string, ModData>>? globalModDataCache;
    private bool requiresSave = false;

    /// <summary>
    /// Cached data loaded from the savefile, to only load once per scope
    /// </summary>
    private Dictionary<string, Dictionary<string, ModData>> GlobalModData
    {
        get
        {
            if (globalModDataCache is null)
            {
                logger.Debug("No data loaded, fetching save file");
                var json = saveGame.GetData<string>(StorageKey);

                try
                {
                    globalModDataCache = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, ModData>>?>(json ?? "null");
                }
                catch( JsonException e)
                {
                    logger.Error("Failed to parse save game data");
                    logger.Error(e);
                }

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
        saveGame = Scope.Inject<ISaveGame>();
        logger = Scope.Inject<IModLogger>();
    }

    public Dictionary<string, ModData>? TryGetModData(ItemSlot slot, IEnumerable<ItemPerishEntry> entries)
    {
        var saveGameData = TryGetModDataFromSaveData(slot);
        
        if (saveGameData is not null)
        {
            return saveGameData;
        }

        var legacyData = TryGetModDataFromEntries(entries);

        return legacyData;
    }

    public void SaveModData(ItemSlot slot, Dictionary<string, ModData> data)
    {
        string uniqueId = string.Format(SlotKeyFormat, slot.Inventory.InventoryID, slot.Inventory.GetSlotId(slot));
        logger.Debug($"Saving data entry with id {uniqueId}");

        GlobalModData[uniqueId] = data;
        requiresSave = true;
    }

    private Dictionary<string, ModData>? TryGetModDataFromSaveData(ItemSlot slot)
    {
        string uniqueId = string.Format(SlotKeyFormat, slot.Inventory.InventoryID, slot.Inventory.GetSlotId(slot));
        logger.Debug($"Retrieving data entry with id {uniqueId}");

        GlobalModData.TryGetValue(uniqueId, out var data);
        GlobalModData.Remove(uniqueId);
        requiresSave = true;

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
                saveGame.StoreData(StorageKey, JsonSerializer.Serialize(globalModDataCache));
            }
            else
            {
                logger.Debug($"Cache not altered, no save required");
            }
        }
    }
}
