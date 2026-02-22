using Tec;

namespace Facepunch;

public class AwareBehavior : BaseBotBehavior
{
	private IBehaviorNode _behavior;

	public override float Score( BotContext ctx )
	{
		// Use perception data that BotPlayerController already populated
		if ( !ctx.HasData( AIConst.ALERT_SOUNDS_KEY ) && !ctx.HasData( AIConst.ALERT_POS ))
			return 0f;

		var sounds = ctx.GetData<List<Vector3>>( AIConst.ALERT_SOUNDS_KEY );
		
		float baseScore = 50f;
		
		if ( sounds == null || sounds.Count == 0 )
		{
			if(!ctx.HasData( AIConst.ALERT_POS ))
				return 0f;
			
			return baseScore;
		}

		if ( ctx.HasData( AIConst.ALERT_POS ) )
		{
			sounds.Add(ctx.GetData<Vector3>( AIConst.ALERT_POS ) );
		}
		
		// Simple scoring: higher when sounds are closer
		float closestDist = sounds.Min( s => s.Distance( ctx.Pawn.WorldPosition ) );
		float proximityBonus = MathF.Max( 0, 50f * (1f - closestDist / 1000f) );
		
		return baseScore + proximityBonus;
	}

	protected override void OnInitialize()
	{
		// Build behavior tree
		_behavior = new SequenceNode(
			new MoveToAlertNode()
		);
	}

	public override NodeResult Update( BotContext ctx )
	{
		return _behavior.Evaluate( ctx );
	}
}
