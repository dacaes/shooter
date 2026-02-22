namespace Tec.AI;

public static class AudioAIManager
{
	public static List<AudioStim> audioStims = new();

	public static void Clear()
	{
		audioStims.Clear();
	}
	
	public static void NewAudioStim(AudioStim audioStim)
	{
		if(audioStim.DurationType != AudioStim.AudioStimDurationType.Instant)
		{
			audioStims.Add(audioStim);
		}
		else
		{
			// TODO Make NPCs check instant audio stims when they happened.
		}
	}

	public static List<AudioStim> GetAudioStims()
	{
		List<AudioStim> validStims = [];
		
		//Reverse loop to avoid problems with removing elements.
		for ( int i = audioStims.Count - 1; i >= 0; i-- )
		{
			if ( audioStims[i].DurationType == AudioStim.AudioStimDurationType.Finite &&
			     audioStims[i].TimeSinceTriggered >= audioStims[i].Duration )
				audioStims.RemoveAt( i );
			else
				validStims.Add( audioStims[i] );
		}

		return validStims;
	}
}
