using TMPro;
using UnityEngine;

public class WorldTextPopupFade : MonoBehaviour
{
    [SerializeField] private Color textColor;
    [SerializeField] private float fadeSpeed;
    private TextMeshPro _textMesh;
    private Camera _camera;
    private float _disappearTimer = 1f;

        
    private void Awake()
    {
        _textMesh = GetComponent<TextMeshPro>();
        _camera = Camera.main;
    }
        
    public static WorldTextPopupFade Create(Vector3 position, string text)
    {
        Transform popupTransform = Instantiate(PrefabManager.Instance.worldTextPopupFade, position, Quaternion.identity);
        WorldTextPopupFade popup = popupTransform.GetComponent<WorldTextPopupFade>();
        popup.Setup(text);

        return popup;
    }

    private void Setup(string text)
    {
        _textMesh.text = text;
        textColor = _textMesh.color;
    }

    private void Update()
    {
        float moveYSpeed = 1f;
        transform.position += new Vector3(0f, moveYSpeed, 0f) * Time.deltaTime;
        transform.eulerAngles = new Vector3(_camera.transform.eulerAngles.x, 0f, 0f);

        _disappearTimer -= Time.deltaTime;
        if (_disappearTimer <= 0)
        {
            float disappearSpeed = 2f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            _textMesh.color = textColor;
            if (textColor.a <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
