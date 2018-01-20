using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Xml.Linq;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Controls;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Hearthstone.Entities;
using Card = Hearthstone_Deck_Tracker.Hearthstone.Card;
using CoreAPI = Hearthstone_Deck_Tracker.API.Core;

namespace DungeonRunner
{
	public class DungeonRunner
	{
		private const string BossesXmlFile = "DungeonRunner.Bosses.xml";
		private readonly AnimatedCardList _bossAnimatedCardList;

		internal string BossName { get; set; }
		internal Boss Boss { get; set; }
		internal List<Boss> Bosses { get; } = new List<Boss>();

		internal List<Entity> Entities =>
			Helper.DeepClone(CoreAPI.Game.Entities).Values.ToList();

		internal Entity Opponent => Entities?.FirstOrDefault(x => x.IsOpponent);

		public DungeonRunner(AnimatedCardList bossAnimatedCardList)
		{
			GenerateBosses();

			_bossAnimatedCardList = bossAnimatedCardList;
			// Hide in menu, if necessary
			//if (Config.Instance.HideInMenu && CoreAPI.Game.IsInMenu)
			//    _list.Hide();

			GameEvents.OnOpponentCreateInDeck.Add(AddCardToBossDeck);
			GameEvents.OnOpponentPlayToDeck.Add(AddCardToBossDeck);
			GameEvents.OnOpponentPlayToHand.Add(AddCardToBossDeck);

			GameEvents.OnOpponentCreateInPlay.Add(RemoveCardFromBossDeck);
			GameEvents.OnOpponentDeckDiscard.Add(RemoveCardFromBossDeck);
			GameEvents.OnOpponentDeckToPlay.Add(RemoveCardFromBossDeck);
			GameEvents.OnOpponentHandDiscard.Add(RemoveCardFromBossDeck);
			GameEvents.OnOpponentPlay.Add(RemoveCardFromBossDeck);
			GameEvents.OnPlayerDraw.Add(OnPlayerDraw);
		}

		///// <summary>
		///// Gets the dungeon boss base, sorted card list.
		///// </summary>
		///// <returns></returns>
		//internal List<Card> GetDungeonBossBaseDeck()
		//   {
		//	Log.Debug("[ImportDungeonBossDeck() invoked.]");
		//    if (CoreAPI.Game.IsDungeonMatch == null) return null;
		//    Log.Debug("CoreAPI.Game.IsDungeonMatch != null");
		//    if (DungeonBossCards != null) return null;
		//    Log.Debug("DungeonBossCards == null");
		//    DungeonBossName = CoreAPI.Game.CurrentGameStats.OpponentHero;
		//    if (DungeonBossName == null) return null;
		//    Log.Debug($"DungeonBossName: {DungeonBossName}");
		//	// Avoid null value retrieval by setting default value pair.
		//    var deckList = "";
		//    //var deckList = DeckLists.DefaultIfEmpty(new KeyValuePair<string, string>("", "")).SingleOrDefault(x => x.Key == DungeonBossName).Value;
		//    if (string.IsNullOrEmpty(deckList))
		//    {
		//	    // If no valid deck use empty card list.
		//	    Log.Debug($"++++++ NO DECK FOUND FOR {DungeonBossName} ++++++");
		//	    return new List<Card>();
		//    }

		//    var list = GetDungeonBossDeckFromString(deckList);
		//    // Sort cards by cost.
		//    list.Sort((c1, c2) => c1.Cost.CompareTo(c2.Cost));
		//    return list;
		//   }

		//private static readonly Regex CardLineRegexCountFirst = new Regex(@"(^(\s*)(?<count>\d)(\s*x)?\s+)(?<cardname>[\w\s'\.:!\-,]+)");
		//   private static readonly Regex CardLineRegexCountLast = new Regex(@"(?<cardname>[\w\s'\.:!\-,]+?)(\s+(x\s*)?(?<count>\d))(\s*)$");
		//   public static char[] Separators = { '\n', '|' };

