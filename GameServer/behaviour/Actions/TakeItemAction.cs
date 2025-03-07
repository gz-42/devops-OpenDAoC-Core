using System;
using System.Collections.Generic;
using DOL.Database;
using DOL.Events;
using DOL.GS.Behaviour.Attributes;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Behaviour.Actions
{
    [ActionAttribute(ActionType = eActionType.TakeItem,DefaultValueQ=1)]
    public class TakeItemAction : AbstractAction<DbItemTemplate, int>
    {

        public TakeItemAction(GameNPC defaultNPC,  Object p, Object q)
            : base(defaultNPC, eActionType.TakeItem, p, q)
        {
        }


        public TakeItemAction(GameNPC defaultNPC,   DbItemTemplate itemTemplate, int quantity)
            : this(defaultNPC, (object)itemTemplate,(object) quantity) { }



        public override void Perform(DOLEvent e, object sender, EventArgs args)
        {
            GamePlayer player = BehaviourUtils.GuessGamePlayerFromNotify(e, sender, args);
            int count = Q;
            DbItemTemplate itemToRemove = P;

			Dictionary<DbInventoryItem, int?> dataSlots = new Dictionary<DbInventoryItem, int?>(10);
            lock (player.Inventory.Lock)
            {
                var allBackpackItems = player.Inventory.GetItemRange(eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack);

                bool result = false;
                foreach (DbInventoryItem item in allBackpackItems)
                {
                    if (item.Name == itemToRemove.Name)
                    {

                        if (item.IsStackable) // is the item is stackable
                        {
                            if (item.Count >= count)
                            {
                                if (item.Count == count)
                                {
                                    dataSlots.Add(item, null);
                                }
                                else
                                {
                                    dataSlots.Add(item, count);
                                }
                                result = true;
                                break;
                            }
                            else
                            {
                                dataSlots.Add(item, null);
                                count -= item.Count;
                            }
                        }
                        else
                        {
                            dataSlots.Add(item, null);
                            if (count <= 1)
                            {
                                result = true;
                                break;
                            }
                            else
                            {
                                count--;
                            }
                        }
                    }
                }
                if (result == false)
                {
                    return;
                }

                GamePlayerInventory playerInventory = player.Inventory as GamePlayerInventory;
                playerInventory.BeginChanges();
				Dictionary<DbInventoryItem, int?>.Enumerator enumerator = dataSlots.GetEnumerator();
                while (enumerator.MoveNext())
                {
		
		KeyValuePair<DbInventoryItem, int?> de = enumerator.Current;
                    
		if (de.Value.HasValue)
                    {
                        playerInventory.RemoveItem(de.Key);
                        InventoryLogging.LogInventoryAction(player, NPC, eInventoryActionType.Quest, de.Key.Template, de.Key.Count);
                    }
                    else
                    {
                        playerInventory.RemoveCountFromStack(de.Key, de.Value.Value);
                        InventoryLogging.LogInventoryAction(player, NPC, eInventoryActionType.Quest, de.Key.Template, de.Value.Value);
                    }
                }
                playerInventory.CommitChanges();

                if (NPC != null)
                {
                    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Behaviour.TakeItemAction.YouGiveItemToNPC", itemToRemove.Name, NPC.GetName(0, false)), eChatType.CT_Loot, eChatLoc.CL_SystemWindow);
                }
                else
                {
                    player.Out.SendMessage(LanguageMgr.GetTranslation(player.Client.Account.Language, "Behaviour.TakeItemAction.YouGiveItem", itemToRemove.Name), eChatType.CT_Loot, eChatLoc.CL_SystemWindow);
                }
            }
        }
    }
}
