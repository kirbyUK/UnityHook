using Hooks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Hooks
{
	[RuntimeHook]
	class LogExport
	{
		public LogExport()
		{
			HookRegistry.Register(OnCall);
		}

		private void InitDynamicTypes() { }

		public static string[] GetExpectedMethods()
		{
			return new string[] {
				"CakeGameLog::PlayToBench",
				"CakeGameLog::PlayToActive",
			};
		}

		// Recreating functions that don't appear to be exported
		/*
		private string GetPlayerName(CakeGameLog thisPtr, dwd.core.match.EntityComponent entity)
		{
			string result = String.Empty;
			if ((entity != null) && (playmat.code.cake.util.EntityUtil.IsPlayer(entity.get_EntityName())))
			{
				if (entity.get_OwningPlayerID() == thisPtr.get_model().B)
				{
					result = thisPtr.manager.player1Piles.A.playerName.get_text();
				}
				else
				{
					result = thisPtr.manager.player2Piles.A.playerName.get_text();
				}
			}
			return result;
		}
		*/

		/*
		private dwd.core.match.EntityComponent GetOwningPlayer(CakeGameLog thisPtr, dwd.core.match.EntityComponent entity)
		{
			dwd.core.match.EntityComponent player;
			if (entity.get_OwningPlayerID() == thisPtr.get_model().B)
			{
				player = thisPtr.get_model().A.a.get_Player();
			}
			else
			{
				player = thisPtr.get_model().A.A.get_Player();
			}
			return player;
		}
		*/

		/*
		private bool PlayerIsOwner(CakeGameLog thisPtr, dwd.core.match.EntityComponent entity)
		{
			bool result = false;
			if (entity != null && entity.get_OwningPlayerID() != null && thisPtr.get_model() != null && thisPtr.get_model().B != null)
			{
				result = (entity.get_OwningPlayerID() == thisPtr.get_model().B);
			}
			return result;
		}
		*/

		private string CardName(CakeGameLog thisPtr, dwd.core.match.EntityComponent entity)
		{
			string text = String.Empty;
			if (entity.Has<e.Q>())
			{
				e.Q q = entity.TryGetOne<e.Q>();
				text = q.get_Name();
				if (!String.IsNullOrEmpty(text))
				{
					text = String.Format("[{0}]{1}[-]", thisPtr.get_PlayerTextColorString(), text);
				}
			}
			if (text == null)
			{
				text = String.Empty;
			}
			return text;
		}

		private void AddLogItem(CakeGameLog thisPtr, l.I item)
		{
			thisPtr.entries.Add(item);
		}

		object OnCall(string typeName, string methodName, object thisObj, object[] args, IntPtr[] refArgs, int[] refIdxMatch)
		{
			var thisPtr = (CakeGameLog)thisObj;

			// If a Pokemon is played
			if ((methodName == "PlayToBench") || (methodName == "PlayToActive"))
			{
				var pokemon = (dwd.core.match.EntityComponent)args[0];

				// Card name
				var name = (dwd.core.attributes.MutableAttribute<string>)pokemon.GetAttribute(0x00030FB6);
				System.IO.File.AppendAllText(@"C:\Users\Alex\Documents\code\tcgo.txt", String.Format("{0}\n", name.get_Value()));

				// Emulate actual function operation
				var i = new l.I();
				i.set_Type((methodName == "PlayToBench") ? GameLogActions.PlayToBench : GameLogActions.NewActivePokemon);
//				i.set_Actor(GetPlayerName(thisPtr, GetOwningPlayer(thisPtr, pokemon)));
//				i.set_playerEntity(GetOwningPlayer(thisPtr, pokemon));
//				i.set_IsPlayer(PlayerIsOwner(thisPtr, pokemon));
				i.set_Source(CardName(thisPtr, pokemon));
				i.set_SourceEntity(pokemon);
//				i.set_ActorEntity(GetOwningPlayer(thisPtr, pokemon));
				AddLogItem(thisPtr, i);
				return i;
			}
			return null;
		}
	}
}
