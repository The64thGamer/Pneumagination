using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Console = media.Laura.SofiaConsole.Console;
using media.Laura.SofiaConsole;

public partial class ServerClient : Node
{
	public static List<PlayerInfo> playerList = new List<PlayerInfo>();

	ENetMultiplayerPeer peer;

	const int maxPlayers = 256;
	const long hostID = 1;

	Node3D mainPlayer = null;

	public override void _Ready()
	{		
		if(!IsInsideTree())
		{
			return;
		}

		if(PlayerPrefs.GetBool("Joining"))
		{		
			JoinServer(Convert.ToInt32(PlayerPrefs.GetString("Joining Port")),PlayerPrefs.GetString("Joining Address"));
		}
		else
		{		
			CreateServer(Convert.ToInt32(PlayerPrefs.GetString("Hosting Port")));
		}

		Multiplayer.PeerConnected += PeerConnected;
		Multiplayer.PeerDisconnected += PeerDisconnected;
		Multiplayer.ConnectedToServer += ConnectedToServer;
		Multiplayer.ConnectionFailed += ConnectionFailed;
		GetTree().AutoAcceptQuit = false;
	}

	void CreateServer(int port)
	{
		peer = new ENetMultiplayerPeer();
		Error error = peer.CreateServer(port,PlayerPrefs.GetBool("Hosting Online") ? maxPlayers : (PlayerPrefs.GetBool("Hosting Headless") ? 0 : 1));
		if(error != Error.Ok)
		{
			GD.Print("Hosting Failed: " + error.ToString());
			return;
		}
		peer.Host.Compress(ENetConnection.CompressionMode.Zlib);
		Multiplayer.MultiplayerPeer = peer;

		GD.Print("Hosting Started");


		if(PlayerPrefs.GetBool("Hosting Headless"))
		{
			Console.Instance.ToggleConsole();
		}
		else
		{
			SendPlayerInfo(hostID, PlayerPrefs.GetString("Name"));
		}
	}

	void JoinServer(int port, string address)
	{
		peer = new ENetMultiplayerPeer();
		peer.CreateClient(address,port);

		peer.Host.Compress(ENetConnection.CompressionMode.Zlib);
		Multiplayer.MultiplayerPeer = peer;

		GD.Print("Joining Started");
	}

     void ConnectionFailed()
    {
		GD.Print("CONNECTION FAILED");
		GetTree().ChangeSceneToFile("res://Scenes/Menu.tscn");
    }

     void ConnectedToServer()
    {
        GD.Print("Connected To Server");

		RpcId(hostID, nameof(SendPlayerInfo),Multiplayer.GetUniqueId(), PlayerPrefs.GetString("Name"));
    }

     void PeerDisconnected(long id)
    {
        GD.Print("Player Disconnected: " + id.ToString());
    }


     void PeerConnected(long id)
    {
        GD.Print("Player Connected! " + id.ToString());
    }

