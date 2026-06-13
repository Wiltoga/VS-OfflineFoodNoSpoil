#if DEBUG
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Vintagestory.API.Common;
using Vintagestory.API.Datastructures;
using Vintagestory.API.Server;

namespace Wiltoga.OfflineFoodNoSpoil
{
    internal static class GetFoodCommand
    {
        private const string redmeat64 = "AQAAAIoDAAA0AAAABg90cmFuc2l0aW9uc3RhdGUDEWNyZWF0ZWRUb3RhbEhvdXJzNYIrZF9Ei0ADFWxhc3RVcGRhdGVkVG90YWxIb3Vyc0a28/2UHY1ADApmcmVzaEhvdXJzAQAAAN+HrkIMD3RyYW5zaXRpb25Ib3VycwEAAADfh5ZCDBF0cmFuc2l0aW9uZWRIb3VycwEAAABWnjBBAAA=";
        private const string claypot64 = "AAAAAFIDAAABAAAABgt0ZW1wZXJhdHVyZQMVdGVtcGVyYXR1cmVMYXN0VXBkYXRlyJi7llBgi0AEC3RlbXBlcmF0dXJlAABIQwAGCGNvbnRlbnRzBwEwAAEAAACKAwAAAQAAAAYPdHJhbnNpdGlvbnN0YXRlAxFjcmVhdGVkVG90YWxIb3Vyc8iYu5ZQXItAAxVsYXN0VXBkYXRlZFRvdGFsSG91cnNGtvP9lB2NQAwKZnJlc2hIb3VycwEAAADn/xJDDA90cmFuc2l0aW9uSG91cnMBAAAAm/8bQgwRdHJhbnNpdGlvbmVkSG91cnMBAAAAoQv4QAAGC3RlbXBlcmF0dXJlAxV0ZW1wZXJhdHVyZUxhc3RVcGRhdGUmCyNuTGyLQAQLdGVtcGVyYXR1cmUAAAAAAAAHATEAAQAAAIoDAAABAAAABg90cmFuc2l0aW9uc3RhdGUDEWNyZWF0ZWRUb3RhbEhvdXJzyJi7llBci0ADFWxhc3RVcGRhdGVkVG90YWxIb3Vyc0a28/2UHY1ADApmcmVzaEhvdXJzAQAAAOf/EkMMD3RyYW5zaXRpb25Ib3VycwEAAACb/xtCDBF0cmFuc2l0aW9uZWRIb3VycwEAAAChC/hAAAYLdGVtcGVyYXR1cmUDFXRlbXBlcmF0dXJlTGFzdFVwZGF0ZSYLI25MbItABAt0ZW1wZXJhdHVyZQAAAAAAAAcBMgABAAAAigMAAAEAAAAGD3RyYW5zaXRpb25zdGF0ZQMRY3JlYXRlZFRvdGFsSG91cnPImLuWUFyLQAMVbGFzdFVwZGF0ZWRUb3RhbEhvdXJzRrbz/ZQdjUAMCmZyZXNoSG91cnMBAAAA5/8SQwwPdHJhbnNpdGlvbkhvdXJzAQAAAJv/G0IMEXRyYW5zaXRpb25lZEhvdXJzAQAAAKEL+EAABgt0ZW1wZXJhdHVyZQMVdGVtcGVyYXR1cmVMYXN0VXBkYXRlJgsjbkxsi0AEC3RlbXBlcmF0dXJlAAAAAAAABwEzAAEAAACKAwAAAQAAAAYPdHJhbnNpdGlvbnN0YXRlAxFjcmVhdGVkVG90YWxIb3Vyc8iYu5ZQXItAAxVsYXN0VXBkYXRlZFRvdGFsSG91cnNGtvP9lB2NQAwKZnJlc2hIb3VycwEAAADn/xJDDA90cmFuc2l0aW9uSG91cnMBAAAAm/8bQgwRdHJhbnNpdGlvbmVkSG91cnMBAAAAoQv4QAAGC3RlbXBlcmF0dXJlAxV0ZW1wZXJhdHVyZUxhc3RVcGRhdGUmCyNuTGyLQAQLdGVtcGVyYXR1cmUAAAAAAAAABBBxdWFudGl0eVNlcnZpbmdzAACAPwUKcmVjaXBlQ29kZQltZWF0eXN0ZXcA";
        private const string pie64 = "AAAAAJwIAAABAAAABghjb250ZW50cwcBMAABAAAAwQIAAAIAAAAGD3RyYW5zaXRpb25zdGF0ZQMRY3JlYXRlZFRvdGFsSG91cnNGtvP9lB2NQAMVbGFzdFVwZGF0ZWRUb3RhbEhvdXJzRrbz/ZQdjUAMCmZyZXNoSG91cnMBAAAAAABAQgwPdHJhbnNpdGlvbkhvdXJzAQAAAAAAwEEMEXRyYW5zaXRpb25lZEhvdXJzAQAAAAAAAAAAAAcBMQABAAAAigMAAAIAAAAGD3RyYW5zaXRpb25zdGF0ZQMRY3JlYXRlZFRvdGFsSG91cnNGtvP9lB2NQAMVbGFzdFVwZGF0ZWRUb3RhbEhvdXJzRrbz/ZQdjUAMCmZyZXNoSG91cnMBAAAAA4iuQgwPdHJhbnNpdGlvbkhvdXJzAQAAAAOIlkIMEXRyYW5zaXRpb25lZEhvdXJzAQAAAAAAAAAAAAcBMgABAAAAigMAAAIAAAAGD3RyYW5zaXRpb25zdGF0ZQMRY3JlYXRlZFRvdGFsSG91cnNGtvP9lB2NQAMVbGFzdFVwZGF0ZWRUb3RhbEhvdXJzRrbz/ZQdjUAMCmZyZXNoSG91cnMBAAAAA4iuQgwPdHJhbnNpdGlvbkhvdXJzAQAAAAOIlkIMEXRyYW5zaXRpb25lZEhvdXJzAQAAAAAAAAAAAAcBMwABAAAAigMAAAIAAAAGD3RyYW5zaXRpb25zdGF0ZQMRY3JlYXRlZFRvdGFsSG91cnNGtvP9lB2NQAMVbGFzdFVwZGF0ZWRUb3RhbEhvdXJzRrbz/ZQdjUAMCmZyZXNoSG91cnMBAAAAA4iuQgwPdHJhbnNpdGlvbkhvdXJzAQAAAAOIlkIMEXRyYW5zaXRpb25lZEhvdXJzAQAAAAAAAAAAAAcBNAABAAAAigMAAAIAAAAGD3RyYW5zaXRpb25zdGF0ZQMRY3JlYXRlZFRvdGFsSG91cnNGtvP9lB2NQAMVbGFzdFVwZGF0ZWRUb3RhbEhvdXJzRrbz/ZQdjUAMCmZyZXNoSG91cnMBAAAAA4iuQgwPdHJhbnNpdGlvbkhvdXJzAQAAAAOIlkIMEXRyYW5zaXRpb25lZEhvdXJzAQAAAAAAAAAAAAcBNQABAAAAwQIAAAIAAAAGD3RyYW5zaXRpb25zdGF0ZQMRY3JlYXRlZFRvdGFsSG91cnNGtvP9lB2NQAMVbGFzdFVwZGF0ZWRUb3RhbEhvdXJzRrbz/ZQdjUAMCmZyZXNoSG91cnMBAAAAAABAQgwPdHJhbnNpdGlvbkhvdXJzAQAAAAAAwEEMEXRyYW5zaXRpb25lZEhvdXJzAQAAAAAAAAAAAAABB3BpZVNpemUEAAAACQhiYWtlYWJsZQEEEHF1YW50aXR5U2VydmluZ3MAAIA/BQx0b3BDcnVzdFR5cGUEZnVsbAYPdHJhbnNpdGlvbnN0YXRlAxFjcmVhdGVkVG90YWxIb3Vyc/PUxLMCG41AAxVsYXN0VXBkYXRlZFRvdGFsSG91cnNGtvP9lB2NQAwKZnJlc2hIb3VycwEAAAC3/z9CDA90cmFuc2l0aW9uSG91cnMBAAAAt/8PQgwRdHJhbnNpdGlvbmVkSG91cnMBAAAAjJKkPgAA";

