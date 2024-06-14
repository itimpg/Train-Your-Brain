using System.Collections.Generic;
using Godot;

public class TextureGroup
{
    public string GroupName { get; set; }
    public IEnumerable<LoadedTexture> Textures { get; set; }
}

public class LoadedTexture
{
    public string Name { get; set; }
    public Texture2D Texture2D { get; set; }
}