	public Node3D GetMainPlayer()
	{
		return mainPlayer;
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	void SendPlayerInfo(long id, string name)
	{	
		if(!CheckHostCalledThisRPC(nameof(SendPlayerInfo)))
		{
			return;
		}

		//Check for duplicate call
		for(int i = 0; i < playerList.Count; i++)
		{
			if(playerList[i].id == id)
			{
				Console.Instance.Print("Duplicate player information was sent to you. (ID " + id + ")");
				return;
			}
		}

		PlayerInfo info = new PlayerInfo(){id = id, name = name};
		//Add player if not headless server
		if(!Multiplayer.IsServer() || (Multiplayer.IsServer() && !PlayerPrefs.GetBool("Hosting Headless")))
		{
			info.playerObject = GD.Load<PackedScene>("res://Prefabs/Player.tscn").Instantiate() as Node3D;
			GetTree().Root.AddChild(info.playerObject);
		}

		playerList.Add(info);
		TestMainPlayer();
		Console.Instance.Print("Player " + name + " (ID " + id + ") Connected.");

		//Send info
		if(Multiplayer.IsServer())
		{				
			RpcCallOnlyClientPlayerIDs(nameof(SendPlayerInfo), id, name);

			foreach (PlayerInfo item in playerList)
			{			
				if(item.id != id)
				{
					RpcId(id, nameof(SendPlayerInfo), item.id, item.name);
				}
			}

			Console.Instance.Print("Synched all clients to new Player " + name + " (ID " + id + ") that has connected.");
		}
	}

	void TestMainPlayer()
	{				
		if(mainPlayer != null)
		{
			return;
		}
		for(int i = 0; i < playerList.Count; i++)
		{
			if(playerList[i].id == Multiplayer.GetUniqueId())
			{
				mainPlayer = playerList[i].playerObject;
				return;
			}
		}
	}

	[Rpc(MultiplayerApi.RpcMode.Authority)]
	void DisconnectPlayer(long id)
	{	 
		if(!CheckHostCalledThisRPC(nameof(SendPlayerInfo)))
		{
			return;
		}

		//Check for missing player
		bool check = false;
		for(int i = 0; i < playerList.Count; i++)
		{
			if(playerList[i].id == Multiplayer.GetRemoteSenderId())
			{
				check = true;
				Console.Instance.Print("Player " + playerList[i].name + " (ID " + playerList[i].id + ") Disconnected.");
				playerList.RemoveAt(i);
				break;
			}
		}
		if(!check)
		{
			Console.Instance.Print("Server sent player disconnect request of: (ID " + Multiplayer.GetRemoteSenderId() + "). ID was not found in playerlist.");
		}
	}

	[Rpc(MultiplayerApi.RpcMode.AnyPeer)]
	void PingServerClientIsDisconnecting()
	{	 
		if(!CheckClientIsCallingHost())
		{
			return;
		}

		//Check for missing player
		bool check = false;
		for(int i = 0; i < playerList.Count; i++)
		{
			if(playerList[i].id == Multiplayer.GetRemoteSenderId())
			{
				check = true;
				Console.Instance.Print("Player " + playerList[i].name + " (ID " + playerList[i].id + ") Disconnected.");
				playerList.RemoveAt(i);
				break;
			}
		}
		if(!check)
		{
			Console.Instance.Print("Player (ID " + Multiplayer.GetRemoteSenderId() + ") sent server disconnect request. ID was not found in playerlist.");
		}
		
		RpcCallOnlyClientPlayerIDs(nameof(DisconnectPlayer), Multiplayer.GetRemoteSenderId());
	}

	void RpcCallOnlyClientPlayerIDs(StringName method, params Variant[] args)
	{
		for(int i = 0; i < playerList.Count; i++)
		{
			if(playerList[i].id != hostID)
			{
				RpcId(playerList[i].id, method, args);
			}
		}
	}

	bool CheckHostCalledThisRPC(string methodName)
	{
		//Anyone can call server including server, only server can call client
		if(!Multiplayer.IsServer() && Multiplayer.GetRemoteSenderId() != hostID)
		{			
			Console.Instance.Print("Player (ID " + Multiplayer.GetRemoteSenderId() +  ") attempted to call server authoritative function: " + methodName);
			return false;
		}
		return true;
	}
	
	bool CheckClientIsCallingHost()
	{
		//Anyone can call server including server, only server can call client
		if(Multiplayer.IsServer() && Multiplayer.GetRemoteSenderId() != hostID)
		{			
			return true;
		}
		return false;
	}


	public override void _Notification(int what)
	{	
		//Application Quit
		if (what == NotificationWMCloseRequest)
		{
			if(Multiplayer.IsServer())
			{
				GetTree().Quit();
				return;
			}
			RpcId(hostID,nameof(PingServerClientIsDisconnecting));
			GetTree().Quit();
		}
	}

	[ConsoleCommand("listplayers", Description = "Prints IDs and names of all currently connected players.")]
	void ListPlayers()
	{
		if(!IsInsideTree())
		{
			Console.Instance.Print("Not currently in game");
			return;
		}

		if(playerList.Count == 0)
		{
			Console.Instance.Print("No Players Connected");
		}
		string finalresult = "";
		for(int i = 0; i < playerList.Count; i++)
		{
			finalresult += playerList[index: i].name + " (ID " + playerList[index: i].id.ToString() + ")\n";
		}
		Console.Instance.Print(finalresult);
	}

	[ConsoleCommand("listmaxplayercount", Description = "Prints max players that can join. Singleplayer = 1. Singleplayer + Headless = 0")]
	void ListMaxPlayerCount()
	{
		if(!IsInsideTree())
		{
			Console.Instance.Print("Not currently in game");
			return;
		}
		Console.Instance.Print(PlayerPrefs.GetBool("Hosting Online") ? maxPlayers.ToString() : "1");
	}

	[ConsoleCommand("getmyposition", Description = "Prints position of currently controlled player.")]
	void GetMyPosition()
	{
		if(!IsInsideTree())
		{
			Console.Instance.Print("Not currently in game");
			return;
		}
		if(mainPlayer == null)
		{
			Console.Instance.Print("No player currently controlled");
			return;
		}
		
		Console.Instance.Print("X " + Mathf.FloorToInt(mainPlayer.GlobalPosition.X) +
		 					   "  Y " + Mathf.FloorToInt(mainPlayer.GlobalPosition.Y) +
		  					   "  Z " + Mathf.FloorToInt(mainPlayer.GlobalPosition.Z));
	}
}

public partial class PlayerInfo : GodotObject
{
	public string name;
	public long id;
	public Node3D playerObject;
}