        public static void RegisterCommand(ICoreServerAPI api)
        {
            api.ChatCommands.Create()
                .WithName("getfood")
                .RequiresPrivilege(Privilege.give)
                .HandleWith(args =>
                {
                    var hours = args.Caller.Entity.World.Calendar.TotalHours;
                    CollectibleObject? redmeat = args.Caller.Entity.World.GetItem("game:redmeat-raw");
                    CollectibleObject? pot = args.Caller.Entity.World.GetBlock("game:claypot-tan-cooked");
                    CollectibleObject? pie = args.Caller.Entity.World.GetBlock("game:pie-raw");

                    ItemStack redmeatStack = new(redmeat);
                    redmeatStack.FromBytes(new(new MemoryStream(Convert.FromBase64String(redmeat64))));
                    redmeatStack.StackSize = 1;
                    ProcessTree(redmeatStack.Attributes, hours);

                    args.Caller.Player.Entity.TryGiveItemStack(redmeatStack);


                    ItemStack potStack = new(pot);
                    potStack.FromBytes(new(new MemoryStream(Convert.FromBase64String(claypot64))));
                    potStack.StackSize = 1;
                    ProcessTree(potStack.Attributes, hours);

                    args.Caller.Player.Entity.TryGiveItemStack(potStack);


                    ItemStack pieStack = new(pie);
                    pieStack.FromBytes(new(new MemoryStream(Convert.FromBase64String(pie64))));
                    pieStack.StackSize = 1;
                    ProcessTree(pieStack.Attributes, hours);

                    args.Caller.Player.Entity.TryGiveItemStack(pieStack);

                    return TextCommandResult.Success("Generated food");
                });
        }

        private static void ProcessTree(ITreeAttribute attributes, double totalHours)
        {
            var transitionstate = attributes["transitionstate"] as ITreeAttribute;
            if (transitionstate is not null)
            {
                (transitionstate["createdTotalHours"] as DoubleAttribute)!.SetValue(totalHours);
                (transitionstate["lastUpdatedTotalHours"] as DoubleAttribute)!.SetValue(totalHours);
                var transitionedHours = transitionstate["transitionedHours"] as FloatArrayAttribute;
                transitionedHours!.value = transitionedHours.value.Select(_ => 0f).ToArray();
                var transitionHours = transitionstate["transitionHours"] as FloatArrayAttribute;
                transitionHours!.value = transitionHours.value.Select(_ => 5 * 24f).ToArray();
                var freshHours = transitionstate["freshHours"] as FloatArrayAttribute;
                freshHours!.value = freshHours.value.Select(_ => 5 * 24f).ToArray();
            }

            var contents = attributes["contents"] as ITreeAttribute;
            if (contents is not null)
            {
                foreach (var content in contents.Values.OfType<ItemstackAttribute>())
                {
                    ProcessTree(content.value.Attributes, totalHours);
                }
            }
        }
    }
}
#endif
