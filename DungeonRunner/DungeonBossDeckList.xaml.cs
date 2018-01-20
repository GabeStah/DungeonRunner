using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Enums;
using Hearthstone_Deck_Tracker.Hearthstone;
using Hearthstone_Deck_Tracker.Utility;

namespace DungeonRunner
{
	public partial class DungeonBossDeckList
	{
		public DungeonBossDeckList()
		{
			InitializeComponent();
		}

		public void Update(List<Card> cards)
		{
			if (cards == null) return;
			ListViewDungeonBoss.Update(cards, false);
		}

		public void UpdateDungeonBossLayout()
		{
			StackPanelDungeonBoss.Children.Clear();
			foreach (var item in Config.Instance.DeckPanelOrderPlayer)
			{
				switch (item)
				{
					case DeckPanel.DrawChances:
						StackPanelDungeonBoss.Children.Add(CanvasDungeonBossChance);
						break;
					case DeckPanel.CardCounter:
						StackPanelDungeonBoss.Children.Add(CanvasDungeonBossCount);
						break;
					case DeckPanel.Fatigue:
						StackPanelDungeonBoss.Children.Add(LblDungeonBossFatigue);
						break;
					case DeckPanel.DeckTitle:
						StackPanelDungeonBoss.Children.Add(LblDeckTitle);
						break;
					case DeckPanel.Wins:
						StackPanelDungeonBoss.Children.Add(LblWins);
						break;
					case DeckPanel.Cards:
						StackPanelDungeonBoss.Children.Add(ViewBoxDungeonBoss);
						break;
				}
			}
		}

		//private void SetWinRates()
		//{
		//	var selectedDeck = DeckList.Instance.ActiveDeck;
		//	if (selectedDeck == null)
		//		return;

		//	LblWins.Text = $"{selectedDeck.WinLossString} ({selectedDeck.WinPercentString})";

		//	if (!string.IsNullOrEmpty(_game.Opponent.Class))
		//	{
		//		var winsVs = selectedDeck.GetRelevantGames().Count(g => g.Result == GameResult.Win && g.OpponentHero == _game.Opponent.Class);
		//		var lossesVs = selectedDeck.GetRelevantGames().Count(g => g.Result == GameResult.Loss && g.OpponentHero == _game.Opponent.Class);
		//		var percent = (winsVs + lossesVs) > 0 ? Math.Round(winsVs * 100.0 / (winsVs + lossesVs), 0).ToString() : "-";
		//		LblWinRateAgainst.Text = $"VS {_game.Opponent.Class}: {winsVs}-{lossesVs} ({percent}%)";
		//	}
		//}

		private void SetDeckTitle() => LblDeckTitle.Text = DeckList.Instance.ActiveDeck?.Name ?? "";

		//private void SetCardCount(int cardCount, int cardsLeftInDeck)
		//{
		//	LblCardCount.Text = cardCount.ToString();
		//	LblDeckCount.Text = cardsLeftInDeck.ToString();

		//	if (cardsLeftInDeck <= 0)
		//	{
		//		LblDungeonBossFatigue.Text = LocUtil.Get(LocFatigue) + " " + (_game.DungeonBoss.Fatigue + 1);

		//		LblDrawChance2.Text = "0%";
		//		LblDrawChance1.Text = "0%";
		//		return;
		//	}
		//	LblDungeonBossFatigue.Text = "";

		//	var drawNextTurn2 = Math.Round(200.0f / cardsLeftInDeck, 1);
		//	LblDrawChance2.Text = (cardsLeftInDeck == 1 ? 100 : drawNextTurn2) + "%";
		//	LblDrawChance1.Text = Math.Round(100.0f / cardsLeftInDeck, 1) + "%";
		//}

		public void UpdateDungeonBossCards(List<Card> cards, bool reset) => ListViewDungeonBoss.Update(cards, reset);
	}
}
