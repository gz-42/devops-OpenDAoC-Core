/*
 * DAWN OF LIGHT - The first free open source DAoC server emulator
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 *
 */

using System;
using System.Linq;
using DOL.Database;
using DOL.GS.Keeps;

namespace DOL.GS.Commands
{
	[CmdAttribute(
		"&repair",
		ePrivLevel.Player,
		"You can repair an item when you are a crafter",
		"/repair")]
	public class RepairCommandHandler : AbstractCommandHandler, ICommandHandler
	{
		private static readonly Logging.Logger log = Logging.LoggerManager.Create(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
		private static readonly string[] woodNames = { "rowan", "elm", "oak", "oaken", "ironwood", "heartwood", "runewood", "stonewood", "ebonwood", "dyrwood", "duskwood" };
		private const int repairDuration = 20;

		public void OnCommand(GameClient client, string[] args)
		{
			if (IsSpammingCommand(client.Player, "repair"))
				return;

			WorldInventoryItem item = client.Player.TargetObject as WorldInventoryItem;
			if (item != null)
			{
				client.Player.RepairItem(item.Item);
				return;
			}

			GameKeepDoor door = client.Player.TargetObject as GameKeepDoor;
			if (door != null)
			{
				if (!PreFireChecks(client.Player, door))
					return;
				StartRepair(client.Player, door);
			}

			GameKeepComponent component = client.Player.TargetObject as GameKeepComponent;
			if (component != null)
			{
				if (!PreFireChecks(client.Player, component))
					return;
				StartRepair(client.Player, component);
			}

			GameSiegeWeapon weapon = client.Player.TargetObject as GameSiegeWeapon;
			if (weapon != null)
			{
				if (!PreFireChecks(client.Player, weapon))
					return;
				StartRepair(client.Player, weapon);
			}
		}

		public bool PreFireChecks(GamePlayer player, GameLiving obj)
		{
			if (obj == null)
				return false;

			if (player.Client.Account.PrivLevel > (int)ePrivLevel.Player)
				return true;

			if (player.Realm != obj.Realm)
				return false;

			// if ((obj as GameLiving).InCombat)
			// {
			// 	DisplayMessage(player, "You can't repair object while it is under attack!");
			//	return false;
			// }
			// if (obj is GameKeepDoor)
			// {
			//	GameKeepDoor doorcomponent = obj as GameKeepDoor;
			//	if (doorcomponent.Component.Keep.InCombat)
			//	{
			//		DisplayMessage(player, "You can't repair the keep door while keep is under attack!");
			//		return false;
			//	}
			// }
			// if (obj is IKeepItem)
			// {
			// 	if (obj.CurrentRegion.Time - obj.LastAttackedByEnemyTick <= 60 * 1000)
			// 	{
			// 		DisplayMessage(player, "You can't repair the keep component while it is under attack!");
			// 		return false;
			// 	}
			// }

			if (obj.HealthPercent == 100)
			{
				DisplayMessage(player, "The component is already at full health!");
				return false;
			}

			if (obj is GameKeepComponent)
			{
				GameKeepComponent component = obj as GameKeepComponent;
				if (component.IsRaized)
				{
					DisplayMessage(player, "You cannot repair a raized tower!");
					return false;
				}
			}

			if (obj is GameSiegeWeapon)
			{
				GameSiegeWeapon siegeweapon = obj as GameSiegeWeapon;
				if(siegeweapon.TimesRepaired > 3)
				{
					DisplayMessage(player,"The siegeweapon has decayed beyond repairs!");
					return false;
				}
			}

			if (player.IsCrafting || player.IsSalvagingOrRepairing)
			{
				DisplayMessage(player, "You must end your current action before you repair anything!");
				return false;
			}

			if (player.IsMoving)
			{
				DisplayMessage(player, "You can't repair while moving");
				return false;
			}

			if (!player.IsAlive)
			{
				DisplayMessage(player, "You can't repair while dead.");
				return false;
			}

			if (player.IsSitting)
			{
				DisplayMessage(player, "You can't repair while sitting.");
				return false;
			}

			if (player.InCombat)
			{
				DisplayMessage(player, "You can't repair while in combat.");
				return false;
			}

			if (obj.InCombat)
			{
				DisplayMessage(player, "You can't repair an object under attack.");
				return false;
			}

			if (!player.IsWithinRadius(obj, WorldMgr.INTERACT_DISTANCE))
			{
				DisplayMessage(player, "You are too far away to repair this component.");
				return false;
			}

			int repairamount = GetNeededWoodForOneTick(obj.Level);
			int playerswood = CalculatePlayersWood(player);
			int woodDifference = repairamount - playerswood;

			if (woodDifference > 0)
			{
				DisplayMessage(player, "You need another " + woodDifference + " unit" + (woodDifference > 1 ? "s" : "") + " of wood!");
				return false;
			}

			if (player.GetCraftingSkillValue(eCraftingSkill.WoodWorking) < 1)
			{
				DisplayMessage(player, "You need woodworking skill to repair.");
				return false;
			}

			player.Stealth(false);
			return true;
		}

		public void StartRepair(GamePlayer player, GameLiving obj)
		{
			player.Out.SendTimerWindow("Repairing: " + obj.Name, repairDuration);
			player.CraftTimer = new ECSGameTimer(player);
			player.CraftTimer.Callback = new ECSGameTimer.ECSTimerCallback(Proceed);
			player.CraftTimer.Properties.SetProperty("repair_player", player);
			player.CraftTimer.Properties.SetProperty("repair_target", obj);
			player.CraftTimer.Start(repairDuration * 1000);
		}

		private int Proceed(ECSGameTimer timer)
		{
			GamePlayer player = timer.Properties.GetProperty<GamePlayer>("repair_player");
			GameLiving obj = timer.Properties.GetProperty<GameLiving>("repair_target");

			if (player == null || obj == null)
			{
				if (log.IsWarnEnabled)
					log.Warn("There was a problem getting back the target or player in door/component repair!");
				return 0;
			}

			player.CraftTimer?.Stop();
			player.CraftTimer = null;
			player.Out.SendCloseTimerWindow();

			if (!PreFireChecks(player, obj))
				return 0;

			if (Util.ChanceDouble(CalculateRepairChance(player,obj)))
			{
				if (obj is GameKeepDoor)
				{
					GameKeepDoor door = obj as GameKeepDoor;
					door.Repair((int)(door.MaxHealth * 0.05));
				}
				else if (obj is GameKeepComponent)
				{
					GameKeepComponent component = obj as GameKeepComponent;
					component.Repair((int)(component.MaxHealth * 0.05));
				}
				else if (obj is GameSiegeWeapon)
				{
					GameSiegeWeapon weapon = obj as GameSiegeWeapon;
					if (weapon.Repair((int)(weapon.MaxHealth * 0.15)))
					{
						RemoveWU(player, GetNeededWoodForOneTick(obj.Level));
						DisplayMessage(player, "You successfully repair the siege weapon by 15%!");
					}
					return 0;
				}

				RemoveWU(player, GetNeededWoodForOneTick(obj.Level));
				DisplayMessage(player, "You successfully repair the component by 5%!");

				/*
				 * - Realm points will now be awarded for successfully repairing a door or outpost piece.
				 * Players will receive approximately 10% of the amount repaired in realm points.
				 * (Note that realm points for repairing a door or outpost piece will not work in the battlegrounds.)
				 */
				// tolakram - we have no idea how many hit points a live door has so this code is not accurate
				// int amount = (finish - start) * obj.Level;  // level of non claimed keep is 4
				// player.GainRealmPoints(Math.Min(150, amount));
			}
			else
			{
				DisplayMessage(player, "You fail to repair the component!");
			}

			return 0;
		}

		private static double CalculateRepairChance(GamePlayer player, GameObject obj)
		{
			if (player.Client.Account.PrivLevel > (int)ePrivLevel.Player)
				return 100;

			double skill = player.GetCraftingSkillValue(eCraftingSkill.WoodWorking);
			int skillneeded = (obj.Level + 1) * 50;
			double chance = skill / skillneeded;
			return chance;
		}

		private static int GetNeededWoodForOneTick(int level)
		{
			switch (level)
			{
				case 0: return 2;
				case 1: return 2;
				case 2: return 44;
				case 3: return 192;
				case 4: return 840; // Might be 832 instead
				case 5: return 3576;
				case 6: return 8640;
				case 7: return 14400;
				case 8: return 27200;
				case 9: return 42432;
				case 10: return 68100;
				default: return 0;
			}
		}
		
		private static int CalculatePlayersWood(GamePlayer player)
		{
			int amount = 0;

			foreach (DbInventoryItem item in player.Inventory.GetItemRange(eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack))
			{
				foreach (string name in woodNames)
				{
					if (item.Name.Replace(" wooden boards", "").ToLower() != name)
						continue;

					int woodvalue = GetWoodValue(item.Name.ToLower());
					amount += item.Count * woodvalue;
					break;
				}
			}

			return amount;
		}

		private static void RemoveWU(GamePlayer player, int woodUnits)
		{
			foreach (var item in player.Inventory.GetItemRange(eInventorySlot.FirstBackpack, eInventorySlot.LastBackpack))
			{
				if (!woodNames.Contains(item.Name.Replace(" wooden boards", "").ToLower()))
					continue;

				if (woodUnits == 0)
					break;

				var woodValue = GetWoodValue(item.Name.ToLower()) * item.Count;
				if (woodValue < woodUnits)
				{
					player.Inventory.RemoveItem(item);
					InventoryLogging.LogInventoryAction(player, "(craft)", eInventoryActionType.Craft, item.Template, item.Count);
					woodUnits -= woodValue;
				}
				else
				{
					var removeCount = (int)Math.Ceiling(woodUnits / (double)GetWoodValue(item.Name.ToLower()));
					player.Inventory.RemoveCountFromStack(item, removeCount);
					InventoryLogging.LogInventoryAction(player, "(craft)", eInventoryActionType.Craft, item.Template, removeCount);
					woodUnits = 0;
				}
			}
			
			// if (item.Count * woodvalue < removeamount)
			// {
			// 	int removecount = removeamount / woodvalue;
			// 	removeamount -= removecount * woodvalue;
			// 	player.Inventory.RemoveCountFromStack(item, removecount);
			// 	
			// }
			// else
			// {
			// 	removeamount -= item.Count * woodvalue;
			// 	player.Inventory.RemoveItem(item);
			// 	InventoryLogging.LogInventoryAction(player, "(craft)", eInventoryActionType.Craft, item.Template, item.Count);
			// }
		}
		
		public static int GetWoodValue(string name)
		{
			switch (name.Replace(" wooden boards", ""))
			{
				case "rowan": return 1;
				case "elm": return 4;
				case "oaken":
				case "oak": return 8;
				case "ironwood": return 16;
				case "heartwood": return 32;
				case "runewood": return 48;
				case "stonewood": return 60;
				case "ebonwood": return 80;
				case "dyrwood": return 104;
				case "duskwood": return 136;
				default: return 0;
			}
		}
	}
}
