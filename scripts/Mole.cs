using Godot;
using System;

public partial class Mole : Control
{
  public string Key { get { return _loadedTexture?.Name; } }
  private LoadedTexture _loadedTexture;
  private TextureButton textureButton;
  private AnimationPlayer animationPlayer;
  private TextureRect _texture;
  public bool IsHiding { get; set; }

  public override void _Ready()
  {
    IsHiding = true;

    textureButton = GetNode<TextureButton>("Control/Node2D/Bolloon");
    animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
    textureButton.Pressed += OnButtonPressed;

    _texture = GetNode<TextureRect>("Control/Node2D/Bolloon/TextureRect");
  }

  public void SetUp(LoadedTexture loadedTexture)
  {
    _loadedTexture = loadedTexture;
    if (_texture != null)
    {
      _texture.Texture = _loadedTexture.Texture2D;
    }
  }

  private void OnButtonPressed()
  {
    var singleton = GetNode<WhackAMatchSingleton>("/root/WhackAMatchSingleton");
    singleton.EmitSignal(nameof(singleton.OnMolePressed), this);
  }

  public void ShowMole()
  {
    animationPlayer.Play("show");
    IsHiding = false;
  }

  public void HideMole()
  {
    animationPlayer.Play("hide");
    IsHiding = true;
  }
}
