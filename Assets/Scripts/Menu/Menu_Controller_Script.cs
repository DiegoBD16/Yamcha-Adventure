using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class Meno_Controller_Script : MonoBehaviour
{
    [SerializeField] Button _StartGameButton;
    [SerializeField] Button _LeaveGameButton;

    private void Start()
    {
        _StartGameButton.onClick.AddListener(Jugar);
        _LeaveGameButton.onClick.AddListener(Salir);
    }
    public void Jugar()
    {
        SceneManager.LoadScene("Tutorial");
    }
    public void Salir()
    {
        Application.Quit();
    }
}
