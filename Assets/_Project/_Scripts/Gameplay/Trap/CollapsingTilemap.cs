using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections;

public class CollapsingTilemap : MonoBehaviour
{
    public Tilemap tilemap;
    public float collapseDelay = 0.05f; // thời gian giữa các ô sập
    public ParticleSystem collapseEffect; // bụi đá (optional)

    public void Collapse(Vector3 hitPosition)
    {
        StartCoroutine(CollapseTiles(hitPosition));
        GetComponent<FallingGround>().TriggerCollapse();

    }

    IEnumerator CollapseTiles(Vector3 hitPosition)
    {
        BoundsInt bounds = tilemap.cellBounds;
        for (int y = bounds.yMax; y >= bounds.yMin; y--)
        {
            for (int x = bounds.xMin; x <= bounds.xMax; x++)
            {
                Vector3Int cell = new Vector3Int(x, y, 0);
                if (tilemap.HasTile(cell))
                {
                    tilemap.SetTile(cell, null);

                    // Hiệu ứng bụi
                    if (collapseEffect)
                        Instantiate(collapseEffect, tilemap.CellToWorld(cell) + new Vector3(0.5f, 0.5f, 0), Quaternion.identity);

                    yield return new WaitForSeconds(collapseDelay);
                }
            }
        }
    }
}
