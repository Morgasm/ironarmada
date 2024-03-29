﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ShipFunctions : MonoBehaviour
{
	float gridSize = 0.16f;

	public Ship getShip( int shipID )
	{		
		GameInfo gameInfo = GameObject.Find("GameManager").GetComponent<GameInfo>();
		Ship[] ships = gameInfo.AllShips;
		if ( ships.Length > 0 )		//if (gameInfo.Get<Ship[]>( "ships", out ships ))	must create a global dictionary to use something similar to rules.get(), GameInfo.get()?
		{
			if (shipID > 0 && shipID <= ships.Length)
			{
				return ships[shipID-1];
			}
		}
		return null;
	}


	public Ship getShip( GameObject myGO )
	{
		Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(myGO.transform.position, 1.0f );
		if ( collidersInRadius.Length > 0 )
		{
			for (uint i = 0; i < collidersInRadius.Length; i++)
			{
				Collider2D coll2D = collidersInRadius[i];
				//coll2D.gameObject.GetComponent<Component_Color>().getColor();		need something like this to get ship part colors later on
				Part_Info partInfo = coll2D.gameObject.GetComponent<Part_Info>();
				if (partInfo != null)
				{
					int shipID = partInfo.ShipID;
					if (shipID > 0)
					{
						return getShip(shipID);
					}
				}
			}
		}
		return null;
	}
		
	public GameObject getShipGO( GameObject myGO )
	{
		GameObject closestGO = null;
		float mDist = 9999;
		Collider2D[] collidersInRadius = Physics2D.OverlapCircleAll(myGO.transform.position, 1.0f ); 
		int numCollidersInRadius = collidersInRadius.Length;
		if ( numCollidersInRadius > 0 )	//custom getIslandBlob();
		{
			for (uint i = 0; i < numCollidersInRadius; i++)
			{
				GameObject gOIter = collidersInRadius[i].gameObject;
				if (gOIter.GetComponent<Part_Info>().ShipID > 0)
				{
					float dist = Vector2.Distance( gOIter.transform.position, myGO.transform.position );
						//this.getDistanceTo( blobsInRadius[i] );
					if ( dist < mDist )
					{
						closestGO = gOIter;
						mDist = dist;
					}
				}
			}
		}
		return closestGO;
	}

	public Vector2 RelSnapToGrid( Vector2 pos )
	{
		float halfGridSize = gridSize/2.0f;

		pos.x = Mathf.Round( pos.x / gridSize);
		pos.y = Mathf.Round( pos.y / gridSize);

		pos.x *= gridSize;
		pos.y *= gridSize;
		return pos;
	}

	public void SetNextId( GameInfo gameInfo, Ship ship )
	{
		//islands.id = this.get_u32("islands id")+1;
		//this.set_u32("islands id", islands.id);
		ship.id = gameInfo.ShipsID+1;
		gameInfo.ShipsID = ship.id;
	}

	public GameObject getMothership( ushort team )
	{
		GameObject[] mothershipParts = GameObject.FindGameObjectsWithTag("mothership");
		for (uint i=0; i < mothershipParts.Length; i++)
		{
			GameObject mSPart = mothershipParts[i];  
			if (mSPart.GetComponent<Generic_Team>().TeamNum == team)
			{
				return mSPart;
			}
		}
		return null;
	}

	public GameObject getMothership( GameObject myGO )
	{
		GameObject core = null;
		int shipID = myGO.GetComponent<Part_Info>().ShipID;
		if ( shipID == 0 ) 
			return core;

		GameObject[] cores = GameObject.FindGameObjectsWithTag("mothership");

		for ( int i = 0; i < cores.Length; i++ )
		{
			if ( cores[i].GetComponent<Part_Info>().ShipID == shipID )
			{
				core = cores[i];
			}
		}

		return core;
	}

	public bool isMothership( GameObject myGO )
	{
		int shipID = myGO.GetComponent<Part_Info>().ShipID;
		if ( shipID == 0 ) 
			return false;

		Ship ship = getShip( shipID );
		if ( ship != null )
			return ship.isMothership;
		else
			return false;
	}

	public string getCaptainName( ushort team )
	{
		GameObject[] cores = GameObject.FindGameObjectsWithTag("mothership");
		for ( ushort i = 0; i < cores.Length; i++ )
		{
			if ( cores[i].GetComponent<Generic_Team>().TeamNum != team )
				continue;

			Ship ship = getShip( cores[i].GetComponent<Part_Info>().ShipID );
			if ( ship != null && ship.owner != "" )
				return ship.owner;
		}
		return "";
	}

	public bool partsOverlappingShip( List<GameObject> parts )
	{
		bool result = false;
		for (int i = 0; i < parts.Count; ++i)
		{
			GameObject part = parts[i];
			if (partOverlappingShip( part ))
				result = true;
		}
		return result; 
	}

	public bool partOverlappingShip( GameObject part )
	{
		Collider2D[] overlappingColliders = Physics2D.OverlapCircleAll(part.transform.position, 8.0f );;
		if ( overlappingColliders.Length > 0 )
		{
			for (uint i = 0; i < overlappingColliders.Length; i++)
			{
				GameObject gO = overlappingColliders[i].gameObject;
				Part_Info partInfo = gO.GetComponent<Part_Info>();
				if ( partInfo == null )
					continue;

				int shipID = partInfo.ShipID;
				if (shipID > 0)
				{
					if ( Vector2.Distance( part.transform.position, gO.transform.position ) < part.GetComponent<BoxCollider2D>().size.x*0.4f )
						return true;
				}
			}
		}
		return false;
	}
}
