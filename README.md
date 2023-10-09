# VS-OfflineFoodNoSpoil
Mod for the Vintage Story game

Prevent, in a multiplayer game, the food spoil inside an offline player inventory.

How it works : when a player disconnects, it saves the current time left of the freshness of every spoilable item in the player inventory, and crank it up to the max possible float value. On reconnect, it forces the time left back to its original value.
