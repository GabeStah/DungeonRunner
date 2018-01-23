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
		private AnimatedCardList _bossAnimatedCardList;

		public string Author => "Gabe Wyatt <gabe@gabewyatt.com>";

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
			_bossAnimatedCardList = new AnimatedCardList();

			// Stick to the Right of the player panel
			var border = Core.OverlayCanvas.FindName("BorderStackPanelOpponent") as Border;
			DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(Border))
				.AddValueChanged(border, Layout);
			DependencyPropertyDescriptor.FromProperty(Canvas.TopProperty, typeof(Border))
				.AddValueChanged(border, Layout);
			DependencyPropertyDescriptor.FromProperty(StackPanel.ActualWidthProperty, typeof(StackPanel))
				.AddValueChanged(_bossAnimatedCardList, Layout);
			
			Core.OverlayCanvas.Children.Add(_bossAnimatedCardList);
			
			var dungeonRunner = new DungeonRunner(_bossAnimatedCardList);

			// TODO: Fix bug, summoning (ressurecting) a minion increases counter by 1.

			GameEvents.OnGameStart.Add(dungeonRunner.GameStart);
			GameEvents.OnGameEnd.Add(dungeonRunner.GameEnd);
			GameEvents.OnInMenu.Add(dungeonRunner.InMenu);
		}

		private void Layout(object obj, EventArgs e)
		{
			var border = Core.OverlayCanvas.FindName("BorderStackPanelOpponent") as Border;
			if (border == null) return;
			Canvas.SetLeft(_bossAnimatedCardList, Canvas.GetLeft(border) - border.ActualWidth * Config.Instance.OverlayOpponentScaling / 100 - 10);
			Canvas.SetTop(_bossAnimatedCardList, Canvas.GetTop(border));
		}

		public void OnUnload()
		{
			Core.OverlayCanvas.Children.Remove(_bossAnimatedCardList);
		}

		public void OnUpdate()
		{
		}

		public Version Version => new Version(0, 3, 16);
	}
}