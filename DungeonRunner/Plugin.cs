//using System;
//using System.ComponentModel;
//using System.Windows.Controls;
//using Hearthstone_Deck_Tracker;
//using Hearthstone_Deck_Tracker.API;
//using Hearthstone_Deck_Tracker.Controls;
//using Hearthstone_Deck_Tracker.Plugins;
//using Hearthstone_Deck_Tracker.Windows;
//using Core = Hearthstone_Deck_Tracker.API.Core;

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Collections.Generic;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.API;
using Hearthstone_Deck_Tracker.Controls;
using Hearthstone_Deck_Tracker.Plugins;
using Hearthstone_Deck_Tracker.Windows;
using Core = Hearthstone_Deck_Tracker.API.Core;
using Card = Hearthstone_Deck_Tracker.Hearthstone.Card;

namespace DungeonRunner
{
	public class Plugin : IPlugin
	{
		private AnimatedCardList _bossAnimatedCardList;
		private StackPanel _bossStackPanel;

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
			_bossStackPanel = new StackPanel();

			// Stick to the Right of the player panel
			var border = Core.OverlayCanvas.FindName("BorderStackPanelOpponent") as Border;
			DependencyPropertyDescriptor.FromProperty(Canvas.LeftProperty, typeof(Border))
				.AddValueChanged(border, Layout);
			DependencyPropertyDescriptor.FromProperty(Canvas.TopProperty, typeof(Border))
				.AddValueChanged(border, Layout);
			DependencyPropertyDescriptor.FromProperty(StackPanel.ActualWidthProperty, typeof(StackPanel))
				.AddValueChanged(_bossAnimatedCardList, Layout);

			_bossStackPanel.Orientation = Orientation.Vertical;
			// Add panel to core UI.
			Core.OverlayCanvas.Children.Add(_bossStackPanel);
			Canvas.SetTop(_bossStackPanel, Settings.Default.TopOffset);
			Canvas.SetLeft(_bossStackPanel, Settings.Default.LeftOffset);

			// Add card list to panel.
			_bossStackPanel.Children.Add(_bossAnimatedCardList);

			//Core.OverlayCanvas.Children.Add(_bossAnimatedCardList);

			var dungeonRunner = new DungeonRunner(_bossAnimatedCardList, _bossStackPanel);

			// TODO: Fix bug, summoning (ressurecting) a minion increases counter by 1.

			GameEvents.OnGameStart.Add(dungeonRunner.GameStart);
			GameEvents.OnGameEnd.Add(dungeonRunner.GameEnd);
			GameEvents.OnInMenu.Add(dungeonRunner.InMenu);
		}

		private void Layout(object obj, EventArgs e)
		{
			//var border = Core.OverlayCanvas.FindName("BorderStackPanelOpponent") as Border;
			//if (border == null) return;
			//Canvas.SetLeft(_bossAnimatedCardList, Canvas.GetLeft(border) - border.ActualWidth * Config.Instance.OverlayOpponentScaling / 100 - 10);
			//Canvas.SetTop(_bossAnimatedCardList, Canvas.GetTop(border));
			AttachToPanel();
		}

		private void AttachToPanel()
		{
			if (Settings.Default.IsFloating == false)
			{
				// Get opponent border frame.
				var border = Core.OverlayCanvas.FindName("BorderStackPanelOpponent") as Border;
				if (border == null) return;

				switch (Settings.Default.AttachmentSide)
				{
					case Settings.AttachmentSides.Left:
						Canvas.SetLeft(_bossStackPanel, Canvas.GetLeft(border) - border.ActualWidth * Config.Instance.OverlayOpponentScaling / 100 - 10);
						Canvas.SetTop(_bossStackPanel, Canvas.GetTop(border));
						break;
					case Settings.AttachmentSides.Right:
						Canvas.SetRight(_bossStackPanel, Canvas.GetRight(border) + border.ActualWidth * Config.Instance.OverlayOpponentScaling / 100 + 10);
						Canvas.SetTop(_bossStackPanel, Canvas.GetTop(border));
						break;
					case Settings.AttachmentSides.Top:
						Canvas.SetLeft(_bossStackPanel, Canvas.GetLeft(border));
						Canvas.SetTop(_bossStackPanel, Canvas.GetTop(border) - border.ActualHeight * Config.Instance.OverlayOpponentScaling / 100 - 10);
						break;
					case Settings.AttachmentSides.Bottom:
						Canvas.SetLeft(_bossStackPanel, Canvas.GetLeft(border));
						Canvas.SetBottom(_bossStackPanel, Canvas.GetBottom(border) + border.ActualHeight * Config.Instance.OverlayOpponentScaling / 100 + 10);
						break;
					default:
						throw new ArgumentOutOfRangeException();
				}
			}
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