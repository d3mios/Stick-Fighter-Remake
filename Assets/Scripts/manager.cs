using UnityEngine;
using UnityEngine.SceneManagement;
using Characters;
public class manager : MonoBehaviour
{
    string scene;
    public static manager Instance;
    bool trainingMode;
    character p1, p2;
    private void Awake() {
        if (Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void endGame(string winner) {}

    void gameUpdate() {
        if (p1.died() && !p2.died()) {
            endGame("p1");
        }
        else if (!p1.died() && p2.died()) {
            endGame("p2");
        }
        else if (p1.died() && p2.died()) {
            endGame("draw");
        }
        
        if (trainingMode && Input.GetKey(KeyCode.Space)) {
            SceneManager.LoadScene("Options");
        }
    }

    void titleUpdate() {
        if (Input.GetKey(KeyCode.Space)) SceneManager.LoadScene("Game");
    }

    void optionsUpdate() {
        if (Input.GetKey(KeyCode.Space)) SceneManager.LoadScene("Title");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        this.scene = scene.name;
        if (this.scene == "Game") {
            var characters = FindObjectsOfType<character>();
            p1 = characters[0];
            p2 = characters[1];
        }
    }

    void Start()
    {
        trainingMode = true;
        Scene currentScene = SceneManager.GetActiveScene();
        scene = currentScene.name;
    }
    void FixedUpdate()
    {
        switch (scene) {
            case "Title":
                titleUpdate();
            break;
            case "Game":
                gameUpdate();
            break;
            case "Options":
                optionsUpdate();
            break;
        }
    }
}
