using UnityEditor;
using UnityEngine;

public class Create3DLevel : MonoBehaviour
{

    public int nrCubes;
    public int maxPos;
    public int maxSize;

    [ContextMenu("CreateRandomCubes")]
    public void CreateRandomCubes()
    {
        for (int c=0; c<nrCubes; ++c)
        {
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.position = Random.insideUnitSphere * maxPos;
            cube.transform.localScale = Random.insideUnitSphere * maxSize;
            cube.transform.SetParent(this.transform);
        }
    }

}
