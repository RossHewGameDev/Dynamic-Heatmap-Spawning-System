using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class CellMapTesting
{
    CellMapping cellmapping = new CellMapping();

    [UnityTest]
    public IEnumerator AxisPercentage_Returns_Correct_Percentage()
    {
        cellmapping.worldSize = Vector3.one * 10;

        Assert.Greater(cellmapping.AxisPercentage(Vector3.one, "x"), 0.59f);
        Assert.Greater(cellmapping.AxisPercentage(Vector3.one, "y"), 0.59f);
        Assert.Greater(cellmapping.AxisPercentage(Vector3.one, "z"), 0.59f);

        Assert.Less(cellmapping.AxisPercentage(Vector3.one, "x"), 0.61f);
        Assert.Less(cellmapping.AxisPercentage(Vector3.one, "y"), 0.61f);
        Assert.Less(cellmapping.AxisPercentage(Vector3.one, "z"), 0.61f);

        yield return null;
    }

    [UnityTest]
    public IEnumerator AxisPercentage_Throws_Error_With_Wrong_String()
    {
        cellmapping.worldSize = Vector3.one * 10;
        Assert.AreEqual(cellmapping.AxisPercentage(Vector3.one, "p"), 0);
       
        yield return null;
    }
}
