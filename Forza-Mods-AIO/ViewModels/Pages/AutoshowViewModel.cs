using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forza_Mods_AIO.Models;
using static Forza_Mods_AIO.Resources.Cheats;

namespace Forza_Mods_AIO.ViewModels.Pages;

public partial class AutoshowViewModel : ObservableObject
{
    [ObservableProperty]
    private bool _uiElementsEnabled = true;
    
    private static Cheats.ForzaHorizon5.Sql SqlFh5 => GetClass<Cheats.ForzaHorizon5.Sql>();
    private static Cheats.ForzaHorizon4.Sql SqlFh4 => GetClass<Cheats.ForzaHorizon4.Sql>();

    private static readonly string[] FreeUpgradeTables =
    [
        "List_UpgradeAntiSwayFront",
        "List_UpgradeAntiSwayRear",
        "List_UpgradeBrakes",
        "List_UpgradeCarBodyChassisStiffness",
        "List_UpgradeCarBody",
        "List_UpgradeCarBodyTireAspectRatioFront",
        "List_UpgradeCarBodyTireAspectRatioRear",
        "List_UpgradeCarBodyTireWidthFront",
        "List_UpgradeCarBodyTireWidthRear",
        "List_UpgradeCarBodyTrackSpacingFront",
        "List_UpgradeCarBodyTrackSpacingRear",
        "List_UpgradeCarBodyWeight",
        "List_UpgradeDrivetrain",
        "List_UpgradeDrivetrainClutch",
        "List_UpgradeDrivetrainDifferential",
        "List_UpgradeDrivetrainDriveline",
        "List_UpgradeDrivetrainTransmission",
        "List_UpgradeEngine",
        "List_UpgradeEngineCamshaft",
        "List_UpgradeEngineCSC",
        "List_UpgradeEngineDisplacement",
        "List_UpgradeEngineDSC",
        "List_UpgradeEngineExhaust",
        "List_UpgradeEngineFlywheel",
        "List_UpgradeEngineFuelSystem",
        "List_UpgradeEngineIgnition",
        "List_UpgradeEngineIntake",
        "List_UpgradeEngineIntercooler",
        "List_UpgradeEngineManifold",
        "List_UpgradeEngineOilCooling",
        "List_UpgradeEnginePistonsCompression",
        "List_UpgradeEngineRestrictorPlate",
        "List_UpgradeEngineTurboQuad",
        "List_UpgradeEngineTurboSingle",
        "List_UpgradeEngineTurboTwin",
        "List_UpgradeEngineValves",
        "List_UpgradeMotor",
        "List_UpgradeMotorParts",
        "List_UpgradeRimSizeFront",
        "List_UpgradeRimSizeRear",
        "List_UpgradeSpringDamper",
        "List_UpgradeTireCompound",
        "List_UpgradeCarBodyFrontBumper",
        "List_UpgradeCarBodyHood",
        "List_UpgradeCarBodyRearBumper",
        "List_UpgradeCarBodySideSkirt",
        "List_UpgradeRearWing"
    ];

    public string InstallFlagsSql =>
        "UPDATE Data_Car SET IsInstalled = 1 WHERE IsInstalled IS NULL OR IsInstalled <> 1;" +
        "UPDATE Data_Car SET IsPurchased = 1 WHERE IsPurchased IS NULL OR IsPurchased <> 1;" +
        "UPDATE Data_Car SET IsDrivable = 1 WHERE IsDrivable IS NULL OR IsDrivable <> 1;";

    public string FullAutoshowSql =>
        "DROP VIEW IF EXISTS Drivable_Data_Car;" +
        "CREATE VIEW Drivable_Data_Car AS SELECT * FROM Data_Car;" +
        "INSERT INTO Data_Car_Buckets(CarId) SELECT Id FROM Data_Car WHERE Id NOT IN (SELECT CarId FROM Data_Car_Buckets);" +
        "UPDATE Data_Car_Buckets SET CarBucket = 0, BucketHero = 0 WHERE CarBucket IS NULL;" +
        "UPDATE Data_Car SET NotAvailableInAutoshow = 0;";

    public string FreeAllUpgradesSql => string.Concat(FreeUpgradeTables.Select(table => $"UPDATE {table} SET price=0;"));

    public string FreeWheelsSql => "UPDATE List_Wheels SET price=1;";

    public string DriftScoreSql => "UPDATE Tracks SET DriftScoreScalar = 10.0;";

    public string MaxTractionSql =>
        "UPDATE Data_Car SET Traction_Road = 10.0;" +
        "UPDATE List_TireCompound SET WetFrictionModFrictionScale = 5.0, WetOffroadFrictionScale = 5.0;";

    public string TorqueScaleSql => "UPDATE Data_Car SET GameTorqueScale = 2.0;";

    public string DragScaleSql => "UPDATE Data_Car SET GameDragScale = 0.5;";

    [RelayCommand]
    private async Task ExecuteSql(object parameter)
    {
        if (parameter is not string sParam)
        {
            return;
        }
        
        UiElementsEnabled = false;
        await Query(sParam);
        UiElementsEnabled = true;
    }

    private static async Task Query(string command)
    {
        switch (GameVerPlat.GetInstance().Type)
        {
            case GameVerPlat.GameType.Fh4:
            {
                if (!SqlFh4.WereScansSuccessful)
                {
                    await SqlFh4.SqlExecAobScan();
                }

                if (!SqlFh4.WereScansSuccessful)
                {
                    goto SkipQuerying;
                }
        
                await Task.Run(() => SqlFh4.Query(command));
                SkipQuerying:
                break;
            }
            case GameVerPlat.GameType.Fh5:
            {
                if (!SqlFh5.WereScansSuccessful)
                {
                    await SqlFh5.SqlExecAobScan();
                }

                if (!SqlFh5.WereScansSuccessful)
                {
                    goto SkipQuerying;
                }
        
                await Task.Run(() => SqlFh5.Query(command));
                SkipQuerying:
                break;
            }
            case GameVerPlat.GameType.None:
            default:
            {
                throw new IndexOutOfRangeException();
            }
        }
    }
}