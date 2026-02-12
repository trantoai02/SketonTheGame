using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TheDoorSorting : MonoBehaviour
{
    public Transform player; // Biến tham chiếu tới người chơi
    public SpriteRenderer playerRenderer; // SpriteRenderer của người chơi
    public SpriteRenderer[] objectsToSort; // Các vật thể cần thay đổi thứ tự sắp xếp

    void Update()
    {
        foreach (var obj in objectsToSort)
        {
            // Lấy vị trí của vật thể
            Vector3 objPosition = obj.transform.position;
            // Lấy vị trí của người chơi
            Vector3 playerPosition = player.position;

            // Kiểm tra vị trí của người chơi so với vật thể
            if (playerPosition.y > objPosition.y)
            {
                obj.sortingOrder = playerRenderer.sortingOrder + 1; // Vật thể nằm dưới player
            }
            else
            {
                if (playerPosition.x < objPosition.x)
                {
                    obj.sortingOrder = playerRenderer.sortingOrder - 1; // Vật thể nằm trên player
                }
                else
                {
                    obj.sortingOrder = playerRenderer.sortingOrder + 1; // Vật thể nằm dưới player
                }
            }
        }
    }
}
