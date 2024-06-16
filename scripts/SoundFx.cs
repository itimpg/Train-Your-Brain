using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class SoundFx : Control
{

	Dictionary<string, AudioStream> _sounds;
	private IEnumerable<AudioStreamPlayer> _audioPlayers;

	public override void _Ready()
	{
		_sounds = new Dictionary<string, AudioStream>
		{
			{ "correct_answer", GD.Load<AudioStream>("res://assets/sound/correct_answer.mp3") },
			{ "correct_all_answer", GD.Load<AudioStream>("res://assets/sound/correct_all_answer.mp3") },
			{ "wrong_answer", GD.Load<AudioStream>("res://assets/sound/wrong_answer.mp3") },
			{ "click", GD.Load<AudioStream>("res://assets/sound/correct_answer.mp3") }
		};

		_audioPlayers = GetChildren()
			.Where(x => x is AudioStreamPlayer)
			.Select(x => (AudioStreamPlayer)x);
	}

	public void Play(string soundName)
	{
		if (!_sounds.ContainsKey(soundName))
		{
			return;
		}
		
		var soundToPlay = _sounds[soundName];
		foreach (var audioPlayer in _audioPlayers)
		{
			if (audioPlayer.Playing)
			{
				continue;
			}

			audioPlayer.Stream = soundToPlay;
			audioPlayer.Play();
		}
	}
}
