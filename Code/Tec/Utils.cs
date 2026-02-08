namespace Tec;

public static class Utils
{
	public static Sandbox.DamageInfo CreateDamageInfo( Facepunch.DamageInfo damageInfo )
	{
		// Log.Info($"----------------- CreateDamageInfo {damageInfo.Damage} {damageInfo.Attacker.GameObject} {damageInfo.Inflictor.GameObject}");
		return new Sandbox.DamageInfo(damageInfo.Damage, damageInfo.Attacker.GameObject, damageInfo.Inflictor.GameObject);
	}
	
	public static Sandbox.DamageInfo CreateDamageInfo( Sandbox.DamageInfo damageInfo, float damage)
	{
		return new Sandbox.DamageInfo(damage, damageInfo.Attacker, damageInfo.Weapon);
	}
}
