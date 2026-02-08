namespace Tec;

public class PropExploder : Component
{
	[Property] public float damageMultiplier { get; set; } = 3f;

	private Component lastValidAttacker = null;
	
	[RequireComponent]
	public Exploder Exploder
	{
		get
		{
			field ??= GetComponent<Exploder>();
			return field;
		}
	}
	
	public Prop Prop
	{
		get
		{
			field ??= GetComponent<Prop>();
			return field;
		}
	}
	
	protected override void OnStart()
	{
		// Invoke( 4f, ExplodeTest);
		Prop.OnPropBreak += Explode;
		Prop.OnPropTakeDamage += OnPropTakeDamage;
	}

	private void OnPropTakeDamage( DamageInfo damage )
	{
		// Log.Info( $"->>>>>>>>>>>>>>>>>>>>>>>>>> {damage.ToString()}" );
		var attacker = damage.Attacker?.GetComponent<Component>();
		if ( attacker != null ) lastValidAttacker = attacker;
	}
	
	private void Explode()
	{
		var attacker = Prop.LastAttacker?.GetComponent<Component>();
		
		// Log.Info( $"Log attacker -------------------> {Prop.LastAttacker?.GetComponent<Component>()}" );
		
		Exploder.Explode( WorldPosition, Prop.Model.Data.ExplosionRadius, Prop.Model.Data.ExplosionDamage * damageMultiplier,
			attacker ?? lastValidAttacker, this, Exploder.DamageFalloff );
	}
}
