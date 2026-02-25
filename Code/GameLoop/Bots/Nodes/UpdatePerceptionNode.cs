using Tec.AI;

namespace Facepunch;

public class UpdatePerceptionNode : BaseBehaviorNode
{
	private readonly float _range;
	private const string ENEMIES_KEY = "visible_enemies";
	private const string TEAMMATES_KEY = "visible_teammates";
	private const string ITEMS_KEY = "visible_items";

	public UpdatePerceptionNode( float range = 2048f )
	{
		_range = range;
	}

	protected override NodeResult OnEvaluate( BotContext context )
	{
		var pawn = context.Pawn;
		var origin = pawn.WorldPosition;
		var rangeSqr = _range * _range;

		var enemies = new List<Pawn>();
		var teammates = new List<Pawn>();
		var items = new List<Component>();

		// Scan all pawns
		foreach ( var otherPawn in pawn.Scene.GetAll<PlayerPawn>() )
		{
			if ( otherPawn == pawn )
				continue;

			if ( !otherPawn.HealthComponent.IsValid() || otherPawn.HealthComponent.Health <= 0 )
				continue;

			if ( otherPawn.WorldPosition.DistanceSquared( origin ) > rangeSqr )
				continue;

			if ( !IsInLineOfSight( pawn, otherPawn ) )
				continue;

			if ( otherPawn.Team != pawn.Team )
				enemies.Add( otherPawn );
			else
				teammates.Add( otherPawn );
		}
		
		// Scan all dropped equipment (or other items)
		foreach ( var pickup in pawn.Scene.GetAll<DroppedEquipment>() )
		{
			if ( pickup.WorldPosition.DistanceSquared( origin ) > rangeSqr )
				continue;

			// optional LOS for items? (if you want)
			// if ( !IsInLineOfSight( pawn, pickup ) ) continue;

			items.Add( pickup );
		}

		context.SetData( ENEMIES_KEY, enemies );
		context.SetData( TEAMMATES_KEY, teammates );
		context.SetData( ITEMS_KEY, items );
		
		
		// Hearing ///////////////////////////////////////////////////////////////////////////////////////
		
		var audioStims = AudioAIManager.Instance.GetAudioStims();
		List<Vector3> sounds = [];

		foreach ( var audioStim in audioStims )
		{
			if ( audioStim.Faction == pawn.Team.ToFaction() )
				continue;
			
			var distSqr = audioStim.Position.DistanceSquared( pawn.Head.WorldPosition );
			if ( distSqr < audioStim.Range * audioStim.Range )
			{
				sounds.Add( audioStim.Position);
			}
		}
		
		context.SetData( AIConst.ALERT_SOUNDS_KEY, sounds );

		//////////////////////////////////////////////////////////////////////////////////////////////////
		
		return (enemies.Count > 0 || teammates.Count > 0 || items.Count > 0 || sounds.Count > 0)
			? NodeResult.Success
			: NodeResult.Failure;
	}

	private bool IsInLineOfSight( Pawn from, Component obj )
	{
		var targetPos = obj is Pawn p ? p.EyePosition : obj.WorldPosition;

		var trace = from.Scene.Trace.Ray( from.EyePosition, targetPos + Vector3.Down * 32f )
			.IgnoreGameObjectHierarchy( from.GameObject.Root )
			.Run();

		return trace.Hit && trace.GameObject.Root == obj.GameObject.Root;
	}
}
