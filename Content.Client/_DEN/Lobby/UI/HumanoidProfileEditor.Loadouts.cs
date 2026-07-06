namespace Content.Client.Lobby.UI;

public sealed partial class HumanoidProfileEditor
{
    private void RefreshDenLoadouts()
    {
        DenLoadouts.OnPreferenceUpdated += profile =>
        {
            Profile = profile;
            SetDirty();
            RefreshDenLoadouts();
        };

        DenLoadouts.SetProfile(Profile);
    }
}
