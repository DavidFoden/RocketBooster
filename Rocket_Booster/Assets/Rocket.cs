using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float MainThrust = 100f;
    [SerializeField] float levelLoadDelay = 2f;



    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip death;
    [SerializeField] AudioClip success;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem successParticles;


    AudioSource RocketBoost;
    Rigidbody rigidBody;

    enum State { Alive, Dying, Transcending }
 State state = State.Alive;

 bool collisionsAreEnabled = true;


    // Start is called before the first frame update
    void Start()
    {
        RocketBoost = GetComponent<AudioSource>();
        rigidBody = GetComponent<Rigidbody>();


    }

    // Update is called once per frame
    void Update() 
    {
        if (state == State.Alive)
        {
        RespondToThrustInput();
        RespondToRotateInput();
       }
       if (Debug.isDebugBuild) {
       RespondToDebugKeys();
      }
    }
private void RespondToDebugKeys()
 {
    if(Input.GetKeyDown(KeyCode.L)) {
        LoadNextLevel();
    }
    else if(Input.GetKeyDown(KeyCode.C)) {
        collisionsAreEnabled = !collisionsAreEnabled;
    }
}

    void OnCollisionEnter(Collision collision) {
        if (state != State.Alive || !collisionsAreEnabled ) {
            return;
        }
        switch (collision.gameObject.tag)
        {
            case "Friendly":
        // do nothing
            break;
            case "Finish":
           StartSuccessSequence();
            break;
        
           default:
           StartDeathSequene();
           break;
        }
    }

    private void StartDeathSequene() {
          state = State.Dying;
          RocketBoost.Stop();
        RocketBoost.PlayOneShot(death);
        deathParticles.Play();          
        Invoke("LoadFirstLevel", levelLoadDelay); 
    }

    private void StartSuccessSequence() {
         state = State.Transcending;
             RocketBoost.Stop();
           RocketBoost.PlayOneShot(success);
           successParticles.Play();
           Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void LoadNextLevel() {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        if (nextSceneIndex == SceneManager.sceneCountInBuildSettings) {
            nextSceneIndex = 0;
        }
        SceneManager.LoadScene(nextSceneIndex);
    }
    

    private void LoadFirstLevel() {
        SceneManager.LoadScene(0);
    }
    

     private void RespondToThrustInput() { 
        if (Input.GetKey(KeyCode.Space)) {
            ApplyThrust();
        }
            else {
                RocketBoost.Stop();
                mainEngineParticles.Stop();

            }
        }
        private void ApplyThrust() {
            rigidBody.AddRelativeForce(Vector3.up * MainThrust * Time.deltaTime);
            if (!RocketBoost.isPlaying) { // So it does not layer the sound
            RocketBoost.PlayOneShot(mainEngine);
            }
            mainEngineParticles.Play();
        }
        
    private void RespondToRotateInput() { //Roatate My ass
         rigidBody.freezeRotation = true; // Take manual control of rotation
        
         float rotationSpeed = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A)) {
          transform.Rotate(Vector3.forward * rotationSpeed); // here as well down!
        }
        
       else if (Input.GetKey(KeyCode.D)) {
          transform.Rotate(-Vector3.forward * rotationSpeed);
        }
         rigidBody.freezeRotation = false; // resume physices control of rotation
    }
}
