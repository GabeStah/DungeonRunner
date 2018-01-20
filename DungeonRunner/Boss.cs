using System;
using System.Collections.Generic;
using Hearthstone_Deck_Tracker.Hearthstone;

namespace DungeonRunner
{
	internal class Boss : ICloneable
	{
		public List<Card> Cards { get; set; } = new List<Card>();
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

		internal Boss(string name, List<Card> cards, int tierMinimum, int tierMaximum)
		{
			Name = name;
			Cards = cards;
			TierMinumum = tierMinimum;
			TierMaximum = tierMaximum;

			Log.Info($"--- CREATING DUNGEON BOSS {name} ---");
			foreach (var card in Cards)
			{
				Log.Debug($"Card: {card.Name}, Count: {card.Count}");
			}
		}

		/// <summary>
		/// Retrieves a deep copy of Boss.
		/// </summary>
		/// <returns>Deep copy of Boss object.</returns>
		public object Clone()
		{
			var boss = (Boss) MemberwiseClone();
			boss.Name = string.Copy(Name);
			boss.Cards = Cards.ConvertAll(c => (Card) c.Clone());
			return boss;
		}
	}
}
