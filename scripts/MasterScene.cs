using Godot;
using System;
using System.Threading.Tasks;

public partial class MasterScene : Node2D
{
	private WhackAMatchSingleton _singleton;

	private TitleScene _titleScene;
	private GameScene _gameScene;
	private ShowScoreScene _showScoreScene;

	public override void _Ready()
	{
		_singleton = GetNode<WhackAMatchSingleton>("/root/WhackAMatchSingleton");
		_singleton.GotoGameScene += GoToGameScene;
		_singleton.GoToTitleScene += GoToTitleScene;
		_singleton.GoToShowScoreScene += GoToShowScoreScene;

		_titleScene = GetNode<TitleScene>("TitleScene");
		_gameScene = GetNode<GameScene>("GameScene");
		_showScoreScene = GetNode<ShowScoreScene>("ShowScoreScene");

		GoToTitleScene();
	}

	private void GoToTitleScene()
	{
		_titleScene.Show();
		_gameScene.Hide();
		_showScoreScene.Hide();
	}

	private void GoToGameScene()
	{
		_titleScene.Hide();
		_showScoreScene.Hide();

		_gameScene.Show();
		_gameScene.ShowGameScene();
	}

	private void GoToShowScoreScene(int score)
	{
		_titleScene.Hide();
		_gameScene.Hide();

		_showScoreScene.Show();
		_showScoreScene.ShowScore(score);
	}
}
