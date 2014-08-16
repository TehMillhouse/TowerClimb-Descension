using System;
using System.Collections.Generic;
using TowerFall;
using Monocle;
using Microsoft.Xna.Framework;

namespace Mod
{
	public class MyMatchVariants : MatchVariants
	{
		[Header("MODS")]
		public Variant NoHeadBounce;
		public Variant NoLedgeGrab;
		public Variant InfiniteArrows;
		public Variant NoDodgeCooldowns;

		public MyMatchVariants()
		{
			this.CreateLinks(NoHeadBounce, NoTimeLimit);
			this.CreateLinks(NoDodgeCooldowns, ShowDodgeCooldown);
		}
	}

	public class MyRollcallElement : RollcallElement
	{
		public MyRollcallElement(int playerIndex) : base(playerIndex){}

		public override int MaxPlayers {
			get {
				return (MainMenu.RollcallMode == MainMenu.RollcallModes.Trials) ? 1 : 4;
			}
		}
	}

	public class MyQuestRoundLogic : QuestRoundLogic
	{
		public MyQuestRoundLogic(Session session) : base(session) {}

		public override void OnLevelLoadFinish()
		{
			base.OnLevelLoadFinish();

			base.Players = 0;
			for (int i = 0; i < 4; i++) {
				if (TFGame.Players[i]) {
					base.Players++;
					if (base.Players <= 2) {
						// the first two players are already taken care of by base.
						continue;
					}
					base.Session.CurrentLevel.Add<QuestPlayerHUD>(this.PlayerHUDs[i] = new QuestPlayerHUD(this, (base.Players % 2 == 0) ? Facing.Left : Facing.Right, i));
					this.SpawnPlayer(i, false);
				}
			}
		}
	}

	public class MyQuestPlayerHUD : QuestPlayerHUD
	{
		public MyQuestPlayerHUD(QuestRoundLogic quest, Facing side, int playerIndex) : base(quest, side, playerIndex)
		{
			/*
			// inline code of base.base.ctor(int LayerIndex)
			this.Active = true;
			this.Visible = true;
			this.Collidable = true;
			this.depth = 0;
			this.actualDepth = 0;
			this.layerIndex = 3;
			this.Position = Vector2.Zero;
			this.Tags = new List<GameTags>();
			///////////////////////////////////////
			// inline code of base.ctor
			this.Quest = quest;
			this.Side = side;
			this.PlayerIndex = playerIndex;
			this.CharacterIndex = TFGame.Characters[this.PlayerIndex];
			this.lastScore = 0;
			this.scoreText = "0";
			this.scoreScale = 1;
			this.livesIcons = new List<Sprite<int>>();
			for (int i = 0; i < quest.Lives[this.PlayerIndex]; i++) {
				Sprite<int> spriteInt = TFGame.SpriteData.GetSpriteInt("Gem" + this.CharacterIndex);
				if (side == Facing.Left) {
					//spriteInt.X = (float)(8 + 10 * i);
				}
				else {
					spriteInt.X = (float)(312 - 10 * i);
				}
				if (this.PlayerIndex < 2) {
					spriteInt.Y = 20;
				} else {
					spriteInt.Y = 312 - 40;
				}
				spriteInt.Play(0, false);
				this.livesIcons.Add(spriteInt);
				base.Add(spriteInt);

			}
			*/
		}

		public override void Render()
		{
			foreach (Sprite<int> current in this.livesIcons) {
				current.DrawOutline(1);
			}
			base.Render();
			float x = TFGame.Font.MeasureString(this.scoreText).X;
			Vector2 loc;
			if (this.Side == Facing.Left) {
				loc = new Vector2(10 + x / 2, 9);
			}
			else {
				loc = new Vector2(310 - x / 2, 9);
			}
			if (this.PlayerIndex > 1)
				loc.Y = 310 - 40;
			Draw.OutlineTextCentered(TFGame.Font, this.scoreText, loc, Player.LightColors[this.CharacterIndex], this.scoreScale);
		}

	}

	public class MyPlayer : Player
	{
		public MyPlayer(int playerIndex, Vector2 position, Allegiance allegiance, Allegiance teamColor, PlayerInventory inventory, Player.HatState hatState, bool frozen = true) : base(playerIndex, position, allegiance, teamColor, inventory, hatState, frozen) {}

		public override bool CanGrabLedge(int a, int b)
		{
			if (((MyMatchVariants)Level.Session.MatchSettings.Variants).NoLedgeGrab)
				return false;
			return base.CanGrabLedge(a, b);
		}

		public override int GetDodgeExitState()
		{
			if (((MyMatchVariants)Level.Session.MatchSettings.Variants).NoDodgeCooldowns) {
				this.DodgeCooldown();
			}
			return base.GetDodgeExitState();

		}

		public override void ShootArrow()
		{
			ArrowTypes[] at = new ArrowTypes[1];
			if (((MyMatchVariants)Level.Session.MatchSettings.Variants).InfiniteArrows) {
				at[0] = this.Arrows.UseArrow();
				this.Arrows.AddArrows(at);
			}
			base.ShootArrow();
			if (((MyMatchVariants)Level.Session.MatchSettings.Variants).InfiniteArrows) {
				this.Arrows.AddArrows(at);
			}

		}

		public override void HurtBouncedOn(int bouncerIndex)
		{
			if (!((MyMatchVariants)Level.Session.MatchSettings.Variants).NoHeadBounce)
				base.HurtBouncedOn(bouncerIndex);
		}
	}
}