		///// <summary>
		///// Gets dungeon boss card list from formatted string version.
		///// </summary>
		///// <param name="deckString">Formatted deck string.</param>
		///// <returns></returns>
		//   internal List<Card> GetDungeonBossDeckFromString(string deckString)
		//   {
		//    try
		//    {
		//	    var lines = deckString.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
		//	    var cards = new List<Card>();
		//	    foreach (var line in lines)
		//	    {
		//		    var count = 1;
		//		    var cardName = line.Trim();
		//		    Match match = null;
		//		    if (CardLineRegexCountFirst.IsMatch(cardName))
		//			    match = CardLineRegexCountFirst.Match(cardName);
		//		    else if (CardLineRegexCountLast.IsMatch(cardName))
		//			    match = CardLineRegexCountLast.Match(cardName);
		//		    if (match != null)
		//		    {
		//			    var tmpCount = match.Groups["count"];
		//			    if (tmpCount.Success)
		//				    count = int.Parse(tmpCount.Value);
		//			    cardName = match.Groups["cardname"].Value.Trim();
		//		    }

		//		    var card = Database.GetCardFromName(cardName.Replace("’", "'"));
		//		    if (string.IsNullOrEmpty(card?.Name) || card.Id == Database.UnknownCardId)
		//			    continue;
		//		    Log.Debug($"ADDING CARD {card.Name} parsed count: {count}.");
		//		    card.Count = count;

		//		    cards.Add(card);
		//	    }

		//	    Log.Debug("--- GetDungeonBossDeckFromString CARDS ---");
		//	    LogCardList(cards);

		//	    return cards;
		//    }
		//    catch (Exception ex)
		//    {
		//	    Log.Error(ex);
		//	    return null;
		//    }
		//   }

		/// <summary>
		///     Retrieve the Card list of boss.
		/// </summary>
		/// <returns>Card list for boss.</returns>
		internal List<Card> BossCards { get; set; }

		private static void OnPlayerDraw(Card obj)
		{
		}

		/// <summary>
		///     Runs when game ends.
		/// </summary>
		internal void GameEnd()
		{
			Reset();
			UpdateBossDeck();
		}

		/// <summary>
		///     Runs when game starts.
		/// </summary>
		internal void GameStart()
		{
			Reset();
			UpdateBossDeck();
		}

		/// <summary>
		///     Gets the dungeon boss base, sorted card list.
		/// </summary>
		/// <returns></returns>
		internal List<Card> GetBossBaseDeck()
		{
			Log.Debug("[ImportBossDeck() invoked.]");
			if (CoreAPI.Game.IsDungeonMatch == null) return null;
			Log.Debug("CoreAPI.Game.IsDungeonMatch != null");
			if (BossCards != null) return null;
			Log.Debug("BossCards == null");
			BossName = CoreAPI.Game.CurrentGameStats.OpponentHero;
			if (BossName == null) return null;
			Log.Debug($"BossName: {BossName}");

			// Get boss by name.
			var boss = Bosses.Single(b => b.Name == BossName);
			// If found, return card list.
			if (boss != null) return boss.CardList;
			// If no valid deck use empty card list.
			Log.Debug($"++++++ NO DECK FOUND FOR {BossName} ++++++");
			return new List<Card>();
		}

		/// <summary>
		///     Adds the passed Card to dungeon boss deck.
		/// </summary>
		/// <param name="card">Card to be added.</param>
		internal void AddCardToBossDeck(Card card)
		{
			if (BossCards == null) return;

			if (BossCards.Any(c => c.Id == card.Id))
			{
				var existing = BossCards.First(c => c.Id == card.Id);
				// Increment count.
				//existing.Count = card.Count + 1;
				existing.Count++;
			}
			else
			{
				// Card not found, add.
				BossCards.Add(card);
			}

			UpdateBossDeck();
		}

		/// <summary>
		///     Removes passed Card from dungeon boss deck.
		/// </summary>
		/// <param name="card">Card to be removed.</param>
		internal void RemoveCardFromBossDeck(Card card)
		{
			if (BossCards == null) return;

			// Check if card exists.
			if (BossCards.Any(c => c.Id == card.Id))
			{
				var existing = BossCards.First(c => c.Id == card.Id);
				Log.Debug($"EXISTING CARD, PREVIOUS: {existing.Name}[{existing.Count}]");
				Log.Debug($"INCOMING CARD: {card.Name} [{card.Count}]");
				existing.Count--;
				Log.Debug($"EXISTING CARD, CURRENT: {existing.Name}[{existing.Count}]");
			}

			UpdateBossDeck();
		}

