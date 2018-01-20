using System;
using System.ComponentModel;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Controls;
using Hearthstone_Deck_Tracker.Plugins;
using Core = Hearthstone_Deck_Tracker.API.Core;

namespace DungeonRunner
{
	public class Plugin : IPlugin
	{
		private AnimatedCardList _dungeonBossAnimatedCardList;

		public string Author => "Gabe Wyatt <gabe@gabewyatt.com";

		public string ButtonText => "Dungeon Runner";

		public string Description => "A dungeon run helper.";

		public MenuItem MenuItem => null;

		public string Name => "Dungeon Runner";

		public void OnButtonPress()
		{
		}

		public void OnLoad()
		{
			// Create container.
			_dungeonBossAnimatedCardList = new AnimatedCardList();

			// Stick to the Right of the player panel
			var border = Core.OverlayCanvas.FindName("BorderStackPanelOpponent") as Border;
			DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(Border))
				.AddValueChanged(border, Layout);
			DependencyPropertyDescriptor.FromProperty(Canvas.TopProperty, typeof(Border))
				.AddValueChanged(border, Layout);
			DependencyPropertyDescriptor.FromProperty(StackPanel.ActualWidthProperty, typeof(StackPanel))
				.AddValueChanged(_dungeonBossAnimatedCardList, Layout);

			Core.OverlayCanvas.Children.Add(_dungeonBossAnimatedCardList);
			
			var dungeonRunner = new DungeonRunner(_dungeonBossAnimatedCardList);

			// TODO: Fix bug, summoning (ressurecting) a minion increases counter by 1.
			// TODO: Add dungeon boss detection/deck load during mulligan.

			GameEvents.OnGameStart.Add(dungeonRunner.GameStart);
			GameEvents.OnGameEnd.Add(dungeonRunner.GameEnd);
			GameEvents.OnInMenu.Add(dungeonRunner.InMenu);
			GameEvents.OnTurnStart.Add(dungeonRunner.TurnStart);
		}

		private void Layout(object obj, EventArgs e)
		{
			var border = Core.OverlayCanvas.FindName("BorderStackPanelOpponent") as Border;
			if (border == null) return;
			//Canvas.SetLeft(_dungeonBossAnimatedCardList, Canvas.GetLeft(border) + border.ActualWidth * Config.Instance.OverlayOpponentScaling / 100 + 10);
			//Canvas.SetRight(_dungeonBossAnimatedCardList, Canvas.GetLeft(border) * Config.Instance.OverlayOpponentScaling / 100 + 10);
			Canvas.SetLeft(_dungeonBossAnimatedCardList, Canvas.GetLeft(border) - border.ActualWidth * Config.Instance.OverlayOpponentScaling / 100 - 10);
			Canvas.SetTop(_dungeonBossAnimatedCardList, Canvas.GetTop(border));
		}

		public void OnUnload()
		{
			Core.OverlayCanvas.Children.Remove(_dungeonBossAnimatedCardList);
		}

		public void OnUpdate()
		{
		}

		public Version Version => new Version(0, 1, 1);
	}
}