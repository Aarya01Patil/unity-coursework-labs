using UnityEngine;
public class ColorChangerOrb : MonoBehaviour
{
    public Color[] orbColors = new Color[4] { Color.red, Color.green, Color.blue, Color.yellow };

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerMove>() == null)
        {
            return;
        }

        SpriteRenderer playerSprite = other.GetComponent<SpriteRenderer>();
        if (playerSprite == null || orbColors.Length == 0)
        {
            return;
        }

        playerSprite.color = GetRandomColor(playerSprite.color);
        Destroy(gameObject);
    }

    private Color GetRandomColor(Color currentColor)
    {
        int randomIndex = Random.Range(0, orbColors.Length);
        return orbColors[randomIndex];
    }
}
