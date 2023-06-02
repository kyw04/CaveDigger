using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    public GameObject[] dungeonPrefab;
    public GameObject[] wallPrefab;
    public GameObject unbrokenWallPrefab;
    public Vector2 mapSize;
    public int dungeonCount;
    public int startRoomSize;

    private List<Transform> dungeonTransform = new List<Transform>();
    private void Start()
    {
        for (int i = (int)mapSize.x / -2; i <= mapSize.x / 2; i++)
        {
            for (int j = (int)mapSize.y / -2; j <= mapSize.y / 2; j++)
            {
                Vector3 pos = new Vector3(i, j, 0);
                GameObject wall;

                if (pos.x <= startRoomSize / 2 && pos.y <= startRoomSize / 2 &&
                    pos.x >= startRoomSize / -2 && pos.y >= startRoomSize / -2)
                {
                    continue;
                }
                if (pos.x == (int)mapSize.x / -2 || pos.x == (int)mapSize.x / 2 ||
                    pos.y == (int)mapSize.y / -2 || pos.y == (int)mapSize.y / 2)
                {
                    wall = unbrokenWallPrefab;
                }
                else
                {
                    wall = wallPrefab[Random.Range(0, wallPrefab.Length)];
                }
                GameObject newWall = Instantiate(wall, pos, Quaternion.identity);
                newWall.transform.parent = transform;
            }
        }

        for (int i = 0; i < dungeonCount; i++)
        {
            int changeCount = 500;
            GameObject selectedDungeon = dungeonPrefab[Random.Range(0, dungeonPrefab.Length)];
            Vector3 pos = Vector3.zero;
            while (pos == Vector3.zero)
            {
                pos = new Vector3(
                    Random.Range((int)(mapSize.x / -2 + selectedDungeon.transform.localScale.x / 2), (int)(mapSize.x / 2 - selectedDungeon.transform.localScale.x / 2)),
                    Random.Range((int)(mapSize.y / -2 + selectedDungeon.transform.localScale.y / 2), (int)(mapSize.y / 2 - selectedDungeon.transform.localScale.y / 2)),
                    0);
                changeCount--;

                int roomSizeX = startRoomSize / 2 + (int)selectedDungeon.transform.localScale.x / 2 + 2;
                int roomSizeY = startRoomSize / 2 + (int)selectedDungeon.transform.localScale.y / 2 + 2;
                if ((pos.x < roomSizeX && pos.x > -roomSizeX) || 
                    (pos.y < roomSizeY && pos.y > -roomSizeY))
                {
                    pos = Vector3.zero;
                    changeCount++;
                    continue;
                }

                if (changeCount == 0)
                {
                    Debug.Log("change count zero");
                    break;
                }

                for (int j = 0; j < dungeonTransform.Count; j++)
                {
                    float scaleX = dungeonTransform[j].localScale.x / 2 + selectedDungeon.transform.localScale.x / 2;
                    scaleX -= scaleX / 3f;
                    float scaleY = dungeonTransform[j].localScale.y / 2 + selectedDungeon.transform.localScale.y / 2;
                    scaleY -= scaleY / 3f;
                    Vector2 distance = new Vector2(Mathf.Abs(dungeonTransform[j].position.x - pos.x), Mathf.Abs(dungeonTransform[j].position.y - pos.y));

                    if (distance.x <= scaleX || distance.y <= scaleY)
                    {
                        pos = Vector3.zero;
                        break;
                    }
                }
            }

            Transform dungeon = Instantiate(selectedDungeon, pos, Quaternion.identity).transform;
            dungeonTransform.Add(dungeon.transform);
            Collider2D[] colliders = Physics2D.OverlapBoxAll(dungeon.position, dungeon.localScale, 0);

            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Wall"))
                {
                    Destroy(collider.gameObject);
                }
            }
        }
    }
}
