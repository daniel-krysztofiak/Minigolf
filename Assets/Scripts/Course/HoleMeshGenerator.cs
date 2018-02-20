﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HoleMeshGenerator : MonoBehaviour {

	private float scale = 2f;

	private int groundHeight;
    private int width;
	private int length;
    private LineRenderer layout;

	Vector3[] vertices;
	Vector3[] normals;

	private Mesh mesh;

    private int xSize, ySize, zSize;

    public void Initialize(int groundLevel, int trackWidth, int segmentLength)
    {
        Debug.Log("groundLevel: " + groundLevel);
        groundHeight = groundLevel;
		width = trackWidth;
		length = segmentLength;
        layout = gameObject.GetComponent<LineRenderer>();
		//layout.enabled = false;

        CreateVertices();
        CreateTriangles();
        CreateColliders();
    }

    private void CreateColliders()
    {
        //throw new NotImplementedException();
    }

    private void CreateTriangles()
    {
        //throw new NotImplementedException();
    }

	private void CreateVertices ()
	{
		xSize = 3;
		ySize = 1;
		zSize = 1;

		Vector3 [ ] dirs = new Vector3 [layout.positionCount];
		for (int i = 0; i < dirs.Length; i++)
		{
			if (i == 0)
			{
				dirs [i] = (layout.GetPosition(i + 1) - layout.GetPosition(i)).normalized;
			} else if (i == dirs.Length - 1)
			{
				dirs [i] = (layout.GetPosition(i) - layout.GetPosition(i - 1)).normalized;
			} else
			{
				Quaternion lastDir = Quaternion.Euler(layout.GetPosition(i) - layout.GetPosition(i - 1));
				Quaternion nextDir = Quaternion.Euler(layout.GetPosition(i + 1) - layout.GetPosition(i));
				dirs [i] = Quaternion.Lerp(lastDir, nextDir, 0.5f).eulerAngles.normalized;
			}
			Debug.Log(i + ". Direction: " + dirs [i]);
		}

		int cornerVertices = 4 * layout.positionCount;
		int edgeVertices = layout.positionCount * ((ySize - 1) * 2 + xSize - 1) + 
			4 * (layout.positionCount - 1) * (zSize - 1) +
			(xSize - 1) * 2;
		int faceVertices = (layout.positionCount - 1) * (2 * (ySize - 1) * (zSize - 1) + (xSize - 1) * (zSize - 1)) +
			2 * (xSize - 1) * (ySize - 1);
		vertices = new Vector3 [cornerVertices + edgeVertices + faceVertices];
		normals = new Vector3 [vertices.Length];
		Debug.Log("corner verts: " + cornerVertices + "; edge verts: " + edgeVertices + "; face verts: " + faceVertices + "; total verts: " + vertices.Length);

		int v = 0;

		for (int y = 0; y <= ySize; y++)
		{
			Vector3 rightShift = new Vector3(- (float) width * scale / 2f, 0, 0);
			Vector3 offset = layout.GetPosition(0) + rightShift;
			for(int x = 0; x <= xSize; x++)
			{
				SetVertex(v++, x, y, 0, offset, dirs[0]);
			}

		}

		mesh.vertices = vertices;
		mesh.normals = normals;
	}

	private void SetVertex (int i, int x, int y, int z, Vector3 offset, Vector3 dir)
	{
		Vector3 inner = vertices [i] = new Vector3(((float) width / (float)(xSize + 1)) * x, y, z * length) * scale + offset;
		Debug.Log(((float) width / (float) (xSize + 1)) + "*" + x + "=" + (((float) width / (float) (xSize + 1)) * x));
		normals [i] = (vertices [i] - inner).normalized;
		vertices [i] = inner;

		Debug.Log(i + ". vertex set: " + vertices [i] + "; normal: " + normals [i]);
		//cubeUV [i] = new Color32((byte) x, (byte) y, (byte) z, 0);
	}

	private void OnDrawGizmos ()
	{
		if (vertices == null)
			return;
		for (int i = 0; i < vertices.Length; i++)
		{
			Gizmos.color = Color.black;
			Gizmos.DrawCube(vertices [i], Vector3.one);
			Gizmos.color = Color.yellow;
			Gizmos.DrawRay(vertices [i], normals [i]);
		}
	}
}
