using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public static class ImageLoader
{
    public static IEnumerable<TextureGroup> LoadFromPath(string path)
    {
        var rootDir = DirAccess.Open(path);

        if (rootDir == null)
        {
            throw new Exception($"directory {path} not found");
        }

        List<TextureGroup> textureGroups = new List<TextureGroup>();

        var directories = rootDir.GetDirectories();
        foreach (var dir in directories)
        {
            var dirPath = path + dir;
            var loadedTextures = DirAccess.Open(dirPath).GetFiles()
                .Where(x => x.Contains(".import"))
                .Select(fileName => new LoadedTexture
                {
                    Name = fileName,
                    Texture2D = ResourceLoader.Load<Texture2D>($"{dirPath}/{fileName}".Replace(".import", ""))
                });
            yield return new TextureGroup
            {
                GroupName = dir,
                Textures = loadedTextures
            };
        }
    }
}