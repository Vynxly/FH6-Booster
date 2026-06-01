using System.Windows.Threading;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Forza_Mods_AIO.Models;
using static Forza_Mods_AIO.Resources.Cheats;

namespace Forza_Mods_AIO.ViewModels.Pages;

public partial class AutoshowViewModel : ObservableObject
{
    private const string FullAutoshowSql =
        "CREATE TABLE IF NOT EXISTS AutoshowTable AS SELECT * FROM Data_Car;" +
        "UPDATE Data_Car SET NotAvailableInAutoshow = 0;" +
        "DROP VIEW IF EXISTS Drivable_Data_Car;" +
        "CREATE VIEW Drivable_Data_Car AS SELECT * FROM Data_Car;" +
        "CREATE TABLE IF NOT EXISTS BucketsOriginal AS SELECT * FROM Data_Car_Buckets;" +
        "INSERT INTO Data_Car_Buckets(CarId) SELECT Id FROM Data_Car WHERE Id NOT IN (SELECT CarId FROM Data_Car_Buckets);" +
        "UPDATE Data_Car_Buckets SET CarBucket=0, BucketHero=0 WHERE CarBucket IS NULL;";

    private const string FreeCarsSql =
        "CREATE TABLE IF NOT EXISTS CostTable(Id INT, BaseCost INT);" +
        "INSERT INTO CostTable(Id, BaseCost) SELECT Id, BaseCost FROM Data_Car WHERE Id NOT IN (SELECT Id FROM CostTable);" +
        "UPDATE Data_Car SET BaseCost = 0;";

    private const string InstallFlagsSql =
        "UPDATE ContentOffersMapping SET IsAutoRedeem = 1, IsPromo = 0, Quantity = 1 WHERE ContentType = 1;" +
        "INSERT INTO OnDiscContent (ContentId, ContentType, IsInstalled, ReleaseOrder) " +
        "SELECT Id, 1, 1, COALESCE((SELECT MAX(ReleaseOrder) FROM OnDiscContent WHERE ContentType = 1), 0) FROM Data_Car WHERE Id NOT IN (SELECT ContentId FROM OnDiscContent WHERE ContentType = 1);" +
        "UPDATE OnDiscContent SET IsInstalled = 1 WHERE ContentType = 1;" +
        "UPDATE Profile0_FreeCars SET FreeCount = 1;";

    private const string AddAllCarsSql =
        "INSERT INTO ContentOffersMapping (OfferId, ContentId, ContentType, IsPromo, IsAutoRedeem, ReleaseDateUTC, Quantity) " +
        "SELECT 99, Id, 1, 0, 1, NULL, 1 FROM Data_Car WHERE Id NOT IN (SELECT ContentId AS Id FROM ContentOffersMapping WHERE ContentId IS NOT NULL) AND Id != 3300 AND Id NOT IN (SELECT CarId FROM Profile0_Career_Garage);" +
        "INSERT INTO Profile0_FreeCars SELECT Id, 1 FROM Data_Car WHERE Id NOT IN (SELECT CarId AS Id FROM Profile0_FreeCars WHERE CarID IS NOT NULL);" +
        "UPDATE Profile0_FreeCars SET FreeCount = 1;";

    private const string UnlockUpgradePresetsSql =
        "CREATE TABLE IF NOT EXISTS UpgradePresetPackagesOrig AS SELECT * FROM UpgradePresetPackages;" +
        "UPDATE UpgradePresetPackages SET Purchasable = 1 WHERE Purchasable = 0;";

    private const string ClearNewTagsSql =
        "UPDATE Profile0_Career_Garage SET HasCurrentOwnerViewedCar = 1;";

    private const string PhysicsPerformanceSql =
        "UPDATE Data_CarPerformance SET DriftScoreMultiplier = 10 WHERE EXISTS (SELECT 1 FROM pragma_table_info('Data_CarPerformance') WHERE name = 'DriftScoreMultiplier');" +
        "UPDATE Data_CarPerformance SET TractionScale = 999 WHERE EXISTS (SELECT 1 FROM pragma_table_info('Data_CarPerformance') WHERE name = 'TractionScale');" +
        "UPDATE Data_CarPerformance SET TorqueScale = TorqueScale * 2 WHERE EXISTS (SELECT 1 FROM pragma_table_info('Data_CarPerformance') WHERE name = 'TorqueScale');" +
        "UPDATE Data_CarPerformance SET DragCoefficient = DragCoefficient * 0.5 WHERE EXISTS (SELECT 1 FROM pragma_table_info('Data_CarPerformance') WHERE name = 'DragCoefficient');";

