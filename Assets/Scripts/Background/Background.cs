using UnityEngine;

public class Background : MonoBehaviour
{
    [SerializeField] private SpriteRenderer m_SpriteRenderer;
    [SerializeField] private Camera m_Camera;

    private float aspectRatio;
    private Vector2 screenSize;

    private void Start()
    {
        SetBackgroundSize(new Vector2(Screen.width, Screen.height));
    }

    

    private void SetBackgroundSize(Vector2 screenSize)
    {
        aspectRatio = m_SpriteRenderer.size.x / m_SpriteRenderer.size.y;
        this.screenSize.y = m_Camera.orthographicSize * 2;
        this.screenSize.x = this.screenSize.y * screenSize.x / screenSize.y;
        m_SpriteRenderer.size = new Vector2(this.screenSize.x, this.screenSize.x / aspectRatio);

        if (m_SpriteRenderer.size.y < this.screenSize.y)
        {
            m_SpriteRenderer.size = new Vector2(this.screenSize.y * aspectRatio, this.screenSize.y);
        }
        transform.position = new Vector3(m_Camera.transform.position.x, m_Camera.transform.position.y - ((m_SpriteRenderer.size.y - this.screenSize.y) / 2), transform.position.z);
    }
}
