using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class SpawningSysChecks
{
    SpawningSystem spawningSystem = new SpawningSystem();
    PlantSpawnLocation plantSpawnLocation = new PlantSpawnLocation();

    Cell inputCell;

    [UnityTest]
    public IEnumerator Spawning_System_Has_Been_Given_Type()
    {

        //Checking a system type has been selected
        Assert.IsNotNull(spawningSystem.systemTypeRunning);

        yield return null;
    }

    [UnityTest]
    public IEnumerator Spawning_Solver_Is_Called_And_Completed()
    {

        //Checking Spawn solver can function with a cell input
        Assert.IsNotNull(spawningSystem.SpawnSolver(inputCell));

        yield return null;
    }

    [UnityTest]
    public IEnumerator Wait_Time_Delay_Test()
    {

        //Confirm that wait time delay reports a time to wait and that it is greater than the first input.
        Assert.IsNotNull(spawningSystem.DynamicWaitTimeDelay());
        Assert.GreaterOrEqual(spawningSystem.DynamicWaitTimeDelay(), 0.2f);
        yield return null;
    }

}