    private static readonly string FreeUpgradesSql = string.Concat(
        GetFreeUpgradeSql("List_UpgradeAntiSwayFront"),
        GetFreeUpgradeSql("List_UpgradeAntiSwayRear"),
        GetFreeUpgradeSql("List_UpgradeBrakes"),
        GetFreeUpgradeSql("List_UpgradeCarBody"),
        GetFreeUpgradeSql("List_UpgradeCarBodyChassisStiffness"),
        GetFreeUpgradeSql("List_UpgradeCarBodyFrontBumper"),
        GetFreeUpgradeSql("List_UpgradeCarBodyHood"),
        GetFreeUpgradeSql("List_UpgradeCarBodyRearBumper"),
        GetFreeUpgradeSql("List_UpgradeCarBodySideSkirt"),
        GetFreeUpgradeSql("List_UpgradeCarBodyTireAspectRatioFront"),
        GetFreeUpgradeSql("List_UpgradeCarBodyTireAspectRatioRear"),
        GetFreeUpgradeSql("List_UpgradeCarBodyTireWidthFront"),
        GetFreeUpgradeSql("List_UpgradeCarBodyTireWidthRear"),
        GetFreeUpgradeSql("List_UpgradeCarBodyTrackSpacingFront"),
        GetFreeUpgradeSql("List_UpgradeCarBodyTrackSpacingRear"),
        GetFreeUpgradeSql("List_UpgradeCarBodyWeight"),
        GetFreeUpgradeSql("List_UpgradeDrivetrain"),
        GetFreeUpgradeSql("List_UpgradeDrivetrainClutch"),
        GetFreeUpgradeSql("List_UpgradeDrivetrainDifferential"),
        GetFreeUpgradeSql("List_UpgradeDrivetrainDriveline"),
        GetFreeUpgradeSql("List_UpgradeDrivetrainTransmission"),
        GetFreeUpgradeSql("List_UpgradeEngine"),
        GetFreeUpgradeSql("List_UpgradeEngineCamshaft"),
        GetFreeUpgradeSql("List_UpgradeEngineCSC"),
        GetFreeUpgradeSql("List_UpgradeEngineDisplacement"),
        GetFreeUpgradeSql("List_UpgradeEngineDSC"),
        GetFreeUpgradeSql("List_UpgradeEngineExhaust"),
        GetFreeUpgradeSql("List_UpgradeEngineFlywheel"),
        GetFreeUpgradeSql("List_UpgradeEngineFuelSystem"),
        GetFreeUpgradeSql("List_UpgradeEngineIgnition"),
        GetFreeUpgradeSql("List_UpgradeEngineIntake"),
        GetFreeUpgradeSql("List_UpgradeEngineIntercooler"),
        GetFreeUpgradeSql("List_UpgradeEngineManifold"),
        GetFreeUpgradeSql("List_UpgradeEngineOilCooling"),
        GetFreeUpgradeSql("List_UpgradeEnginePistonsCompression"),
        GetFreeUpgradeSql("List_UpgradeEngineRestrictorPlate"),
        GetFreeUpgradeSql("List_UpgradeEngineTurboQuad"),
        GetFreeUpgradeSql("List_UpgradeEngineTurboSingle"),
        GetFreeUpgradeSql("List_UpgradeEngineTurboTwin"),
        GetFreeUpgradeSql("List_UpgradeEngineValves"),
        GetFreeUpgradeSql("List_UpgradeMotor"),
        GetFreeUpgradeSql("List_UpgradeMotorParts"),
        GetFreeUpgradeSql("List_UpgradeRearWing"),
        GetFreeUpgradeSql("List_UpgradeRimSizeFront"),
        GetFreeUpgradeSql("List_UpgradeRimSizeRear"),
        GetFreeUpgradeSql("List_UpgradeSpringDamper"),
        GetFreeUpgradeSql("List_UpgradeTireCompound"));

    private const string FreeWheelsSql = "UPDATE List_Wheels SET Price = 0;";

    private readonly DispatcherTimer _persistentLocksTimer;

    [ObservableProperty]
    private bool _uiElementsEnabled = true;

    [ObservableProperty]
    private bool _persistentLocksEnabled;
    
    private static Cheats.ForzaHorizon5.Sql SqlFh5 => GetClass<Cheats.ForzaHorizon5.Sql>();
    private static Cheats.ForzaHorizon4.Sql SqlFh4 => GetClass<Cheats.ForzaHorizon4.Sql>();

    public AutoshowViewModel()
    {
        _persistentLocksTimer = new DispatcherTimer
        {
            Interval = TimeSpan.FromSeconds(10)
        };
        _persistentLocksTimer.Tick += async (_, _) => await ApplyPersistentLocks();
    }

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

    [RelayCommand]
    private async Task UnlockEverything()
    {
        UiElementsEnabled = false;

        foreach (var sql in GetUnlockEverythingSqlBatches())
        {
            await Query(sql);
        }

        UiElementsEnabled = true;
    }

    [RelayCommand]
    private async Task TogglePersistentLocks(bool enabled)
    {
        PersistentLocksEnabled = enabled;

        if (enabled)
        {
            UiElementsEnabled = false;
            await ApplyPersistentLocks();
            UiElementsEnabled = true;
            _persistentLocksTimer.Start();
            return;
        }

        _persistentLocksTimer.Stop();
    }

    private static string GetFreeUpgradeSql(string tableName)
    {
        return $"UPDATE {tableName} SET Price = 0;";
    }

    private static IEnumerable<string> GetUnlockEverythingSqlBatches()
    {
        yield return string.Concat(FreeCarsSql, FullAutoshowSql, InstallFlagsSql);
        yield return AddAllCarsSql;
        yield return string.Concat(FreeUpgradesSql, FreeWheelsSql);
        yield return string.Concat(UnlockUpgradePresetsSql, ClearNewTagsSql);
        yield return PhysicsPerformanceSql;
    }

    private async Task ApplyPersistentLocks()
    {
        await Query(string.Concat(FreeCarsSql, FullAutoshowSql, InstallFlagsSql));
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
