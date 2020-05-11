﻿/*
 *
 *	Adventure Creator
 *	by Chris Burton, 2013-2020
 *	
 *	"ActionSpeechWait.cs"
 * 
 *	This Action waits until a particular character has stopped speaking.
 * 
 */

using UnityEngine;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AC
{
	
	[System.Serializable]
	public class ActionSpeechWait : Action
	{

		public int constantID = 0;
		public int parameterID = -1;

		public bool isPlayer;
		public int playerID = -1;
		public int playerParameterID = -1;

		public Char speaker;
		protected Char runtimeSpeaker;


		public ActionSpeechWait ()
		{
			this.isDisplayed = true;
			category = ActionCategory.Dialogue;
			title = "Wait for speech";
			description = "Waits until a particular character has stopped speaking.";
		}
		
		
		public override void AssignValues (List<ActionParameter> parameters)
		{
			if (isPlayer)
			{
				runtimeSpeaker = AssignPlayer (playerID, parameters, playerParameterID);
			}
			else
			{
				runtimeSpeaker = AssignFile<Char> (parameters, parameterID, constantID, speaker);
			}
		}


		public override float Run ()
		{
			if (runtimeSpeaker == null)
			{
				LogWarning ("No speaker set.");
			}
			else if (!isRunning)
			{
				isRunning = true;

				if (KickStarter.dialog.CharacterIsSpeaking (runtimeSpeaker))
				{
					return defaultPauseTime;
				}
			}
			else
			{
				if (KickStarter.dialog.CharacterIsSpeaking (runtimeSpeaker))
				{
					return defaultPauseTime;
				}
				else
				{
					isRunning = false;
				}
			}
			
			return 0f;
		}


		public override void Skip ()
		{
			return;
		}
		
		
		#if UNITY_EDITOR
		
		public override void ShowGUI (List<ActionParameter> parameters)
		{
			isPlayer = EditorGUILayout.Toggle ("Player line?",isPlayer);
			if (isPlayer)
			{
				if (KickStarter.settingsManager != null && KickStarter.settingsManager.playerSwitching == PlayerSwitching.Allow)
				{
					playerParameterID = ChooseParameterGUI ("Player ID:", parameters, playerParameterID, ParameterType.Integer);
					if (playerParameterID < 0)
						playerID = ChoosePlayerGUI (playerID, true);
				}
			}
			else
			{
				parameterID = Action.ChooseParameterGUI ("Speaker:", parameters, parameterID, ParameterType.GameObject);
				if (parameterID >= 0)
				{
					constantID = 0;
					speaker = null;
				}
				else
				{
					speaker = (Char) EditorGUILayout.ObjectField ("Speaker:", speaker, typeof(Char), true);
					
					constantID = FieldToID <Char> (speaker, constantID);
					speaker = IDToField <Char> (speaker, constantID, false);
				}
			}
			
			AfterRunningOption ();
		}


		public override void AssignConstantIDs (bool saveScriptsToo, bool fromAssetFile)
		{
			if (!isPlayer)
			{
				AssignConstantID<Char> (speaker, constantID, parameterID);
			}
		}
		
		
		public override string SetLabel ()
		{
			if (parameterID == -1)
			{
				if (isPlayer)
				{
					return "Player";
				}
				else if (speaker != null)
				{
					return speaker.gameObject.name;
				}
			}
			return string.Empty;
		}


		/**
		 * <summary>Creates a new instance of the 'Dialogue: Wait for speech' Action</summary>
		 * <param name = "speakingCharacter">The speaking character to wait for</param>
		 * <returns>The generated Action</returns>
		 */
		public static ActionSpeechWait CreateNew (AC.Char speakingCharacter)
		{
			ActionSpeechWait newAction = (ActionSpeechWait) CreateInstance <ActionSpeechWait>();
			newAction.speaker = speakingCharacter;
			return newAction;
		}


		public override bool ReferencesObjectOrID (GameObject gameObject, int id)
		{
			if (!isPlayer && parameterID < 0)
			{
				if (speaker != null && speaker.gameObject == gameObject) return true;
				if (constantID == id) return true;
			}
			if (isPlayer && gameObject.GetComponent <Player>()) return true;
			return false;
		}


		public override bool ReferencesPlayer (int _playerID = -1)
		{
			if (!isPlayer) return false;
			if (_playerID < 0) return true;
			if (playerID < 0 && playerParameterID < 0) return true;
			return (playerParameterID < 0 && playerID == _playerID);
		}

		#endif

	}
	
}