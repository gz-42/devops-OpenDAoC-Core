using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DOL.Database;
using DOL.Events;
using DOL.GS.Behaviour;
using DOL.GS.PacketHandler;
using DOL.Language;

namespace DOL.GS.Quests
{

	/// <summary>
	/// BaseQuest provides some helper classes for writing quests and
	/// integrates a new QuestPart Based QuestSystem.
	/// </summary>
	public abstract class BaseQuest : AbstractQuest
	{
		/// <summary>
		/// Defines a logger for this class.
		/// </summary>
		private static readonly Logging.Logger log = Logging.LoggerManager.Create(MethodBase.GetCurrentMethod().DeclaringType);


		/// <summary>
		/// DO NOT USE, always true.
		/// Tolakram - this is not used anymore due to the fact items were saved based on the same setting
		/// With the new inventory system all items must be saved. To optionally not save NPC's use ServerProperties.Properties.SAVE_QUEST_MOBS_INTO_DATABASE
		/// </summary>
		public static bool SAVE_INTO_DATABASE = true;
		
		// Tolakram - this is not used anymore due to the fact items were saved based on the same setting
		// With the new inventory system all items must be saved.
		// To optionally not save NPC's use ServerProperties.Properties.SAVE_QUEST_MOBS_INTO_DATABASE

		public Queue m_animEmoteTeleportTimerQueue = new Queue();
		public Queue m_animEmoteObjectQueue = new Queue();

		public Queue m_animSpellTeleportTimerQueue = new Queue();
		public Queue m_animSpellObjectQueue = new Queue();

		public Queue m_portTeleportTimerQueue = new Queue();
		public Queue m_portObjectQueue = new Queue();
		public Queue m_portDestinationQueue = new Queue();

		// /// <summary>
		// /// List of all QuestParts that can be fired on interact Events.
		// /// </summary>
		//private static IDictionary interactQuestParts = new HybridDictionary();

		/// <summary>
		/// Create an empty Quest
		/// </summary>
		public BaseQuest()
			: base()
		{
		}

		/// <summary>
		/// Constructs a new empty Quest
		/// </summary>
		public BaseQuest(GamePlayer questingPlayer)
			: base(questingPlayer)
		{
		}

		/// <summary>
		/// Constructs a new Quest
		/// </summary>
		/// <param name="questingPlayer">The player doing this quest</param>
		/// <param name="step">The current step the player is on</param>
		public BaseQuest(GamePlayer questingPlayer, int step)
			: base(questingPlayer, step)
		{
		}

		/// <summary>
		/// Constructs a new Quest from a database Object
		/// </summary>
		/// <param name="questingPlayer">The player doing the quest</param>
		/// <param name="dbQuest">The database object</param>
		public BaseQuest(GamePlayer questingPlayer, DbQuest dbQuest)
			: base(questingPlayer, dbQuest)
		{
		}


		[ScriptUnloadedEvent]
		public static void ScriptUnloadedBase(DOLEvent e, object sender, EventArgs args)
		{
			if (questParts != null)
			{
				for (int i = questParts.Count - 1; i >= 0; i--)
				{
					RemoveBehaviour((QuestBehaviour)questParts[i]);
				}
			}
			questParts = null;
		}

		// Base QuestPart methods

		/// <summary>
		/// Remove all registered handlers for this quest,
		/// this will not remove the questPart from the quest.
		/// </summary>
		/// <param name="questPart">QuestPart to remove handlers from</param>
		protected static void UnRegisterBehaviour(QuestBehaviour questPart)
		{
			if (questPart.Triggers == null)
				return;

			foreach (IBehaviourTrigger trigger in questPart.Triggers)
			{
				trigger.Unregister();
			}
		}
		/// <summary>
		/// Adds the given questpart to the quest depending on the added triggers it will either
		/// be added as InteractQuestPart as NotifyQuestPart or both and also register the needed event handler.
		/// </summary>
		/// <param name="questPart">QuestPart to be added</param>
		public static void AddBehaviour(QuestBehaviour questPart)
		{
			if (questParts == null)
				questParts = new ArrayList();

			if (!questParts.Contains(questPart))
				questParts.Add(questPart);
			
			questPart.ID = questParts.Count; // fake id but ids only have to be unique quest wide its enough to use the number in the list as id.
		}

		/// <summary>
		/// Remove the given questpart from the quest and also unregister the handlers
		/// </summary>
		/// <param name="questPart">QuestPart to be removed</param>
		public static void RemoveBehaviour(QuestBehaviour questPart)
		{
			if (questParts == null)
				return;

			UnRegisterBehaviour(questPart);
			questParts.Remove(questPart);
		}

		/// <summary>
		/// Quest internal Notify method only fires if player already has the quest assigned
		/// </summary>
		/// <param name="e"></param>
		/// <param name="sender"></param>
		/// <param name="args"></param>
		public override void Notify(DOLEvent e, object sender, EventArgs args)
		{
			if (sender is GamePlayer && e == GameObjectEvent.InteractWith)
			{
				InteractWithEventArgs iArgs = args as InteractWithEventArgs;
				if (iArgs.Target is GameStaticItem)
				{
					InteractWithObject(sender as GamePlayer, iArgs.Target as GameStaticItem);
					return;
				}
			}

			if (questParts == null)
				return;

			foreach (QuestBehaviour questPart in questParts)
			{
				questPart.Notify(e, sender, args);
			}
		}


