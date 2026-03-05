using UnityEngine;

public class RESET : MonoBehaviour
{
    public void RESETDATA()
    {
        FileHandler.Reset(SaverFilenames.ValueFilaname);
        FileHandler.Reset(SaverFilenames.PlacedBuilddingsFilaname);
        FileHandler.Reset(SaverFilenames.LevelFilaname);
        FileHandler.Reset(SaverFilenames.EvengyFilename);
        FileHandler.Reset(SaverFilenames.MissionsFilename);
        FileHandler.Reset(SaverFilenames.PlotFilaname);
        FileHandler.Reset(SaverFilenames.MusicSettingsFilaname);
        FileHandler.Reset(SaverFilenames.SFXSettingsFilaname);
        Debug.Log("All saves have been deleted.");
    }
}
