using UnityEngine;

public class VerticalShaking_process : MonoBehaviour
{
    public GameObject MedalDai;
    public GameObject Field;

    Vector3 daiPos;
    Vector3 fieldPos;

    float timer;
    float duration;
    bool isShaking;

    public void StartShake(float duration)
    {
        this.duration = duration;
        timer = 0f;
        isShaking = true;
        fieldPos = Field.transform.position;
    }

    void Update()
    {
        if (!isShaking) return;

        timer += Time.deltaTime;

        // áŠ±‚Ìƒ‰ƒ“ƒ_ƒ€‚ğ‰Á‚¦‚Äã‰º‚É—h‚ç‚·

        float random = Random.Range(duration * 0.8f, duration * 1.2f);

        float offset = Mathf.Sin(timer * 30f) * 0.3f * random;

        // rigidbody‚ğg‚Á‚ÄField‚ğã‰º‚É—h‚ç‚·

        Rigidbody rb = Field.GetComponent<Rigidbody>();

        rb.MovePosition(fieldPos + new Vector3(0f, offset, 0f));
        


        if (timer >= duration)
        {
            Stop();
        }
    }

    void Stop()
    {
        isShaking = false;

        Field.transform.position = fieldPos;
    }
}