		//timer callbacks
		protected virtual int MakeAnimSpellSequence(ECSGameTimer callingTimer)
		{
			if (m_animSpellTeleportTimerQueue.Count > 0)
			{
				m_animSpellTeleportTimerQueue.Dequeue();
				GameLiving animObject = (GameLiving)m_animSpellObjectQueue.Dequeue();
				foreach (GamePlayer player in animObject.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
				{
					player.Out.SendSpellCastAnimation(animObject, 1, 20);
				}
			}
			return 0;
		}

		protected virtual int MakeAnimEmoteSequence(ECSGameTimer callingTimer)
		{
			if (m_animEmoteTeleportTimerQueue.Count > 0)
			{
				m_animEmoteTeleportTimerQueue.Dequeue();
				GameLiving animObject = (GameLiving)m_animEmoteObjectQueue.Dequeue();
				foreach (GamePlayer player in animObject.GetPlayersInRadius(WorldMgr.VISIBILITY_DISTANCE))
				{
					player.Out.SendEmoteAnimation(animObject, eEmote.Bind);
				}
			}
			return 0;
		}

		protected virtual void TeleportTo(GameObject target, GameObject caster, GameLocation location)
		{
			TeleportTo(target, caster, location, 0, 0);
		}

		protected virtual void TeleportTo(GameObject target, GameObject caster, GameLocation location, uint delay)
		{
			TeleportTo(target, caster, location, delay, 0);
		}

		protected virtual void TeleportTo(GameObject target, GameObject caster, GameLocation location, uint delay, int fuzzyLocation)
		{
			delay *= 100; // 1/10sec to milliseconds
			if (delay <= 0)
				delay = 1;
			m_animSpellObjectQueue.Enqueue(caster);
			m_animSpellTeleportTimerQueue.Enqueue(new ECSGameTimer(caster, new ECSGameTimer.ECSTimerCallback(MakeAnimSpellSequence), (int)delay));

			m_animEmoteObjectQueue.Enqueue(target);
			m_animEmoteTeleportTimerQueue.Enqueue(new ECSGameTimer(target, new ECSGameTimer.ECSTimerCallback(MakeAnimEmoteSequence), (int)delay + 2000));

			m_portObjectQueue.Enqueue(target);

			location.X += Util.Random(0 - fuzzyLocation, fuzzyLocation);
			location.Y += Util.Random(0 - fuzzyLocation, fuzzyLocation);

			m_portDestinationQueue.Enqueue(location);
			m_portTeleportTimerQueue.Enqueue(new ECSGameTimer(target, new ECSGameTimer.ECSTimerCallback(MakePortSequence), (int)delay + 3000));

			if (location.Name != null)
			{
                m_questPlayer.Out.SendMessage(LanguageMgr.GetTranslation(m_questPlayer.Client, "BaseQuest.TeleportTo.Text1", target.Name, location.Name), eChatType.CT_System, eChatLoc.CL_SystemWindow);
            }

		}

		protected virtual int MakePortSequence(ECSGameTimer callingTimer)
		{
			if (m_portTeleportTimerQueue.Count > 0)
			{
				m_portTeleportTimerQueue.Dequeue();
				GameObject gameObject = (GameObject)m_portObjectQueue.Dequeue();
				GameLocation location = (GameLocation)m_portDestinationQueue.Dequeue();
				gameObject.MoveTo(location.RegionID, location.X, location.Y, location.Z, location.Heading);
			}
			return 0;
		}

		#region World Item Interaction

		protected struct QuestStepInteraction
		{
			public string objectName;
			public int numRequired;
			public DbItemTemplate itemResult;
			public string interactText;
		}

		Dictionary<int, QuestStepInteraction> m_interactions = new Dictionary<int, QuestStepInteraction>();
		const int INTERACT_ITEM_RESPAWN_SECONDS = 120;

		/// <summary>
		/// Add an interact item associated with a step for this quest
		/// </summary>
		/// <param name="step">What step is this item valid for</param>
		/// <param name="staticObjectName">the name of the static item to interact with</param>
		/// <param name="numRequired">How many times to interact before this step is complete</param>
		/// <param name="itemResult">What item is given to the player when interacting</param>
		/// <param name="interactText">Text presented to player when interacting with the object</param>
		protected void AddInteractStep(int step, string objectName, int numRequired, DbItemTemplate itemResult, string interactText)
		{
			try
			{
				QuestStepInteraction info = new QuestStepInteraction();
				info.objectName = objectName;
				info.numRequired = numRequired;
				info.itemResult = itemResult;
				info.interactText = interactText;

				m_interactions.Add(step, info);
			}
			catch (Exception ex)
			{
				log.Error("Error adding Interact Step, possible duplicate?", ex);
			}
		}

		/// <summary>
		/// We are interacting with an object, check to see if this quest and step needs to respond
		/// </summary>
		/// <param name="player"></param>
		/// <param name="staticItem"></param>
		protected void InteractWithObject(GamePlayer player, GameStaticItem staticItem)
		{
			if (m_interactions.Count > 0)
			{
				if (m_interactions.TryGetValue(Step, out QuestStepInteraction info))
				{
					if (staticItem.Name == info.objectName)
					{
						if (GiveItem(player, info.itemResult, false))
						{
							player.Out.SendMessage(info.interactText, eChatType.CT_System, eChatLoc.CL_SystemWindow);
							staticItem.RemoveFromWorld(INTERACT_ITEM_RESPAWN_SECONDS);
							OnObjectInteract(info);
						}
					}
				}
			}
		}

		/// <summary>
		/// When an object is interacted with this message is sent after world item is removed and inventory item added
		/// </summary>
		/// <param name="info"></param>
		protected virtual void OnObjectInteract(QuestStepInteraction info)
		{
			// this is needed in order to support both Base and Reward quests
			log.Error("Override OnObjectInteract to advance goal progress");
		}

		#endregion World Item Interaction

	}
}
