using System.Data.SqlTypes;
using Sandbox.Events;
using Sandbox;
using Facepunch;

namespace Tec;

public class Interactable : Component, IUse
{
	[RequireComponent]
	public Collider Collider { get; set; }

	[Property] public bool CanInteract { get; protected set; } = true;
	
	[Property]
	public Action OnInteract { get; set; }
	
	public GrabAction GetGrabAction()
	{
		return GrabAction.PushButton;
	}

	public UseResult CanUse( PlayerPawn player )
	{
		return CanInteract;
	}

	public void OnUse( PlayerPawn player )
	{
		OnInteract?.Invoke();
	}
}
