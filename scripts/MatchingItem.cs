using Godot;
using System;
using System.Threading.Tasks;

public partial class MatchingItem : Control
{
	public string Key { get { return LoadedTexture?.Name; } }
	public LoadedTexture LoadedTexture { get; set; }

	private TextureRect _texture;
	private AnimationPlayer _animationPlayer;

	public override async void _Ready()
	{
		_texture = GetNode<TextureRect>("TextureRect");
		if (LoadedTexture != null)
		{
			_texture.Texture = LoadedTexture.Texture2D;
		}
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		await Blink();
	}

	public async Task Blink()
	{
		_animationPlayer.Play("blink");
		await Task.Delay(600);
	}

	public void Init(LoadedTexture loadedTexture)
	{
		LoadedTexture = loadedTexture;
		AddToGroup("MatchTarget");
	}

	public void HideImage()
	{
		_texture.Hide();
	}
}
