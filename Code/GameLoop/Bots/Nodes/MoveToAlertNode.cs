using Tec;

namespace Facepunch;

/// <summary>
/// Moves the bot to a target position using navmesh pathfinding
/// </summary>
public class MoveToAlertNode : BaseBehaviorNode
{
	private readonly float _searchingTime;
	private readonly float _arrivalDistance;
	private readonly bool _faceDirection;
	private bool running;
	private TimeSince timeSinceLastAlertStim;

	public MoveToAlertNode( float searchingTime = 30f, float arrivalDistance = 128f, bool faceDirection = true )
	{
		_searchingTime = searchingTime;
		_arrivalDistance = arrivalDistance;
		_faceDirection = faceDirection;
	}

	protected override NodeResult OnEvaluate( BotContext context )
	{
		if ( running && timeSinceLastAlertStim > _searchingTime )
		{
			running = false;
			context.RemoveData( AIConst.ALERT_POS );
			PrintOverlay.Print("I give up searching!");
			return NodeResult.Failure;
		}
		
		var sounds = context.GetData<List<Vector3>>( AIConst.ALERT_SOUNDS_KEY );

		if ( context.HasData( AIConst.ALERT_POS ) )
		{
			sounds.Add(context.GetData<Vector3>( AIConst.ALERT_POS ) );
		}
		
		var targetPos = sounds.MinBy( s => s.Distance( context.Pawn.WorldPosition ) );

		if ( running && context.HasData( AIConst.ALERT_POS ) && targetPos != context.GetData<Vector3>( AIConst.ALERT_POS ) )
		{
			timeSinceLastAlertStim = 0;
		}
		
		context.SetData( AIConst.ALERT_POS, targetPos );

		if ( !running )
		{
			timeSinceLastAlertStim = 0;
			running = true;
		}
		
		var pawn = context.Pawn;
		var agent = context.MeshAgent;

		if ( !agent.IsValid() )
			return NodeResult.Failure;
		
		// Step agent toward target
		agent.MoveTo( targetPos );

		// Check if we've reached target
		float distSqr = pawn.WorldPosition.DistanceSquared( targetPos );
		if ( distSqr < _arrivalDistance * _arrivalDistance )
		{
			context.RemoveData( AIConst.ALERT_POS );
			return NodeResult.Success;
		}

		// Face movement direction if desired
		if ( _faceDirection && agent.WishVelocity.Length > 0.1f )
		{
			var targetRot = Rotation.LookAt( agent.WishVelocity.Normal );
			pawn.EyeAngles = pawn.EyeAngles.LerpTo( targetRot.Angles(), Time.Delta * 5f );
		}

		return NodeResult.Running;
	}
}