		/// <summary>
		///     Updates dungeon boss deck.
		/// </summary>
		internal void UpdateBossDeck()
		{
			if (BossCards == null) return;

			Log.Debug("UpdateBossDeck");
			LogCardList(BossCards);

			_bossAnimatedCardList.Update(BossCards, false);

			// Make visible.
			SetVisibility(true);
		}

		/// <summary>
		///		Executes when entering menu.
		/// </summary>
		internal void InMenu()
		{
			SetVisibility(!CoreAPI.Game.IsInMenu);
			if (Config.Instance.HideInMenu)
			{
				//_dungeonBossAnimatedCardList.Visibility = System.Windows.Visibility.Hidden;
			}
		}

		/// <summary>
		///     Executes on turn start.
		///     Performs base deck retrieval and updates display.
		/// </summary>
		/// <param name="player">Active player.</param>
		internal void TurnStart(ActivePlayer player)
		{
			if (player != ActivePlayer.Player || Opponent == null) return;

			// Get base boss deck, if necessary.
			if (BossCards != null) return;
			BossCards = GetBossBaseDeck();
			//DungeonBossCards = GetDungeonBossBaseDeck();

			UpdateBossDeck();
		}

		/// <summary>
		///     Sets visibility of dungeon boss deck.
		/// </summary>
		/// <param name="visibility">Indicates deck visibility.</param>
		internal void SetVisibility(Visibility visibility = Visibility.Visible)
		{
			_bossAnimatedCardList.Visibility = visibility;
		}

		/// <summary>
		///     Sets visibility of dungeon boss deck.
		/// </summary>
		/// <param name="visible">Indicates if deck should be visible.</param>
		internal void SetVisibility(bool visible = true)
		{
			_bossAnimatedCardList.Visibility = visible ? Visibility.Visible : Visibility.Hidden;
		}

		/// <summary>
		///     Resets dungeon boss deck.
		/// </summary>
		internal void Reset()
		{
			Log.Debug("RESETTING DUNGEON BOSS DECK");
			BossCards = null;
		}

		internal void LogEntityList(IEnumerable<Entity> list)
		{
			foreach (var element in list)
				Log.Debug($"{element.Card.Id}: {element.Card.Name} x {element.Card.Count}");
		}

		internal void LogCardList(IEnumerable<Card> list)
		{
			foreach (var element in list)
				Log.Debug($"{element.Id}: {element.Name} x {element.Count}");
		}

		/// <summary>
		///		Generate Bosses collection from xml.
		/// </summary>
		internal void GenerateBosses()
		{
			try
			{
				var assembly = Assembly.GetExecutingAssembly();
				var textStreamReader = new StreamReader(assembly.GetManifestResourceStream(BossesXmlFile));
				var xml = XElement.Load(textStreamReader);

				var bossElements = xml.Elements("boss");
				foreach (var bossElement in bossElements)
				{
					var name = bossElement.Attribute("name")?.Value;
					var tierMinimum = Convert.ToInt32(bossElement.Element("tierMinimum")?.Value);
					var tierMaximum = Convert.ToInt32(bossElement.Element("tierMaximum")?.Value);
					var cardElements = bossElement.Element("deck")?.Elements("card");
					var cardList = new List<Card>();
					if (cardElements != null)
						foreach (var cardElement in cardElements)
						{
							var cardName = cardElement.Element("name")?.Value.Replace("’", "'");
							var card = Database.GetCardFromName(cardName, false, true, false);
							if (string.IsNullOrEmpty(card?.Name) || card.Id == Database.UnknownCardId)
							{
								Log.Error($"Could not import '{cardName}' card from boss deck '{name}'.");
								continue;
							}
							card.Count = Convert.ToInt32(cardElement.Element("count")?.Value);
							cardList.Add(card);
						}
					// Sort card list by cost.
					cardList.Sort((c1, c2) => c1.Cost.CompareTo(c2.Cost));
					// Add boss to collection.
					Bosses.Add(new Boss(name, cardList, tierMinimum, tierMaximum));
				}
			}
			catch (Exception ex)
			{
				Hearthstone_Deck_Tracker.Utility.Logging.Log.Error(ex);
			}
		}
	}
}