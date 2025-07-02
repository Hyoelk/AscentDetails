using BepInEx;
using BepInEx.Logging;

namespace AscentDetails;

// Here are some basic resources on code style and naming conventions to help
// you in your first CSharp plugin!
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/coding-conventions
// https://learn.microsoft.com/en-us/dotnet/csharp/fundamentals/coding-style/identifier-names
// https://learn.microsoft.com/en-us/dotnet/standard/design-guidelines/names-of-namespaces

// This BepInAutoPlugin attribute comes from the Hamunii.BepInEx.AutoPlugin
// NuGet package, and it will generate the BepInPlugin attribute for you!
// For more info, see https://github.com/Hamunii/BepInEx.AutoPlugin
[BepInAutoPlugin]
public partial class Plugin : BaseUnityPlugin
{
    internal static ManualLogSource Log { get; private set; } = null!;

    private void Awake()
    {
        // BepInEx gives us a logger which we can use to log information.
        // See https://lethal.wiki/dev/fundamentals/logging
        Log = Logger;

        // BepInEx also gives us a config file for easy configuration.
        // See https://lethal.wiki/dev/intermediate/custom-configs

        // We can apply our hooks here.
        // See https://lethal.wiki/dev/fundamentals/patching-code

        // Log our awake here so we can see it in LogOutput.log file
        Log.LogInfo($"Plugin {Name} is loaded!");

        On.BoardingPass.Initialize += BoardingPass_Initialize;
    }

    private void BoardingPass_Initialize(On.BoardingPass.orig_Initialize orig, BoardingPass self)
    {
        int storedAscentIndex = Ascents.currentAscent;

        Ascents.currentAscent = -1;
        self.ascentData.ascents[Ascents.currentAscent + 1].description = "Climbing takes "+ getPositivePercentage(1f - Ascents.climbStaminaMultiplier) +" less stamina.  Hunger grows "+ getPositivePercentage(1f - Ascents.hungerRateMultiplier)+" slower. No time limit.";

        Ascents.currentAscent = 1;
        self.ascentData.ascents[Ascents.currentAscent + 1].description = "Fall damage is " + getNegativePercentage(Ascents.fallDamageMultiplier- 1f) + " greater.";

        Ascents.currentAscent = 2;
        self.ascentData.ascents[Ascents.currentAscent + 1].description = "Hunger grows "+ getNegativePercentage(Ascents.hungerRateMultiplier - 1f) + " faster.";

        Ascents.currentAscent = 3;
        self.ascentData.ascents[Ascents.currentAscent + 1].description = "Items have their weight increased by "+ printAsRed(Ascents.itemWeightModifier.ToString())+", equivalent to an extra apple for every item carried.";

        Ascents.currentAscent = 5;
        float percentage = Ascents.nightColdRate * 100;
        float seconds = 1;
        while (percentage < 1f) {
            percentage *= 10;
            seconds *= 10;
        }
        self.ascentData.ascents[Ascents.currentAscent + 1].description = "While in the night, get " + getNegativePercentage(percentage / 100) + " cold every " + printSeconds(seconds) +".";

        Ascents.currentAscent = 6;
        self.ascentData.ascents[Ascents.currentAscent + 1].description = " Climbing takes additional "+ getNegativePercentage (Ascents.climbStaminaMultiplier - 1f) + " stamina.";
       
        orig(self);
    }

    private string getPositivePercentage(float percentage)
    {
        return printAsGreen((percentage * 100).ToString()) + "%";
    }

    private string getNegativePercentage(float percentage)
    {
        return printAsRed((percentage * 100).ToString()) + "%";
    }

    private string printSeconds(float seconds)
    {
        if (seconds < 1)
            return printAsBlue("second");
        return printAsBlue(seconds + " seconds");
    }

    private string printAsGreen(string text)
    {
        return "<color=#2DD911>" + text + "</color>";
    }

    private string printAsRed(string text)
    {
        return "<color=#EF2E49>" + text + "</color>";
    }

    private string printAsBlue(string text)
    {
        return "<color=#2e49ef>" + text + "</color>";
    }
}
