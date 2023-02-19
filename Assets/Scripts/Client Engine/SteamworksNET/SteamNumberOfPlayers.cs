using Steamworks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteamNumberOfPlayers : MonoBehaviour
{
	private CallResult<NumberOfCurrentPlayers_t> m_NumberOfCurrentPlayers;

	public bool trigger = false;

    private void OnEnable()
	{
		if (SteamManager.Initialized)
		{
			m_NumberOfCurrentPlayers = CallResult<NumberOfCurrentPlayers_t>.Create(OnNumberOfCurrentPlayers);
		}
	}

	private void Update()
	{
		if (trigger)
		{
			trigger = false;
			SteamAPICall_t handle = SteamUserStats.GetNumberOfCurrentPlayers();
			m_NumberOfCurrentPlayers.Set(handle);
			Debug.Log("Called GetNumberOfCurrentPlayers()");
		}
	}

	private void OnNumberOfCurrentPlayers(NumberOfCurrentPlayers_t pCallback, bool bIOFailure)
	{
		if (pCallback.m_bSuccess != 1 || bIOFailure)
		{
			Debug.Log("There was an error retrieving the NumberOfCurrentPlayers.");
		}
		else
		{
			Debug.Log("The number of players playing your game: " + pCallback.m_cPlayers);
		}
	}	
}
