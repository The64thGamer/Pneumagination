using Godot;
using System.Collections.Generic;
using System.Linq;
using Console = media.Laura.SofiaConsole.Console;
using MemoryPack;

public partial class FileSaver : Node
{
	//Locals
	Godot.Collections.Dictionary<string, Variant> loadedWorldData;

	string loadedFolderPath;
	float autoSaveTimer;


	//Consts
	public const string savePath = "user://Your Precious Save Files/";
	public const string worldSaveDataFile = "World Save Data";
	public const string regionPath = "/Chunks/Region ";
	public const string regionExtension = ".pneuChunks";
	public const int regionSize = 16;
		
	public override void _Ready()
	{    			
		autoSaveTimer =  Mathf.Max(1,PlayerPrefs.GetFloat("Autosave Timer")) * 60;
	}

	public override void _Process(double delta)
	{
		//Autosaving

			autoSaveTimer -= Mathf.Min((float)delta,0.2f);
			if(autoSaveTimer <= 0)
			{
				GD.Print("Autosave Incoming");
				autoSaveTimer = Mathf.Max(1,PlayerPrefs.GetFloat("Autosave Timer")) * 60;
				//SaveAllChunks();
				Console.Instance.Print("Autosave! " + System.DateTime.Now.ToUniversalTime().ToString(@"MM\/dd\/yyyy h\:mm tt"),Console.PrintType.Success);
			}
	}

	public void CreateNewSaveFile(Godot.Collections.Dictionary<string, Variant> data, bool alsoLoadFile)
	{
		//Ensure saves folder exists
		if(!DirAccess.DirExistsAbsolute(savePath))
		{
			DirAccess.MakeDirAbsolute(savePath);
		}

		//Create random folder name
		string hashFolderName;
		while(true)
		{
			hashFolderName = RandomString(32);
			if(!DirAccess.DirExistsAbsolute(savePath + hashFolderName))
			{
				break;
			}
			Console.Instance.Print("One in a 218,169,540,588,403,680 chance you got the same random folder name, freak!", Console.PrintType.Error);
		}
		DirAccess.MakeDirAbsolute(savePath + hashFolderName);

		//Create 
		FileAccess.Open(savePath + hashFolderName + "/" + worldSaveDataFile, FileAccess.ModeFlags.Write).StoreString(Json.Stringify(data));
		if(alsoLoadFile)
		{
			loadedWorldData = data;
			loadedFolderPath = hashFolderName;
		}
		Console.Instance.Print("Saved World File to :" + savePath + hashFolderName, Console.PrintType.Success);
	}

	//folderPath is just the random string folder name.
	public bool LoadNewSaveFile(string folderPath)
	{
		Godot.Collections.Dictionary<string, Variant> data = AcquireSaveData(folderPath);
		if(data == null)
		{
			return false;
		}
		loadedWorldData = data;
		loadedFolderPath = folderPath;
		Console.Instance.Print("Save File Loaded :" + savePath + folderPath, Console.PrintType.Success);
		return true;
	}

	public Godot.Collections.Dictionary<string, Variant> AcquireSaveData(string folderPath)
	{
		if(!DirAccess.DirExistsAbsolute(savePath))
		{
			Console.Instance.Print("No parent save folder found", Console.PrintType.Error);
			return null;
		}
		if (!DirAccess.DirExistsAbsolute(savePath + folderPath))
		{
			Console.Instance.Print("Given save folder name not found", Console.PrintType.Error);
			return null;
		}
		if (!FileAccess.FileExists(savePath + folderPath + "/" + worldSaveDataFile))
		{
			Console.Instance.Print("Save folder found but no World Data file found (?????)", Console.PrintType.Error);
			return null;
		}
    	FileAccess saveGame = FileAccess.Open(savePath + folderPath + "/" + worldSaveDataFile, FileAccess.ModeFlags.Read);
        Json json = new Json();
        Error parseResult = json.Parse(saveGame.GetLine());
        if (parseResult != Error.Ok)
        {
            Console.Instance.Print($"JSON Parse Error: {json.GetErrorMessage()} at line {json.GetErrorLine()}", Console.PrintType.Error);
			return null;
        }

        return new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);
	}

	public string GetSeed()
	{
		if(loadedWorldData.TryGetValue("World Seed",out Variant value))
		{
			return value.ToString();
		}
		return "";
	}

	#region pure functions
	string RandomString(int length)
	{
		System.Random random = new System.Random();
		const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		return new string(Enumerable.Repeat(chars, length)
			.Select(s => s[random.Next(s.Length)]).ToArray());
	}
	int mod(int x, int m)
	{
		int r = x % m;
		return r < 0 ? r + m : r;
	}

	#endregion
}