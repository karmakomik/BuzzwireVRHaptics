using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuzzwireProcGenScript : MonoBehaviour
{
	List<CylinderSlice> cylinderSlices;
	Mesh cylinderMesh;
	MeshFilter cylinderMeshFilter;
	bool showEvaluationGizmos;
	float sliceAngleResolution;

	public Texture2D barTexture;

	//Public properties to be edited in the editor window
	public int numSlices;// = 2;
	public int sliceSideCount;// = 18;
	public float rebarRadius;// = 0.065f;
	public float interSliceDistance;// = 8.19f;


	public enum CylinderFaceType { side, edge };
	// Start is called before the first frame update
	void Start()
    {
		cylinderSlices = new List<CylinderSlice>();

		createPipe();
	}

	void createPipe() 
	{
		cylinderMesh = new Mesh();
		cylinderMeshFilter = GetComponent<MeshFilter>();
		cylinderMeshFilter.mesh = cylinderMesh;

		List<Vector3> cylinderVerticesList = new List<Vector3>();
		List<Vector3> cylinderNormalsList = new List<Vector3>();
		List<Vector2> cylinderUVList = new List<Vector2>();
		List<int> trianglesList = new List<int>();

		showEvaluationGizmos = false;

		sliceAngleResolution = 360 / sliceSideCount;

		for (int i = 0; i < numSlices; i++)
		{
			Vector3 centerLoc = new Vector3((i * interSliceDistance), 0.0f, 0.0f);
			//rebarSkeleton.Add(centerLoc);
			cylinderSlices.Add(new CylinderSlice(centerLoc, rebarRadius, sliceSideCount, i, numSlices, CylinderFaceType.side, 45));
			cylinderVerticesList.AddRange(cylinderSlices[i].verts);
			cylinderNormalsList.AddRange(cylinderSlices[i].normals);
			cylinderUVList.AddRange(cylinderSlices[i].uvs);

			if (i > 0)
			{
				for (int j = 0; j < sliceSideCount; j++)
				{
					//Debug.Log(k);

					/* Now for some conventions
					 *  		*----------*
					 *  		|         /|
					 *  		|  a    /  |	
					 * ith Slice|     /    |  (i+1)th Slice
					 *  		|   /  b   |
					 *  		| /        |
					 *  		*----------*
					 * Here, triangle a points at slice 2 and triangle b has its base in slice 2
					 */

					//(a)Here three triangle indices point at (i+1)th. Each triangle index points to a vertex in the vertex list. 3 triangle indices form one triangle.
					trianglesList.Add(sliceSideCount * (i - 1) + j);
					trianglesList.Add(sliceSideCount * (i - 1) + (j + 1) % sliceSideCount);
					trianglesList.Add(sliceSideCount * (i) + (j + 1) % sliceSideCount);

					//(b) Triangle with bases at ith slice
					trianglesList.Add(sliceSideCount * (i - 1) + j);
					trianglesList.Add(sliceSideCount * (i) + (j + 1) % sliceSideCount);
					trianglesList.Add(sliceSideCount * (i) + j);

				}
			}
		}


		cylinderMesh.vertices = cylinderVerticesList.ToArray();
		cylinderMesh.normals = cylinderNormalsList.ToArray();
		//cylinderMesh.RecalculateNormals();

		cylinderMesh.uv = cylinderUVList.ToArray();
		cylinderMesh.triangles = trianglesList.ToArray();

		//GetComponent<Renderer>().sharedMaterial.mainTexture = barTexture;

	}


	// Update is called once per frame
	void Update()
    {
        
    }
}


/// <summary>
/// Cylinder slice.
/// </summary>
public class CylinderSlice
{
	//static 
	public Vector3[] verts;
	public Vector3[] normals;
	public Vector2[] uvs;
	public float yUVStepFactor;
	public Vector3 centerLoc;
	//int[] tris;
	//sliceSideCount

	/// <summary>
	/// Initializes a new instance of the <see cref="CylinderSlice"/> class.
	/// </summary>
	/// <param name="sliceSideCount">Slice side count.</param>
	public CylinderSlice(int sliceSideCount)
	{
		verts = new Vector3[sliceSideCount];
		normals = new Vector3[sliceSideCount];
		uvs = new Vector2[sliceSideCount];
	}

	/// <summary>
	/// Initializes a new instance of the <see cref="CylinderSlice"/> class.
	/// </summary>
	/// <param name="centerLoc">Center location.</param>
	/// <param name="radius">Radius.</param>
	/// <param name="sliceSideCount">Slice side count.</param>
	/// <param name="currSliceIndex">Curr slice index.</param>
	/// <param name="numSlices">Number slices.</param>
	/// <param name="cylFacetype">Cyl facetype.</param>
	public CylinderSlice(Vector3 centerLoc, float radius, int sliceSideCount, int currSliceIndex, int numSlices, BuzzwireProcGenScript.CylinderFaceType cylFacetype, float bendAngle)
	{
		this.centerLoc = centerLoc;
		float sliceAngleResolution = 360 / sliceSideCount;
		verts = new Vector3[sliceSideCount];
		normals = new Vector3[sliceSideCount];
		uvs = new Vector2[sliceSideCount];
		float resultantBendAngle = bendAngle;

		yUVStepFactor = currSliceIndex / (float)numSlices;

		for (int i = 0; i < sliceSideCount; i++)
		{
			verts[i] = new Vector3();

			float vertX = Mathf.Sin(Mathf.Deg2Rad * (sliceAngleResolution * i - 90)) * Mathf.Sin(Mathf.Deg2Rad * resultantBendAngle);
			verts[i].x = centerLoc.x + radius * vertX;
			float vertY = Mathf.Cos(Mathf.Deg2Rad * (sliceAngleResolution * i - 90));
			verts[i].y = radius * vertY;
			float vertZ = Mathf.Sin(Mathf.Deg2Rad * (sliceAngleResolution * i - 90)) * Mathf.Cos(Mathf.Deg2Rad * resultantBendAngle);
			verts[i].z = radius * vertZ;

			//verts[i] = trans.TransformPoint(verts[i]);
			/*verts[i].x = centerLoc.x;
			//Debug.Log    (sliceAngleResolution * i);
			float vertY =  Mathf.Sin(Mathf.Deg2Rad * sliceAngleResolution * i); 
			verts[i].y = radius * vertY;
			float vertZ =  Mathf.Cos(Mathf.Deg2Rad * sliceAngleResolution * i); 
			verts[i].z = radius * vertZ;*/
			//Gizmos.DrawSphere(verts[i],0.01f);

			if (cylFacetype == BuzzwireProcGenScript.CylinderFaceType.side)
			{
				normals[i] = new Vector3(0, verts[i].y, verts[i].z);
				uvs[i] = new Vector2((float)(i / (double)sliceSideCount), yUVStepFactor);
				//Debug.Log(uvs[i]);
			}
			else if (cylFacetype == BuzzwireProcGenScript.CylinderFaceType.edge)
			{
				if (yUVStepFactor == 0.0)
				{
					normals[i] = new Vector3(-radius * vertX, verts[i].y, verts[i].z);
				}
				else if (yUVStepFactor == 1.0)
				{
					normals[i] = new Vector3(radius * vertX, verts[i].y, verts[i].z);
				}

				uvs[i] = new Vector2(0.1f, 0.1f);
			}

			//Vector2 test = new Vector2(float.Parse((0.05555556f).ToString("F6")),0.3888889f);
			//Debug.Log(test);


			//Debug.Log((float)(i/(double)sliceSideCount));
		}

	}

}
