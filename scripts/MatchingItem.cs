using Godot;
using System;
using System.Threading.Tasks;

public partial class MatchingItem : Control
{
	public string Key { get { return LoadedTexture?.Name; } }
	public LoadedTexture LoadedTexture { get; set; }

	private TextureRect _texture;
	private AnimationPlayer _animationPlayer;

	public override void _Ready()
	{
		_texture = GetNode<TextureRect>("TextureRect");
		if (LoadedTexture != null)
		{
			_texture.Texture = LoadedTexture.Texture2D;
		}
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		Blink();
	}

	public Task Blink()
	{
		return PlayAnimationAndWait("blink");
	}

	private async Task PlayAnimationAndWait(string animationName)
	{
		var tcs = new TaskCompletionSource<bool>();

		void OnAnimationFinished(StringName anim)
		{
			if (anim == animationName)
			{
				_animationPlayer.AnimationFinished -= OnAnimationFinished; // Unsubscribe from the signal
				tcs.SetResult(true); // Complete the task
			}
		}

		_animationPlayer.AnimationFinished += OnAnimationFinished;
		_animationPlayer.Play(animationName);

		await tcs.Task;
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
