using System.Collections.Generic;
using Hearthstone_Deck_Tracker.Hearthstone;

namespace DungeonRunner
{
	internal class Boss
	{
		public List<Card> CardList { get; set; } = new List<Card>();
		public string Name { get; set; }
		public int TierMinumum { get; set; }
		public int TierMaximum { get; set; }

		internal Boss()
		{
		}

		internal Boss(string name)
		{
			Name = name;
		}

		internal Boss(string name, List<Card> cardList, int tierMinimum, int tierMaximum)
		{
			Name = name;
			CardList = cardList;
			TierMinumum = tierMinimum;
			TierMaximum = tierMaximum;

			Log.Debug($"--- CREATING DUNGEON BOSS {name} ---");
			foreach (var card in CardList)
			{
				Log.Debug($"Card: {card.Name}, Count: {card.Count}");
			}
		}
	}
}
